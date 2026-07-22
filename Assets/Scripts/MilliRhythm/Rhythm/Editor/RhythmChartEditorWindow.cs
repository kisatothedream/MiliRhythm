using System;
using UnityEditor;
using UnityEngine;

namespace MilliRhythm.Rhythm.Editor
{
	/* This editor tool was developed with assistance from generative AI.
	 * The feature requirements, integration, review, and modifications
	 * were performed by the project author. */
	public class RhythmChartEditorWindow : EditorWindow
	{
		private static int KeyCodeToLane(KeyCode keyCode)
		{
			return keyCode switch
			{
				KeyCode.A => 0,
				KeyCode.W => 1,
				KeyCode.S => 2,
				KeyCode.D => 3,
				_ => -1,
			};
		}

		private static string LaneToKey(int lane)
		{
			return lane switch
			{
				0 => "A",
				1 => "W",
				2 => "S",
				3 => "D",
				_ => throw new ArgumentOutOfRangeException(nameof(lane), lane, null),
			};
		}

		private enum InteractionMode
		{
			None,
			DragNote,
			PanTimeline,
			MovePlayHead,
		}

		private const float ToolbarHeight = 22f;
		private const float SettingsHeight = 24f;
		private const float TimelineHeaderHeight = 28f;
		private const float TrackLabelWidth = 80f;
		private const float WaveformHeight = 80f;
		private const float LaneHeight = 36f;
		private const float NoteSize = 7f;

		private RhythmChart chart;

		private double viewStartTime;
		private float pixelsPerSecond = 120f;
		private Vector2 verticalScroll;

		private int selectedNoteIndex = -1;
		private InteractionMode interactionMode;

		private double playHeadTime;
		private bool isPlaying;
		private double playbackStartDspTime;
		private double playbackStartChartTime;

		private SnapDivision snapDivision = SnapDivision.Quarter;
		private bool isRecording;

		private readonly bool[] laneKeyPressed = new bool[4];
		private AudioClip cachedWaveformClip;
		private float[] waveformSamples;

		private int waveformFrameCount;
		private int waveformChannels;

		private bool waveformCacheFailed;
		private string waveformErrorMessage;
		private const float WaveformAmplitudeScale = 1f;


		private const int FixedLaneCount = 4;

		private const float TimelineTopMargin = 3f;
		private const float SectionSpacing = 6f;

		private const float NoteInspectorHorizontalMargin = 6f;
		private const float NoteInspectorHeight = 68f;
		private const float NoteInspectorMargin = 6f;

		private bool isPreviewEnabled;
		private RhythmGamePlayer previewPlayer;

		[MenuItem("Tools/Rhythm Game/Chart Editor")]
		private static void Open()
		{
			GetWindow<RhythmChartEditorWindow>("Rhythm Chart");
		}

		public static void Open(RhythmChart chart)
		{
			var window = GetWindow<RhythmChartEditorWindow>("Rhythm Chart");

			window.SetChart(chart);
			window.Focus();
		}

		private void OnEnable()
		{
			EditorApplication.update += OnEditorUpdate;
		}

		private void OnDisable()
		{
			EditorApplication.update -=
				OnEditorUpdate;

			DisablePreview();

			StopPlayback();
			ClearWaveformCache();
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 50f;
			DrawToolbar();

			if (chart == null)
			{
				EditorGUILayout.HelpBox(
					"RhythmChart 에셋을 선택해 주세요.",
					MessageType.Info);

				return;
			}

			DrawSettings();

			var timelineAreaRect = GetTimelineAreaRect();

			DrawTimeline(timelineAreaRect);
			DrawSelectedNoteInspector();

			HandleRecordingKeyboardInput(Event.current);
		}

		private void HandleRecordingKeyboardInput(Event currentEvent)
		{
			if (!isRecording || !isPlaying || chart == null)
			{
				return;
			}

			if (currentEvent.type == EventType.KeyDown)
			{
				var lane = KeyCodeToLane(currentEvent.keyCode);

				if (lane < 0 || lane >= 4)
				{
					return;
				}

				if (laneKeyPressed[lane])
				{
					currentEvent.Use();
					return;
				}

				laneKeyPressed[lane] = true;

				RecordNoteAtCurrentTime(lane);

				currentEvent.Use();
				return;
			}

			if (currentEvent.type == EventType.KeyUp)
			{
				var lane = KeyCodeToLane(currentEvent.keyCode);

				if (lane < 0 || lane >= 4)
				{
					return;
				}

				laneKeyPressed[lane] = false;

				currentEvent.Use();
			}
		}

		private void DrawToolbar()
		{
			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				var newChart = (RhythmChart)EditorGUILayout.ObjectField(
					chart,
					typeof(RhythmChart),
					false,
					GUILayout.Width(260f));

				if (newChart != chart)
				{
					SetChart(newChart);
				}

				GUILayout.Space(8f);

				DrawRecordButton();

				GUILayout.Space(4f);

				DrawPreviewButton();

				if (GUILayout.Button(
					    isPlaying ? "Pause" : "Play",
					    EditorStyles.toolbarButton,
					    GUILayout.Width(50f)))
				{
					if (isPlaying)
					{
						PausePlayback();
					}
					else
					{
						StartPlayback();
					}
				}

				if (GUILayout.Button(
					    "Stop",
					    EditorStyles.toolbarButton,
					    GUILayout.Width(45f)))
				{
					StopPlayback();
				}

				GUILayout.Space(8f);
				GUILayout.Label($"Time {playHeadTime:F3}", GUILayout.Width(90f));

				GUILayout.FlexibleSpace();
				GUILayout.Label($"Zoom {pixelsPerSecond:F0}px/s");
			}
		}

		private void DrawSettings()
		{
			using (new EditorGUILayout.HorizontalScope(
				       EditorStyles.helpBox,
				       GUILayout.Height(SettingsHeight)))
			{
				DrawBpmEditor();

				DrawOffsetEditor();

				GUILayout.Space(10f);

				DrawLaneCreateButtons();

				GUILayout.Space(10f);

				DrawSnapButtons();

				GUILayout.FlexibleSpace();
			}
		}

