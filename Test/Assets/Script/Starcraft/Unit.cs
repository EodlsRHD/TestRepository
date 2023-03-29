using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Starcraft;

namespace Starcraft
{
    public class UNIT
    {
        private float _id = 0f;

        private Sprite _unitImage = null;
        public Sprite UnitImage
        {
            get
            {
                return _unitImage;
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        private float _damage = 0f;
        public float Damage
        {
            get
            {
                return _damage;
            }
        }

        private float _movespeed = 0f;
        public float MoveSpeed
        {
            get
            {
                return _movespeed;
            }
        }

        public UNIT(Sprite unitImage, string name, float damage, float movespeed)
        {
            _unitImage = unitImage;
            _name = name;
            _damage = damage;
            _movespeed = movespeed;
        }

        public void SetId(float id)
        {
            _id = id;
        }
    }

    public class Unit : MonoBehaviour
    {
        [SerializeField]
        private Sprite _unitImage = null; // TestCode

        private UNIT _info;

        public UNIT UNIT
        {
            get
            {
                return _info;
            }
        }

        private Queue<Vector3> _reservationOrder = new Queue<Vector3>();

        private Vector3 _orderPoint = Vector3.zero;

        private bool _isExecution = false;

        [SerializeField]
        private bool _isReservation = false;

        private void Start() // TestCord
        {
            _orderPoint = this.transform.position;
            _info = new UNIT(_unitImage, "TestUnit", 10f, 8f);
            _info.SetId(Time.deltaTime * Random.Range(0, 100f));
        }

        private void OnEnable()
        {
            _orderPoint = this.transform.position;
            _info = new UNIT(_unitImage, "TestUnit", 10f, 8f);
            _info.SetId(Time.deltaTime * Random.Range(0, 100f));
        }

        public void Order(Vector3 point, bool reservation)
        {
            _isReservation = reservation;

            Vector3 Point = point;
            Point.y = this.transform.position.y;

            if (_isReservation == true)
            {
                _reservationOrder.Enqueue(Point);
                return;
            }

            _orderPoint = Point;
            _reservationOrder.Clear();
        }

        private void Update()
        {
            Vector3 pos = this.transform.position;

            if(_isReservation == false)
            {
                this.transform.position = Vector3.MoveTowards(pos, _orderPoint, Time.deltaTime * _info.MoveSpeed);
                return;
            }

            if(_reservationOrder.Count > 0)
            {
                if(_isExecution == false)
                {
                    _orderPoint = _reservationOrder.Dequeue();
                    _orderPoint.y = pos.y;
                }
            }

            if (pos != _orderPoint)
            {
                _isExecution = true;
                this.transform.position = Vector3.MoveTowards(pos, _orderPoint, Time.deltaTime * _info.MoveSpeed);
                return;
            }

            _isExecution = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, _orderPoint);
            Gizmos.DrawSphere(_orderPoint, 0.5f);
        }
    }
}
