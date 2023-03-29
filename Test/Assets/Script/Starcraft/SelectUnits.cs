using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Starcraft;

namespace Starcraft
{
    public class SelectUnits : MonoBehaviour
    {

        [SerializeField]
        private UIManager _uimanager = null;

        private List<Unit> _selectUnits = new List<Unit>();

        private Vector3 _order = Vector3.zero;

        public List<Unit> GetSelectUnits()
        {
            return _selectUnits;
        }

        public void AddUnits(List<Unit> addUnits)
        {
            ClearUnits();

            foreach (var unit in addUnits)
            {
                _selectUnits.Add(unit);
            }

            _uimanager.SetArmyInfomation(_selectUnits, AddUnits);
        }

        public void AddUnits(Unit addUnit)
        {
            ClearUnits();

            _selectUnits.Add(addUnit);

            _uimanager.SetArmyInfomation(_selectUnits, AddUnits);
        }

        public void OrderUnits(Vector3 order, bool isReservation)
        {
            float MeanX = 0f;
            float MeanZ = 0f;
            foreach(var unit in _selectUnits)
            {
                MeanX += unit.transform.position.x;
                MeanZ += unit.transform.position.z;
            }
            MeanX = MeanX / _selectUnits.Count;
            MeanZ = MeanZ / _selectUnits.Count;

            _order = order;
            Vector3 MeanPoint = new Vector3(MeanX, 0f, MeanZ);

            foreach(var unit in _selectUnits)
            {
                Vector3 dir = (unit.transform.position - MeanPoint).normalized;
                float dis = Vector3.Distance(unit.transform.position, MeanPoint);
                unit.Order(_order + (dir * dis), isReservation);
            }
        }

        public void ClearUnits()
        {
            _selectUnits.Clear();
        }

        private void OnDrawGizmos()
        {
            if(_selectUnits.Count > 0)
            {
                float MeanX = 0f;
                float MeanZ = 0f;
                foreach (var unit in _selectUnits)
                {
                    MeanX += unit.transform.position.x;
                    MeanZ += unit.transform.position.z;
                }
                MeanX = MeanX / _selectUnits.Count;
                MeanZ = MeanZ / _selectUnits.Count;

                Vector3 MeanPoint = new Vector3(MeanX, 0f, MeanZ);
                Gizmos.DrawSphere(MeanPoint, 1f);

                foreach (var unit in _selectUnits)
                {
                    Vector3 dir = (unit.transform.position - MeanPoint).normalized;
                    float dis = Vector3.Distance(unit.transform.position, MeanPoint);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(unit.transform.position, MeanPoint + dir * dis);
                    Gizmos.DrawSphere(_order + (dir * dis), 0.5f);
                }
            }
        }
    }
}
