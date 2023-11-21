using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate
{

    [CreateAssetMenu()]
    public class BiomeSettings : UpdatableData
    {
        private const int _textureSize = 512;
        private const TextureFormat _textureFormat = TextureFormat.RGB565;

        [System.Serializable]
        public class BiomeLayer
        {
            public Texture2D texture;

            [Range(0, 100)]
            public int appearanceProbability;

            public Color tint;
            [Range(0, 1)]
            public float tintStrength;
            [Range(0, 1)]
            public float startHeignt;
            [Range(0, 1)]
            public float blendStrength;

            public float textureScale;
        }

        public BiomeLayer[] biomeLayers;
    }
}
