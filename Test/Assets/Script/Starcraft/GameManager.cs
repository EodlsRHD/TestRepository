using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Starcraft;

namespace Starcraft
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _minimapCamera = null;

        [SerializeField]
        private GameObject _fild = null;

        // Start is called before the first frame update
        void Start()
        {
            _minimapCamera.orthographicSize = _fild.transform.localScale.x / 2;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}