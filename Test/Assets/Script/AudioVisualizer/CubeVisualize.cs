using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeVisualize : MonoBehaviour
{
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
    private float _senservility = 0f;

    [SerializeField]
    private float _lerpTime = 5f;

    [SerializeField]
    private float _scaleY = 20f;

    [SerializeField]
    private eSoundChannel _soundChannel = eSoundChannel.Non;

    private Vector3 _scaleOriginalState = Vector3.zero;

    void Start()
    {
        _scaleOriginalState = this.gameObject.transform.localScale;
    }

    void Update()
    {
        //switch(_soundChannel)
        //{
        //    case eSoundChannel.left:
        //        if (MakeVisualize._spectrum_L[_channel] * 3f >= _senservility)
        //        {
        //            this.transform.localScale = new Vector3(1f, MakeVisualize._spectrum_L[_channel] * _scaleY, 1f);
        //        }
        //        else
        //        {
        //            this.transform.localScale = Vector3.Lerp(this.transform.localScale, _scaleOriginalState, Time.deltaTime * _lerpTime);
        //        }
        //        break;

        //    case eSoundChannel.right:
        //        if (MakeVisualize._spectrum_R[_channel] * 3f >= _senservility)
        //        {
        //            this.transform.localScale = new Vector3(1f, MakeVisualize._spectrum_R[_channel] * _scaleY, 1f);
        //        }
        //        else
        //        {
        //            this.transform.localScale = Vector3.Lerp(this.transform.localScale, _scaleOriginalState, Time.deltaTime * _lerpTime);
        //        }
        //        break;
        //}
    }
}
