using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCount : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _fpsTEXT = null;

    [SerializeField]
    private TMP_Text _highTEXT = null;

    [SerializeField]
    private TMP_Text _lowTEXT = null;

    private float _fps = 0f;

    private float _high = 0f;

    private float _low = 0f;

    private void Start()
    {
        _low = 100f;
    }

    void Update()
    {
        _fps = 1.0f / Time.deltaTime;

        if(_fps > _high)
        {
            _high = _fps;
        }

        if(_fps < _low)
        {
            _low = _fps;
        }

        _fpsTEXT.text = _fps.ToString("F1");
        _highTEXT.text = _high.ToString("F1");
        _lowTEXT.text = _low.ToString("F1");
    }
}
