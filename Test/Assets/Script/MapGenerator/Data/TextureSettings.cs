using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MapGanerate
{

    [CreateAssetMenu()]
    public class TextureSettings : UpdatableData
    {
        private const int _textureSize = 512;
        private const TextureFormat _textureFormat = TextureFormat.RGB565;

        [System.Serializable]
        public class TextureLayer
        {
            public Texture2D texture;

            public Color tint;
            [Range(0, 1)]
            public float tintStrength;
            [Range(0, 1)]
            public float startHeignt;
            [Range(0, 1)]
            public float blendStrength;

            public float textureScale;
        }

        public TextureLayer[] layer;

        private float savedminHeight;
        private float savedmaxHeight;

        public void ApplyMaterial(Material mat)
        {
            mat.SetInt("layersCount", layer.Length);
            mat.SetColorArray("baseColours", layer.Select(x => x.tint).ToArray());
            mat.SetFloatArray("baseStartHeights", layer.Select(x => x.startHeignt).ToArray());
            mat.SetFloatArray("baseBlends", layer.Select(x => x.blendStrength).ToArray());
            mat.SetFloatArray("baseColourStrength", layer.Select(x => x.tintStrength).ToArray());
            mat.SetFloatArray("baseTextureScale", layer.Select(x => x.textureScale).ToArray());

            Texture2DArray baseTextures = GenerateTextureArray(layer.Select(x => x.texture).ToArray());
            mat.SetTexture("baseTextures", baseTextures);

            UpdateMeshHeight(mat, savedminHeight, savedmaxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            Texture2DArray textureArray = new Texture2DArray(_textureSize, _textureSize, textures.Length, _textureFormat, true);

            for (int i = 0; i < textures.Length; i++)
            {
                textureArray.SetPixels(textures[i].GetPixels(), i);
            }

            textureArray.Apply();
            return textureArray;
        }

        public void UpdateMeshHeight(Material mat, float minHeight, float maxHeight)
        {
            savedminHeight = minHeight;
            savedmaxHeight = maxHeight;

            mat.SetFloat("minHeight", minHeight);
            mat.SetFloat("maxHeight", maxHeight);
        }
    }
}
