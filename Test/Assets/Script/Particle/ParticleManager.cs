using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Particle
{
    // The number of particles and the number of 'enum eParticleType' must be the same.
    public enum eParticleType
    {
        Non,
        fire_blue,
        fire_green,
        fire_orange,
        fire_red,
        fire_yellow,
        nuke,
        smoke_blue,
        smoke_green,
        smoke_red,
        smoke_yellow
    }

    public class ParticleManager : MonoBehaviour
    {
        [Header("Particle")]
        [SerializeField]
        private eParticleType _type = eParticleType.Non;

        [SerializeField][Tooltip("The number of particles and the number of 'enum eParticleType' must be the same.")]
        private List<ParticleClass> _particleList = new List<ParticleClass>();

        [Header("Pool")]
        [SerializeField]
        private ParticlePool _particlePool = null;

        [Header("Particles Spread Position")]
        [SerializeField]
        private List<ParticleType> _particleObjs = new List<ParticleType>();

        private List<ParticleClass> _activeParticles = new List<ParticleClass>();

        public void Initialize()
        {
            _particlePool.Initialize(_particleList);
            foreach (var one in _particleObjs)
            {
                one.Initialize(_particlePool.RequestPool, AddListActiveParticle);
            }
        }

        public void StartParticle(bool isControll, eParticleType type)
        {
            if (isControll == true)
            {
                _type = type;

                if (_type == eParticleType.Non)
                {
                    Debug.LogError("Please select the particle TYPE");
                    return;
                }

                foreach (var particleObj in _particleObjs)
                {
                    particleObj.StartParticle(_type);
                }

                return;
            }

            foreach (var particleObj in _particleObjs)
            {
                particleObj.StartParticle();
            }
        }

        private void AddListActiveParticle(ParticleClass particleClass)
        {
            _activeParticles.Add(particleClass);
        }

        public void StopParticle()
        {
            foreach(var particle in _activeParticles)
            {
                particle.StopParticle();
            }

            _activeParticles.Clear();
        }
    }
}