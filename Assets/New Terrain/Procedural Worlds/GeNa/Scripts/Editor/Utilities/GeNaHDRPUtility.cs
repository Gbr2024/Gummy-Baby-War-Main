using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
#if GeNa_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif
namespace GeNa.Core
{
    public static class GeNaHDRPUtility
    {
#if GeNa_HDRP
        public static void DisplayWarnings()
        {
            var pipeline = GeNaUtility.GetActivePipeline();
            if (pipeline == Constants.RenderPipeline.HighDefinition)
            {
                var frameSettings = GetCurrentDefaultFrameSettings();
                if (!frameSettings.IsEnabled(FrameSettingsField.AfterPostprocess))
                {
                    EditorGUILayout.HelpBox("WARNING! 'AfterPostprocess' is currently Disabled in the current Pipeline asset.\n" +
                                            "In order to see visualization, you must have this turned on.", MessageType.Warning);
                    if (GUILayout.Button("Turn on AfterPostprocess"))
                    {
                        frameSettings.SetEnabled(FrameSettingsField.AfterPostprocess, true);
                        SetCurrentDefaultFrameSettings(frameSettings);
                    }
                }
            }
        }
        public static FrameSettings GetCurrentDefaultFrameSettings()
        {
            var pipelineAsset = GraphicsSettings.defaultRenderPipeline;
            if (pipelineAsset != null)
            {
                HDRenderPipelineAsset hdrpPipelineAsset = pipelineAsset as HDRenderPipelineAsset;
                if (hdrpPipelineAsset != null)
                {
                    MemberInfo[] defaultFrameSettingsMember = pipelineAsset.GetType().GetMember("m_RenderingPathDefaultCameraFrameSettings", BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MemberInfo memberInfo in defaultFrameSettingsMember)
                    {
                        if (memberInfo.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fieldInfo = memberInfo as FieldInfo;
                            var frameSettings = (FrameSettings)fieldInfo.GetValue(pipelineAsset);
                            return frameSettings;
                        }
                    }
                }
            }
            return default;
        }
        public static void SetCurrentDefaultFrameSettings(FrameSettings frameSettings)
        {
            var pipelineAsset = GraphicsSettings.defaultRenderPipeline;
            if (pipelineAsset != null)
            {
                HDRenderPipelineAsset hdrpPipelineAsset = pipelineAsset as HDRenderPipelineAsset;
                if (hdrpPipelineAsset != null)
                {
                    MemberInfo[] defaultFrameSettingsMember = pipelineAsset.GetType().GetMember("m_RenderingPathDefaultCameraFrameSettings", BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (MemberInfo memberInfo in defaultFrameSettingsMember)
                    {
                        if (memberInfo.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fieldInfo = memberInfo as FieldInfo;
                            fieldInfo.SetValue(pipelineAsset, frameSettings);
                        }
                    }
                    EditorUtility.SetDirty(pipelineAsset);
                }
            }
        }
#endif
    }
}