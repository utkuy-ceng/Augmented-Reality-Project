using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Zappar.Editor
{
    [Flags]
    public enum ZProjectSettings
    {
        None = 0,
        General = 1,    //Scripting back-end; stripping engine code; development build; Realtime reflection
        WebGLCompressionAndTemplate = 2,  //Compression format/fallback; Template
        GraphicsOptimum = 4,  //Platform specific graphics API;
        IosPermissions = 8, // Camera usage description;
        AndroidArchitexture = 16    //Minimum android SDK version
        //RealtimeReflection = 32
    }

    public enum ZProjectSettingsConfig : byte
    {
        WebGLAll = ZProjectSettings.General | ZProjectSettings.WebGLCompressionAndTemplate | ZProjectSettings.GraphicsOptimum,  //7
        WebGLMin = ZProjectSettings.WebGLCompressionAndTemplate,
        AndroidAll = ZProjectSettings.General | ZProjectSettings.GraphicsOptimum | ZProjectSettings.AndroidArchitexture,
        AndroidMin = ZProjectSettings.AndroidArchitexture,
        IosAll = ZProjectSettings.General | ZProjectSettings.GraphicsOptimum | ZProjectSettings.IosPermissions,
        IosMin = ZProjectSettings.IosPermissions
    }

    static class ZResources
    {
#if ZAPPAR_SRP
        public const string TransparentMat = "Packages/com.zappar.uar/Materials/URP/Transparent.mat";
        public const string DepthMaskMat = "Packages/com.zappar.uar/Materials/URP/Depth Mask.mat";
        public const string UVMat = "Packages/com.zappar.uar/Materials/URP/UV.mat";
        public const string DefaultMat = "Packages/com.zappar.uar/Materials/URP/Default.mat";
        public const string InvertedSurfaceMat = "Packages/com.zappar.uar/Materials/InvertedSurface.mat";
#else
        public const string TransparentMat = "Packages/com.zappar.uar/Materials/Transparent.mat";
        public const string DepthMaskMat = "Packages/com.zappar.uar/Materials/Depth Mask.mat";
        public const string UVMat = "Packages/com.zappar.uar/Materials/UV.mat";
        public const string DefaultMat = "Packages/com.zappar.uar/Materials/Default.mat";
        public const string InvertedSurfaceMat = "Packages/com.zappar.uar/Materials/InvertedSurface.mat";
#endif
    }

    internal class ZAssistant
    {
        public static bool MatchConfigSettings(ZProjectSettingsConfig config, ZProjectSettings setting)
        {
            return ((int)config & (int)setting) != 0;
        }


        public static void UpdateUnityProjectSettings(ZProjectSettingsConfig config)
        {
            var uarSettings = AssetDatabase.LoadAssetAtPath<ZapparUARSettings>(ZapparUARSettings.MySettingsPathInPackage);
#if UNITY_WEBGL
            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.General))
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.WebGL, ScriptingImplementation.IL2CPP); //default is IL2CPP
                PlayerSettings.stripEngineCode = true;
                EditorUserBuildSettings.development = false;
                PlayerSettings.runInBackground = true;

                if (uarSettings != null && uarSettings.EnableRealtimeReflections && !QualitySettings.realtimeReflectionProbes)
                {
                    QualitySettings.realtimeReflectionProbes = true;
                    Debug.Log("[NOTE] Enabled realtime reflections. However your build target platform setting may vary! Make sure it's enabled there!!");
                }
            }

            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.WebGLCompressionAndTemplate))
            {
                PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
                PlayerSettings.WebGL.dataCaching = true;

                //Build Settings
#if UNITY_2020_1_OR_NEWER
                PlayerSettings.WebGL.decompressionFallback = true;
                PlayerSettings.WebGL.template = "PROJECT:Zappar";
#elif UNITY_2019_1_OR_NEWER
                PlayerSettings.WebGL.template = "PROJECT:Zappar2019";
#else
                Debug.LogError("Please upgrade to newer versions of Unity");
#endif
            }

            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.GraphicsOptimum))
            {
                PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new[] { GraphicsDeviceType.OpenGLES2 });
            }

#elif UNITY_ANDROID

            if(ZAssistant.MatchConfigSettings(config,ZProjectSettings.General))
            {
                EditorUserBuildSettings.development = false;

                if (uarSettings != null && uarSettings.EnableRealtimeReflections && !QualitySettings.realtimeReflectionProbes)
                {
                    QualitySettings.realtimeReflectionProbes = true;
                    Debug.Log("[NOTE] Enabled realtime reflections. However your build target platform setting may vary! Make sure it's enabled there!!");
                }
            }

            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.GraphicsOptimum))
            {
                PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
            }

            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.AndroidArchitexture))
            {
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
            }

