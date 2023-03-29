using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightVisualize : MonoBehaviour
{
    enum eSoundChannel
    {
        NON,
        left,
        right
    }

    [SerializeField]
    private int _channel = 0;

    public int SetChannel
    {
        set
        {
            _channel = value;
        }
    }

    [SerializeField]
    private Light _light = null;

    [SerializeField]
    private float _senservility = 0f;

    [SerializeField]
    private float _lerpTime = 5f;

    [SerializeField]
    private float _intensityDelta = 20f;

    [SerializeField]
    private eSoundChannel _soundChannel = eSoundChannel.NON;

    private float _originalIntensity = 0f;

    void Start()
    {
        _originalIntensity = _light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        //switch (_soundChannel)
        //{
        //    case eSoundChannel.left:
        //        if (MakeVisualize._spectrum_L[_channel] * 3f >= _senservility)
        //        {
        //            _light.intensity = MakeVisualize._spectrum_L[_channel] * _intensityDelta;
        //        }
        //        else
        //        {
        //            _light.intensity = _originalIntensity;
        //        }
        //        break;

        //    case eSoundChannel.right:
        //        if (MakeVisualize._spectrum_R[_channel] * 3f >= _senservility)
        //        {
        //            _light.intensity = MakeVisualize._spectrum_R[_channel] * _intensityDelta;
        //        }
        //        else
        //        {
        //            _light.intensity = _originalIntensity;
        //        }
        //        break;
        //}
    }
}
