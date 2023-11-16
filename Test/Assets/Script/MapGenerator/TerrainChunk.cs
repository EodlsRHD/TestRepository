using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate
{
    public class TerrainChunk
    {
        public Vector2 coord;

        private event System.Action<TerrainChunk, bool> _onVisibilityChanged;

        private const float _colliderGenerationDistanceThreshole = 5;

        private GameObject _meshObject = null;
        private MeshRenderer _meshRenderer = null;
        private MeshFilter _meshFilter = null;
        private MeshCollider _meshCollider = null;

        private HeightMapSettings _heightMapSettings = null;
        private MeshSettings _meshSettings = null;

        private Transform _viewerPosition = null;

        private Vector2 _simpleCenter;
        private Bounds _bounds;

        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;
        private int _colliderLODIndex;
        private int previousLODIndex = -1;
        private float _maxViewDistance;
        private bool hasSetCollider;

        private HeightMap _heigntMapData;
        private bool _heightMapReceived;

        Vector2 viewerPosition
        {
            get { return new Vector2(_viewerPosition.position.x, _viewerPosition.position.z); }
        }

        public TerrainChunk(System.Action<TerrainChunk, bool> onVisibilityChanged, Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewerPosition, Material material)
        {
            if(onVisibilityChanged != null)
            {
                _onVisibilityChanged = onVisibilityChanged;
            }

            _heightMapSettings = heightMapSettings;
            _meshSettings = meshSettings;

            _viewerPosition = viewerPosition;

            this.coord = coord;
            _detailLevels = detailLevels;
            _colliderLODIndex = colliderLODIndex;

            _simpleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
            Vector2 position = coord * meshSettings.meshWorldSize;
            _bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

            _meshObject = new GameObject("Terrain Chunk   " + coord);

            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();

            _meshRenderer.material = material;

            _meshObject.transform.parent = parent;
            _meshObject.transform.position = new Vector3(position.x, 0, position.y);

            _maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDestanceThreshole;

            SetVisible(false);

            _lodMeshes = new LODMesh[_detailLevels.Length];

            for (int i = 0; i < _detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(_detailLevels[i].lod);
                _lodMeshes[i].updateCallback += UpdateTerrainChunk;

                if (i == colliderLODIndex)
                {
                    _lodMeshes[i].updateCallback += UpdateCollitionMesh;
                }
            }
        }

        public void Load()
        {
            ThreadDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(_meshSettings.numVertsPerLine, _meshSettings.numVertsPerLine, _heightMapSettings, _simpleCenter), OnHeightMapDataReceived);
        }

        private void OnHeightMapDataReceived(object mapData)
        {
            _heigntMapData = (HeightMap)mapData;

            _heightMapReceived = true;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (_heightMapReceived == false)
            {
                return;
            }

            bool wasVivible = isVisible();

            float viewerDesFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewerPosition));
            bool visible = viewerDesFromNearestEdge <= _maxViewDistance;

            if (visible == true)
            {
                int lodIndex = 0;

                for (int i = 0; i < _detailLevels.Length - 1; i++)
                {
                    if (viewerDesFromNearestEdge > _detailLevels[i].visibleDestanceThreshole)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = _lodMeshes[lodIndex];

                    if (lodMesh.hasMesh == true)
                    {
                        previousLODIndex = lodIndex;

                        _meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (lodMesh.hasRequestedMesh == false)
                    {
                        lodMesh.RequestMesh(_heigntMapData, _meshSettings);
                    }
                }
            }

            if (wasVivible != visible)
            {
                if(_onVisibilityChanged != null)
                {
                    _onVisibilityChanged(this, visible);
                }

                SetVisible(visible);
            }
        }

        public void UpdateCollitionMesh()
        {
            if (hasSetCollider == true)
            {
                return;
            }

            float sqrDesFromViewerToEdge = _bounds.SqrDistance(viewerPosition);

            if (sqrDesFromViewerToEdge < _detailLevels[_colliderLODIndex].sqrVisibleDstThreshold)
            {
                if (_lodMeshes[_colliderLODIndex].hasRequestedMesh == false)
                {
                    _lodMeshes[_colliderLODIndex].RequestMesh(_heigntMapData, _meshSettings);
                }
            }

            if (sqrDesFromViewerToEdge < _colliderGenerationDistanceThreshole * _colliderGenerationDistanceThreshole)
            {
                if (_lodMeshes[_colliderLODIndex].hasMesh == true)
                {
                    _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
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
}
