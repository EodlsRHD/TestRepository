using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MinecraftCam
{
    public class Block : MonoBehaviour
    {
        [SerializeField]
        private int _number;

        [SerializeField]
        private eBlockType _type = eBlockType.Non;
        public void Initialized(int number, Vector3 pos)
        {
            _number = number;
            this.transform.position = pos;
        }

        public float GetY()
        {
            return this.transform.position.y;
        }
    }
}
