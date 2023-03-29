using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Starcraft;


namespace Starcraft
{
    public class OrderManager : MonoBehaviour
    {
        [SerializeField]
        private UIManager _uimanager = null;

        [SerializeField]
        private SelectUnits _selectUnits = null;

        [Header("Army Unit")]
        [SerializeField]
        private List<Unit> _armyFir = new List<Unit>();

        [SerializeField]
        private List<Unit> _armySec = new List<Unit>();

        [SerializeField]
        private List<Unit> _armyThr = new List<Unit>();

        [SerializeField]
        private List<Unit> _armyFour = new List<Unit>();

        public void SetSelectUnit(List<Unit> units)
        {
            _selectUnits.ClearUnits();

            _selectUnits.AddUnits(units);
        }

        public void GetArmyUnit(KeyCode alpha)
        {
            switch(alpha)
            {
                case KeyCode.Alpha1:
                    SetSelectUnit(_armyFir);
                    break;

                case KeyCode.Alpha2:
                    SetSelectUnit(_armySec);
                    break;

                case KeyCode.Alpha3:
                    SetSelectUnit(_armyThr);
                    break;

                case KeyCode.Alpha4:
                    SetSelectUnit(_armyFour);
                    break;
            }
        }

        public void SetArmyUnit(KeyCode alpha)
        {
            switch (alpha)
            {
                case KeyCode.Alpha1:
                    SetArmyUnitList(_armyFir);
                    break;

                case KeyCode.Alpha2:
                    SetArmyUnitList(_armySec);
                    break;

                case KeyCode.Alpha3:
                    SetArmyUnitList(_armyThr);
                    break;

                case KeyCode.Alpha4:
                    SetArmyUnitList(_armyFour);
                    break;
            }
        }

        private void SetArmyUnitList(List<Unit> armyUnit)
        {
            if (armyUnit.Count > 0)
            {
                armyUnit.Clear();
            }

            foreach (var unit in _selectUnits.GetSelectUnits())
            {
                armyUnit.Add(unit);
            }
        }

        public void SetOrder(Vector3 hitpoint, bool isReservation)
        {
            _selectUnits.OrderUnits(hitpoint, isReservation);
        }
    }
}

