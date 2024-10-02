using UnityEngine;
using UnityEditor;
namespace GeNa.Core
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GeNaTerrainDecorator))]
    public class GeNaTerrainDecoratorEditor : GeNaDecoratorEditor<GeNaTerrainDecorator>
    {
        [MenuItem("GameObject/GeNa/Decorators/Terrain Decorator")]
        public static void AddDecorator(MenuCommand command) => GeNaDecoratorEditorUtility.CreateDecorator<GeNaTerrainDecorator>(command);
        protected GeNaTerrainDecorator[] m_tree;
        protected TerrainTools m_terrainTools;
        protected TerrainEntity m_terrainEntity;
        public TerrainTools TerrainTools
        {
            get
            {
                if (m_terrainTools == null)
                {
                    GeNaManager gm = GeNaManager.GetInstance();
                    if (gm != null)
                    {
                        m_terrainTools = gm.TerrainTools;
                    }
                }
                return m_terrainTools;
            }
        }
        protected void SelectTree(bool isSelected)
        {
            Transform transform = Decorator.transform;
            Transform root = transform.root;
            m_tree = root.GetComponentsInChildren<GeNaTerrainDecorator>();
            foreach (GeNaTerrainDecorator tree in m_tree)
                tree.IsSelected = isSelected;
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (Decorator == null)
                return;
            SelectTree(true);
        }
        private void OnDisable()
        {
            if (Decorator == null)
                return;
            SelectTree(false);
        }
        public override void OnSceneGUI()
        {
            GeNaTerrainDecorator decorator = target as GeNaTerrainDecorator;
            if (decorator == null)
                return;
            Transform transform = decorator.transform;
            TerrainModifier modifier = decorator.TerrainModifier;
            GeNaEditorUtility.RenderTerrainModifier(transform, modifier);
            if (m_terrainEntity != null)
            {
                m_terrainEntity.Dispose();
                m_terrainEntity = null;
            }
            m_terrainEntity = modifier.GenerateTerrainEntity();
            if (m_terrainEntity != null)
            {
                TerrainTools.Visualize(m_terrainEntity);
            }
        }
        protected void AddBrushTexture(Texture2D texture2D)
        {
            var terrainModifier = Decorator.TerrainModifier;
            if (GeNaSpawner != null)
            {
                var palette = GeNaSpawner.Palette;
                if (palette != null)
                {
                    int id = palette.AddObject(texture2D);
                    if (palette.IsValidID(id))
                    {
                        terrainModifier.AddBrushTexture(texture2D);
                        terrainModifier.BrushTextureIDs.Add(id);
                    }
                }
            }
            else
            {
                terrainModifier.AddBrushTexture(texture2D);
            }
        }
        protected override void RenderPanel(bool helpEnabled)
        {
            EditorGUI.BeginChangeCheck();
            {
                EditorUtils.TerrainModifier(Decorator.TerrainModifier, helpEnabled, AddBrushTexture);
            }
            if (EditorGUI.EndChangeCheck())
            {
                // EditorUtility.SetDirty(Decorator);
                foreach (Object o in targets)
                {
                    GeNaTerrainDecorator decorator = (GeNaTerrainDecorator)o;
                    decorator.TerrainModifier.CopyFrom(Decorator.TerrainModifier);
                    EditorUtility.SetDirty(decorator);
                }
            }
        }
    }
}