		private void DrawBpmEditor()
		{
			var controlRect = EditorGUILayout.GetControlRect(
				false,
				EditorGUIUtility.singleLineHeight,
				GUILayout.Width(150f));


			EditorGUI.BeginChangeCheck();

			var newBpm = EditorGUI.DoubleField(
				controlRect,
				new GUIContent("BPM"),
				chart.Bpm);

			if (EditorGUI.EndChangeCheck())
			{
				newBpm = Math.Max(1.0, newBpm);

				Undo.RecordObject(chart, "Change Rhythm Chart BPM");

				chart.SetBpm(newBpm);

				EditorUtility.SetDirty(chart);
				RefreshPreview();
				Repaint();
			}
		}

		private void DrawOffsetEditor()
		{
			var controlRect = EditorGUILayout.GetControlRect(
				false,
				EditorGUIUtility.singleLineHeight,
				GUILayout.Width(190f));

			EditorGUI.BeginChangeCheck();

			var newOffsetSeconds = EditorGUI.DoubleField(
				controlRect,
				new GUIContent("Offset"),
				chart.OffsetSeconds);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(chart, "Change Rhythm Chart Offset");

				chart.SetOffsetSeconds(newOffsetSeconds);

				EditorUtility.SetDirty(chart);
				RefreshPreview();
				Repaint();
			}
		}

		private void DrawSnapButtons()
		{
			GUILayout.Label("Snap", GUILayout.Width(35f));

			DrawSnapButton("1/4", SnapDivision.Quarter);
			DrawSnapButton("1/8", SnapDivision.Eighth);
			DrawSnapButton("1/12", SnapDivision.Twelfth);
			DrawSnapButton("1/16", SnapDivision.Sixteenth);
			DrawSnapButton("1/24", SnapDivision.TwentyFourth);
			DrawSnapButton("1/32", SnapDivision.ThirtySecond);
			DrawSnapButton("1/48", SnapDivision.FortyEighth);
			DrawSnapButton("1/64", SnapDivision.SixtyFourth);
		}

		private void DrawSnapButton(string label, SnapDivision division)
		{
			var previousBackgroundColor = GUI.backgroundColor;

			if (snapDivision == division)
			{
				GUI.backgroundColor = new Color(0.45f, 0.65f, 1f);
			}

			if (GUILayout.Button(label, EditorStyles.miniButton, GUILayout.Width(42f)))
			{
				snapDivision = division;
				GUI.FocusControl(null);
				Repaint();
			}

			GUI.backgroundColor = previousBackgroundColor;
		}

		private void DrawRecordButton()
		{
			var previousBackgroundColor = GUI.backgroundColor;

			if (isRecording)
			{
				GUI.backgroundColor = new Color(1f, 0.35f, 0.35f);
			}

			if (GUILayout.Button(
				    isRecording ? "Recording" : "Record",
				    EditorStyles.toolbarButton,
				    GUILayout.Width(70f)))
			{
				isRecording = !isRecording;

				if (!isRecording)
				{
					ClearLaneKeyStates();
				}

				Focus();
				Repaint();
			}

			GUI.backgroundColor = previousBackgroundColor;
		}

		private void DrawPreviewButton()
		{
			var previousBackgroundColor =
				GUI.backgroundColor;

			if (isPreviewEnabled)
			{
				GUI.backgroundColor =
					new Color(0.45f, 0.75f, 1f);
			}

			if (GUILayout.Button(
				    isPreviewEnabled
					    ? "Preview On"
					    : "Preview",
				    EditorStyles.toolbarButton,
				    GUILayout.Width(80f)))
			{
				if (isPreviewEnabled)
				{
					DisablePreview();
				}
				else
				{
					EnablePreview();
				}
			}

			GUI.backgroundColor =
				previousBackgroundColor;
		}

		private void EnablePreview()
		{
			if (chart == null)
			{
				return;
			}

			previewPlayer = FindPreviewPlayer();

			if (previewPlayer == null)
			{
				Debug.LogWarning(
					"현재 씬에서 RhythmGamePlayer를 찾지 못했습니다.");

				return;
			}

			previewPlayer.CreateEditorPreview(chart);

			isPreviewEnabled = true;

			UpdatePreviewAtPlayHead();

			Repaint();
		}

		private void DisablePreview()
		{
			DestroyAllScenePreviews();

			isPreviewEnabled = false;
			previewPlayer = null;

			Repaint();
		}

		private void UpdatePreviewAtPlayHead()
		{
			if (!isPreviewEnabled)
			{
				return;
			}

			if (previewPlayer == null)
			{
				previewPlayer = FindPreviewPlayer();
			}

			if (previewPlayer == null)
			{
				Debug.LogWarning("RhythmGamePlayer를 찾지 못했습니다.");
				return;
			}


			previewPlayer.UpdateEditorPreview(playHeadTime);

			EditorApplication.QueuePlayerLoopUpdate();
			SceneView.RepaintAll();
			Repaint();
		}

		private static void DestroyAllScenePreviews()
		{
			var players =
				Resources.FindObjectsOfTypeAll<RhythmGamePlayer>();

			for (var i = 0; i < players.Length; i++)
			{
				var player = players[i];

				if (player == null ||
				    EditorUtility.IsPersistent(player) ||
				    !player.gameObject.scene.IsValid())
				{
					continue;
				}

				player.DestroyEditorPreview();
			}
		}

		private static RhythmGamePlayer FindPreviewPlayer()
		{
			var players =
				Resources.FindObjectsOfTypeAll<
					RhythmGamePlayer>();

			for (var i = 0; i < players.Length; i++)
			{
				var player = players[i];

				if (player == null)
				{
					continue;
				}

				if (EditorUtility.IsPersistent(player))
				{
					continue;
				}

				if (!player.gameObject.scene.IsValid())
				{
					continue;
				}

				return player;
			}

			return null;
		}

		private void DrawTimeline(Rect fullRect)
		{
			var labelRect = new Rect(
				fullRect.x,
				fullRect.y,
				TrackLabelWidth,
				fullRect.height);

			var timelineRect = new Rect(
				labelRect.xMax,
				fullRect.y,
				fullRect.width - TrackLabelWidth,
				fullRect.height);

			EditorGUI.DrawRect(
				fullRect,
				new Color(0.13f, 0.13f, 0.13f));

			EditorGUI.DrawRect(
				labelRect,
				new Color(0.18f, 0.18f, 0.18f));

			DrawRecordingIndicator(timelineRect);

			DrawWaveform(timelineRect);

			DrawBeatGrid(timelineRect);
			DrawNotes(timelineRect);
			DrawPlayhead(timelineRect);
			DrawTimelineHeader(timelineRect);
			DrawLaneBackgrounds(labelRect, timelineRect);
			DrawWaveformLabel(labelRect);

			HandleTimelineInput(timelineRect);
		}

		private Rect GetTimelineAreaRect()
		{
			var y = ToolbarHeight
			        + SettingsHeight
			        + TimelineTopMargin;

			var height = TimelineHeaderHeight
			             + WaveformHeight
			             + LaneHeight * FixedLaneCount;

			return new Rect(
				0f,
				y,
				position.width,
				height);
		}

		private Rect GetSelectedNoteInspectorRect()
		{
			var timelineRect = GetTimelineAreaRect();

			var x = NoteInspectorHorizontalMargin;
			var y = timelineRect.yMax + SectionSpacing;
			var width = Mathf.Max(
				0f,
				position.width - NoteInspectorHorizontalMargin * 2f);

			var availableHeight = Mathf.Max(
				0f,
				position.height - y - SectionSpacing);

			var height = Mathf.Min(
				NoteInspectorHeight,
				availableHeight);

			return new Rect(
				x,
				y,
				width,
				height);
		}

		private void DrawLaneCreateButtons()
		{
			GUILayout.Label("Add", GUILayout.Width(25f));

			DrawLaneCreateButton("W", 0);
			DrawLaneCreateButton("A", 1);
			DrawLaneCreateButton("S", 2);
			DrawLaneCreateButton("D", 3);
		}

		private void DrawLaneCreateButton(string label, int lane)
		{
			if (!GUILayout.Button(
				    label,
				    EditorStyles.miniButton,
				    GUILayout.Width(28f)))
			{
				return;
			}

			AddNoteAtPlayHead(lane);

			GUI.FocusControl(null);
			Focus();
		}

		private void DrawTimelineHeader(Rect timelineRect)
		{
			var headerRect = new Rect(
				timelineRect.x,
				timelineRect.y,
				timelineRect.width,
				TimelineHeaderHeight);

			EditorGUI.DrawRect(headerRect, new Color(0.1f, 0.1f, 0.1f));

			var viewEndTime = viewStartTime + timelineRect.width / pixelsPerSecond;
			var labelInterval = GetTimeLabelInterval();
			var firstTime = Math.Floor(viewStartTime / labelInterval) * labelInterval;

			for (var time = firstTime; time <= viewEndTime; time += labelInterval)
			{
				var x = TimeToX(time, timelineRect);

				Handles.color = new Color(1f, 1f, 1f, 0.25f);
				Handles.DrawLine(
					new Vector3(x, headerRect.y),
					new Vector3(x, headerRect.yMax));

				GUI.Label(
					new Rect(x + 3f, headerRect.y + 3f, 70f, 20f),
					$"{time:F2}");
			}
		}

		private void DrawLaneBackgrounds(Rect labelRect, Rect timelineRect)
		{
			for (var lane = 0; lane < chart.LaneCount; lane++)
			{
				var y = GetLaneY(lane, timelineRect);

				var laneLabelRect = new Rect(
					labelRect.x,
					y,
					labelRect.width,
					LaneHeight);

				var laneTimelineRect = new Rect(
					timelineRect.x,
					y,
					timelineRect.width,
					LaneHeight);

				if (lane % 2 == 0)
				{
					EditorGUI.DrawRect(laneTimelineRect, new Color(1f, 1f, 1f, 0.025f));
				}

				GUI.Label(laneLabelRect, $"Lane {LaneToKey(lane)}");

				Handles.color = new Color(1f, 1f, 1f, 0.1f);
				Handles.DrawLine(
					new Vector3(timelineRect.x, y + LaneHeight),
					new Vector3(timelineRect.xMax, y + LaneHeight));
			}
		}

		private void DrawBeatGrid(Rect timelineRect)
		{
			var contentTop = timelineRect.y + TimelineHeaderHeight;
			var contentBottom = timelineRect.yMax;

			var viewEndTime = viewStartTime + timelineRect.width / pixelsPerSecond;
			var startTick = chart.TimeToTick(viewStartTime);
			var endTick = chart.TimeToTick(viewEndTime);

			var gridTickInterval = GetSnapTickInterval();
			var firstTick = FloorToMultiple(startTick, gridTickInterval);

			for (var tick = firstTick; tick <= endTick; tick += gridTickInterval)
			{
				var time = chart.TickToTime(tick);
				var x = TimeToX(time, timelineRect);

				var ticksPerMeasure = chart.TicksPerBeat * 4;
				var isMeasure = tick % ticksPerMeasure == 0;
				var isBeat = tick % chart.TicksPerBeat == 0;

				if (isMeasure)
				{
					Handles.color = new Color(1f, 1f, 1f, 0.45f);
				}
				else if (isBeat)
				{
					Handles.color = new Color(1f, 1f, 1f, 0.25f);
				}
				else
				{
					Handles.color = new Color(1f, 1f, 1f, 0.08f);
				}

				Handles.DrawLine(
					new Vector3(x, contentTop),
					new Vector3(x, contentBottom));
			}
		}

		private void DrawSelectedNoteInspector()
		{
			var inspectorRect = GetSelectedNoteInspectorRect();

			if (inspectorRect.height < EditorGUIUtility.singleLineHeight)
			{
				return;
			}

			GUI.Box(
				inspectorRect,
				GUIContent.none,
				EditorStyles.helpBox);

			var contentRect = new Rect(
				inspectorRect.x + 8f,
				inspectorRect.y + 6f,
				inspectorRect.width - 16f,
				EditorGUIUtility.singleLineHeight);

			if (selectedNoteIndex < 0 ||
			    selectedNoteIndex >= chart.Notes.Count)
			{
				GUI.Label(
					contentRect,
					"선택된 노트가 없습니다.",
					EditorStyles.centeredGreyMiniLabel);

				return;
			}

			var note = chart.Notes[selectedNoteIndex];

			GUI.Label(
				contentRect,
				$"Selected Note #{selectedNoteIndex}",
				EditorStyles.boldLabel);

			contentRect.y += EditorGUIUtility.singleLineHeight + 4f;

			DrawSelectedNoteFields(note, contentRect);
		}

		private void DrawSelectedNoteFields(RhythmNote note, Rect contentRect)
		{
			var tickRect = new Rect(
				contentRect.x,
				contentRect.y,
				150f,
				EditorGUIUtility.singleLineHeight);

			var laneRect = new Rect(
				tickRect.xMax + 8f,
				contentRect.y,
				130f,
				EditorGUIUtility.singleLineHeight);

			var lengthRect = new Rect(
				laneRect.xMax + 8f,
				contentRect.y,
				180f,
				EditorGUIUtility.singleLineHeight);

			EditorGUI.BeginChangeCheck();

			var newTick = EditorGUI.IntField(
				tickRect,
				new GUIContent("Tick"),
				note.Tick);

			var newLane = EditorGUI.IntField(
				laneRect,
				new GUIContent("Lane"),
				note.Lane);

			var newLengthTick = EditorGUI.IntField(
				lengthRect,
				new GUIContent("Length"),
				note.LengthTick);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(
					chart,
					"Edit Rhythm Note");

				note.Tick = Mathf.Max(0, newTick);
				note.Lane = Mathf.Clamp(
					newLane,
					0,
					FixedLaneCount - 1);

				note.LengthTick = Mathf.Max(
					0,
					newLengthTick);

				SortNotes();

				selectedNoteIndex = chart.Notes.IndexOf(note);

				EditorUtility.SetDirty(chart);
				RefreshPreview();
				Repaint();
			}

			DrawSelectedNoteTimeInfo(
				note,
				contentRect);
		}

		private void DrawSelectedNoteTimeInfo(
			RhythmNote note,
			Rect contentRect)
		{
			var endTick = note.Tick + note.LengthTick;

			var startTime = chart.TickToTime(note.Tick);
			var endTime = chart.TickToTime(endTick);
			var duration = endTime - startTime;

			var infoX = contentRect.x + 490f;

			var infoRect = new Rect(
				infoX,
				contentRect.y,
				Mathf.Max(0f, contentRect.xMax - infoX),
				EditorGUIUtility.singleLineHeight);

			GUI.Label(
				infoRect,
				$"End {endTick} / {duration:F3}s",
				EditorStyles.miniLabel);
		}

		private Rect GetSelectedNoteInspectorRect(Rect timelineRect)
		{
			var laneBottom = timelineRect.y
			                 + TimelineHeaderHeight
			                 + WaveformHeight
			                 + chart.LaneCount * LaneHeight
			                 - verticalScroll.y;

			return new Rect(
				timelineRect.x + NoteInspectorMargin,
				laneBottom + NoteInspectorMargin,
				timelineRect.width - NoteInspectorMargin * 2f,
				NoteInspectorHeight);
		}

		private void DrawNotes(Rect timelineRect)
		{
			for (var i = 0; i < chart.Notes.Count; i++)
			{
				var note = chart.Notes[i];

				if (note.Lane < 0 || note.Lane >= chart.LaneCount)
				{
					continue;
				}

				var center = GetNoteCenter(note, timelineRect);

				if (note.LengthTick > 0)
				{
					DrawLongNoteLine(note, center, timelineRect, i == selectedNoteIndex);
				}

				if (center.x < timelineRect.x - NoteSize || center.x > timelineRect.xMax + NoteSize)
				{
					continue;
				}

				Handles.color = i == selectedNoteIndex
					? new Color(1f, 0.75f, 0.2f)
					: Color.white;

				DrawDiamond(center, NoteSize);
			}
		}

		private void DrawLongNoteLine(
			RhythmNote note,
			Vector2 startCenter,
			Rect timelineRect,
			bool isSelected)
		{
			var endTick = note.Tick + note.LengthTick;
			var endTime = chart.TickToTime(endTick);
			var endX = TimeToX(endTime, timelineRect);

			if (endX < timelineRect.x || startCenter.x > timelineRect.xMax)
			{
				return;
			}

			var visibleStartX = Mathf.Max(startCenter.x, timelineRect.x);
			var visibleEndX = Mathf.Min(endX, timelineRect.xMax);

			Handles.color = isSelected
				? new Color(1f, 0.75f, 0.2f, 0.5f)
				: new Color(1f, 1f, 1f, 0.25f);

			Handles.DrawAAPolyLine(
				3f,
				new Vector3(visibleStartX, startCenter.y),
				new Vector3(visibleEndX, startCenter.y));
		}

		private void DrawPlayhead(Rect timelineRect)
		{
			var x = TimeToX(playHeadTime, timelineRect);

			if (x < timelineRect.x || x > timelineRect.xMax)
			{
				return;
			}

			Handles.color = new Color(1f, 0.2f, 0.2f);
			Handles.DrawLine(
				new Vector3(x, timelineRect.y),
				new Vector3(x, timelineRect.yMax));

			var triangleSize = 6f;

			Handles.DrawAAConvexPolygon(
				new Vector3(x - triangleSize, timelineRect.y),
				new Vector3(x + triangleSize, timelineRect.y),
				new Vector3(x, timelineRect.y + triangleSize));
		}

		private void HandleTimelineInput(Rect timelineRect)
		{
			var currentEvent = Event.current;

			HandleZoom(currentEvent, timelineRect);

			switch (currentEvent.type)
			{
				case EventType.MouseDown:
					HandleMouseDown(currentEvent, timelineRect);
					break;

				case EventType.MouseDrag:
					HandleMouseDrag(currentEvent, timelineRect);
					break;

				case EventType.MouseUp:
					HandleMouseUp(currentEvent);
					break;

				case EventType.KeyDown:
					HandleKeyDown(currentEvent);
					break;
			}
		}

		private void HandleMouseDown(Event currentEvent, Rect timelineRect)
		{
			if (!timelineRect.Contains(currentEvent.mousePosition))
			{
				return;
			}

			if (GetSelectedNoteInspectorRect(timelineRect)
			    .Contains(currentEvent.mousePosition))
			{
				return;
			}

			if (currentEvent.button == 2)
			{
				interactionMode = InteractionMode.PanTimeline;
				currentEvent.Use();
				return;
			}

			if (currentEvent.button != 0)
			{
				return;
			}

			var noteIndex = FindNoteIndexAtPosition(currentEvent.mousePosition, timelineRect);

			if (noteIndex >= 0)
			{
				selectedNoteIndex = noteIndex;
				interactionMode = InteractionMode.DragNote;

				Undo.RecordObject(chart, "Move Rhythm Note");

				currentEvent.Use();
				return;
			}

			selectedNoteIndex = -1;

			if (currentEvent.clickCount == 2 && IsInLaneArea(currentEvent.mousePosition, timelineRect))
			{
				CreateNote(currentEvent.mousePosition, timelineRect);
				currentEvent.Use();
				return;
			}

			interactionMode = InteractionMode.MovePlayHead;
			SetPlayheadFromMouse(currentEvent.mousePosition.x, timelineRect);

			currentEvent.Use();
			Repaint();
		}

		private void HandleMouseDrag(Event currentEvent, Rect timelineRect)
		{
			switch (interactionMode)
			{
				case InteractionMode.DragNote:
					DragSelectedNote(currentEvent.mousePosition, timelineRect);
					currentEvent.Use();
					break;

				case InteractionMode.PanTimeline:
					viewStartTime -= currentEvent.delta.x / pixelsPerSecond;
					viewStartTime = Math.Max(0.0, viewStartTime);

					currentEvent.Use();
					Repaint();
					break;

				case InteractionMode.MovePlayHead:
					SetPlayheadFromMouse(currentEvent.mousePosition.x, timelineRect);

					currentEvent.Use();
					Repaint();
					break;
			}
		}

		private void HandleMouseUp(Event currentEvent)
		{
			if (interactionMode == InteractionMode.None)
			{
				return;
			}

			if (interactionMode == InteractionMode.DragNote)
			{
				SortNotes();
				EditorUtility.SetDirty(chart);
				RefreshPreview();
			}

			interactionMode = InteractionMode.None;
			currentEvent.Use();
			Repaint();
		}

		private void HandleKeyDown(Event currentEvent)
		{
			if (currentEvent.keyCode != KeyCode.Delete &&
			    currentEvent.keyCode != KeyCode.Backspace)
			{
				return;
			}

			DeleteSelectedNote();
			currentEvent.Use();
		}

		private void HandleZoom(Event currentEvent, Rect timelineRect)
		{
			if (currentEvent.type != EventType.ScrollWheel ||
			    !timelineRect.Contains(currentEvent.mousePosition))
			{
				return;
			}

			var mouseTimeBeforeZoom = XToTime(currentEvent.mousePosition.x, timelineRect);
			var zoomFactor = Mathf.Exp(-currentEvent.delta.y * 0.05f);

			pixelsPerSecond = Mathf.Clamp(pixelsPerSecond * zoomFactor, 20f, 1600f);

			var localMouseX = currentEvent.mousePosition.x - timelineRect.x;
			viewStartTime = mouseTimeBeforeZoom - localMouseX / pixelsPerSecond;
			viewStartTime = Math.Max(0.0, viewStartTime);

			currentEvent.Use();
			Repaint();
		}

		private void CreateNote(Vector2 mousePosition, Rect timelineRect)
		{
			var lane = PositionToLane(mousePosition.y, timelineRect);
			var time = XToTime(mousePosition.x, timelineRect);
			var tick = chart.TimeToTick(time);
			var snappedTick = SnapTick(tick);

			Undo.RecordObject(chart, "Create Rhythm Note");

			chart.Notes.Add(new RhythmNote
			{
				Tick = Mathf.Max(0, snappedTick),
				Lane = lane,
			});

			SortNotes();

			selectedNoteIndex = FindNoteIndex(snappedTick, lane);

			EditorUtility.SetDirty(chart);
			RefreshPreview();
			Repaint();
		}

		private void DragSelectedNote(Vector2 mousePosition, Rect timelineRect)
		{
			if (selectedNoteIndex < 0 || selectedNoteIndex >= chart.Notes.Count)
			{
				return;
			}

			var time = XToTime(mousePosition.x, timelineRect);
			var tick = SnapTick(chart.TimeToTick(time));
			var lane = PositionToLane(mousePosition.y, timelineRect);

			var note = chart.Notes[selectedNoteIndex];
			note.Tick = Mathf.Max(0, tick);
			note.Lane = lane;

			EditorUtility.SetDirty(chart);
			RefreshPreview();
		}

		private void DeleteSelectedNote()
		{
			if (selectedNoteIndex < 0 || selectedNoteIndex >= chart.Notes.Count)
			{
				return;
			}

			Undo.RecordObject(chart, "Delete Rhythm Note");

			chart.Notes.RemoveAt(selectedNoteIndex);
			selectedNoteIndex = -1;

			EditorUtility.SetDirty(chart);
			RefreshPreview();
			Repaint();
		}

		private int FindNoteIndexAtPosition(Vector2 mousePosition, Rect timelineRect)
		{
			for (var i = chart.Notes.Count - 1; i >= 0; i--)
			{
				var center = GetNoteCenter(chart.Notes[i], timelineRect);
				var hitRect = new Rect(center.x - 9f, center.y - 9f, 18f, 18f);

				if (hitRect.Contains(mousePosition))
				{
					return i;
				}
			}

			return -1;
		}

		private Vector2 GetNoteCenter(RhythmNote note, Rect timelineRect)
		{
			var time = chart.TickToTime(note.Tick);
			var x = TimeToX(time, timelineRect);
			var y = GetLaneY(note.Lane, timelineRect) + LaneHeight * 0.5f;

			return new Vector2(x, y);
		}

		private float GetLaneY(int lane, Rect timelineRect)
		{
			return timelineRect.y
			       + TimelineHeaderHeight
			       + WaveformHeight
			       + lane * LaneHeight
			       - verticalScroll.y;
		}

		private int PositionToLane(float mouseY, Rect timelineRect)
		{
			var localY = mouseY
			             - timelineRect.y
			             - TimelineHeaderHeight
			             - WaveformHeight
			             + verticalScroll.y;

			var lane = Mathf.FloorToInt(localY / LaneHeight);

			return Mathf.Clamp(lane, 0, chart.LaneCount - 1);
		}

		private bool IsInLaneArea(Vector2 mousePosition, Rect timelineRect)
		{
			var laneTop = timelineRect.y
			              + TimelineHeaderHeight
			              + WaveformHeight;

			var laneBottom = laneTop + chart.LaneCount * LaneHeight;

			return mousePosition.y >= laneTop &&
			       mousePosition.y <= laneBottom;
		}

		private float TimeToX(double time, Rect timelineRect)
		{
			return timelineRect.x + (float)((time - viewStartTime) * pixelsPerSecond);
		}

		private double XToTime(float x, Rect timelineRect)
		{
			return viewStartTime + (x - timelineRect.x) / pixelsPerSecond;
		}

		private int SnapTick(int tick)
		{
			var interval = GetSnapTickInterval();
			var snapIndex = Mathf.RoundToInt((float)tick / interval);

			return snapIndex * interval;
		}

		private int GetSnapTickInterval()
		{
			if (chart == null || chart.TicksPerBeat <= 0)
			{
				return 1;
			}

			var division = (int)snapDivision;
			var ticksPerMeasure = chart.TicksPerBeat * 4;

			if (division <= 0)
			{
				Debug.LogError($"잘못된 Snap Division입니다: {division}");
				return 1;
			}

			if (ticksPerMeasure % division != 0)
			{
				Debug.LogWarning(
					$"TPB {chart.TicksPerBeat}에서는 1/{division} 스냅을 정확히 표현할 수 없습니다.");
			}

			return Mathf.Max(1, ticksPerMeasure / division);
		}

		private double GetTimeLabelInterval()
		{
			if (pixelsPerSecond >= 800f)
			{
				return 0.1;
			}

			if (pixelsPerSecond >= 400f)
			{
				return 0.25;
			}

			if (pixelsPerSecond >= 180f)
			{
				return 0.5;
			}

			if (pixelsPerSecond >= 80f)
			{
				return 1.0;
			}

			return 2.0;
		}

		private void SetPlayheadFromMouse(float mouseX, Rect timelineRect)
		{
			playHeadTime = Math.Max(0.0, XToTime(mouseX, timelineRect));

			if (isPlaying)
			{
				StartPlaybackFromCurrentTime();
			}

			UpdatePreviewAtPlayHead();
			Repaint();
		}

		private void StartPlayback()
		{
			if (chart == null || chart.AudioClip == null)
			{
				return;
			}

			StartPlaybackFromCurrentTime();
		}

		private void StartPlaybackFromCurrentTime()
		{
			StopAudioPreview();

			playbackStartChartTime = playHeadTime;
			playbackStartDspTime = EditorApplication.timeSinceStartup;
			isPlaying = true;

			AudioPreviewUtility.Play(chart.AudioClip, (float)playHeadTime);
		}

		private void PausePlayback()
		{
			if (!isPlaying)
			{
				return;
			}

			UpdatePlayheadTime();

			isPlaying = false;

			ClearLaneKeyStates();
			StopAudioPreview();

			Repaint();
		}

		private void StopPlayback()
		{
			isPlaying = false;
			playHeadTime = 0.0;

			ClearLaneKeyStates();
			StopAudioPreview();

			Repaint();
		}

		private void OnEditorUpdate()
		{
			if (isPlaying &&
			    chart != null &&
			    chart.AudioClip != null)
			{
				UpdatePlayheadTime();

				if (playHeadTime >=
				    chart.AudioClip.length)
				{
					StopPlayback();
				}
				else
				{
					FollowPlayhead();
					Repaint();
				}
			}

			UpdatePreviewAtPlayHead();
		}

		private void UpdatePlayheadTime()
		{
			var elapsed = EditorApplication.timeSinceStartup - playbackStartDspTime;
			playHeadTime = playbackStartChartTime + elapsed;
			RefreshPreview();
		}

		private void FollowPlayhead()
		{
			var visibleDuration = Math.Max(0.1, (position.width - TrackLabelWidth) / pixelsPerSecond);
			var followThreshold = viewStartTime + visibleDuration * 0.85;

			if (playHeadTime > followThreshold)
			{
				viewStartTime = playHeadTime - visibleDuration * 0.25;
			}
		}

		private void SetChart(
			RhythmChart newChart)
		{
			DisablePreview();
			StopPlayback();

			chart = newChart;
			selectedNoteIndex = -1;
			viewStartTime = 0.0;
			playHeadTime = 0.0;

			ClearWaveformCache();

			Repaint();
		}

		private void SortNotes()
		{
			chart.Notes.Sort((left, right) =>
			{
				var tickComparison = left.Tick.CompareTo(right.Tick);

				if (tickComparison != 0)
				{
					return tickComparison;
				}

				return left.Lane.CompareTo(right.Lane);
			});
		}

		private int FindNoteIndex(int tick, int lane)
		{
			for (var i = 0; i < chart.Notes.Count; i++)
			{
				var note = chart.Notes[i];

				if (note.Tick == tick && note.Lane == lane)
				{
					return i;
				}
			}

			return -1;
		}

		private static int FloorToMultiple(int value, int multiple)
		{
			if (multiple <= 0)
			{
				return value;
			}

			var remainder = value % multiple;

			if (remainder == 0)
			{
				return value;
			}

			if (value >= 0)
			{
				return value - remainder;
			}

			return value - remainder - multiple;
		}

		private static void DrawDiamond(Vector2 center, float size)
		{
			Handles.DrawAAConvexPolygon(
				new Vector3(center.x, center.y - size),
				new Vector3(center.x + size, center.y),
				new Vector3(center.x, center.y + size),
				new Vector3(center.x - size, center.y));
		}

		private static void StopAudioPreview()
		{
			AudioPreviewUtility.Stop();
		}

		private void RecordNoteAtCurrentTime(int lane)
		{
			if (chart == null)
			{
				return;
			}

			UpdatePlayheadTime();

			var rawTick = chart.TimeToTick(playHeadTime);
			var recordedTick = SnapTick(rawTick);

			recordedTick = Mathf.Max(0, recordedTick);

			if (HasNote(recordedTick, lane))
			{
				return;
			}

			Undo.RecordObject(chart, "Record Rhythm Note");

			chart.Notes.Add(new RhythmNote
			{
				Tick = recordedTick,
				Lane = lane,
			});

			SortNotes();

			selectedNoteIndex = FindNoteIndex(recordedTick, lane);

			EditorUtility.SetDirty(chart);
			RefreshPreview();
			Repaint();
		}

		private void RefreshPreview()
		{
			if (!isPreviewEnabled || previewPlayer == null)
			{
				return;
			}

			previewPlayer.CreateEditorPreview(chart);
			previewPlayer.UpdateEditorPreview(playHeadTime);
		}

		private bool HasNote(int tick, int lane)
		{
			for (var i = 0; i < chart.Notes.Count; i++)
			{
				var note = chart.Notes[i];

				if (note.Tick == tick && note.Lane == lane)
				{
					return true;
				}
			}

			return false;
		}

		private void ClearLaneKeyStates()
		{
			for (var i = 0; i < laneKeyPressed.Length; i++)
			{
				laneKeyPressed[i] = false;
			}
		}

		private void OnLostFocus()
		{
			ClearLaneKeyStates();
		}

		private void DrawRecordingIndicator(Rect timelineRect)
		{
			if (!isRecording)
			{
				return;
			}

			var indicatorRect = new Rect(
				timelineRect.x + 8f,
				timelineRect.y + 4f,
				120f,
				20f);

			var previousColor = GUI.color;
			GUI.color = new Color(1f, 0.45f, 0.45f);

			GUI.Label(
				indicatorRect,
				isPlaying ? "● Recording" : "● Record Armed",
				EditorStyles.boldLabel);

			GUI.color = previousColor;
		}

		private static Rect GetWaveformRect(Rect timelineRect)
		{
			return new Rect(
				timelineRect.x,
				timelineRect.y + TimelineHeaderHeight,
				timelineRect.width,
				WaveformHeight);
		}

		private static Rect GetWaveformLabelRect(Rect labelRect)
		{
			return new Rect(
				labelRect.x,
				labelRect.y + TimelineHeaderHeight,
				labelRect.width,
				WaveformHeight);
		}

		private void DrawWaveformLabel(Rect labelRect)
		{
			var waveformLabelRect = GetWaveformLabelRect(labelRect);

			EditorGUI.DrawRect(
				waveformLabelRect,
				new Color(0.15f, 0.15f, 0.15f));

			GUI.Label(
				waveformLabelRect,
				"Waveform",
				EditorStyles.centeredGreyMiniLabel);

			Handles.color = new Color(1f, 1f, 1f, 0.1f);

			Handles.DrawLine(
				new Vector3(waveformLabelRect.x, waveformLabelRect.yMax),
				new Vector3(waveformLabelRect.xMax, waveformLabelRect.yMax));
		}

		private void CacheWaveform()
		{
			var audioClip = chart?.AudioClip;

			if (audioClip == null)
			{
				ClearWaveformCache();
				return;
			}

			if (cachedWaveformClip == audioClip &&
			    (waveformSamples != null || waveformCacheFailed))
			{
				return;
			}

			ClearWaveformCache();

			cachedWaveformClip = audioClip;

			var assetPath = AssetDatabase.GetAssetPath(audioClip);
			var audioImporter = AssetImporter.GetAtPath(assetPath) as AudioImporter;

			if (audioImporter != null)
			{
				var sampleSettings = audioImporter.defaultSampleSettings;

				if (sampleSettings.loadType != AudioClipLoadType.DecompressOnLoad)
				{
					waveformCacheFailed = true;
					waveformErrorMessage = "AudioClip의 Load Type을 Decompress On Load로 설정해 주세요.";
					return;
				}
			}

			if (audioClip.loadState == AudioDataLoadState.Unloaded)
			{
				audioClip.LoadAudioData();
			}

			if (audioClip.loadState != AudioDataLoadState.Loaded)
			{
				waveformCacheFailed = true;
				waveformErrorMessage = "AudioClip 데이터가 아직 로드되지 않았습니다.";
				return;
			}

			var frameCount = audioClip.samples;

			if (frameCount <= 0)
			{
				waveformCacheFailed = true;
				waveformErrorMessage = "AudioClip의 샘플 수가 0입니다.";
				return;
			}

			var sampleCount = audioClip.samples;
			var samples = new float[sampleCount];

			if (!audioClip.GetData(samples, 0))
			{
				waveformCacheFailed = true;
				waveformErrorMessage = "AudioClip.GetData()로 샘플을 읽지 못했습니다.";
				return;
			}

			waveformSamples = samples;
			waveformFrameCount = sampleCount;
			waveformChannels = 1;

			waveformCacheFailed = false;
			waveformErrorMessage = null;
		}

		private void ClearWaveformCache()
		{
			cachedWaveformClip = null;
			waveformSamples = null;

			waveformFrameCount = 0;
			waveformChannels = 0;

			waveformCacheFailed = false;
			waveformErrorMessage = null;
		}

		private void DrawWaveform(Rect timelineRect)
		{
			var waveformRect = GetWaveformRect(timelineRect);

			EditorGUI.DrawRect(
				waveformRect,
				new Color(0.075f, 0.075f, 0.075f));

			if (chart.AudioClip == null)
			{
				DrawWaveformMessage(waveformRect, "AudioClip이 없습니다.");
				return;
			}

			CacheWaveform();

			if (waveformCacheFailed)
			{
				DrawWaveformMessage(
					waveformRect,
					waveformErrorMessage ?? "파형을 생성하지 못했습니다.");

				return;
			}

			if (waveformSamples == null ||
			    waveformFrameCount <= 0 ||
			    waveformChannels <= 0)
			{
				DrawWaveformMessage(waveformRect, "파형 데이터를 불러오는 중입니다.");
				return;
			}

			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			DrawCachedWaveform(waveformRect, timelineRect);
		}

		private void DrawCachedWaveform(
			Rect waveformRect,
			Rect timelineRect)
		{
			var centerY = waveformRect.center.y;
			var halfHeight = waveformRect.height * 0.42f;

			var firstPixel = Mathf.FloorToInt(waveformRect.x);
			var lastPixel = Mathf.CeilToInt(waveformRect.xMax);

			var previousColor = Handles.color;
			Handles.color = Color.green;

			for (var pixelX = firstPixel; pixelX < lastPixel; pixelX++)
			{
				var startTime = XToTime(pixelX, timelineRect);
				var endTime = XToTime(pixelX + 1f, timelineRect);

				if (endTime < 0.0 ||
				    startTime > chart.AudioClip.length)
				{
					continue;
				}

				var startFrame = TimeToWaveformFrame(startTime);
				var endFrame = TimeToWaveformFrame(endTime);

				if (endFrame <= startFrame)
				{
					endFrame = startFrame + 1;
				}

				startFrame = Mathf.Clamp(
					startFrame,
					0,
					waveformFrameCount - 1);

				endFrame = Mathf.Clamp(
					endFrame,
					startFrame + 1,
					waveformFrameCount);

				GetWaveformMinMax(
					startFrame,
					endFrame,
					out var minValue,
					out var maxValue);
				var scaledMinValue = Mathf.Clamp(
					minValue * WaveformAmplitudeScale,
					-1f,
					1f);

				var scaledMaxValue = Mathf.Clamp(
					maxValue * WaveformAmplitudeScale,
					-1f,
					1f);

				var upperY = centerY - scaledMaxValue * halfHeight;
				var lowerY = centerY - scaledMinValue * halfHeight;

				Handles.DrawLine(
					new Vector3(pixelX, upperY),
					new Vector3(pixelX, lowerY));
			}

			Handles.color = Color.white;

			Handles.DrawLine(
				new Vector3(waveformRect.x, centerY),
				new Vector3(waveformRect.xMax, centerY));

			Handles.color = new Color(1f, 1f, 1f, 0.1f);

			Handles.DrawLine(
				new Vector3(waveformRect.x, waveformRect.yMax),
				new Vector3(waveformRect.xMax, waveformRect.yMax));

			Handles.color = previousColor;
		}

		private static void DrawWaveformMessage(Rect waveformRect, string message)
		{
			GUI.Label(
				waveformRect,
				message,
				EditorStyles.centeredGreyMiniLabel);
		}

		private int TimeToWaveformFrame(double time)
		{
			if (chart?.AudioClip == null ||
			    waveformFrameCount <= 0)
			{
				return 0;
			}

			var clampedTime = Math.Clamp(
				time,
				0.0,
				chart.AudioClip.length);

			var normalizedTime = clampedTime / chart.AudioClip.length;

			return Mathf.Clamp(
				Mathf.FloorToInt(
					(float)(normalizedTime * waveformFrameCount)),
				0,
				waveformFrameCount - 1);
		}

		private void GetWaveformMinMax(
			int startFrame,
			int endFrame,
			out float minValue,
			out float maxValue)
		{
			minValue = 1f;
			maxValue = -1f;

			startFrame = Mathf.Clamp(
				startFrame,
				0,
				waveformFrameCount - 1);

			endFrame = Mathf.Clamp(
				endFrame,
				startFrame + 1,
				waveformFrameCount);

			for (var frame = startFrame;
			     frame < endFrame;
			     frame++)
			{
				var channelTotal = 0f;

				for (var channel = 0;
				     channel < waveformChannels;
				     channel++)
				{
					var sampleIndex =
						frame * waveformChannels + channel;

					channelTotal += waveformSamples[sampleIndex];
				}

				var value = channelTotal / waveformChannels;

				minValue = Mathf.Min(minValue, value);
				maxValue = Mathf.Max(maxValue, value);
			}

			if (minValue <= maxValue)
			{
				return;
			}

			minValue = 0f;
			maxValue = 0f;
		}

		private void AddNoteAtPlayHead(int lane)
		{
			if (chart == null)
			{
				return;
			}

			if (lane < 0 || lane >= 4)
			{
				return;
			}

			if (isPlaying)
			{
				UpdatePlayheadTime();
			}

			var rawTick = chart.TimeToTick(playHeadTime);
			var tick = Mathf.Max(0, SnapTick(rawTick));

			if (HasNote(tick, lane))
			{
				selectedNoteIndex = FindNoteIndex(tick, lane);
				Repaint();
				return;
			}

			Undo.RecordObject(chart, "Create Rhythm Note");

			chart.Notes.Add(new RhythmNote
			{
				Tick = tick,
				Lane = lane,
			});

			SortNotes();

			selectedNoteIndex = FindNoteIndex(tick, lane);

			EditorUtility.SetDirty(chart);
			RefreshPreview();
			Repaint();
		}
	}

	public enum SnapDivision
	{
		Quarter = 4,
		Eighth = 8,
		Twelfth = 12,
		Sixteenth = 16,
		TwentyFourth = 24,
		ThirtySecond = 32,
		FortyEighth = 48,
		SixtyFourth = 64,
	}
}
