using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Starcraft;

namespace Starcraft
{
    public class InputManager : MonoBehaviour
    {
        enum eDragMode
        {
            start,
            end,
            order
        }

        [SerializeField]
        private OrderManager _orderManager = null;

        [SerializeField]
        private UIManager _uimanager = null;

        [SerializeField]
        private BoxCollider _boxCollider = null;

        private List<Unit> _selectUnit = new List<Unit>(12);

        private int _layerMaskFild = 1 << 8; // Fild

        private int _layerMaskUnit = 1 << 7; // Unit

        private int _layerMaskUI = 1 << 5; // UI

        private Vector3 _dragStartPoint = Vector3.zero;

        private Vector3 _dragEndPoint = Vector3.zero;

        private Vector3 _dragCenter = Vector3.zero;

        private float _dragWidth = 0f;

        private float _dragLength = 0f;

        private bool _isDragging = false;

        private bool _isReservation = false;

        void Update()
        {
            Vector3 mousePos = Input.mousePosition;

            UnitControll(mousePos);
            AddArmy();
            SelectArmy();
        }

        void UnitControll(Vector3 mousePos)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDragging = true;
                Ray(eDragMode.start, mousePos);
            }

            if (Input.GetMouseButton(0))
            {
                Ray(eDragMode.end, mousePos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                InSideUnit(_dragCenter, _dragWidth, _dragLength);
                _isDragging = false;
            }

            if(Input.GetMouseButtonDown(1))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _isReservation = true;
                }
                Ray(eDragMode.order, mousePos);
            }

            if (Input.GetMouseButtonUp(1))
            {
                _isReservation = false;
            }
        }

        void AddArmy()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && Input.GetKey(KeyCode.LeftControl))
            {
                _orderManager.SetArmyUnit(KeyCode.Alpha1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && Input.GetKey(KeyCode.LeftControl))
            {
                _orderManager.SetArmyUnit(KeyCode.Alpha2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && Input.GetKey(KeyCode.LeftControl))
            {
                _orderManager.SetArmyUnit(KeyCode.Alpha3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4) && Input.GetKey(KeyCode.LeftControl))
            {
                _orderManager.SetArmyUnit(KeyCode.Alpha4);
            }
        }

        void SelectArmy()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _orderManager.GetArmyUnit(KeyCode.Alpha1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _orderManager.GetArmyUnit(KeyCode.Alpha2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _orderManager.GetArmyUnit(KeyCode.Alpha3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _orderManager.GetArmyUnit(KeyCode.Alpha4);
            }
        }

        void Ray(eDragMode dragmode, Vector3 mousePos)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMaskFild))
            {
                GameFildRay(dragmode, hit.point);
            }
        }

        void GameFildRay(eDragMode dragmode, Vector3 hitpoint)
        {
            switch (dragmode)
            {
                case eDragMode.start:
                    _dragStartPoint = hitpoint;
                    _dragStartPoint.y = 0f;
                    break;

                case eDragMode.end:
                    _dragEndPoint = hitpoint;
                    _dragEndPoint.y = 0f;
                    break;

                case eDragMode.order:
                    _orderManager.SetOrder(hitpoint, _isReservation);
                    return;
            }

            Vector3 dir = (_dragEndPoint - _dragStartPoint).normalized;
            float Halfdis = Vector3.Distance(_dragEndPoint, _dragStartPoint) * 0.5f;
            _dragWidth = Vector3.Distance(new Vector3(_dragStartPoint.x, 0f, _dragEndPoint.z), _dragStartPoint);
            _dragLength = Vector3.Distance(new Vector3(_dragEndPoint.x, 0f, _dragStartPoint.z), _dragStartPoint);
            _dragCenter = _dragStartPoint + (dir * Halfdis);
        }

        void InSideUnit(Vector3 dragCenter, float dragWidth, float dragLength)
        {
            _selectUnit.Clear();

            _boxCollider.center = dragCenter;
            _boxCollider.size = new Vector3(dragLength, 1f, dragWidth);
            Collider[] colls = Physics.OverlapBox(dragCenter, _boxCollider.size / 2, Quaternion.identity, _layerMaskUnit);

            if(colls.Length == 0)
            {
                return;
            }

            for(int i = 0; i < colls.Length; i++)
            {
                if(i >= 12)
                {
                    break;
                }

                _selectUnit.Add(colls[i].GetComponent<Unit>());
            }

            _orderManager.SetSelectUnit(_selectUnit);
        }

        private void OnDrawGizmos()
        {
            if (_isDragging == true)
            {
                Vector3 dir = (_dragEndPoint - _dragStartPoint).normalized;
                float Halfdis = Vector3.Distance(_dragEndPoint, _dragStartPoint) * 0.5f;

                Vector3 Centrt = _dragStartPoint + (dir * Halfdis);
                Vector3 W = new Vector3(_dragStartPoint.x, 0f, _dragEndPoint.z);
                Vector3 L = new Vector3(_dragEndPoint.x, 0f, _dragStartPoint.z);

                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(Centrt, 1f);

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(W, 1f);
                Gizmos.DrawSphere(L, 1f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(_dragStartPoint, W);
                Gizmos.DrawLine(_dragStartPoint, L);
                Gizmos.DrawLine(_dragEndPoint, W);
                Gizmos.DrawLine(_dragEndPoint, L);
            }
        }
    }
}
