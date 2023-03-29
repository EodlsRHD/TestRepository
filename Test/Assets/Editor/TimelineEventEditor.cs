using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using RenderHeads.Media.AVProVideo;

[CanEditMultipleObjects]
[CustomEditor(typeof(TimelineEvent))]
public class TimelineEventEditor : Editor
{
	private TimelineEvent _timelineEvent = null;

	private SerializedProperty _mediaPlayer = null;

	private SerializedProperty _events = null;

	private float _time = 0f;

	private bool _isLoadDuration = true;

	private string _currentMode = string.Empty;

	private Rect _textureRect;

	private float _textureRatio = 0f;

	private float _rectRatio = 0f;

	private static int _previewTextureFrameCount = -1;

	private Material _materialResolve;

	private Material _materialIMGUI;

	private RenderTexture _previewTexture;

	[MenuItem("GameObject/Video/Media Player", false, 103)]
	public static void CreateMediaPlayerEditor()
	{
		GameObject obj = new GameObject("MediaPlayer");
		obj.AddComponent<TimelineEvent>();
		Selection.activeGameObject = obj;
	}

	private void OnEnable()
	{
		FixRogueEditorBug();

		CreateSections();
	}

	private void FixRogueEditorBug()
	{
		var remainingBuggedEditors = FindObjectsOfType<TimelineEventEditor>();
		foreach (var editor in remainingBuggedEditors)
		{
			if (editor == this)
			{
				continue;
			}
			DestroyImmediate(editor);
		}
	}

	private void CreateSections()
    {
		_timelineEvent = (this.target) as TimelineEvent;

		_mediaPlayer = serializedObject.FindProperty("_mediaPlayer");
		_events = serializedObject.FindProperty("_events");
	}

	public override void OnInspectorGUI()
	{
		_timelineEvent.mediaPlayer = (MediaPlayer)EditorGUILayout.ObjectField("Media Player", _timelineEvent.mediaPlayer, typeof(MediaPlayer), true);

		if (_timelineEvent.mediaPlayer == null)
        {
            return;
        }

		Preview(_timelineEvent.mediaPlayer, _timelineEvent.mediaPlayer.TextureProducer);

		EditorGUILayout.Space(10);

		Timeline();

		EditorGUILayout.Space(10);

		Buttons();

		EditorGUILayout.Space(10);

		serializedObject.Update();

		Events();

		serializedObject.ApplyModifiedProperties();
	}

	private void Preview(MediaPlayer mediaPlayer, ITextureProducer textureSource)
    {
		EditorGUILayout.Space(10);

		EditorGUILayout.LabelField("Media Source  :  " + mediaPlayer.MediaSource);

		switch (mediaPlayer.MediaSource)
        {
			case MediaSource.Reference:
				EditorGUILayout.LabelField("Media Reference  :  " + mediaPlayer.MediaReference);
				break;

			case MediaSource.Path:
				EditorGUILayout.LabelField("Media Path  :  " + mediaPlayer.MediaPath.Path);
				EditorGUILayout.LabelField("Media Path Type  :  " + mediaPlayer.MediaPath.PathType);
				break;
		}

		EditorGUILayout.Space(5);

		if (textureSource == null || textureSource.GetTexture() == null)
        {
			_textureRatio = 16f / 9f;
			_rectRatio = Mathf.Max(1f, _textureRatio);
			_textureRect = GUILayoutUtility.GetAspectRect(_rectRatio, GUILayout.ExpandWidth(true), GUILayout.Height(256f));

			GUI.color = Color.gray;
			EditorGUI.DrawTextureTransparent(_textureRect, Texture2D.blackTexture, ScaleMode.StretchToFill);
			GUI.color = Color.white;

			return;
        }

		Texture texture = textureSource.GetTexture();
		if (_previewTexture)
		{
			texture = _previewTexture;
		}

		_textureRatio = (((float)texture.width / (float)texture.height) * textureSource.GetTexturePixelAspectRatio());
		_rectRatio = Mathf.Max(1f, _textureRatio);
		_textureRect = GUILayoutUtility.GetAspectRect(_rectRatio, GUILayout.ExpandWidth(true), GUILayout.Height(256f));

		GUI.color = Color.gray;
		EditorGUI.DrawTextureTransparent(_textureRect, Texture2D.blackTexture, ScaleMode.StretchToFill);
		GUI.color = Color.white;

		int textureFrameCount = mediaPlayer.TextureProducer.GetTextureFrameCount();
		if (textureFrameCount != _previewTextureFrameCount)
		{
			_previewTextureFrameCount = textureFrameCount;

			if (!_materialResolve)
			{
				_materialResolve = VideoRender.CreateResolveMaterial(false);

				VideoRender.SetupResolveMaterial(_materialResolve, VideoResolveOptions.Create());
			}

			if (!_materialIMGUI)
			{
				_materialIMGUI = VideoRender.CreateIMGUIMaterial();
			}

			VideoRender.SetupMaterialForMedia(_materialResolve, mediaPlayer, -1);
			VideoRender.ResolveFlags resolveFlags = (VideoRender.ResolveFlags.ColorspaceSRGB | VideoRender.ResolveFlags.Mipmaps | VideoRender.ResolveFlags.PackedAlpha | VideoRender.ResolveFlags.StereoLeft);

			_previewTexture = VideoRender.ResolveVideoToRenderTexture(_materialResolve, _previewTexture, mediaPlayer.TextureProducer, resolveFlags);
		}

		EditorGUI.DrawPreviewTexture(_textureRect, _previewTexture, _materialIMGUI, ScaleMode.ScaleToFit, _textureRatio);
	}

