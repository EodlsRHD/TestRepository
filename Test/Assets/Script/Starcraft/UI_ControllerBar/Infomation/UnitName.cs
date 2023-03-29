using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Starcraft;

namespace Starcraft
{
    public class UnitName : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _name;

        public string Name
        {
            set
            {
                _name.text = value;
            }
        }

        public void RemoveName()
        {
            _name.text = string.Empty;
        }
    }
}
