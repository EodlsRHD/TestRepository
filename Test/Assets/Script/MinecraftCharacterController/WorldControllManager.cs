using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftCam
{
    public class WorldControllManager : MonoBehaviour
    {
        private static WorldControllManager _instance;
        public static WorldControllManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new WorldControllManager();
                }

                return _instance;
            }
        }

        [SerializeField]
        private MinecraftCam _player = null;

        [SerializeField]
        private GameObject _loading = null;

        [SerializeField]
        private int _worldSize = 0;

        [SerializeField]
        private int _worldHeight = 0;

        [SerializeField]
        private int _chunkSize = 0;

        [SerializeField]
        private int _chunkGenerationRange = 0;

        [SerializeField]
        private float _biomeSize = 0;

        //[Header("Camera")]
        //[SerializeField]
        //private Camera _camera = null;

        //[SerializeField]
        //private float _fOV = 60f;

        //[SerializeField]
        //private float _radius = 10f;

        [SerializeField]
        private GridMaker _gridMaker = null;

        [SerializeField]
        private BlockMaker _blockMaker = null;

        [SerializeField]
        private ChunkMaker _chunkMaker = null;

        [SerializeField]
        private BiomeMaker _biomeMaker = null;

        [SerializeField]
        private List<GameObject> _worldWall = new List<GameObject>();

        private Chunk _chunksAroundPlayer = null;

        void Start()
        {
            _instance = this;
            //_camera.fieldOfView = _fOV;

            _gridMaker.Initialized(_worldSize, _worldHeight);
            _biomeMaker.Initialized(_biomeSize, _worldSize, _worldHeight, _chunkMaker.ClassificationNode);
            _chunkMaker.Initialized(_worldSize, _chunkSize, _chunkGenerationRange, DrawBlock, _blockMaker.MakechunkBlock);
            _blockMaker.Initialized(_worldSize, _worldHeight, LoadDone);

            //_biomeMaker.MekeBiome();
            _gridMaker.MakeGrid();
        }

        private void DrawBlock(Chunk chunksAroundPlayer)
        {
            _chunksAroundPlayer = chunksAroundPlayer;
            //Debug.Log("  chunksAroundPlayer.Number   : " + chunksAroundPlayer.Number + "     " + chunksAroundPlayer.GetChunkCoordinate);
            _chunkMaker.ChunksAroundPlayer(_chunksAroundPlayer);
        }

        public void LoadDone()
        {
            Debug.Log("LOAD DONE");
            _player.transform.position = new Vector3(_chunksAroundPlayer.GetChunkCoordinate.x, 200f, _chunksAroundPlayer.GetChunkCoordinate.z);
            _loading.SetActive(false);
        }

        private void Update()
        {
            //_chunkManager.ChunksAroundPlayer(_player.transform.position);
        }

        //private void FixedUpdate()
        //{
        //    Transform playerPos = _player.gameObject.transform;
        //    Collider[] targetsInViewRadius = Physics.OverlapSphere(_player.transform.position, _radius);
        //    Debug.Log(targetsInViewRadius.Length);

        //    for (int i = 0; i < targetsInViewRadius.Length; i++)
        //    {
        //        Transform target = targetsInViewRadius[i].gameObject.transform;
        //        Vector3 dirToTarget = target.position - playerPos.position;
        //        float angle = Vector3.Angle(playerPos.forward, dirToTarget);

        //        if (angle < _fOV * 0.5f)
        //        {
        //            float disToTarget = Vector3.Distance(playerPos.position, target.position);

        //            RaycastHit hit;
        //            if (Physics.Raycast(playerPos.position, target.position, out hit, disToTarget))
        //            {
        //                hit.collider.gameObject.SetActive(true);
        //            }
        //        }
        //    }
        //}
    }
}
