using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PoolTest
{
    public class Common : MonoBehaviour
    {
        [SerializeField]
        private Button _useButton = null;

        [SerializeField]
        private TMP_InputField _howUseNum = null;

        [SerializeField]
        private GameObject _original = null;

        [SerializeField]
        protected int _maxNum = 10;

        [SerializeField]
        protected Queue<GameObject> _use = new Queue<GameObject>();

        void Start()
        {
            _useButton.onClick.AddListener(SetButton);

            for (int i = 0; i < _maxNum; i++)
            {
                GameObject obj = Instantiate(_original, transform);
                obj.SetActive(false);
                Add(obj);
            }
        }

        private void SetButton()
        {
            int num = int.Parse(_howUseNum.text);
            Debug.Log(num);
            Use(num);
        }

        protected virtual void Add(GameObject obj)
        {

        }

        protected virtual void Use(int num)
        {
            Debug.Log(num + "            " + _use.Count);
        }
    }
}