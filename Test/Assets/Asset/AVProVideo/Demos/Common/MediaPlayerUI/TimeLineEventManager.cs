using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RenderHeads.Media.AVProVideo.Demos
{
    public class TimeLineEventManager : MonoBehaviour
    {
        [System.Serializable]
        public class Event
        {
            [SerializeField]
            [Range(0, 1)]
            public float zeroToOne = 0f;

            private bool _isUse = false;

            public bool isUse
            {
                get { return _isUse; }
                set { _isUse = value; }
            }

            public void EventLog()
            {
                Debug.Log("Time is " + zeroToOne + "   Event Active");

                _isUse = true;
            }
        }
        [SerializeField] 
        Image _timelineEventMaker = null;

        [SerializeField] 
        Transform _timelineEventMakerParent = null;

        [SerializeField]
        private List<Event> _events = new List<Event>();

        private Slider _sliderTime = null;

        public List<Event> events
        {
            get { return _events; }
        }

        public void Initialize(Slider sliderTime)
        {
            _sliderTime = sliderTime;

            Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this._sliderTime.GetComponent<RectTransform>());
            float _size = bounds.size.x;

            foreach (var @event in _events)
            {
                Image maker = Instantiate(_timelineEventMaker, _timelineEventMakerParent);

                float x = @event.zeroToOne * _size;
                maker.rectTransform.localPosition += new Vector3(x, 0f, 0f);

                maker.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (_events.Count > 0)
            {
                ChackEvent(_sliderTime.value);
            }
        }

        private void ChackEvent(float value)
        {
            foreach (var @event in _events)
            {
                if ((@event.zeroToOne < value && @event.zeroToOne + 0.01f > value) == false)
                {
                    @event.isUse = false;
                    continue;
                }

                if (@event.isUse == true)
                {
                    continue;
                }

                @event.EventLog();

                break;
            }
        }
    }
}