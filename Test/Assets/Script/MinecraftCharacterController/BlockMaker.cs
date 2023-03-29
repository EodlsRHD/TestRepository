using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Jobs;
using Unity.Collections;

namespace MinecraftCam
{
    public enum eBlockType
    {
        Non = -1,
        rock,
        grass,
        water,
        badrock
    }

    public class BlockMaker : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField]
        private GameObject _rock = null;

        [SerializeField]
        private GameObject _grass = null;

        [SerializeField]
        private GameObject _badrock = null;

        [SerializeField]
        private GameObject _water = null;

        [SerializeField]
        private Transform _blockParant = null;

        [SerializeField]
        private Transform _waterParant = null;

        private int _worldWidth = 0;

        private int _worldHeight = 0;

        private WaitForSeconds _waitTime = new WaitForSeconds(0.001f);

        private Action _onCallbackLoadDone = null;

        private float _worldHeightHalf
        {
            get
            {
                return _worldHeight * 0.5f;
            }
        }

        public void Initialized(int worldWidth, int worldHeight, Action onCallbackLoadDone)
        {
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
            _onCallbackLoadDone = onCallbackLoadDone;
        }

        public void MakechunkBlock(List<Chunk> chunkList)
        {
            Debug.Log("Make Block Start");

            StartCoroutine(MakechunkBlockCo(chunkList));
        }

        IEnumerator MakechunkBlockCo(List<Chunk> chunkList)
        {
            for (int i = 0; i < chunkList.Count; i++)
            {
                yield return null;
                StartCoroutine(MakechunkBlockCo_Co(chunkList[i]));
            }
        }

        IEnumerator MakechunkBlockCo_Co(Chunk chunk)
        {
            int Number = 0;
            for (int i = 0; i < chunk.NodeList.Count; i++)
            {
                for (int j = 0; j < chunk.NodeList[i].MaxHeight; j++)
                {
                    GameObject obj = null;

                    int badrockarea = UnityEngine.Random.Range(1, 3);
                    int rockarea = chunk.NodeList[i].MaxHeight - UnityEngine.Random.Range(5, 30);

                    //if (j <= badrockarea)
                    //{
                    //    obj = Instantiate(_badrock, _blockParant);
                    //}
                    //else if (j > badrockarea && j <= rockarea)
                    //{
                    //    obj = Instantiate(_rock, _blockParant);
                    //}
                    //else if (j > rockarea && j < chunk.NodeList[i].MaxHeight)
                    //{
                    //    obj = Instantiate(_grass, _blockParant);
                    //}
                    //obj.SetActive(true);

                    if(j >= chunk.NodeList[i].MaxHeight - 1) // Test Code
                    {
                        obj = Instantiate(_grass, _blockParant);
                        var tmp = obj.GetComponent<Block>();
                        tmp.Initialized(Number, new Vector3(chunk.NodeList[i].GetNodeCoordinate.x, j, chunk.NodeList[i].GetNodeCoordinate.z));
                        obj.SetActive(true);
                        Number++;
                    }

                    //var tmp = obj.GetComponent<Block>();
                    //tmp.Initialized(Number, new Vector3(chunk.NodeList[i].GetNodeCoordinate.x, j, chunk.NodeList[i].GetNodeCoordinate.z));

                    //Number++;
                    yield return _waitTime;
                }

                if (chunk.NodeList[i].MaxHeight <= _worldHeightHalf)
                {
                    for (int j = chunk.NodeList[i].MaxHeight; j < _worldHeightHalf; j++)
                    {
                        if(j >= _worldHeightHalf - 1) // Test Code
                        {
                            GameObject obj = Instantiate(_water, _waterParant);
                            obj.SetActive(true);

                            var tmp = obj.GetComponent<Block>();
                            tmp.Initialized(0, new Vector3(chunk.NodeList[i].GetNodeCoordinate.x, j, chunk.NodeList[i].GetNodeCoordinate.z));
                        }
                        //GameObject obj = Instantiate(_water, _waterParant);
                        //obj.SetActive(true);

                        //var tmp = obj.GetComponent<Block>();
                        //tmp.Initialized(0, new Vector3(chunk.NodeList[i].GetNodeCoordinate.x, j, chunk.NodeList[i].GetNodeCoordinate.z));

                        yield return _waitTime;
                    }
                }
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            
        }
    }
}
