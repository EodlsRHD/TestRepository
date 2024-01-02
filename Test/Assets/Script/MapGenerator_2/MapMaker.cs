using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MapGanerate_2
{
    public class MapMaker : MonoBehaviour
    {
        private MapMaker _instance = null;

        [SerializeField]
        private NoiseMaker _noiseMaker = null;

        private MapData _data = null;

        private bool isStart = false;

        #region GetSet

        public MapMaker instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapMaker();
                }

                return _instance;
            }
        }

        #endregion

        private void Awake()
        {
            _instance = this;
        }

        public void Generate(MapData data)
        {
            _data = data;
            isStart = true;
        }

        public void Update()
        {
            if(isStart == false)
            {
                return;
            }

            _noiseMaker.Generate(_data);
        }
    }
}
