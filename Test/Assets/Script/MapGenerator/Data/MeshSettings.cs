using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate
{
    [CreateAssetMenu()]
    public class MeshSettings : UpdatableData
    {
        public const int numSupportedLODs = 5;
        public const int numSupportedChunkSizes = 9;
        public const int numSupportedFlatChunkSizes = 3;

        public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        public float meshScale = 2.5f;
        public bool useFlatShading;

        [SerializeField, Range(0, numSupportedChunkSizes - 1)]
        private int _chunkSizeIndex;

        [SerializeField, Range(0, numSupportedFlatChunkSizes - 1)]
        private int _flatChunkSizeIndex;

        //num verts per line of mesh rendererd at lod 0.
        //Includes the 2 extra verts that are exclued from final mesh, but used for calculating normals.
        public int numVertsPerLine
        {
            get { return supportedChunkSizes[(useFlatShading) ? _flatChunkSizeIndex : _chunkSizeIndex] + 5; }
        }

        public float meshWorldSize
        {
            get { return (numVertsPerLine - 3) * meshScale; }
        }
    }

}