	private void Timeline()
    {
		if (_timelineEvent.mediaPlayer.Info == null)
        {
			return;
        }

		if(_isLoadDuration == false)
        {
			_timelineEvent.ChackDuration();

			if (_timelineEvent.GetDuration > 0)
			{
				_timelineEvent.ChackEventTime();
			}

			_isLoadDuration = true;
		}

		EditorGUILayout.BeginHorizontal(GUILayout.Height(EditorGUIUtility.singleLineHeight / 2f));

		Rect sliderRect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.horizontalSlider, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

		EditorGUILayout.EndHorizontal();

		float newTime = GUI.HorizontalSlider(sliderRect, _timelineEvent.GetCurrentTime, 0, _timelineEvent.GetDuration);

		if (_timelineEvent.mediaPlayer.Info == null)
		{
			return;
		}

		if (_timelineEvent.eventsCount > 0)
		{
			Rect eventRect = sliderRect;
			eventRect.yMin += sliderRect.height * 0.5f;

			for (int i = 0; i < _timelineEvent.eventsCount; i++)
			{
				float eventValue = sliderRect.xMin + ((_timelineEvent.SelectEvent(i).time / _timelineEvent.GetDuration) * sliderRect.width);

				eventRect.xMin = eventValue;
				eventRect.xMax = eventValue + 3f;

				EditorGUI.DrawRect(eventRect, Color.yellow);
			}
		}

		if (newTime != _timelineEvent.GetCurrentTime)
		{
			if (_timelineEvent.mediaPlayer.Control != null)
			{
				_timelineEvent.mediaPlayer.Control.Seek(newTime);
			}
		}

		Repaint();

		EditorGUILayout.Space(5);

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField(_timelineEvent.currentTime, GUILayout.Height(20));

		string timeTotal = "infinity";
		if (float.IsInfinity(_timelineEvent.GetDuration) == false)
        {
			timeTotal = _timelineEvent.durationTime;
		}
		EditorGUILayout.LabelField(timeTotal, GUILayout.Width(40), GUILayout.Height(20));

		EditorGUILayout.EndHorizontal();
	}

	private void Buttons()
    {
		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Play", GUILayout.Width(50), GUILayout.Height(30)))
		{
			_timelineEvent.Play();

			_isLoadDuration = false;
			_currentMode = "Play";
		}
		
		if (GUILayout.Button("Stop", GUILayout.Width(50), GUILayout.Height(30)))
		{
			_timelineEvent.Stop();

			_currentMode = "Stop";
		}

		if (GUILayout.Button("Reset", GUILayout.Width(50), GUILayout.Height(30)))
		{
			_timelineEvent.ResetTime();

			_currentMode = "Stop";
		}

		if (GUILayout.Button("Load Media", GUILayout.Width(80), GUILayout.Height(30)))
		{
			_timelineEvent.mediaPlayer.AutoStart = false;
			switch (_timelineEvent.mediaPlayer.MediaSource)
			{
				case MediaSource.Reference:
					_timelineEvent.mediaPlayer.OpenMedia(_timelineEvent.mediaPlayer.MediaReference, _timelineEvent.mediaPlayer.AutoStart);
					break;

				case MediaSource.Path:
					_timelineEvent.mediaPlayer.OpenMedia(_timelineEvent.mediaPlayer.MediaPath.PathType, _timelineEvent.mediaPlayer.MediaPath.Path, _timelineEvent.mediaPlayer.AutoStart);
					break;
			}

			_currentMode = "Stop";
		}

		EditorGUILayout.Space(5);

		EditorGUILayout.LabelField("Current mode  :  " + _currentMode, GUILayout.Height(30));

		EditorGUILayout.EndHorizontal();
	}

	private void Events()
    {
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField("Enter Event Time", GUILayout.Width(100), GUILayout.Height(20));

		_time = EditorGUILayout.FloatField(_time, GUILayout.Width(150));

		if (GUILayout.Button("Add", GUILayout.Width(60), GUILayout.Height(20)))
		{
			_timelineEvent.NewEvent(_time);

			_time = 0f;
		}

		if (GUILayout.Button("List Clear", GUILayout.Width(100), GUILayout.Height(20)))
        {
			_timelineEvent.ListClear();
		}

		EditorGUILayout.EndHorizontal();

		if (_events == null)
		{
			return;
		}

		EditorGUILayout.PropertyField(_events);
	}
}