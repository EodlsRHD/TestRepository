using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Particle
{
    public class ParticleType : MonoBehaviour
    {
        [SerializeField]
        private eParticleType _type = eParticleType.Non;

        private Action<eParticleType, Action<ParticleClass>> _onCallbackRequestPool = null;

        private Action<ParticleClass> _onCallbackAddListActiveParticle = null;

        public void Initialize(Action<eParticleType, Action<ParticleClass>> onCallbackRequestPool, Action<ParticleClass> onCallbackAddListActiveParticle)
        {
            _onCallbackRequestPool = onCallbackRequestPool;
            _onCallbackAddListActiveParticle = onCallbackAddListActiveParticle;
        }

        public void StartParticle(eParticleType type = eParticleType.Non)
        {
            if (type == eParticleType.Non)
            {
                type = _type;
            }

            _onCallbackRequestPool(type, (particleClass) => {

                if(particleClass == null)
                {
                    return;
                }

                _onCallbackAddListActiveParticle(particleClass);
                particleClass.StartPariticle(this.transform);
            });
        }
    }
}