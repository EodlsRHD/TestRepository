using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MinecraftCam
{
    public class ChunkMaker : MonoBehaviour
    {
        private GridMaker _gridMaker = null;

        private int _worldSize = 0;

        private int _chunkSize = 0;

        private int _chunkGenerationRange = 0;

        private List<Chunk> _chunkList = new List<Chunk>();

        private List<Chunk> activeChunkList = new List<Chunk>();

        private Action<Chunk> _onCallbackDrawBlock = null;

        private Action<List<Chunk>> _onCallbackMakeBlock = null;

        public void Initialized(int worldSize, int chunkSize, int chunkGenerationRange, Action<Chunk> onCallbackDrawBlock, Action<List<Chunk>> onCallbackMakeBlock)
        {
            _worldSize = worldSize;
            _chunkSize = chunkSize;
            _chunkGenerationRange = chunkGenerationRange;
            _onCallbackDrawBlock = onCallbackDrawBlock;
            _onCallbackMakeBlock = onCallbackMakeBlock;
        }

        public void ClassificationNode(List<Node> nodeList)
        {
            Debug.Log("Make Chunk Start");

            StartCoroutine(ClassificationNodeCo(nodeList));
        }

        IEnumerator ClassificationNodeCo(List<Node> nodeList)
        {
            int Number = 0;

            for(int L = 0; L < nodeList.Count;)
            {
                for (int W = L; W < L + _worldSize;)
                {
                    Chunk chunk = new Chunk(Number, new Vector3(nodeList[W].GetNodeCoordinate.x + (_chunkSize * 0.5f), 0f, nodeList[W].GetNodeCoordinate.z + (_chunkSize * 0.5f)), (_worldSize / _chunkSize));

                    for (int n = W; n < W + _chunkSize; n++)
                    {
                        for (int m = n; m < n + (_worldSize * _chunkSize);)
                        {
                            chunk.SetNode(nodeList[m]);

                            m += _worldSize;
                        }
                    }
                    _chunkList.Add(chunk);

                    Number++;
                    W += _chunkSize;
                    yield return null;
                }
                L += _worldSize * _chunkSize;
            }

            for (int i = 0; i < _chunkList.Count; i++)
            {
                _chunkList[i].HeightDetermination(_chunkList);
            }

            _onCallbackDrawBlock?.Invoke(_chunkList[(int)UnityEngine.Random.Range((float)(_chunkList.Count * 0.5) - 30, (float)(_chunkList.Count * 0.5) + 30)]);
        }

        public void ChunksAroundPlayer(Chunk chunksAroundPlayer)
        {
            activeChunkList.Clear();

            Debug.Log("ChunksAroundPlayer Start");

            ChunkChackRecursion(chunksAroundPlayer, _chunkGenerationRange);
            activeChunkList = activeChunkList.Distinct().ToList();

            _onCallbackMakeBlock(activeChunkList);
            //_onCallbackMakeBlock(_chunkList);
        }

        public void ChunksAroundPlayer(Vector3 playerPos)
        {
            int W = (int)(playerPos.x / _chunkSize);
            int L = (int)(playerPos.z / _chunkSize);

            Debug.Log("  W   : " + W + "  L   : " + L + "  ChunkNumbrt   : " + (L * _worldSize + W));
        }

        private void ChunkChackRecursion(Chunk chunk, int count)
        {
            if(count == -1)
            {
                return;
            }

            count--;
            activeChunkList.Add(chunk);

            for (int i = 0; i < chunk.ExistenceSideChunkNumList.Count; i++)
            {
                if(chunk.ExistenceSideChunkNumList[i] != -1)
                {
                    ChunkChackRecursion(_chunkList[chunk.ExistenceSideChunkNumList[i]], count);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if(_chunkList.Count > 0)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _chunkList.Count; i++)
                {
                    Gizmos.DrawWireCube(_chunkList[i].GetChunkCoordinate, new Vector3(_chunkSize, 2f, _chunkSize));
                }
            }


            if (activeChunkList.Count > 0)
            {
                for (int i = 0; i < activeChunkList.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(activeChunkList[i].GetChunkCoordinate, new Vector3(_chunkSize, 0.4f, _chunkSize));
                }
            }
        }
    }

    public enum eSideChunk
    {
        up,
        down,
        right,
        left
    }

    public class Chunk
    {
        private int _number = 0;

        private int _upChunkNum = 0;

        private int _downChunkNum = 0;

        private int _rightChunkNum = 0;

        private int _leftChunkNum = 0;

        private List<int> _existenceSideChunkNumList = new List<int>();

        private Vector3 _chunkCenter = Vector3.zero;

        private List<Node> _nodeList = new List<Node>();
        public int Number
        {
            get
            {
                return _number;
            }
        }
        public List<int> ExistenceSideChunkNumList
        {
            get
            {
                return _existenceSideChunkNumList;
            }
        }
        public Vector3 GetChunkCoordinate
        {
            get
            {
                return _chunkCenter;
            }
        }
        public List<Node> NodeList
        {
            get
            {
                return _nodeList;
            }
        }

        public Chunk(int number, Vector3 chunkCenter, int chunkSize)
        {
            _number = number;
            _chunkCenter = chunkCenter;

            _upChunkNum = _number - chunkSize;
            _downChunkNum = _number + chunkSize;
            _rightChunkNum = _number - 1;
            _leftChunkNum = _number + 1;
        }

        public void HeightDetermination(List<Chunk> chunkList)
        {
            bool up = SideChack(_upChunkNum, chunkList, true);
            bool down = SideChack(_downChunkNum, chunkList, true);
            bool right = SideChack(_rightChunkNum, chunkList, false);
            bool left = SideChack(_leftChunkNum, chunkList, false);

            if (up != true)
            {
                _upChunkNum = -1;
            }

            if (down != true)
            {
                _downChunkNum = -1;
            }

            if (right != true)
            {
                _rightChunkNum = -1;
            }

            if (left != true)
            {
                _leftChunkNum = -1;
            }

            _existenceSideChunkNumList.Add(_upChunkNum);
            _existenceSideChunkNumList.Add(_downChunkNum);
            _existenceSideChunkNumList.Add(_rightChunkNum);
            _existenceSideChunkNumList.Add(_leftChunkNum);
        }

        private bool SideChack(int chunkNum, List<Chunk> chunkList, bool isWidth)
        {
            if (-1 < chunkNum && chunkNum < chunkList.Count)
            {
                if (isWidth == true)
                {
                    if (chunkList[chunkNum].GetChunkCoordinate.x != _chunkCenter.x)
                    {
                        return false;
                    }

                    return true;
                }

                if (isWidth == false)
                {
                    if (chunkList[chunkNum].GetChunkCoordinate.z != _chunkCenter.z)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public void SetNode(Node node)
        {
            _nodeList.Add(node);
        }
    }
}
