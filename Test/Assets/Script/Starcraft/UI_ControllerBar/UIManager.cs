using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Starcraft;

namespace Starcraft
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private ArmyInfomation _armyInfomation = null;

        [SerializeField]
        private Minimap _minimap = null;

        public void SetArmyInfomation(List<Unit> selectUnit, UnityAction<Unit> onCallback)
        {
            _armyInfomation.SetActiveUnitInfo(selectUnit, onCallback);
        }
    }
}
