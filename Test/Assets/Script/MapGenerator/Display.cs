using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate
{
    public class Display : MonoBehaviour
    {
        [SerializeField]
        private GameObject _plane = null;

        [SerializeField]
        private GameObject _mesh = null;

        private Renderer _renderer = null;

        private MeshFilter _meshFilter = null;

        private MeshRenderer _meshRenderer = null;

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
            _renderer.transform.localScale = new Vector3(texture.width, 1, texture.height);

            _plane.gameObject.SetActive(true);
            _mesh.gameObject.SetActive(false);
        }

        public void DrawMesh(MeshData meshData, Texture2D textute)
        {
            _meshFilter.sharedMesh = meshData.CreateMesh();
            _meshRenderer.sharedMaterial.mainTexture = textute;

            _plane.gameObject.SetActive(false);
            _mesh.gameObject.SetActive(true);
        }
    }
}
