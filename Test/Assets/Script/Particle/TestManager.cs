using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Particle
{

    public class TestManager : MonoBehaviour
    {
        [SerializeField]
        private ParticleManager _particleManager = null;

        [Header("Test")]
        [SerializeField]
        private Button _startButton = null; // TestCode

        [SerializeField]
        private Button _stopButton = null; // TestCode

        [Space(20)]

        [SerializeField]
        private bool _isControll = false;

        [SerializeField]
        private eParticleType _type = eParticleType.Non;

        private void Start()
        {
            _particleManager.Initialize();

            _startButton.onClick.AddListener(() => { _particleManager.StartParticle(_isControll, _type); });
            _stopButton.onClick.AddListener(_particleManager.StopParticle);
        }
    }
}
