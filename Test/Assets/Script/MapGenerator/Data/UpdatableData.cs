using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGanerate
{
    public class UpdatableData : ScriptableObject
    {
        public event System.Action onValuesUpdated;

        public bool autoUpdate;

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (autoUpdate == true)
            {
                UnityEditor.EditorApplication.update += NotifyOfUodatedValues;
            }
        }

        public void NotifyOfUodatedValues()
        {
            if (onValuesUpdated != null)
            {
                onValuesUpdated();
            }
        }

#endif
    }
}
