using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PWCommon5;
namespace GeNa.Core
{
    [CustomEditor(typeof(RoadProfile))]
    public class RoadProfileEditor : GeNaRoadProfileEditor
    {
        private string[] statList =
        {
            "PW Shader",
            "Material",
            // Shouldn't exist for roads
            // "RiverFlow"
        };
        private EditorUtils m_editorUtils;
        protected override void OnEnable()
        {
            if (m_editorUtils == null)
            {
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this, "GeNaRoadProfileEditor", null);
            }
        }
        public override void OnInspectorGUI()
        {
            m_editorUtils.Initialize();
            if (m_profile == null)
            {
                m_profile = (RoadProfile)target;
            }
            m_editorUtils.Panel("ProfileSettings", ProfilePanel, false);
        }
        private void ProfilePanel(bool helpEnabled)
        {
            if (m_profile == null)
                 return;
            EditorGUI.BeginChangeCheck();
            Constants.RenderPipeline pipeline = GeNaUtility.GetActivePipeline();
            m_profile.RoadParameters.m_renderMode = (Constants.ProfileRenderMode)EditorGUILayout.Popup("Render Mode", (int)m_profile.RoadParameters.m_renderMode, statList);
            m_editorUtils.InlineHelp("RenderMode", helpEnabled);
            if (m_profile.RoadParameters.m_renderMode != Constants.ProfileRenderMode.RiverFlow)
            {
                m_editorUtils.Heading("RoadShadersSettings");
                m_editorUtils.InlineHelp("RoadShadersSettings", helpEnabled);
                EditorGUI.indentLevel++;
                switch (m_profile.RoadParameters.m_renderMode)
                {
                    case Constants.ProfileRenderMode.PWShader:
                        m_profile.RoadParameters.BuiltInRoadShader = (Shader)m_editorUtils.ObjectField("RoadBuilt-InShader", m_profile.RoadParameters.BuiltInRoadShader, typeof(Shader), false, helpEnabled);
                        m_profile.RoadParameters.UniversalRoadShader = (Shader)m_editorUtils.ObjectField("RoadUniversalShader", m_profile.RoadParameters.UniversalRoadShader, typeof(Shader), false, helpEnabled);
                        m_profile.RoadParameters.HighDefinitionRoadShader = (Shader)m_editorUtils.ObjectField("RoadHighDefinitionShader", m_profile.RoadParameters.HighDefinitionRoadShader, typeof(Shader), false, helpEnabled);
                        break;
                    case Constants.ProfileRenderMode.Material:
                        m_profile.RoadParameters.m_builtInRoadMaterial = (Material)m_editorUtils.ObjectField("RoadBuilt-InMaterial", m_profile.RoadParameters.m_builtInRoadMaterial, typeof(Material), false, helpEnabled);
                        m_profile.RoadParameters.m_universalRoadMaterial = (Material)m_editorUtils.ObjectField("RoadUniversalMaterial", m_profile.RoadParameters.m_universalRoadMaterial, typeof(Material), false, helpEnabled);
                        m_profile.RoadParameters.m_highDefinitionRoadMaterial = (Material)m_editorUtils.ObjectField("RoadHighDefinitionMaterial", m_profile.RoadParameters.m_highDefinitionRoadMaterial, typeof(Material), false, helpEnabled);
                        break;
                }
                EditorGUI.indentLevel--;
                m_editorUtils.Heading("IntersectionShadersSettings");
                m_editorUtils.InlineHelp("IntersectionShadersSettings", helpEnabled);
                EditorGUI.indentLevel++;
                switch (m_profile.RoadParameters.m_renderMode)
                {
                    case Constants.ProfileRenderMode.PWShader:
                        m_profile.RoadParameters.BuiltInIntersectionRoadShader = (Shader)m_editorUtils.ObjectField("IntersectionBuilt-InShader", m_profile.RoadParameters.BuiltInIntersectionRoadShader, typeof(Shader), false, helpEnabled);
                        m_profile.RoadParameters.UniversalIntersectionRoadShader = (Shader)m_editorUtils.ObjectField("IntersectionUniversalShader", m_profile.RoadParameters.UniversalIntersectionRoadShader, typeof(Shader), false, helpEnabled);
                        m_profile.RoadParameters.HighDefinitionIntersectionRoadShader = (Shader)m_editorUtils.ObjectField("IntersectionHighDefinitionShader", m_profile.RoadParameters.HighDefinitionIntersectionRoadShader, typeof(Shader), false, helpEnabled);
                        break;
                    case Constants.ProfileRenderMode.Material:
                        m_profile.RoadParameters.m_builtInIntersectionMaterial = (Material)m_editorUtils.ObjectField("IntersectionBuilt-InMaterial", m_profile.RoadParameters.m_builtInIntersectionMaterial, typeof(Material), false, helpEnabled);
                        m_profile.RoadParameters.m_universalIntersectionMaterial = (Material)m_editorUtils.ObjectField("IntersectionUniversalMaterial", m_profile.RoadParameters.m_universalIntersectionMaterial, typeof(Material), false, helpEnabled);
                        m_profile.RoadParameters.m_highDefinitionIntersectionMaterial = (Material)m_editorUtils.ObjectField("IntersectionHighDefinitionMaterial", m_profile.RoadParameters.m_highDefinitionIntersectionMaterial, typeof(Material), false, helpEnabled);
                        break;
                }
                EditorGUI.indentLevel--;
                if (m_profile.RoadParameters.m_renderMode == Constants.ProfileRenderMode.PWShader)
                {
                    EditorGUILayout.Space();
                    m_editorUtils.Heading("AlbedoSettings");
                    m_editorUtils.InlineHelp("AlbedoSettings", helpEnabled);
                    EditorGUI.indentLevel++;
                    m_editorUtils.LabelField("Road");
                    EditorGUI.indentLevel++;
                    m_profile.RoadParameters.m_roadAlbedoMap = (Texture2D)m_editorUtils.ObjectField("AlbedoMap", m_profile.RoadParameters.m_roadAlbedoMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                    m_profile.RoadParameters.m_roadTintColor = m_editorUtils.ColorField("TintColor", m_profile.RoadParameters.m_roadTintColor, helpEnabled);
                    EditorGUI.indentLevel--;
                    m_editorUtils.LabelField("Intersection");
                    EditorGUI.indentLevel++;
                    m_profile.RoadParameters.m_intersectionAlbedoMap = (Texture2D)m_editorUtils.ObjectField("AlbedoMap", m_profile.RoadParameters.m_intersectionAlbedoMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                    m_profile.RoadParameters.m_intersectionTintColor = m_editorUtils.ColorField("TintColor", m_profile.RoadParameters.m_intersectionTintColor, helpEnabled);
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                    m_editorUtils.Heading("NormalSettings");
                    m_editorUtils.InlineHelp("NormalSettings", helpEnabled);
                    EditorGUI.indentLevel++;
                    m_editorUtils.LabelField("Road");
                    EditorGUI.indentLevel++;
                    m_profile.RoadParameters.m_roadNormalMap = (Texture2D)m_editorUtils.ObjectField("NormalMap", m_profile.RoadParameters.m_roadNormalMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                    m_profile.RoadParameters.m_roadNormalStrength = m_editorUtils.Slider("NormalStrength", m_profile.RoadParameters.m_roadNormalStrength, 0f, 5f, helpEnabled);
                    EditorGUI.indentLevel--;
                    m_editorUtils.LabelField("Intersection");
                    EditorGUI.indentLevel++;
                    m_profile.RoadParameters.m_intersectionNormalMap = (Texture2D)m_editorUtils.ObjectField("NormalMap", m_profile.RoadParameters.m_intersectionNormalMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                    m_profile.RoadParameters.m_intersectionNormalStrength = m_editorUtils.Slider("NormalStrength", m_profile.RoadParameters.m_intersectionNormalStrength, 0f, 5f, helpEnabled);
                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                    if (pipeline != Constants.RenderPipeline.HighDefinition)
                    {
                        bool displayOldSettings = true;

                        //URP
                        if (pipeline == Constants.RenderPipeline.Universal)
                        {
                            bool customShaderMatch = GeNaRoadShaderID.URPRoadShaderCustom == m_profile.RoadParameters.m_universalRoadShaderName;
                            if (customShaderMatch)
                            {
                                displayOldSettings = false;
                            }
                        }
                        //Built in
                        else
                        {
                            bool customShaderMatch = GeNaRoadShaderID.BuiltInRoadShaderCustom == m_profile.RoadParameters.m_builtInRoadShaderName;
                            if (customShaderMatch)
                            {
                                displayOldSettings = false;
                            }
                        }
                        if (displayOldSettings)
                        {
                            m_editorUtils.Heading("PBRSettings");
                            m_editorUtils.InlineHelp("PBRSettings", helpEnabled);
                            EditorGUI.indentLevel++;
                            m_editorUtils.LabelField("Road");
                            EditorGUI.indentLevel++;
                            if (m_profile.RoadParameters.m_roadMetallicMap == null)
                            {
                                m_profile.RoadParameters.m_roadMetallicMap = (Texture2D)m_editorUtils.ObjectField("MetallicMap", m_profile.RoadParameters.m_roadMetallicMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                                m_profile.RoadParameters.m_roadMetallic = m_editorUtils.Slider("Metallic", m_profile.RoadParameters.m_roadMetallic, 0f, 1f, helpEnabled);
                            }
                            else
                            {
                                m_profile.RoadParameters.m_roadMetallicMap = (Texture2D)m_editorUtils.ObjectField("MetallicMap", m_profile.RoadParameters.m_roadMetallicMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                            }
                            m_profile.RoadParameters.m_roadOcclusionMap = (Texture2D)m_editorUtils.ObjectField("OcclusionMap", m_profile.RoadParameters.m_roadOcclusionMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                            m_profile.RoadParameters.m_roadOcclusionStrength = m_editorUtils.Slider("OcclusionStrength", m_profile.RoadParameters.m_roadOcclusionStrength, 0f, 1f, helpEnabled);
                            m_profile.RoadParameters.m_roadSmoothness = m_editorUtils.Slider("Smoothness", m_profile.RoadParameters.m_roadSmoothness, 0f, 1f, helpEnabled);
                            EditorGUI.indentLevel--;
                            m_editorUtils.LabelField("Intersection");
                            EditorGUI.indentLevel++;
                            if (m_profile.RoadParameters.m_intersectionMetallicMap == null)
                            {
                                m_profile.RoadParameters.m_intersectionMetallicMap = (Texture2D)m_editorUtils.ObjectField("MetallicMap", m_profile.RoadParameters.m_intersectionMetallicMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                                m_profile.RoadParameters.m_intersectionMetallic = m_editorUtils.Slider("Metallic", m_profile.RoadParameters.m_intersectionMetallic, 0f, 1f, helpEnabled);
                            }
                            else
                            {
                                m_profile.RoadParameters.m_intersectionMetallicMap = (Texture2D)m_editorUtils.ObjectField("MetallicMap", m_profile.RoadParameters.m_intersectionMetallicMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                            }
                            m_profile.RoadParameters.m_intersectionOcclusionMap = (Texture2D)m_editorUtils.ObjectField("OcclusionMap", m_profile.RoadParameters.m_intersectionOcclusionMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                            m_profile.RoadParameters.m_intersectionOcclusionStrength = m_editorUtils.Slider("OcclusionStrength", m_profile.RoadParameters.m_intersectionOcclusionStrength, 0f, 1f, helpEnabled);
                            m_profile.RoadParameters.m_intersectionSmoothness = m_editorUtils.Slider("Smoothness", m_profile.RoadParameters.m_intersectionSmoothness, 0f, 1f, helpEnabled);
                            EditorGUI.indentLevel--;
                            EditorGUILayout.Space();
                            m_editorUtils.Heading("HeightSettings");
                            m_editorUtils.InlineHelp("HeightSettings", helpEnabled);
                            m_editorUtils.LabelField("Road");
                            EditorGUI.indentLevel++;
                            m_profile.RoadParameters.m_roadHeightMap = (Texture2D)m_editorUtils.ObjectField("HeightMap", m_profile.RoadParameters.m_roadHeightMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                            m_profile.RoadParameters.m_roadHeightStrength = m_editorUtils.Slider("HeightStrength", m_profile.RoadParameters.m_roadHeightStrength, 0f, 1f, helpEnabled);
                            EditorGUI.indentLevel--;
                            m_editorUtils.LabelField("Intersection");
                            EditorGUI.indentLevel++;
                            m_profile.RoadParameters.m_intersectionHeightMap = (Texture2D)m_editorUtils.ObjectField("HeightMap", m_profile.RoadParameters.m_intersectionHeightMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                            m_profile.RoadParameters.m_intersectionHeightStrength = m_editorUtils.Slider("HeightStrength", m_profile.RoadParameters.m_intersectionHeightStrength, 0f, 1f, helpEnabled);
                            EditorGUI.indentLevel--;
                            EditorGUILayout.Space();
                        }
                    }
                    else
                    {
                        m_editorUtils.Heading("MaskMapSettings");
                        m_editorUtils.InlineHelp("MaskMapSettings", helpEnabled);
                        m_editorUtils.LabelField("Road");
                        EditorGUI.indentLevel++;
                        m_profile.RoadParameters.m_roadMaskMap = (Texture2D)m_editorUtils.ObjectField("MaskMap", m_profile.RoadParameters.m_roadMaskMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                        EditorGUI.indentLevel--;
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                        EditorGUI.indentLevel++;
                        m_editorUtils.LabelField("Intersection");
                        EditorGUI.indentLevel++;
                        m_profile.RoadParameters.m_intersectionMaskMap = (Texture2D)m_editorUtils.ObjectField("MaskMap", m_profile.RoadParameters.m_intersectionMaskMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                    }
                    if (pipeline != Constants.RenderPipeline.HighDefinition)
                    {
                        m_editorUtils.Heading("MaskMapSettings");
                        m_editorUtils.InlineHelp("MaskMapSettings", helpEnabled);
                        m_editorUtils.LabelField("Road");
                        EditorGUI.indentLevel++;
                        m_profile.RoadParameters.m_roadMaskMap = (Texture2D)m_editorUtils.ObjectField("MaskMap", m_profile.RoadParameters.m_roadMaskMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                        m_editorUtils.LabelField("Intersection");
                        EditorGUI.indentLevel++;
                        m_profile.RoadParameters.m_intersectionMaskMap = (Texture2D)m_editorUtils.ObjectField("MaskMap", m_profile.RoadParameters.m_intersectionMaskMap, typeof(Texture2D), false, helpEnabled, GUILayout.MaxHeight(16f));
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                    }
                    bool displayNewSettings = false;
                    switch (pipeline)
                    {
                        case Constants.RenderPipeline.BuiltIn:
                            if (m_profile.RoadParameters.m_builtInRoadShaderName != null)
                            {
                                displayNewSettings = (GeNaRoadShaderID.BuiltInRoadShaderCustom == m_profile.RoadParameters.m_builtInRoadShaderName);
                            }
                            break;
                        case Constants.RenderPipeline.Universal:
                            if (m_profile.RoadParameters.m_universalRoadShaderName != null)
                            {
                                displayNewSettings = (GeNaRoadShaderID.URPRoadShaderCustom == m_profile.RoadParameters.m_universalRoadShaderName);
                            }
                            break;
                        case Constants.RenderPipeline.HighDefinition:
                            if (m_profile.RoadParameters.m_highDefinitionRoadShaderName != null)
                            {
                                displayNewSettings = (GeNaRoadShaderID.HDRPRoadShaderCustom == m_profile.RoadParameters.m_highDefinitionRoadShaderName);
                            }
                            break;
                    }
                    if (displayNewSettings)
                    {
                        //Mask Map Parameters
                        EditorGUI.indentLevel++;
                        m_editorUtils.LabelField("Mask Parameters");
                        EditorGUI.indentLevel++;
                        m_profile.RoadParameters.m_maskMapMetallic = m_editorUtils.Slider("(R) - Metallic", m_profile.RoadParameters.m_maskMapMetallic, 0.01f, 2f, helpEnabled);
                        m_profile.RoadParameters.m_maskMapAO = m_editorUtils.Slider("(G) - AO", m_profile.RoadParameters.m_maskMapAO, 0.01f, 2f, helpEnabled);
                        m_profile.RoadParameters.m_maskMapHeight = m_editorUtils.Slider("(B) - Height", m_profile.RoadParameters.m_maskMapHeight, 0.01f, 2f, helpEnabled);
                        m_profile.RoadParameters.m_maskMapSmoothness = m_editorUtils.Slider("(A) - Smoothness", m_profile.RoadParameters.m_maskMapSmoothness, 0.01f, 2f, helpEnabled);
                        EditorGUI.indentLevel--;
                        EditorGUILayout.Space();
                        if (pipeline != Constants.RenderPipeline.BuiltIn)
                        {
                            m_editorUtils.Heading("Tiling");
                            m_editorUtils.InlineHelp("Tiling", helpEnabled);
                            m_profile.RoadParameters.m_scaleFromCenter = m_editorUtils.Toggle("Scale From Center", m_profile.RoadParameters.m_scaleFromCenter, helpEnabled);
                            m_profile.RoadParameters.m_uvTiling = m_editorUtils.Vector2Field("UV Tiling", m_profile.RoadParameters.m_uvTiling, helpEnabled);
                            EditorGUILayout.Space();
                            m_editorUtils.Heading("Edge Blending");
                            m_editorUtils.InlineHelp("Edge Blending", helpEnabled);
                            m_profile.RoadParameters.m_edgeBlend = m_editorUtils.Slider("Edge Distance", 1 - m_profile.RoadParameters.m_edgeBlend, 0f, 1f, helpEnabled);
                            m_profile.RoadParameters.m_edgeBlend = 1 - m_profile.RoadParameters.m_edgeBlend;
                            m_profile.RoadParameters.m_edgeBlendPower = m_editorUtils.Slider("Edge Contrast", m_profile.RoadParameters.m_edgeBlendPower, 1f, 60f, helpEnabled);
                            EditorGUILayout.Space();
                            m_editorUtils.Heading("Road Pattern");
                            m_editorUtils.InlineHelp("Road Pattern", helpEnabled);
                            m_profile.RoadParameters.m_doubleTrackRoads = m_editorUtils.Toggle("Double Track Roads", m_profile.RoadParameters.m_doubleTrackRoads, helpEnabled);
                            if (m_profile.RoadParameters.m_doubleTrackRoads)
                            {
                                m_profile.RoadParameters.m_roadShape = 1f;
                                m_profile.RoadParameters.m_doubleTrackPosition = m_editorUtils.Slider("Double Track Position", m_profile.RoadParameters.m_doubleTrackPosition, 1f, 30f, helpEnabled);
                                m_profile.RoadParameters.m_doubleTrackRange = m_editorUtils.Slider("Double Track Range", m_profile.RoadParameters.m_doubleTrackRange, 1f, 5f, helpEnabled);
                            }
                            else
                            {
                                m_profile.RoadParameters.m_roadShape = 0f;
                                m_profile.RoadParameters.m_middleTrackPosition = m_editorUtils.Slider("Middle Track Position", m_profile.RoadParameters.m_middleTrackPosition, 0f, 1f, helpEnabled);
                                m_profile.RoadParameters.m_middleTrackRange = m_editorUtils.Slider("Middle Track Range", m_profile.RoadParameters.m_middleTrackRange, 1f, 20f, helpEnabled);
                            }
                            EditorGUILayout.Space();
                            m_editorUtils.Heading("Height Adjustments");
                            m_editorUtils.InlineHelp("Height Adjustments", helpEnabled);
                            m_profile.RoadParameters.m_heightContrast = m_editorUtils.Slider("Height Contrast", m_profile.RoadParameters.m_heightContrast, 0f, 1f, helpEnabled);
                            m_profile.RoadParameters.m_heightTransition = m_editorUtils.Slider("Height Transition", m_profile.RoadParameters.m_heightTransition, 0f, 5f, helpEnabled);
                            EditorGUILayout.Space();
                            m_editorUtils.Heading("Ground Blending");
                            m_editorUtils.InlineHelp("Ground Blending", helpEnabled);
                            m_profile.RoadParameters.m_blendWithGround = m_editorUtils.Slider("Blend With Ground", m_profile.RoadParameters.m_blendWithGround, 0f, 1f, helpEnabled);
                            m_profile.RoadParameters.m_inGroundPush = m_editorUtils.Slider("Ground Height", m_profile.RoadParameters.m_inGroundPush, 0f, 1f, helpEnabled);
                        }
                        EditorGUILayout.Space();
                        m_editorUtils.Heading("Distance Offset");
                        m_editorUtils.InlineHelp("Distance Offset", helpEnabled);
                        m_profile.RoadParameters.m_terrainLODOffset = m_editorUtils.Slider("Terrain LOD Offset", m_profile.RoadParameters.m_terrainLODOffset, 0f, 5f, helpEnabled);
                        m_profile.RoadParameters.m_terrainLODDistance = m_editorUtils.FloatField("Terrain LOD Distance", m_profile.RoadParameters.m_terrainLODDistance, helpEnabled);
                        EditorGUI.indentLevel--;
                        if (pipeline != Constants.RenderPipeline.BuiltIn)
                        {
                            m_editorUtils.Heading("Noise");
                            m_editorUtils.InlineHelp("Noise", helpEnabled);
                            EditorGUI.indentLevel++;
                            m_editorUtils.Fractal(m_profile.RoadParameters.m_noise, helpEnabled, false);
                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("River Profile Mode disabled for Road Profiles!", EditorStyles.boldLabel);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(m_profile);
            }
        }
    }
}