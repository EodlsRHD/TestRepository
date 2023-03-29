using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Starcraft;

namespace Starcraft
{
    public class ArmyInfomation : MonoBehaviour
    {
        [SerializeField]
        private List<Unitinfo> _infoPool = new List<Unitinfo>(12);

        public void SetActiveUnitInfo(List<Unit> selectUnit, UnityAction<Unit> clickUnitCallback)
        {
            RemoveUnitInfo();

            for (int i = 0; i < selectUnit.Count; i++)
            {
                _infoPool[i].Active(selectUnit[i], clickUnitCallback);
                _infoPool[i].gameObject.SetActive(true);
            }
        }

        public void RemoveUnitInfo()
        {
            foreach(var info in _infoPool)
            {
                info.RemoveImage();
                info.gameObject.SetActive(false);
            }
        }
    }
}