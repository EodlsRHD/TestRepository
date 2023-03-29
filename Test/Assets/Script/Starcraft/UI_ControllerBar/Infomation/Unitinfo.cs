using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Starcraft;
using System;

namespace Starcraft
{
    public class Unitinfo : MonoBehaviour
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private Button _button = null;

        [SerializeField]
        private UnitName _name = null;

        private Unit _unit = null;

        private UnityAction<Unit> _clickUnitCallback = null;

        private UnityAction<string> _viewUnitName = null;

        private void Start()
        {
            _button.onClick.AddListener(() => _clickUnitCallback(_unit));
        }

        public void Active(Unit unit, UnityAction<Unit> clickUnitCallback)
        {
            _unit = unit;
            _clickUnitCallback = clickUnitCallback;

            _name.Name = _unit.UNIT.Name;
            _image.sprite = _unit.UNIT.UnitImage;
        }

        public void ActiveName(bool active)
        {
            _name.gameObject.SetActive(active);
        }

        public void RemoveImage()
        {
            _image.sprite = null;
        }
    }
}
