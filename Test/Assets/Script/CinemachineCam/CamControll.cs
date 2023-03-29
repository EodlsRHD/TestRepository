using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamControll : MonoBehaviour
{
    [SerializeField]
    private CinemachineSmoothPath _path = null;

    [SerializeField]
    private CinemachineVirtualCamera _cam = null;

    private CinemachineTrackedDolly _vcam = null;

    [System.Serializable]
    class InputKeyCode
    {
        public KeyCode keycode = KeyCode.None;

        public int camNumber = 0;
    }

    [SerializeField]
    private float _speed = 0f;

    [Space(10)]

    [SerializeField]
    private List<InputKeyCode> _cams = new List<InputKeyCode>();

    private int _inputNum = 0;

    private void Start()
    {
        _vcam = _cam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }

    private void Update()
    {
        if (_path.Looped == true)
        {
            Debug.Log("is Looped");
            return;
        }

        float cartPosition = _vcam.m_PathPosition;

        InputKey(cartPosition);
    }

    private void InputKey(float cartPosition)
    {
        foreach(var key in _cams)
        {
            if(Input.GetKeyDown(key.keycode))
            {
                _inputNum = key.camNumber;
                break;
            }
        }

        MoveCart(_inputNum, cartPosition);
    }

    private void MoveCart(int num, float cartPosition)
    {
        if (num > _path.m_Waypoints.Length - 1)
        {
            return;
        }

        _vcam.m_PathPosition = Mathf.Lerp(cartPosition, num, Time.deltaTime * _speed);
    }
}
