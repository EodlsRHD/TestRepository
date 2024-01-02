using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapGanerate_2
{
    [System.Serializable]
    public class MapData
    {
        public int seed = 0;

        public int worldSize = 500;

        public int octaves = 0; // 합성되는 신호의 개수

        public float persistance = 0f; // amplitude의 변화량

        public float lacunarity = 0f; // frequancy의 변화량

        public Vector2 offSet;

        public int heightMultiplier = 0;

        public AnimationCurve heightCurve = null;
    }

    public class MapGenerator_2Manager : MonoBehaviour
    {
        [SerializeField]
        private Button _buttonGenerate = null;

        [SerializeField]
        private MapMaker _mapMaker = null;

        [SerializeField]
        private Transform _playerTR = null;

        [Header("Data")]
        private MapData _data = null;

        private void Start()
        {
            _buttonGenerate.onClick.AddListener(Generate);
        }

        private void Generate()
        {
            if(_data == null)
            {
                Debug.LogError("data is Null");
                return;
            }

            _mapMaker.Generate(_data);
        }
    }
}
