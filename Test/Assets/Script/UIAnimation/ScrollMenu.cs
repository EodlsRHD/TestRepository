using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollMenu : MonoBehaviour
{
    [SerializeField]
    private float _time = 0f;

    [SerializeField]
    private Button _menuButton = null;

    [SerializeField]
    private RectTransform _menuBackground = null;

    [Header("Test")]
    [SerializeField]
    private Button _test1 = null;

    [SerializeField]
    private Button _test2 = null;

    [SerializeField]
    private Button _test3 = null;

    private bool _isOpen = false;

    private void Start()
    {
        _menuButton.onClick.AddListener(() => {

            if(_isOpen == false) // close
            {
                _isOpen = true;
                _menuBackground.transform.localPosition = new Vector3(-158.9f, 0f, 0f);
                _menuBackground.DOAnchorPos(new Vector2(271f, 0f), _time, false).SetEase(Ease.InOutQuint);

                _menuButton.transform.DORotate(new Vector3(0f, 0f, -90f), _time * 0.5f, RotateMode.Fast);

                return;
            }

            if(_isOpen == true) // open
            {
                _isOpen = false;
                _menuBackground.transform.localPosition = new Vector3(271f, 0f, 0f);
                _menuBackground.DOAnchorPos(new Vector2(-158.9f, 0f), _time, false).SetEase(Ease.InOutQuint);

                _menuButton.transform.DORotate(new Vector3(0f, 0f, 90f), _time * 0.5f, RotateMode.Fast);

                return;
            }
        });

        _test1.onClick.AddListener(() => { Debug.Log("_test1"); });
        _test2.onClick.AddListener(() => { Debug.Log("_test2"); });
        _test3.onClick.AddListener(() => { Debug.Log("_test3"); });
    }
}