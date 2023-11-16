using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate
{
    public enum eDrawType
    {
        NoiseMap,
        MeshMap
    }

    public class MapPerview : MonoBehaviour
    {
        [SerializeField]
        private GameObject _plane = null;

        [SerializeField]
        private GameObject _mesh = null;

        private Renderer _renderer = null;

        private MeshFilter _meshFilter = null;

        private MeshRenderer _meshRenderer = null;

        [Space(10)]

        [SerializeField, Range(0, MeshSettings.numSupportedLODs - 1)]
        private int _editorPreviewLOD;

        [Header("Data")]

        [SerializeField]
        private HeightMapSettings _heightMapSettings = null;

        [SerializeField]
        private MeshSettings _meshSettings = null;

        [SerializeField]
        private TextureSettings _textureSettings = null;

        [SerializeField]
        private Material _terrainMaterial;

        [Space(10)]

        public bool autoUpdate;

        public HeightMapSettings heigntMapSettings
        {
            get { return _heightMapSettings; }
        }

        public MeshSettings meshSettings
        {
            get { return _meshSettings; }
        }

        [SerializeField, Header("Map Color")]
        private eDrawType _drawType;

        public void Initialize()
        {
            if(_plane.TryGetComponent(out Renderer renderer) == true)
            {
                _renderer = renderer;
            }

            if (_mesh.TryGetComponent(out MeshFilter meshFilter) == true)
            {
                _meshFilter = meshFilter;
            }

            if (_mesh.TryGetComponent(out MeshRenderer meshRenderer) == true)
            {
                _meshRenderer = meshRenderer;
            }
        }

        public void DrawTexture(Texture2D texture)
        {
            _renderer.sharedMaterial.mainTexture = texture;
            _renderer.transform.localScale = new Vector3(texture.width, 1, texture.height) * 0.1f;

            _plane.gameObject.SetActive(true);
            _mesh.gameObject.SetActive(false);
        }

        public void DrawMesh(MeshData meshData)
        {
            Mesh mesh = meshData.CreateMesh();

            if (mesh == null)
            {
                return;
            }

            _meshFilter.sharedMesh = mesh;

            _plane.gameObject.SetActive(false);
            _mesh.gameObject.SetActive(true);
        }

        private void OnValuesUpdated()
        {
            if (Application.isPlaying == false)
            {
                DrawMapInEditor();
            }
        }

        private void OnTextureValiesUpdated()
        {
            _textureSettings.ApplyMaterial(_terrainMaterial);
        }

        public void DrawMapInEditor()
        {
            _textureSettings.ApplyMaterial(_terrainMaterial);

            _textureSettings.UpdateMeshHeight(_terrainMaterial, _heightMapSettings.minHeight, _heightMapSettings.maxHeight);
            HeightMap mapData = HeightMapGenerator.GenerateHeightMap(_meshSettings.numVertsPerLine, _meshSettings.numVertsPerLine, _heightMapSettings, Vector2.zero);

            switch (_drawType)
            {
                case eDrawType.NoiseMap:
                    {
                        Texture2D textute = TextureGenerator.TextureFromNoiseMap(mapData);

                        DrawTexture(textute);
                    }
                    break;

                case eDrawType.MeshMap:
                    {
                        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, _meshSettings, _editorPreviewLOD);

                        DrawMesh(meshData);
                    }
                    break;
            }
        }

        private void OnValidate()
        {
            if (_meshSettings != null)
            {
                _meshSettings.onValuesUpdated -= OnValuesUpdated;
                _meshSettings.onValuesUpdated += OnValuesUpdated;
            }

            if (_heightMapSettings != null)
            {
                _heightMapSettings.onValuesUpdated -= OnValuesUpdated;
                _heightMapSettings.onValuesUpdated += OnValuesUpdated;
            }

            if (_textureSettings != null)
            {
                _textureSettings.onValuesUpdated -= OnTextureValiesUpdated;
                _textureSettings.onValuesUpdated += OnTextureValiesUpdated;
            }
        }
    }
}
