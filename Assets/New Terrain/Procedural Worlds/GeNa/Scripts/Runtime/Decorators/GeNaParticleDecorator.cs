using System;
using System.Collections;
using UnityEngine;
namespace GeNa.Core
{
    [ExecuteAlways]
    public class GeNaParticleDecorator : MonoBehaviour, IDecorator
    {
        #region Variables
        [SerializeField] protected bool m_updateChildren = true;
        [SerializeField] protected float m_time = 0f;
        [NonSerialized] protected ParticleSystem[] m_particles;
        #endregion
        #region Properties
        // Should GeNa Unpack the Prefab that this Decorator is attached to? 
        public bool UnpackPrefab => false;
        public bool UpdateChildren
        {
            get => m_updateChildren;
            set => m_updateChildren = value;
        }
        public float Time
        {
            get => m_time;
            set => m_time = value;
        }
        public ParticleSystem[] Particles => m_particles;
        #endregion
        #region Methods
        private void OnEnable()
        {
            m_time = 0f;
            if (m_updateChildren)
                m_particles = GetComponentsInChildren<ParticleSystem>();
            else
                m_particles = GetComponents<ParticleSystem>();
        }
        // Called when Decorator is Ingested into GeNa
        public void OnIngest(Resource resource)
        {
        }
        // Runs once this Decorator is Spawned
        public IEnumerator OnSelfSpawned(Resource resource)
        {
            yield break;
        }
        // Runs directly after Spawning Children Decorators
        public void OnChildrenSpawned(Resource resource)
        {
        }
        public void LoadReferences(Palette palette)
        {
        }
        #endregion
    }
}