#elif UNITY_IOS

            if(ZAssistant.MatchConfigSettings(config,ZProjectSettings.General))
            {
                EditorUserBuildSettings.development = false;

                if (uarSettings != null && uarSettings.EnableRealtimeReflections && !QualitySettings.realtimeReflectionProbes)
                {
                    QualitySettings.realtimeReflectionProbes = true;
                    Debug.Log("[NOTE] Enabled realtime reflections. However your build target platform setting may vary! Make sure it's enabled there!!");
                }
            }

            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.GraphicsOptimum))
            {
                PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] { GraphicsDeviceType.Metal });
            }

            if (ZAssistant.MatchConfigSettings(config, ZProjectSettings.IosPermissions))
            {
                if (string.IsNullOrEmpty(PlayerSettings.iOS.cameraUsageDescription))
                    PlayerSettings.iOS.cameraUsageDescription = "Zappar camera access request!";
            }

#endif
        }

#region ZapparResources

        public static GameObject GetZapparCamera(bool userFacing)
        {
            GameObject go = new GameObject("Zappar Camera", new[] { typeof(Camera), typeof(ZapparCamera) });
            GameObject child = new GameObject("Zappar Camera Background", new[] { typeof(Camera), typeof(ZapparCameraBackground) });
            child.GetComponent<Camera>().cullingMask = 0;
            child.tag = "MainCamera";
            child.transform.SetParent(go.transform);
            go.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;
            go.GetComponent<Camera>().depth = 1;
            if(userFacing)
            {
                go.GetComponent<ZapparCamera>().UseFrontFacingCamera = true;
                go.GetComponent<ZapparCamera>().MirrorCamera = true;
            }
            else
            {
                go.GetComponent<ZapparCamera>().UseFrontFacingCamera = false;
                go.GetComponent<ZapparCamera>().MirrorCamera = false;
            }
            return go;
        }

        public static GameObject GetZapparMultiFaceTrackingTarget()
        {
            GameObject go = new GameObject("Zappar Multi Face Tracking Target", new[] { typeof(ZapparMultiFaceTrackingTarget) });
            return go;
        }

        public static GameObject GetZapparFaceTrackingAnchor()
        {
            GameObject go = new GameObject("Zappar Face Tracking Anchor", new[] { typeof(ZapparFaceTrackingAnchor) });
            GameObject child = GetZapparFullHeadModel();
            child.transform.SetParent(go.transform);
            child = GetZapparFullHeadDepthMask();
            child.transform.SetParent(go.transform);
            return go;
        }

        public static GameObject GetZapparFaceMeshTarget()
        {
            GameObject go = new GameObject("Zappar Face Mesh", new[]
            {
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(ZapparFaceMeshTarget)
            });
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(ZResources.UVMat);
            go.GetComponent<ZapparFaceMeshTarget>().FaceMaterial = mat;
            go.GetComponent<ZapparFaceMeshTarget>().UseDefaultFullHead = true;
            go.GetComponent<MeshRenderer>().material = mat;
            return go;
        }

        public static GameObject GetZapparFaceLandmark()
        {
            GameObject go = new GameObject("Zappar Face Landmark", new[] { typeof(ZapparFaceLandmark) });
            return go;
        }

        public static GameObject GetZapparImageTrackingTarget()
        {
            GameObject go = new GameObject("Zappar Image Tracking Target", new[] { typeof(ZapparImageTrackingTarget) });
            return go;
        }

        public static GameObject GetZapparInstantTrackingTarget()
        {
            GameObject go = new GameObject("Zappar Instant Tracking Target", new[] { typeof(ZapparInstantTrackingTarget) });
            return go;
        }

        public static GameObject GetZapparFullHeadModel()
        {
            GameObject go = new GameObject("Zappar Full Head Model", new[] {typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(ZapparFullHeadModel)});
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(ZResources.TransparentMat);
            go.GetComponent<ZapparFullHeadModel>().HeadMaterial = mat;
            go.GetComponent<MeshRenderer>().material = mat;
            go.tag = "EditorOnly";
            return go;
        }

        public static GameObject GetZapparFullHeadDepthMask()
        {
            GameObject go = new GameObject("Zappar Full Head Depth Mask", new[] {typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(ZapparFaceDepthMask)});
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(ZResources.DepthMaskMat);
            go.GetComponent<ZapparFaceDepthMask>().FaceMaterial = mat;
            go.GetComponent<ZapparFaceDepthMask>().UseDefaultFullHead = true;
            go.GetComponent<MeshRenderer>().material = mat;
            return go;
        }
        
#endregion
    }
}