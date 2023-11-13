using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapGanerate;


namespace MapGanerate
{
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;

        public float visibleDestanceThreshole;
    }

    public class EndlessTerrain : MonoBehaviour
    {
        private const float _viewerMoveThresholdForChunkUpdate = 25f;
        private const float _sqrViewerMoveThresholdForChunkUpdate = _viewerMoveThresholdForChunkUpdate * _viewerMoveThresholdForChunkUpdate;

        private static MapGenerator _mapGenerator = null;

        public Material mapMaterial = null;

        [Space(10)]

        [SerializeField]
        private LODInfo[] _detailLevels;

        private static float _maxViewDestance;

        [SerializeField]
        private Transform _viewer = null;

        public static Vector2 viewerPosition;
        private Vector2 viewerPositionOld;

        private int _chunkSize;
        private int _chunkVisibleInViewDestance;

        private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

        private void Start()
        {
            _mapGenerator = FindObjectOfType<MapGenerator>();

            _maxViewDestance = _detailLevels[_detailLevels.Length - 1].visibleDestanceThreshole;

            _chunkSize = MapGenerator.mapChunkSize - 1;
            _chunkVisibleInViewDestance = Mathf.RoundToInt(_maxViewDestance / _chunkSize);

            UpdateVisibleChunkSize();
        }

        private void Update()
        {
            viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

            if((viewerPositionOld - viewerPosition).sqrMagnitude > _sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;

                UpdateVisibleChunkSize();
            }
        }

        private void UpdateVisibleChunkSize()
        {
            for (int i = 0; i < _terrainChunksVisibleLastUpdate.Count; i++)
            {
                _terrainChunksVisibleLastUpdate[i].SetVisible(false);
            }

            _terrainChunksVisibleLastUpdate.Clear();

            int currntChunkCoordX = Mathf.RoundToInt(viewerPosition.x / _chunkSize);
            int currntChunkCoordY = Mathf.RoundToInt(viewerPosition.y / _chunkSize);

            for (int yOffset =  -_chunkVisibleInViewDestance; yOffset <= _chunkVisibleInViewDestance; yOffset++)
            {
                for (int xOffset = -_chunkVisibleInViewDestance; xOffset <= _chunkVisibleInViewDestance; xOffset++)
                {
                    Vector2 viewerChunkCoord = new Vector2(currntChunkCoordX + xOffset, currntChunkCoordY + yOffset);

                    if (_terrainChunkDictionary.ContainsKey(viewerChunkCoord) == true)
                    {
                        _terrainChunkDictionary[viewerChunkCoord].UpdateTerrainChunk();
                        
                        if(_terrainChunkDictionary[viewerChunkCoord].isVisible() == true)
                        {
                            _terrainChunksVisibleLastUpdate.Add(_terrainChunkDictionary[viewerChunkCoord]);
                        }

                        continue;
                    }
                   
                    _terrainChunkDictionary.Add(viewerChunkCoord, new TerrainChunk(viewerChunkCoord, _detailLevels, _chunkSize, this.transform, mapMaterial));
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_viewer.position, _maxViewDestance);
        }

        public class TerrainChunk
        {
            private GameObject _meshObject = null;
            private MeshRenderer _meshRenderer = null;
            private MeshFilter _meshFilter = null;

            private Vector2 _position;
            private Bounds _bounds;

            private LODInfo[] _detailLevels;
            private LODMesh[] lodMeshes;

            private MapData _mapData;
            private bool _mapDataReceived;
            private int previousLODIndex;

            public TerrainChunk(Vector2 coord, LODInfo[] detailLevels, int size, Transform parent, Material material)
            {
                _detailLevels = detailLevels;

                _position = coord * size;
                _bounds = new Bounds(_position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

                _meshObject = new GameObject("Terrain Chunk   " + coord);

                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshFilter = _meshObject.AddComponent<MeshFilter>();

                _meshRenderer.material = material;

                _meshObject.transform.parent = parent;
                _meshObject.transform.position = positionV3;

                SetVisible(false);

                lodMeshes = new LODMesh[_detailLevels.Length];

                for (int i = 0; i < _detailLevels.Length; i++)
                {
                    lodMeshes[i] = new LODMesh(_detailLevels[i].lod, UpdateTerrainChunk);
                }

                _mapGenerator.RequestMapData(_position, OnMapDataReceived);
            }

            private void OnMapDataReceived(MapData mapData)
            {
                _mapData = mapData;

                _mapDataReceived = true;

                Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
                _meshRenderer.material.mainTexture = texture;

                UpdateTerrainChunk();
            }

            public void UpdateTerrainChunk()
            {
                if (_mapDataReceived == false)
                {
                    return;
                }

                float viewerDesFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
                bool visible = viewerDesFromNearestEdge <= _maxViewDestance;

                if(visible == true)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < _detailLevels.Length - 1; i++)
                    {
                        if(viewerDesFromNearestEdge > _detailLevels[i].visibleDestanceThreshole)
                        {
                            lodIndex = i + 1;

                            continue;
                        }

                        break;
                    }

                    if(lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];

                        if(lodMesh.hasMesh == true)
                        {
                            previousLODIndex = lodIndex;

                            _meshFilter.mesh = lodMesh.mesh;
                        }
                        else if(lodMesh.hasRequestedMesh == false)
                        {
                            lodMesh.RequestMesh(_mapData);
                        }
                    }
                }

                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }

            public bool isVisible()
            {
                return _meshObject.activeSelf;
            }
        }

        private class LODMesh
        {
            public Mesh mesh;

            public bool hasRequestedMesh;

            public bool hasMesh;

            private int _lod;

            private System.Action _updateCallback;

            public LODMesh(int lod, System.Action updateCallback)
            {
                this._lod = lod;
                this._updateCallback = updateCallback;
            }

            private void OnMeshDataReceived(MeshData meshData)
            {
                mesh = meshData.CreateMesh();

                hasMesh = true;

                _updateCallback();
            }

            public void RequestMesh(MapData mapData)
            {
                hasRequestedMesh = true;

                _mapGenerator.RequestMeshData(mapData, _lod, OnMeshDataReceived);
            }
        }
    }
}