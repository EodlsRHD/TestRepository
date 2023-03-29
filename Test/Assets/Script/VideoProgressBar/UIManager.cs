using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] 
    private MediaPlayer _mediaPlayer = null;

    [SerializeField]
    private TimelineEvent _timelineEventManager = null;

    [SerializeField] 
    private Slider _sliderTime = null;

    [SerializeField]
    private TMP_Text _fullTimeText = null;

    [SerializeField]
    private TMP_Text _timeRemainingText = null;

    [SerializeField]
    private Button _startButton = null;

    private TimeRange timelineRange;

    private void Awake()
    {
        _startButton.onClick.AddListener(StartVideo);
    }

    private void StartVideo()
    {
		_mediaPlayer.Play();

        //_timelineEventManager.Initialize(_sliderTime, _mediaPlayer.Info.GetDuration());
        timelineRange = GetTimelineRange();

        int min = (int)(timelineRange.duration / 60);
        int sec = (int)(timelineRange.duration - (60 * min));

        _fullTimeText.text = min + " : " + sec;
    }

	private TimeRange GetTimelineRange()
    {
        if (_mediaPlayer.Info != null)
        {
            return Helper.GetTimelineRange(_mediaPlayer.Info.GetDuration(), _mediaPlayer.Control.GetSeekableTimes());
        }
        return new TimeRange();
    }

    private void Update()
    {
        if (_timelineEventManager.eventsCount > 0)
        {
            //_timelineEventManager.ChackEvent(_mediaPlayer.Control.GetCurrentTime());
        }

        if (_sliderTime == true)
        {
            double t = 0.0;
            if (timelineRange.duration > 0.0)
            {
                t = ((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
            }
            _sliderTime.value = Mathf.Clamp01((float)t);

            int min = (int)(_mediaPlayer.Control.GetCurrentTime() / 60);
            int sec = (int)(_mediaPlayer.Control.GetCurrentTime() - (60 * min));

            _timeRemainingText.text = min + " : " + sec;
        }
    }
}
