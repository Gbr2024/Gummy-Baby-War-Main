using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
#endif
namespace GeNa.Core
{
    /// <summary>
    /// Decorator for modifying Terrain
    /// </summary>
    [ExecuteAlways]
    public class GeNaTerrainDecorator : MonoBehaviour, IDecorator
    {
        [SerializeField] protected TerrainModifier m_terrainModifier = new TerrainModifier();
        [NonSerialized] private bool m_isSelected = false;
        public TerrainModifier TerrainModifier => m_terrainModifier;
        public bool IsSelected
        {
            get => m_isSelected;
            set => m_isSelected = value;
        }
        public bool UnpackPrefab => false;
        public void Update()
        {
            if (!m_isSelected)
                return;
            m_terrainModifier.Position = transform.position;
            m_terrainModifier.RotationY = transform.eulerAngles.y;
            m_terrainModifier.UpdateTerrain = false;
        }
        public void OnIngest(Resource resource)
        {
            resource.HasHeights = true;
            Palette palette = resource.Palette;
            m_terrainModifier.AddBrushTextures(m_terrainModifier.BrushTextures, palette);
        }
        public IEnumerator OnSelfSpawned(Resource resource)
        {
            if (TerrainModifier != null && TerrainModifier.Enabled)
            {
                TerrainModifier.Position = transform.position;
                TerrainModifier.UpdateTerrain = true;
                TerrainEntity terrainEntity = TerrainModifier.GenerateTerrainEntity();
                GeNaSpawnerData spawnerData = resource.SpawnerData;
                UndoRecord undoRecord = spawnerData.UndoRecord;
                // TODO : Manny : Need to ensure this works at Runtime!
                if (terrainEntity != null)
                {
                    // Append the Undo Record (if there is one)
                    undoRecord.Record(terrainEntity);
                    terrainEntity.Perform();
                }
            }
            yield break;
        }
        public void OnChildrenSpawned(Resource resource)
        {
            GeNaEvents.Destroy(this);
        }
        public void LoadReferences(Palette palette)
        {          
            m_terrainModifier.BrushTextures.Clear();
            foreach (int id in m_terrainModifier.BrushTextureIDs)
            {
                Texture2D texture2D = palette.GetObject<Texture2D>(id);
                if (texture2D != null)
                    m_terrainModifier.BrushTextures.Add(texture2D);
            }
        }
    }
}