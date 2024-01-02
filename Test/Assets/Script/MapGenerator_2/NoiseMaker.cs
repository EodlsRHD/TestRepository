using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate_2
{
    public class NoiseMaker : MonoBehaviour
    {
        public void Generate(MapData data)
        {
            float[,] noiseMap = PerlinNoise(data);
        }

        private float[,] PerlinNoise(MapData data)
        {
            int worldSize = data.worldSize;
            float maxPossobleHeight = 0;
            float amplitude = 1; // ÁøÆø
            float frequancy = 1; // ºóµµ(Áøµ¿)

            float[,] noiseMap = new float[worldSize, worldSize];

            Vector2[] octavesOffset = new Vector2[data.octaves];

            System.Random ranSeed = new System.Random(data.seed);

            for (int i = 0; i < data.octaves; i++)
            {
                float offsetX = ranSeed.Next(-100000, 100000) + data.offSet.x;
                float offsetY = ranSeed.Next(-100000, 100000) + data.offSet.y;

                octavesOffset[i] = new Vector2(offsetX, offsetY);

                maxPossobleHeight += amplitude;
                amplitude *= data.persistance;
            }

            if (worldSize <= 0)
            {
                data.worldSize = 1;
                worldSize = 1;
            }

            float maxLocalNormalizeNoiseHeight = float.MinValue;
            float minLocalNornalizeNoiseHeight = float.MaxValue;

            float halfWidth = worldSize * 0.5f;
            float halfHeight = worldSize * 0.5f;

            for (int y = 0; y < worldSize; y++)
            {
                for (int x = 0; x < worldSize; x++)
                {
                    float perlins = Mathf.PerlinNoise(x, y);


                }
            }

            return noiseMap;
        }
    }
}
