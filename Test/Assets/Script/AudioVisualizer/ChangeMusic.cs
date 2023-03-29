using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChangeMusic : MonoBehaviour
{
    [SerializeField]
    private Button _stereoButton = null;

    [SerializeField]
    private Button _monoButton = null;

    [SerializeField]
    private TMP_Text _musicName = null;

    [SerializeField]
    private TMP_Text _musicType = null;

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip _stereoAudio = null;

    [SerializeField]
    private AudioClip _monoAudio = null;

    [SerializeField]
    private VisualizeManager _visualizeManager = null;

    void Start()
    {
        _stereoButton.onClick.AddListener(() => {
            _visualizeManager.streoandMono = eStreoandMono.streo;

            _visualizeManager.Play(_stereoAudio);

            _musicName.text = _stereoAudio.name;
            _musicType.text = "streo";
        });

        _monoButton.onClick.AddListener(() => {
            _visualizeManager.streoandMono = eStreoandMono.mono;
            
            _visualizeManager.Play(_monoAudio);

            _musicName.text = _monoAudio.name;
            _musicType.text = "mono";
        });
    }
}
