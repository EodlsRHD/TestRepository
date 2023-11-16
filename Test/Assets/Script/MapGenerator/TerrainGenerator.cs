using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGanerate
{
    [System.Serializable]
    public struct LODInfo
    {
        [Range(0, MeshSettings.numSupportedLODs)]
        public int lod;

        public float visibleDestanceThreshole;

        public float sqrVisibleDstThreshold
        {
            get { return visibleDestanceThreshole * visibleDestanceThreshole; }
        }
    }

    public class LODMesh
    {
        public Mesh mesh;

        public bool hasRequestedMesh = false;

        public bool hasMesh = false;

        private int _lod;

        public event System.Action updateCallback;

        public LODMesh(int lod)
        {
            this._lod = lod;
        }

        private void OnMeshDataReceived(object meshData)
        {
            MeshData data = (MeshData)meshData;
            mesh = data.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(HeightMap heightData, MeshSettings meshSettings)
        {
            hasRequestedMesh = true;

            ThreadDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightData.noiseMap, meshSettings, _lod), OnMeshDataReceived);
        }
    }

    public class TerrainGenerator : MonoBehaviour
    {
        private const float _viewerMoveThresholdForChunkUpdate = 25f;
        private const float _sqrViewerMoveThresholdForChunkUpdate = _viewerMoveThresholdForChunkUpdate * _viewerMoveThresholdForChunkUpdate;

        public Material mapMaterial = null;

        public MeshSettings _meshSettings;
        public HeightMapSettings _heightMapSettings;
        public TextureSettings _textureSettings;

        [Space(10)]

        [SerializeField]
        private int colliderLODIndex;

        [SerializeField]
        private LODInfo[] _detailLevels;

        [SerializeField]
        public Transform _viewer = null;

        [HideInInspector]
        public Vector2 viewerPosition;
        private Vector2 viewerPositionOld;

        private float _meshWorldSize;
        private int _chunkVisibleInViewDestance;

        private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private static List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

        private void Start()
        {
            _textureSettings.ApplyMaterial(mapMaterial);
            _textureSettings.UpdateMeshHeight(mapMaterial, _heightMapSettings.minHeight, _heightMapSettings.maxHeight);

            float maxViewDestance = _detailLevels[_detailLevels.Length - 1].visibleDestanceThreshole;

            _meshWorldSize = _meshSettings.meshWorldSize;
            _chunkVisibleInViewDestance = Mathf.RoundToInt(maxViewDestance / _meshWorldSize);

            UpdateVisibleChunkSize();
        }

        private void Update()
        {
            viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

            if(viewerPosition != viewerPositionOld)
            {
                foreach (var chunk in _visibleTerrainChunks)
                {
                    chunk.UpdateCollitionMesh();
                }
            }

            if((viewerPositionOld - viewerPosition).sqrMagnitude > _sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;

                UpdateVisibleChunkSize();
            }
        }

        private void UpdateVisibleChunkSize()
        {
            HashSet<Vector2> alreadyUpdatedchunkCoords = new HashSet<Vector2>();

            for (int i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedchunkCoords.Add(_visibleTerrainChunks[i].coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            int currntChunkCoordX = Mathf.RoundToInt(viewerPosition.x / _meshWorldSize);
            int currntChunkCoordY = Mathf.RoundToInt(viewerPosition.y / _meshWorldSize);

            for (int yOffset =  -_chunkVisibleInViewDestance; yOffset <= _chunkVisibleInViewDestance; yOffset++)
            {
                for (int xOffset = -_chunkVisibleInViewDestance; xOffset <= _chunkVisibleInViewDestance; xOffset++)
                {
                    Vector2 viewerChunkCoord = new Vector2(currntChunkCoordX + xOffset, currntChunkCoordY + yOffset);

                    if(alreadyUpdatedchunkCoords.Contains(viewerChunkCoord) == true)
                    {
                        continue;
                    }

                    if (_terrainChunkDictionary.ContainsKey(viewerChunkCoord) == true)
                    {
                        _terrainChunkDictionary[viewerChunkCoord].UpdateTerrainChunk();

                        continue;
                    }

                    TerrainChunk chunk = new TerrainChunk(OnTerrainChunkViviblityChange, viewerChunkCoord, _heightMapSettings, _meshSettings, _detailLevels, colliderLODIndex, this.transform, _viewer, mapMaterial);

                    _terrainChunkDictionary.Add(viewerChunkCoord, chunk);

                    chunk.Load();
                }
            }
        }

        private void OnTerrainChunkViviblityChange(TerrainChunk chunk, bool visible)
        {
            if(visible == true)
            {
                _visibleTerrainChunks.Add(chunk);

                return;
            }

            _visibleTerrainChunks.Remove(chunk);
        }
    }
}