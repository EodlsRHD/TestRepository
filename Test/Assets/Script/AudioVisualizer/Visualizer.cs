using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum eSoundChannel
{
    Non,
    mono,
    left,
    right
}

public enum eAxisStrech
{
    Non,
    dx, 
    dy, 
    dz, 
    dyAndDz, 
    all
}

public class Visualizer : MonoBehaviour
{
    [SerializeField]
    private VisualizeManager _visualizeManager = null;

    [Header("Types")]
    [SerializeField]
    private eSoundChannel _soundChannel = eSoundChannel.Non;

    [SerializeField]
    private eAxisStrech _axisStrech = eAxisStrech.Non;

    [Header("High and Low")]
    [SerializeField]
    private float _high = 10f;

    [SerializeField]
    private float _low = 0.01f;

    [Space(10)]

    [SerializeField]
    private int _waveCount = 60;

    [SerializeField]
    private float _sizePower = 20f;

    [Space(10)]

    [SerializeField]
    private Vector3 _defultScale = Vector3.zero;

    [Header("Color")]
    [SerializeField]
    private Color _originalBarColor;

    [SerializeField]
    private Color _changeBarColor;

    [SerializeField]
    private float _colorPower = 20f;

    private float currentRed = 0f;

    private float currentGreen = 0f;

    private float currentBlue = 0f;

    private float currentAlpha = 0f;

    float colorPulse = 0f;

    [SerializeField]
    private List<GameObject> _cubes = new List<GameObject>();

    float power = 0f;

    public eSoundChannel soundChannel
    {
        set { _soundChannel = value; }
    }

    private void Awake()
    {
        currentRed = _changeBarColor.r;
        currentGreen = _changeBarColor.g;
        currentBlue = _changeBarColor.b;
        currentAlpha = _changeBarColor.a;

        for (int i = 0; i < _waveCount; i++)
        {
            _cubes.Add(this.transform.GetChild(i).gameObject);
        }

        _visualizeManager.AddVisualizerAction(Initialize);
    }

    private void Update()
    {
        if (_visualizeManager.isPlaying == false)
        {
            return;
        }

        for (int i = 0; i < _cubes.Count; i++)
        {
            MakeVisualizer(i);
        }
    }

    private void MakeVisualizer(int i)
    {
        Vector3 cubeScale = _cubes[i].transform.localScale;

        switch (_soundChannel)
        {
            case eSoundChannel.mono:
                power = _visualizeManager.spectrum[i] * _sizePower;
                colorPulse = _visualizeManager.spectrum[i] * _colorPower;
                break;

            case eSoundChannel.left:
                power = _visualizeManager.spectrum_L[i] * _sizePower;
                colorPulse = _visualizeManager.spectrum_L[i] * _colorPower;
                break;

            case eSoundChannel.right:
                power = _visualizeManager.spectrum_R[i] * _sizePower;
                colorPulse = _visualizeManager.spectrum_R[i] * _colorPower;
                break;
        }

        if (power < _low)
        {
            power = _low;
        }

        if (power > _high)
        {
            power = _high;
        }

        switch (_axisStrech)
        {
            case eAxisStrech.dx:
                cubeScale.x = Mathf.Lerp(cubeScale.x, power, _sizePower * Time.deltaTime);
                break;

            case eAxisStrech.dy:
                cubeScale.y = Mathf.Lerp(cubeScale.y, power, _sizePower * Time.deltaTime);
                break;

            case eAxisStrech.dz:
                cubeScale.z = Mathf.Lerp(cubeScale.z, power, _sizePower * Time.deltaTime);
                break;

            case eAxisStrech.dyAndDz:
                cubeScale.y = Mathf.Lerp(cubeScale.y, power, _sizePower * Time.deltaTime);
                cubeScale.z = Mathf.Lerp(cubeScale.z, power, _sizePower * Time.deltaTime);
                break;

            case eAxisStrech.all:
                cubeScale.x = Mathf.Lerp(cubeScale.x, power, _sizePower * Time.deltaTime);
                cubeScale.y = Mathf.Lerp(cubeScale.y, power, _sizePower * Time.deltaTime);
                cubeScale.z = Mathf.Lerp(cubeScale.z, power, _sizePower * Time.deltaTime);
                break;
        }

        _cubes[i].transform.localScale = cubeScale;

        //Color
        _originalBarColor.r = (colorPulse / _changeBarColor.r);
        _originalBarColor.g = (colorPulse / _changeBarColor.g);
        _originalBarColor.b = (colorPulse / _changeBarColor.b);
        _originalBarColor.a = (colorPulse / _changeBarColor.a);

        _cubes[i].GetComponent<Renderer>().material.color = _originalBarColor;
        _cubes[i].GetComponent<Renderer>().material.SetColor("_TintColor", _originalBarColor);
    }

    private void Initialize()
    {
        for(int i = 0; i < _cubes.Count; i++)
        {
            _cubes[i].transform.localScale = _defultScale;
        }
    }
}
