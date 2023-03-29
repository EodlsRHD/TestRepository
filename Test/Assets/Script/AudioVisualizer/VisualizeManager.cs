using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum eStreoandMono
{
    Non,
    streo,
    mono
}

public class VisualizeManager : MonoBehaviour
{
    [SerializeField]
    private eStreoandMono _streoandMono = eStreoandMono.Non;

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip _stereoAudio = null;

    [SerializeField]
    private AudioClip _monoAudio = null;

    [SerializeField]
    private AudioSource _player = null;

    [Header("Spectrum")]
    [SerializeField][Range(64, 8196)]
    private int _spectrumRange = 1024;

    [Header("Test")]
    [SerializeField]
    private Button _play = null;

    [SerializeField]
    private Button _stop = null;

    private float[] _spectrum;

    private float[] _spectrum_L;

    private float[] _spectrum_R;

    private bool _isPlaying = false;

    private List<Action> _onCallbackVisualizerInitailizeList = new List<Action>();

    public eStreoandMono streoandMono
    {
        set { _streoandMono = value; }
    }

    public float[] spectrum
    {
        get { return _spectrum; }
    }

    public float[] spectrum_L
    {
        get { return _spectrum_L; }
    }

    public float[] spectrum_R
    {
        get { return _spectrum_R; }
    }

    public bool isPlaying
    {
        get { return _isPlaying; }
    }

    private void Start()
    {
        if(_play != null && _stop != null)
        {
            _play.onClick.AddListener(Play);
            _stop.onClick.AddListener(Stop);
        }

        _spectrum_L = new float[_spectrumRange];
        _spectrum_R = new float[_spectrumRange];
        _spectrum = new float[_spectrumRange];
    }

    public void AddVisualizerAction(Action onCallbackVisualizerInitailize)
    {
        _onCallbackVisualizerInitailizeList.Add(onCallbackVisualizerInitailize);
    }

    private void Play()
    {
        _player.Stop();

        _player.clip = _monoAudio;
        _player.Play();

        _isPlaying = true;
    }

    public void Play(AudioClip clip)
    {
        _player.Stop();

        _player.clip = clip;
        _player.Play();

        _isPlaying = true;
    }

    private void Stop()
    {
        _player.Stop();
        _player.clip = null;

        _isPlaying = false;

        foreach(var Callback in _onCallbackVisualizerInitailizeList)
        {
            Callback();
        }
    }

    void Update()
    {
        if(_isPlaying == false)
        {
            return;
        }

        if (_streoandMono == eStreoandMono.streo)
        {
            AudioListener.GetSpectrumData(_spectrum_L, 0, FFTWindow.BlackmanHarris);
            AudioListener.GetSpectrumData(_spectrum_R, 1, FFTWindow.BlackmanHarris);
        }
        else if (_streoandMono == eStreoandMono.mono)
        {
            AudioListener.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        }
    }
}
