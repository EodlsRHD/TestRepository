using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Angle : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _xInput = null;

    [SerializeField]
    private TMP_InputField _yInput = null;

    [SerializeField]
    private TMP_InputField _zInput = null;


    [SerializeField]
    private Button _rotateButton = null;

    [SerializeField]
    private Button _reset= null;


    [SerializeField]
    private TMP_Text _resultX = null;

    [SerializeField]
    private TMP_Text _resultY = null;

    [SerializeField]
    private TMP_Text _resultZ = null;


    [SerializeField]
    private TMP_Dropdown _modeDropdown = null;


    [SerializeField]
    private GameObject _obj = null;


    private float X, Y, Z;

    private void Start()
    {
        _reset.onClick.AddListener(ResetObj);
        _modeDropdown.onValueChanged.AddListener(changeMode);
    }

    void changeMode(int mode)
    {
        switch (mode)
        {
            case 0:
                _rotateButton.onClick.AddListener(OnEulerRotateObj);
                break;

            case 1:
                _rotateButton.onClick.AddListener(OnQuaternionRotateObj);
                break;
        }
    }

    void ChangeTextToFloat()
    {
        if(_xInput.text.Equals(string.Empty))
        {
            _xInput.text = 0.ToString();
        }
        if (_yInput.text.Equals(string.Empty))
        {
            _yInput.text = 0.ToString();
        }
        if (_zInput.text.Equals(string.Empty))
        {
            _zInput.text = 0.ToString();
        }

        X = float.Parse(_xInput.text);
        Y = float.Parse(_yInput.text);
        Z = float.Parse(_zInput.text);
    }

    void OnEulerRotateObj()
    {
        ChangeTextToFloat();

        Vector3 rotateCube = new Vector3(X, Y, Z);
        _obj.transform.Rotate(rotateCube);

        Result();
    }

    void OnQuaternionRotateObj()
    {
        ChangeTextToFloat();

        Vector3 rotateCube = new Vector3(X, Y, Z);
        _obj.transform.rotation = Quaternion.Euler(rotateCube);

        Result();
    }

    void Result()
    {
        //_resultX.text = (_obj.transform.rotation.x * 360).ToString();
        //_resultY.text = (_obj.transform.rotation.y * 360).ToString();
        //_resultZ.text = (_obj.transform.rotation.z * 360).ToString();
    }

    void ResetObj()
    {
        _obj.transform.rotation = Quaternion.Euler(Vector3.zero);

        X = 0f;
        Y = 0f;
        Z = 0f;

        _resultX.text = X.ToString();
        _resultY.text = Y.ToString();
        _resultZ.text = Z.ToString();

        _xInput.text = string.Empty;
        _yInput.text = string.Empty;
        _zInput.text = string.Empty;
    }
}
