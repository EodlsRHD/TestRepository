using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MinecraftCam
{
    public class GridMaker : MonoBehaviour
    {
        private int _worldSize = 0;

        private int _worldHeight = 0;

        private List<Node> _nodeList = new List<Node>();

        public List<Node> NodeList
        {
            get
            {
                return _nodeList;
            }
        }

        public void Initialized(int worldSize, int worldHeight)
        {
            _worldSize = worldSize;
            _worldHeight = worldHeight;
        }

        public void MakeGrid()
        {
            Debug.Log("Make Grid Start");

            int Number = 0;

            for (int L = 0; L < _worldSize; L++)
            {
                for (int W = 0; W < _worldSize; W++)
                {
                    Node node = new Node(Number, W, L, _worldSize);
                    _nodeList.Add(node);
                    Number++;
                }
            }

            StartCoroutine(MakeTerrainHeightMap());
        }

        IEnumerator MakeTerrainHeightMap()
        {
            for (int i = 0; i < _worldSize; i++)
            {
                for(int j = 0; j < _worldSize; j++)
                {
                    int count = (i * _worldSize) + j;

                    _nodeList[count].SearchNearbyAndHeightDetermination(_nodeList);
                }
                yield return null;
            }

            //int firstHeight = UnityEngine.Random.Range((_worldHeightHalf) - 10, (_worldHeightHalf) + 10);

            //for (int i = 0; i < _worldSize; i++)
            //{
            //    for (int j = 0; j < _worldSize; j++)
            //    {
            //        int count = (i * _worldSize) + j;

            //        if (i == 0 && j == 0)
            //        {
            //            _nodeList[count].MaxHeight = firstHeight;
            //        }
            //        else
            //        {
            //            _nodeList[count].SetMaxHeight(_nodeList);
            //        }
            //    }
            //    yield return null;
            //}

            //_onCallbackMakeBiome?.Invoke(_nodeList);
        }

        private void OnDrawGizmos()
        {
            if (_nodeList.Count > 0)
            {
                for (int i = 0; i < _nodeList.Count; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(_nodeList[i].GetNodeCoordinate, Vector3.one);

                    if (_nodeList[i].Biome == eBiomeType.land)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawCube(_nodeList[i].GetNodeCoordinate, Vector3.one);
                    }
                }
            }
        }
    }

    public class Node
    {
        private int _number = 0;

        private float _x = 0f;

        private float _z = 0f;

        private int _maxHeight = -1;

        private short _contourLine = 0; // -1 down , 0 maintenance , 1 up

        private int _upNodeNum = 0;

        private int _downNodeNum = 0;

        private int _rightNodeNum = 0;

        private int _leftNodeNum = 0;

        private int _maxheight = 0;

        private eBiomeType _biome = eBiomeType.Non;

        public int Number
        {
            get
            {
                return _number;
            }
        }

        public Vector3 GetNodeCoordinate
        {
            get
            {
                return new Vector3(_x, 0f, _z);
            }
        }

        public int MaxHeight
        {
            get
            {
                return _maxHeight;
            }
            set
            {
                _maxHeight = value;
            }
        }

        public short ContourLine
        {
            get
            {
                return _contourLine;
            }
        }

        public eBiomeType Biome
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

        public Node(int number, int x, int z, int worldSize)
        {
            _number = number;
            _x = x + 0.5f;
            _z = z + 0.5f;

            _upNodeNum = _number - worldSize;
            _downNodeNum = _number + worldSize;
            _rightNodeNum = _number - 1;
            _leftNodeNum = _number + 1;

            _contourLine = (short)UnityEngine.Random.Range(-1, 2);
        }

        public void SearchNearbyAndHeightDetermination(List<Node> nodeList)
        {
            bool up = SideChack(_upNodeNum, nodeList, true);
            bool down = SideChack(_downNodeNum, nodeList, true);
            bool right = SideChack(_rightNodeNum, nodeList, false);
            bool left = SideChack(_leftNodeNum, nodeList, false);

            if (up != true)
            {
                _upNodeNum = -1;
            }

            if (down != true)
            {
                _downNodeNum = -1;
            }

            if (right != true)
            {
                _rightNodeNum = -1;
            }

            if (left != true)
            {
                _leftNodeNum = -1;
            }

            if (_contourLine > 1)
            {
                _contourLine = 1;
            }

            if(_contourLine < -1)
            {
                _contourLine = -1;
            }

            if (up == false && down == false && right == false && left == false)
            {
                _contourLine = 0;
            }
        }

        private bool SideChack(int nodeNum, List<Node> nodeList, bool isWidth)
        {
            if (-1 < nodeNum && nodeNum < nodeList.Count)
            {
                if(isWidth == false)
                {
                    if(nodeList[nodeNum].GetNodeCoordinate.z != _z)
                    {
                        return false;
                    }

                    _contourLine += nodeList[nodeNum].ContourLine;
                    return true;
                }
                
                if(isWidth == true)
                {
                    if(nodeList[nodeNum].GetNodeCoordinate.x != _x)
                    {
                        return false;
                    }

                    _contourLine += nodeList[nodeNum].ContourLine;
                    return true;
                }
            }

            return false;
        }

        public void SetMaxHeight(List<Node> nodeList)
        {
            int count = 0;
            count += ChackMaxHeight(_upNodeNum, nodeList);
            count += ChackMaxHeight(_downNodeNum, nodeList);
            count += ChackMaxHeight(_rightNodeNum, nodeList);
            count += ChackMaxHeight(_leftNodeNum, nodeList);

            if(count == 0)
            {
                _maxHeight = _contourLine + _maxheight;
                return;
            }

            _maxHeight = _contourLine + _maxheight / count;

            if(_maxheight < 1)
            {
                _maxheight = 1;
            }
        }

        private int ChackMaxHeight(int nodeNum, List<Node> nodeList)
        {
            if (-1 < nodeNum && nodeNum < nodeList.Count)
            {
                if (nodeList[nodeNum].MaxHeight != -1)
                {
                    _maxheight += nodeList[nodeNum].MaxHeight;
                    return 1;
                }
            }
            return 0;
        }
    }
}
