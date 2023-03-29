using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftCam
{
    public enum eBiomeType
    {
        Non = -1,
        land,
        sea
    }
    public class BiomeMaker : MonoBehaviour
    {
        private float _biomeSize = 0;

        private int _worldSize = 0;

        private int _worldHeight = 0;

        private Action<List<Node>> onCallbackClassificationNode = null;

        public void Initialized(float biomeSize, int worldSize, int worldHeight, Action<List<Node>> _onCallbackClassificationNode)
        {
            _biomeSize = biomeSize;
            _worldSize = worldSize;
            _worldHeight = worldHeight;
            onCallbackClassificationNode = _onCallbackClassificationNode;
        }

        public void MekeBiome()
        {
            Debug.Log("Make Biome Start");

            //StartCoroutine(MakeBiomeCo());
        }

        IEnumerator MakeBiomeCo(List<Biome> biomeList)
        {
            yield return null;

            //onCallbackClassificationNode?.Invoke(nodeList);
        }

        private void OnDrawGizmos()
        {

        }
    }

    public class Biome
    {
        private float _number = 0;

        private float _x = 0;

        private float _z = 0;

        private float _upBiomeNum = 0;

        private float _downBiomeNum = 0;

        private float _rightBiomeNum = 0;

        private float _leftBiomeNum = 0;

        private eBiomeType _biome = eBiomeType.Non;

        public Vector3 GetNodeCoordinate
        {
            get
            {
                return new Vector3(_x, 0f, _z);
            }
        }

        public eBiomeType BiomeType
        {
            get
            {
                return _biome;
            }
            set
            {
                _biome = value;
            }
        }

        public Biome(float number, int x, int z, float biomeSize)
        {
            _number = number;
            _x = x;
            _z = z;

            _upBiomeNum = _number - biomeSize;
            _downBiomeNum = _number + biomeSize;
            _rightBiomeNum = _number - 1;
            _leftBiomeNum = _number + 1;
        }

        public void SetBiome(List<Biome> biomeList)
        {
            Debug.Log(_biome);

            if (_biome == eBiomeType.sea)
            {
                int count = 1;

                if (_upBiomeNum != -1 && biomeList[(int)_upBiomeNum].BiomeType == eBiomeType.land)
                {
                    count++;
                }

                if (_downBiomeNum != -1 && biomeList[(int)_downBiomeNum].BiomeType == eBiomeType.land)
                {
                    count++;
                }

                if (_rightBiomeNum != -1 && biomeList[(int)_rightBiomeNum].BiomeType == eBiomeType.land)
                {
                    count++;
                }

                if (_leftBiomeNum != -1 && biomeList[(int)_leftBiomeNum].BiomeType == eBiomeType.land)
                {
                    count++;
                }

                if (count == 0)
                {
                    return;
                }

                int random = UnityEngine.Random.Range(0, count);
                if (random == 0)
                {
                    return;
                }

                _biome = eBiomeType.land;
            }
        }
    }
}
