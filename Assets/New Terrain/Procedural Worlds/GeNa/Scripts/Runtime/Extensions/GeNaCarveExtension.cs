using System;
using UnityEngine;
namespace GeNa.Core
{
    /// <summary>
    /// Spline Extension for performing Terrain Carve operations (note: only works on Terrains)
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Carve", menuName = "Procedural Worlds/GeNa/Extensions/Carve", order = 0)]
    public class GeNaCarveExtension : GeNaSplineExtension
    {
        #region Constants
        /// <summary>
        /// Maximum length of Spline before Preview defaults to off (on selection)
        /// </summary>
        private const float MAX_PREVIEW_LENGTH = 5000.0f;
        #endregion
        #region Variables
        // Compute Shader
        [SerializeField] protected float m_width = 5f;
        [SerializeField] protected float m_heightOffset = 0.0f;
        [SerializeField] protected float m_shoulder = 1.5f;
        [SerializeField] protected bool m_roadLike = false;
        [SerializeField] protected bool m_showPreview = true;
        [SerializeField] protected Fractal m_maskFractal = new Fractal();
        [SerializeField] protected Color m_positiveColor = new Color(0.5611f, 0.9716f, 0.5362f, 1f);
        [SerializeField] protected Color m_negativeColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        [NonSerialized] protected TerrainTools m_terrainTools;
        [SerializeField] protected AnimationCurve m_shoulderFalloff = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
        [NonSerialized] private bool m_isDirty = false;
        [NonSerialized] private TerrainEntity m_terrainEntity;
        
        [SerializeField] protected TerrainModifier m_terrainModifier = new TerrainModifier();
        #endregion
        #region Properties
        public bool ShowPreview
        {
            get => m_showPreview;
            set => m_showPreview = value;
        }
        public float Width
        {
            get => m_width;
            set => m_width = value;
        }
        public float HeightOffset
        {
            get => m_heightOffset;
            set => m_heightOffset = value;
        }
        // Smoothness
        public float Shoulder
        {
            get => m_shoulder;
            set => m_shoulder = value;
        }

        public bool RoadLike
        {
            get => m_roadLike;
            set => m_roadLike = value;
        }
        public Fractal MaskFractal
        {
            get => m_maskFractal;
            set => m_maskFractal = value;
        }
        public Color PositiveColor
        {
            get => m_positiveColor;
            set => m_positiveColor = value;
        }
        public Color NegativeColor
        {
            get => m_negativeColor;
            set => m_negativeColor = value;
        }
        public AnimationCurve ShoulderFalloff
        {
            get => m_shoulderFalloff;
            set => m_shoulderFalloff = value;
        }
        #endregion
        #region Methods
        private TerrainTools GetTerrainTools()
        {
            if (m_terrainTools == null)
            {
                GeNaManager geNaManager = GeNaManager.GetInstance();
                if (geNaManager != null)
                {
                    m_terrainTools = geNaManager.TerrainTools;
                }
            }
            return m_terrainTools;
        }
        protected  override void OnSceneGUI()
        {
            Visualize();
        }
        protected void UpdateTerrainModifier()
        {
            m_terrainModifier.EffectType = EffectType.Flatten;
        }
        private void Modify(bool recordUndo = true)
        {
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return;
            tools.Width = Width;
            tools.Shoulder = Shoulder;
            tools.RoadLike = RoadLike;
            tools.HeightOffset = HeightOffset;
            tools.MaskFractal = MaskFractal;
            tools.PositiveColor = PositiveColor;
            tools.NegativeColor = NegativeColor;
            tools.ShoulderFalloff = ShoulderFalloff;
            UpdateTerrainModifier();
            TerrainEntity terrainEntity = tools.GenerateTerrainEntity(m_terrainModifier, Spline);
            if (terrainEntity != null)
            {
                if (recordUndo)
                {
                    terrainEntity.name = "Clear Trees";
                    GeNaUndoRedo.RecordUndo(terrainEntity);
                }
                terrainEntity.Perform();
            }
        }
        public void Visualize()
        {
            if (!m_isSelected || !m_showPreview)
                return;
            TerrainTools tools = GetTerrainTools();
            if (tools == null)
                return;
            tools.Width = Width;
            tools.Shoulder = Shoulder;
            tools.RoadLike = RoadLike;
            tools.HeightOffset = HeightOffset;
            tools.MaskFractal = MaskFractal;
            tools.PositiveColor = PositiveColor;
            tools.NegativeColor = NegativeColor;
            tools.ShoulderFalloff = ShoulderFalloff;
            UpdateTerrainModifier();
            if (m_isDirty)
            {
                if (m_terrainEntity != null)
                {
                    m_terrainEntity.Dispose();
                }
                m_terrainEntity = tools.GenerateTerrainEntity(m_terrainModifier, Spline);
                m_isDirty = false;
            }
            if (m_terrainEntity != null)
            {
                tools.Visualize(m_terrainEntity);
            }
        }
        public void Carve()
        {
            Modify();
        }
        protected  override GameObject OnBake(GeNaSpline spline)
        {
            Modify(false);
            return null;
        }
        public override void Execute()
        {
            if (!m_isSelected)
                return;
            Visualize();
        }
        protected override void OnSelect()
        {
            Visualize();
            m_isSelected = true;
            OnSplineDirty();
            // Show the preview if the Spline length is below the maximum preview length
            m_showPreview = Spline.Length <= MAX_PREVIEW_LENGTH;
        }
        protected  override void OnDeselect()
        {
            m_isSelected = false;
        }
        protected  override void OnDelete()
        {
        }
        protected  override void OnSplineDirty()
        {
            m_isDirty = true;
        }
        #endregion
    }
}