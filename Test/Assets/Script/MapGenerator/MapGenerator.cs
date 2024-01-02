using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace MapGanerate
{
    public struct HeightMap
    {
        public readonly float[,] noiseMap;
        public readonly float minValue;
        public readonly float maxValue;

        public HeightMap(float[,] noiseMap, float minValue, float maxValue)
        {
            this.noiseMap = noiseMap;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }

    public enum eNormalizeMode
    {
        Local,
        Global
    }

    [System.Serializable]
    public class NoiseSettings
    {
        public eNormalizeMode normalizeMode;

        public float scale = 50;

        public int octaves = 6;

        [Range(0, 6)]
        public float persistance = 0.6f;

        public float lacunarity = 2f;

        [Space(10f)]

        public int seed;

        public Vector2 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            persistance = Mathf.Clamp01(persistance);
        }
    }

    public class MapGenerator : MonoBehaviour
    {
        [SerializeField]
        private MapPerview _perview = null;

        public void Initialize()
        {
            _perview.Initialize();
        }
    }

    public class MeshData
    {
        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _triangles;

        private Vector3[] _bakedNormals;

        private Vector3[] _outOfMeshVertices;
        private int[] _outOfMeshTriangles;

        private int _triangleIndex;
        private int _outOfMeshTriangleIndex;

        private bool _useFlasShading;

        public MeshData(int numVerticesPerLine, int skipIncrement, bool useFlasShading)
        {
            _useFlasShading = useFlasShading;

            int numMeshEdgeVertices = (numVerticesPerLine - 2) * 4 - 4;
            int numEdgeConnectionVertices = (skipIncrement - 1) * (numVerticesPerLine - 5) / skipIncrement * 4;
            int numMainVerticesPerLine = (numVerticesPerLine - 5) / skipIncrement + 1;
            int numMainVerteices = numMainVerticesPerLine * numMainVerticesPerLine;

            _vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVerteices];
            _uvs = new Vector2[_vertices.Length];

            int numMeshEdgeTriangles = 8 * (numVerticesPerLine - 4);
            int numMainTriangles = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;

            _triangles = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];

            _outOfMeshVertices = new Vector3[numVerticesPerLine * 4 - 4];
            _outOfMeshTriangles = new int[24 * (numVerticesPerLine - 2)];
        }

        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
        {
            if (vertexIndex < 0)
            {
                _outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
            }
            else
            {
                _vertices[vertexIndex] = vertexPosition;
                _uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                _outOfMeshTriangles[_outOfMeshTriangleIndex] = a;
                _outOfMeshTriangles[_outOfMeshTriangleIndex + 1] = b;
                _outOfMeshTriangles[_outOfMeshTriangleIndex + 2] = c;

                _outOfMeshTriangleIndex += 3;
            }
            else
            {
                _triangles[_triangleIndex] = a;
                _triangles[_triangleIndex + 1] = b;
                _triangles[_triangleIndex + 2] = c;

                _triangleIndex += 3;
            }
        }

        private Vector3[] CalculateNormals()
        {
            Vector3[] vertexNormals = new Vector3[_vertices.Length];
            int triangleCount = _triangles.Length / 3;

            for (int i = 0; i < triangleCount; i++)
            {
                int nromalTriangleIndex = i * 3;
                int vertexIndexA = _triangles[nromalTriangleIndex];
                int vertexIndexB = _triangles[nromalTriangleIndex + 1];
                int vertexIndexC = _triangles[nromalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFronIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }

            int borderTriangleCount = _outOfMeshTriangles.Length / 3;

            for (int i = 0; i < borderTriangleCount; i++)
            {
                int nromalTriangleIndex = i * 3;
                int vertexIndexA = _outOfMeshTriangles[nromalTriangleIndex];
                int vertexIndexB = _outOfMeshTriangles[nromalTriangleIndex + 1];
                int vertexIndexC = _outOfMeshTriangles[nromalTriangleIndex + 2];

                Vector3 triangleNormal = SurfaceNormalFronIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                if (vertexIndexA >= 0)
                {
                    vertexNormals[vertexIndexA] += triangleNormal;
                }

                if (vertexIndexB >= 0)
                {
                    vertexNormals[vertexIndexB] += triangleNormal;
                }

                if (vertexIndexC >= 0)
                {
                    vertexNormals[vertexIndexC] += triangleNormal;
                }
            }

            for (int i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }

            return vertexNormals;
        }

        private Vector3 SurfaceNormalFronIndices(int a, int b, int c)
        {
            Vector3 pointA = (a < 0) ? _outOfMeshVertices[-a - 1] : _vertices[a];
            Vector3 pointB = (b < 0) ? _outOfMeshVertices[-b - 1] : _vertices[b];
            Vector3 pointC = (c < 0) ? _outOfMeshVertices[-c - 1] : _vertices[c];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            return Vector3.Cross(sideAB, sideAC).normalized;

        }

        public void ProcessMesh()
        {
            if (_useFlasShading == true)
            {
                FlatShading();
            }
            else
            {
                BakedNormals();
            }
        }

        void BakedNormals()
        {
            _bakedNormals = CalculateNormals();
        }

        private void FlatShading()
        {
            Vector3[] flatShadedVertices = new Vector3[_triangles.Length];
            Vector2[] flatShaderUvs = new Vector2[_triangles.Length];

            for (int i = 0; i < _triangles.Length; i++)
            {
                flatShadedVertices[i] = _vertices[_triangles[i]];
                flatShaderUvs[i] = _uvs[_triangles[i]];

                _triangles[i] = i;
            }

            _vertices = flatShadedVertices;
            _uvs = flatShaderUvs;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = this._vertices;
            mesh.uv = this._uvs;
            mesh.triangles = this._triangles;

            if (_useFlasShading == true)
            {
                mesh.RecalculateNormals();
            }
            else
            {
                mesh.normals = _bakedNormals;
            }

            return mesh;
        }
    }

    public static class NoiseMap
    {
        public static float[,] CreateNoiseMap(int mapWidth, int mapHeight, NoiseSettings noiseSettings, Vector2 sampleCenter)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random ranSeed = new System.Random(noiseSettings.seed);
            Vector2[] octaveOffsets = new Vector2[noiseSettings.octaves];

            float maxPossobleHeight = 0;
            float amplitude = 1;
            float frequancy = 1;

            for (int i = 0; i < noiseSettings.octaves; i++)
            {
                float offsetX = ranSeed.Next(-100000, 100000) + noiseSettings.offset.x + sampleCenter.x;
                float offsetY = ranSeed.Next(-100000, 100000) - noiseSettings.offset.y - sampleCenter.y;

                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossobleHeight += amplitude;
                amplitude *= noiseSettings.persistance;
            }

            if (noiseSettings.scale <= 0)
            {
                noiseSettings.scale = 0.0001f;
            }

            float maxLocalNormalizeNoiseHeight = float.MinValue;
            float minLocalNornalizeNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth * 0.5f;
            float halfHeight = mapHeight * 0.5f;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    amplitude = 1;
                    frequancy = 1;
                    float noiseHeight = 0;

                    for (int o = 0; o < noiseSettings.octaves; o++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[o].x) / noiseSettings.scale * frequancy;
                        float sampleY = (y - halfHeight + octaveOffsets[o].y) / noiseSettings.scale * frequancy;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= noiseSettings.persistance;
                        frequancy *= noiseSettings.lacunarity;
                    }

                    if (noiseHeight > maxLocalNormalizeNoiseHeight)
                    {
                        maxLocalNormalizeNoiseHeight = noiseHeight;
                    }

                    if (noiseHeight < minLocalNornalizeNoiseHeight)
                    {
                        minLocalNornalizeNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;

                    if (noiseSettings.normalizeMode == eNormalizeMode.Global)
                    {
                        float normalizeHeight = (noiseMap[x, y] + 1) / (2f * maxPossobleHeight / 1.405f);
                        noiseMap[x, y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
                    }
                }
            }

            if (noiseSettings.normalizeMode == eNormalizeMode.Local)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNornalizeNoiseHeight, maxLocalNormalizeNoiseHeight, noiseMap[x, y]);
                    }
                }
            }

            return AddedVariousTerrain(noiseMap);
        }

        private static float[,] AddedVariousTerrain(float[,] noiseMap)
        {


            return noiseMap;
        }
    }

    public static class BiomeGenerator
    {
        public static float[,] GenerateBiome(int mapWidth, int mapHeight, float[,] noiseMap, BiomeSettings biomeSettings, NoiseSettings noiseSettings, TextureSettings textureSettings, Vector2 sampleCenter)
        {
            float[,] biomeMap = new float[mapWidth, mapHeight];

            for (int i = 0; i < biomeSettings.biomeLayers.Length; i++)
            {

            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    biomeMap[x, y] = noiseMap[x, y];

                    if (noiseMap[x, y] > textureSettings.borderBetweenSeaAndLandLayerIndex_SeaSide) // under the sea
                    {
                        continue;
                    }


                }
            }

            return null;
        }
    }

    public static class TextureGenerator
    {
        public static Texture2D TextureFromColourMap(Color[] colorMap, int mapWidth, int mapHeight)
        {
            Texture2D texture = new Texture2D(mapWidth, mapHeight);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            texture.SetPixels(colorMap);
            texture.Apply();

            return texture;
        }

        public static Texture2D TextureFromNoiseMap(HeightMap heightMap)
        {
            int mapWidht = heightMap.noiseMap.GetLength(0);
            int mapHeight = heightMap.noiseMap.GetLength(1);

            Color[] colorMap = new Color[mapWidht * mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidht; x++)
                {
                    colorMap[y * mapWidht + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.noiseMap[x, y]));
                }
            }

            return TextureFromColourMap(colorMap, mapWidht, mapHeight);
        }
    }

    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] noiseMap, MeshSettings meshSettings, int levelOfDetail)
        {
            int skipIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            int numVertsPerLine = meshSettings.numVertsPerLine;

            Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize * 0.5f;

            MeshData meshData = new MeshData(numVertsPerLine, skipIncrement, meshSettings.useFlatShading);

            int[,] vertexIndicesMap = new int[numVertsPerLine, numVertsPerLine];
            int meshVertexIdnex = 0;
            int outOfMeshVertexIndex = -1;

            for (int y = 0; y < numVertsPerLine; y ++)
            {
                for (int x = 0; x < numVertsPerLine; x ++)
                {
                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                    if (isOutOfMeshVertex == true)
                    {
                        vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                        outOfMeshVertexIndex--;
                    }
                    else if(isSkippedVertex == false)
                    {
                        vertexIndicesMap[x, y] = meshVertexIdnex;
                        meshVertexIdnex++;
                    }
                }
            }

            for (int y = 0; y < numVertsPerLine; y ++)
            {
                for (int x = 0; x < numVertsPerLine; x++)
                {
                    bool isSkippedVertex = x > 2 && x < numVertsPerLine - 3 && y > 2 && y < numVertsPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                    if (isSkippedVertex == true)
                    {
                        continue;
                    }

                    bool isOutOfMeshVertex = y == 0 || y == numVertsPerLine - 1 || x == 0 || x == numVertsPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numVertsPerLine - 2 || x == 1 || x == numVertsPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numVertsPerLine - 3 || x == 2 || x == numVertsPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertexIndex = vertexIndicesMap[x, y];
                    float height = noiseMap[x, y];

                    Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;

                    if (isEdgeConnectionVertex == true)
                    {
                        bool isVerical = x == 2 || x == numVertsPerLine - 3;

                        int dstToMainVertexA = ((isVerical == true) ? y - 2 : x - 2) % skipIncrement;
                        int dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;

                        float heightMainVertexA = noiseMap[(isVerical == true) ? x : x - dstToMainVertexA, (isVerical == true) ? y - dstToMainVertexA : y];
                        float heightMainVertexB = noiseMap[(isVerical == true) ? x : x + dstToMainVertexB, (isVerical == true) ? y + dstToMainVertexB : y];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }

                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    bool createTriangle = x < numVertsPerLine - 1 && y < numVertsPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if(createTriangle == true)
                    {
                        int currentIncrement = (isMainVertex && x != numVertsPerLine - 3 && y != numVertsPerLine - 3) ? skipIncrement : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                }
            }

            meshData.ProcessMesh();

            return meshData;
        }
    }

    public static class HeightMapGenerator
    {
        public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
        {
            float[,] values = NoiseMap.CreateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

            AnimationCurve heigntcurveThreadSafe = new AnimationCurve(settings.heightCurve.keys);

            float minVelue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    values[i, j] *= heigntcurveThreadSafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                    if(values[i,j] > maxValue)
                    {
                        maxValue = values[i, j];
                    }

                    if(values[i, j] < minVelue)
                    {
                        minVelue = values[i, j];
                    }
                }
            }

            return new HeightMap(values, minVelue, maxValue);
        }
    }
}


