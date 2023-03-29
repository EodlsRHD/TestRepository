using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PoolTest
{
    public class QueuePool : MonoBehaviour
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
        protected List<GameObject> _use = new List<GameObject>();

        private Queue<GameObject> _pool = new Queue<GameObject>();


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

        private void Add(GameObject obj)
        {
            _pool.Enqueue(obj);
        }

        private void Use(int num)
        {
            Debug.Log(num + "       " + _use.Count);

            for (int i = _use.Count - 1; i >= 0; i--)
            {
                GameObject obj = _use[i];
                obj.SetActive(false);

                _pool.Enqueue(obj);
                _use.Remove(obj);
            }

            Debug.Log(num + "       " + _use.Count);

            for (int i = 0; i < num; i++)
            {
                GameObject obj = _pool.Dequeue();
                obj.SetActive(true);

                _use.Add(obj);
            }

            Debug.Log(num + "       " + _use.Count);
        }
    }
}