using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace MapGanerate
{
    [System.Serializable]
    public struct TerrainType
    {
        public string name;

        public float height;

        public Color colour;
    }

    public struct MapData
    {
        public readonly float[,] noiseMap;

        public readonly Color[] colorMap;

        public MapData(float[,] _noiseMap, Color[] _colorMap)
        {
            this.noiseMap = _noiseMap;
            this.colorMap = _colorMap;
        }
    }

    public enum DrawType
    {
        NoiseMap,
        ColorMap,
        MeshMap
    }

    public class MapGenerator : MonoBehaviour
    {
        private struct MapThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public MapThreadInfo(Action<T> _callback, T _parameter)
            {
                this.callback = _callback;
                this.parameter = _parameter;
            }
        }

        [SerializeField]
        private Display _display = null;

        [Space(10)]

        public bool autoUpdate;

        [Space(10), Header("Info")]

        [SerializeField]
        public const int mapChunkSize = 241;

        [SerializeField, Range(0, 6)]
        private int editorPreviewLOD;

        [SerializeField]
        private float _mapScale = 0;

        [SerializeField]
        private int _octaves = 0;

        [SerializeField, Range(0, 1)]
        private float _persistance = 0;

        [SerializeField]
        private float _lacunarity = 0;

        [SerializeField]
        private float _heightMultiplier = 1;

        [SerializeField]
        private AnimationCurve _animationCurve = null;

        [SerializeField]
        private int _seed = 0;

        [SerializeField]
        private Vector2 _offset;

        [Space(10), Header("Map Color")]

        [SerializeField]
        private DrawType _drawType;

        [SerializeField]
        private TerrainType[] _terrainTypes;

        private Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
        private Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

        public void Initialize()
        {
            _display.Initialize();
        }

        public void DrawMapInEditor()
        {
            MapData mapData = GenerateMapData(Vector2.zero);

            switch (_drawType)
            {
                case DrawType.NoiseMap:
                    {
                        Texture2D textute = TextureGenerator.TextureFromNoiseMap(mapData.noiseMap);

                        _display.DrawTexture(textute);
                    }
                    break;

                case DrawType.ColorMap:
                    {
                        Texture2D textute = TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunkSize, mapChunkSize);

                        _display.DrawTexture(textute);
                    }
                    break;

                case DrawType.MeshMap:
                    {
                        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, _heightMultiplier, _animationCurve, editorPreviewLOD);
                        Texture2D textute = TextureGenerator.TextureFromColourMap(mapData.colorMap, mapChunkSize, mapChunkSize);

                        _display.DrawMesh(meshData, textute);
                    }
                    break;
            }
        }

        public void RequestMapData(Vector2 center, Action<MapData> onCallback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(center, onCallback);
            };

            new Thread(threadStart).Start();
        }

        private void MapDataThread(Vector2 center, Action<MapData> onCallback)
        {
            MapData mapData = GenerateMapData(center);
            lock(_mapDataThreadInfoQueue)
            {
                _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(onCallback, mapData));
            }
        }

        public void RequestMeshData(MapData mapData, int lod, Action<MeshData> onCallback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, lod, onCallback);
            };

            new Thread(threadStart).Start();
        }

        private void MeshDataThread(MapData mapData, int lod, Action<MeshData> onCallback)
        {
            MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, _heightMultiplier, _animationCurve, lod);
            lock (_meshDataThreadInfoQueue)
            {
                _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(onCallback, meshData));
            }
        }

        private void Update()
        {
            if(_mapDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> info = _mapDataThreadInfoQueue.Dequeue();
                    info.callback(info.parameter);
                }
            }
            if (_meshDataThreadInfoQueue.Count > 0)
            {
                for (int i = 0; i < _meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MeshData> info = _meshDataThreadInfoQueue.Dequeue();
                    info.callback(info.parameter);
                }
            }
        }

        private MapData GenerateMapData(Vector2 center)
        {
            float[,] noiseMap = NoiseMap.CreateNoiseMap(mapChunkSize, mapChunkSize, _seed, _mapScale, _octaves, _persistance, _lacunarity, center + _offset);

            Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    for (int t = 0; t < _terrainTypes.Length; t++)
                    {
                        if (noiseMap[x, y] <= _terrainTypes[t].height)
                        {
                            colorMap[y * mapChunkSize + x] = _terrainTypes[t].colour;
                            break;
                        }
                    }
                }
            }

            return new MapData(noiseMap, colorMap);
        }

        private void OnValidate()
        {
            if (_lacunarity < 1)
            {
                _lacunarity = 1;
            }

            if (_octaves < 0)
            {
                _octaves = 0;
            }

            if(_mapScale < 0)
            {
                _mapScale = 0.1f;
            }
        }
    }

    public static class NoiseMap
    {
        public static float[,] CreateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random ranSeed = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = ranSeed.Next(-100000, 100000) + offset.x;
                float offsetY = ranSeed.Next(-100000, 100000) + offset.y;

                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if(scale <= 0)
            {
                scale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth * 0.5f;
            float halfHeight = mapHeight * 0.5f;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float amplitude = 1;
                    float frequancy = 1;
                    float noiseHeight = 0;

                    for (int o = 0; o < octaves; o++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequancy + octaveOffsets[o].x;
                        float sampleY = (y - halfHeight) / scale * frequancy + octaveOffsets[o].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequancy *= lacunarity;
                    }

                    if(noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if(noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseHeight);
                }
            }

            return noiseMap;
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

        public static Texture2D TextureFromNoiseMap(float[,] noiseMap)
        {
            int mapWidht = noiseMap.GetLength(0);
            int mapHeight = noiseMap.GetLength(1);

            Color[] colorMap = new Color[mapWidht * mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidht; x++)
                {
                    colorMap[y * mapWidht + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }

            return TextureFromColourMap(colorMap, mapWidht, mapHeight);
        }
    }

    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] noiseMap, float heightMultiplier, AnimationCurve curve, int levelOfDetail)
        {
            AnimationCurve heightCurve = new AnimationCurve(curve.keys);

            int mapWidth = noiseMap.GetLength(0);
            int mapHeight = noiseMap.GetLength(1);
            float topLeftX = (mapWidth - 1) / -2f;
            float topLeftZ = (mapHeight - 1) / 2f;

            int meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            int verticesPerLine = (mapWidth - 1) / meshSimplificationIncrement + 1;

            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            int vertexIndex = 0;

            for (int y = 0; y < mapHeight; y += meshSimplificationIncrement)
            {
                for (int x = 0; x < mapWidth; x += meshSimplificationIncrement)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier, topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidth, y / (float)mapHeight);

                    if (x < mapWidth - 1 && y < mapHeight - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public Vector2[] uvs;
        public int[] triangles;

        private int triangleIndex;

        public MeshData(int mapWidth, int mapHeight)
        {
            vertices = new Vector3[mapWidth * mapHeight];
            uvs = new Vector2[mapWidth * mapHeight];
            triangles = new int[(mapWidth - 1) * (mapHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;

            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = this.vertices;
            mesh.uv = this.uvs;
            mesh.triangles = this.triangles;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}


