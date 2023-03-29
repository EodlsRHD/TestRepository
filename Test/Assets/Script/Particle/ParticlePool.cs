using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Particle
{
    public class ParticlePool : MonoBehaviour
    {
        [Header("Pool")]
        [SerializeField]
        private int _objPoolCount = 3;

        [SerializeField]
        private int _poolMaxCount = 10;

        [Header("Number of Instantiate particle")]
        [SerializeField]
        private int _instantiateCount = 0;

        private Dictionary<eParticleType, ParticleClass> _originalParticles = new Dictionary<eParticleType, ParticleClass>();

        private Dictionary<eParticleType, Stack<ParticleClass>> _particlePools = new Dictionary<eParticleType, Stack<ParticleClass>>();

        public void Initialize(List<ParticleClass> originalParticles)
        {
            foreach(var particle in originalParticles)
            {
                _originalParticles.Add(particle.type, particle);

                Initialize_InstantiateParticle(particle.type, out Stack<ParticleClass> pool);
                _particlePools.Add(particle.type, pool);
            }
        }

        public void RequestPool(eParticleType type, Action<ParticleClass> onCallbackInitalize)
        {
            RequestPool_InstantiateParticle(type, out ParticleClass particleClass);
            onCallbackInitalize?.Invoke(particleClass);
        }

        private void ReturnPool(ParticleClass particleClass)
        {
            if (_particlePools[particleClass.type].Count >= _poolMaxCount)
            {
                Destroy(particleClass.gameObject);
                return;
            }

            particleClass.Reset(this.transform);

            _particlePools[particleClass.type].Push(particleClass);
        }

        private void Initialize_InstantiateParticle(eParticleType type, out Stack<ParticleClass> pool)
        {
            pool = new Stack<ParticleClass>();

            for (int i = 0; i < _objPoolCount; i++)
            {
                ParticleClass particle = Instantiate(_originalParticles[type], this.transform);
                particle.Initialize(MakeID(), ReturnPool);
                pool.Push(particle);
            }
        }

        private void RequestPool_InstantiateParticle(eParticleType type, out ParticleClass particleClass)
        {
            if (_particlePools[type].Count == 0)
            {
                ParticleClass particle = Instantiate(_originalParticles[type], this.transform);
                particle.Initialize(MakeID(), ReturnPool);
                _particlePools[type].Push(particle);
            }

            particleClass = _particlePools[type].Pop();
        }

        private int MakeID()
        {
            return _instantiateCount++;
        }
    }
}