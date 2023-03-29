using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Starcraft
{
    public class Region
    {
        public float x;
        public float y;

        private List<Node> _nodeList = new List<Node>();

        public void FindNode()
        {
            
        }
    }

    public class Node
    {
        public bool Walkable = false;
    }

    public class WalkabilityMap : MonoBehaviour
    {
        [SerializeField]
        private Transform _mapCenter = null;

        [SerializeField]
        private uint _mapSize = 256;

        private Node[] _row;

        private Node[] _col;

        public void WorldInitialize()
        {
            _row = new Node[_mapSize];
            _col = new Node[_mapSize];

            //for(int i = 0; i < _row)
        }
    }
}