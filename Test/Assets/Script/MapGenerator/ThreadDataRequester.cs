using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace MapGanerate
{
    public class ThreadDataRequester : MonoBehaviour
    {
        static ThreadDataRequester _instance;

        private struct ThreadInfo
        {
            public readonly Action<object> callback;
            public readonly object parameter;

            public ThreadInfo(Action<object> _callback, object _parameter)
            {
                this.callback = _callback;
                this.parameter = _parameter;
            }
        }

        private Queue<ThreadInfo> _mapDataThreadInfoQueue = new Queue<ThreadInfo>();

        private void Awake()
        {
            _instance = FindObjectOfType<ThreadDataRequester>();
        }

        public static void RequestData(Func<object> generateData, Action<object> onCallback)
        {
            ThreadStart threadStart = delegate
            {
                _instance.DataThread(generateData, onCallback);
            };

            new Thread(threadStart).Start();
        }

        private void DataThread(Func<object> generateData, Action<object> onCallback)
        {
            object data = generateData();
            lock (_mapDataThreadInfoQueue)
            {
                _mapDataThreadInfoQueue.Enqueue(new ThreadInfo(onCallback, data));
            }
        }

        private void Update()
        {
            if (_mapDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
                {
                    ThreadInfo info = _mapDataThreadInfoQueue.Dequeue();
                    info.callback(info.parameter);
                }
            }
        }
    }
}
