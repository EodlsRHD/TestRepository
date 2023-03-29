using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

namespace pageIconControll
{
    public class Icon : MonoBehaviour
    {
        [SerializeField]
        protected Image _stayPage = null;

        [SerializeField]
        protected Button _thisObjButton = null;

        [SerializeField]
        protected TMP_Text _pageText = null;

        [SerializeField]
        protected int thisObjecPage = -1;
        public virtual void Initializ(Action<int> onSelectPage)
        {
            _thisObjButton.onClick.AddListener(() => { onSelectPage?.Invoke(thisObjecPage); });
            this.gameObject.SetActive(false);
        }

        public virtual void SetThisIconPageNum(int num)
        {
            thisObjecPage = num;
            _pageText.text = num.ToString();
        }

        public virtual void SetStayPage(int num)
        {
            bool active = thisObjecPage == num ? true : false;
            _stayPage.gameObject.SetActive(active);
        }

        public virtual void Destroy()
        {
            Destroy(this.gameObject);
        }
    }
}
