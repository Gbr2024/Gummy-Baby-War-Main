using System;
using System.Collections;
using UnityEngine;
namespace GeNa.Core
{
    [ExecuteAlways]
    public class GeNaAnimatorDecorator : MonoBehaviour, IDecorator
    {
        #region Variables
        [SerializeField] protected bool m_updateChildren = true;
        [NonSerialized] protected Animator[] m_animators;
        #endregion
        #region Properties
        // Should GeNa Unpack the Prefab that this Decorator is attached to? 
        public bool UnpackPrefab => false;
        public bool UpdateChildren
        {
            get => m_updateChildren;
            set => m_updateChildren = value;
        }
        public Animator[] Animators => m_animators;
        #endregion
        #region Methods
        private void OnEnable()
        {
            if (m_updateChildren)
                m_animators = GetComponentsInChildren<Animator>();
            else
                m_animators = GetComponents<Animator>();
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