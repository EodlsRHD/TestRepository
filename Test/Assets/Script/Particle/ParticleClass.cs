using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Particle
{
    public class ParticleClass : MonoBehaviour
    {
        [SerializeField]
        private eParticleType _type = eParticleType.Non;

        [SerializeField]
        private ParticleSystem _particleSystem = null;

        [SerializeField]
        private int _id = 0;

        private Action<ParticleClass> _onCallbackReturnPool = null;

        public eParticleType type
        {
            get
            {
                return _type;
            }
        }

        public void Initialize(int id, Action<ParticleClass> onCallbackReturnPool)
        {
            _id = id;
            _onCallbackReturnPool = onCallbackReturnPool;
        }

        public void StartPariticle(Transform parant)
        {
            this.transform.SetParent(parant);
            this.transform.position = parant.position;
            this.gameObject.SetActive(true);

            this._particleSystem.Play();
        }

        public void StopParticle()
        {
            this._particleSystem.Stop();
        }

        public void Reset(Transform parant)
        {
            this.transform.SetParent(parant);
            this.transform.position = parant.position;
            this.gameObject.SetActive(false);
        }

        private void OnParticleSystemStopped()
        {
            _onCallbackReturnPool?.Invoke(this);
        }
    }
}
