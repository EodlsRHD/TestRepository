using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RenderHeads.Media.AVProVideo;

public class TimelineEvent : MonoBehaviour
{
    [System.Serializable]
    public class EventProperty
    {
        [SerializeField]
        private int _id = 0;

        [SerializeField]
        private float _time = 0;

        private bool _isUse = false;

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }

        public float time
        {
            get { return _time; }
            set { _time = value; }
        }

        public bool isUse
        {
            get { return _isUse; }
            set { _isUse = value; }
        }
    }

    [SerializeField]
    private MediaPlayer _mediaPlayer = null;

    [SerializeField]
    private List<EventProperty> _events = new List<EventProperty>();

    [SerializeField]
    private bool _isStart = false;

    private int _durationMin = 0;

    private int _durationSec = 0;

    private int _currentMin = 0;

    private int _currentSec = 0;

    public string durationTime
    {
        get
        {
            return _durationMin + " : " + _durationSec;
        }
    }

    public string currentTime
    {
        get
        {
            return _currentMin + " : " + _currentSec;
        }
    }

    public MediaPlayer mediaPlayer
    {
        get
        {
            return _mediaPlayer;
        }
        set
        {
            _mediaPlayer = value;
        }
    }

    public int eventsCount
    {
        get
        {
            return _events.Count;
        }
    }

    public float GetDuration
    {
        get 
        {
            return Mathf.Floor((float)_mediaPlayer.Info.GetDuration() * 10) / 10; 
        }
    }

    public float GetCurrentTime
    {
        get 
        {
            return Mathf.Floor((float)_mediaPlayer.Control.GetCurrentTime() * 10) / 10; 
        }
    }

    private System.Action<int, float, MediaPlayer> _onCallback = null;

    public void Initialize(System.Action<int, float, MediaPlayer> onCallback)
    {
        _onCallback = onCallback;
    }

    private void Update()
    {
        if(_isStart == true)
        {
            _currentMin = Mathf.FloorToInt(GetCurrentTime / 60f);
            _currentSec = Mathf.FloorToInt(GetCurrentTime - (60f * _currentMin));

            if(mediaPlayer.MediaSource == MediaSource.Reference)
            {
                for (int i = 0; i < _events.Count; i++)
                {
                    if (GetCurrentTime != _events[i].time)
                    {
                        _events[i].isUse = false;
                        continue;
                    }

                    if (_events[i].isUse == true)
                    {
                        continue;
                    }

                    _events[i].isUse = true;
                    _onCallback(_events[i].id, _events[i].time, _mediaPlayer);
                }
            }
        }
    }

    public void ChackDuration()
    {
        _durationMin = Mathf.FloorToInt(GetDuration / 60f);
        _durationSec = Mathf.FloorToInt(GetDuration - (60f * _durationMin));
    }

    public void ChackEventTime()
    {
        for (int i = _events.Count - 1; i >= 0; i--)
        {
            if (0 <= _events[i].time && _events[i].time <= GetDuration)
            {
                continue;
            }

            _events.Remove(_events[i]);
        }
    }

    public void NewEvent(float time)
    {
        EventProperty newEvent = new EventProperty();
        newEvent.id = eventsCount;
        newEvent.time = time;

        _events.Add(newEvent);
    }

    public EventProperty SelectEvent(int num)
    {
        return _events[num];
    }

    public void ListClear()
    {
        _events.Clear();
    }

    public void Play()
    {
        _isStart = true;

        _mediaPlayer.Play();
    }

    public void Stop()
    {
        _isStart = false;
         
        _mediaPlayer.Stop();
    }

    public void ResetTime()
    {
        _isStart = false;

        _mediaPlayer.Stop();
        _mediaPlayer.Control.Seek(0);
    }
}
