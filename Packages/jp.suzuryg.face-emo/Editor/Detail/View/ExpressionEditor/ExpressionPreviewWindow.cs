using System;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Suzuryg.FaceEmo.Domain;
using Suzuryg.FaceEmo.Detail.AV3;
using Suzuryg.FaceEmo.Detail.Drawing;
using System.Reflection;

namespace Suzuryg.FaceEmo.Detail.View
{
    public class ExpressionPreviewWindow : SceneView, ISubWindow
    {
        public bool IsInitialized { get; set; } = false;

        public bool IsDocked
        {
            get
            {
#if UNITY_2019
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo method = GetType().GetProperty( "docked", flags ).GetGetMethod( true );
                return (bool)method.Invoke( this, null );
#else
                return docked;
#endif
            }
        }

        private AV3.ExpressionEditor _expressionEditor;
        private Texture2D _renderCache;
        private SceneView _lastActiveSceneViewCache;

        public void Initialize(AV3.ExpressionEditor expressionEditor)
        {
            // Dependencies
            _expressionEditor = expressionEditor;

            // Initialization
            // _lastActiveSceneViewCache is set when this window is opened.
            if (_lastActiveSceneViewCache != null && _lastActiveSceneViewCache.cameraSettings != null)
            {
                var copied = new CameraSettings();

                copied.speed = _lastActiveSceneViewCache.cameraSettings.speed;
                copied.speedNormalized = _lastActiveSceneViewCache.cameraSettings.speedNormalized;
                copied.speedMin = _lastActiveSceneViewCache.cameraSettings.speedMin;
                copied.speedMax = _lastActiveSceneViewCache.cameraSettings.speedMax;
                copied.easingEnabled = _lastActiveSceneViewCache.cameraSettings.easingEnabled;
                copied.easingDuration = _lastActiveSceneViewCache.cameraSettings.easingDuration;
                copied.accelerationEnabled = _lastActiveSceneViewCache.cameraSettings.accelerationEnabled;
                copied.fieldOfView = _lastActiveSceneViewCache.cameraSettings.fieldOfView;
                copied.nearClip = _lastActiveSceneViewCache.cameraSettings.nearClip;
                copied.farClip = _lastActiveSceneViewCache.cameraSettings.farClip;
                copied.dynamicClip = _lastActiveSceneViewCache.cameraSettings.dynamicClip;
                copied.occlusionCulling = _lastActiveSceneViewCache.cameraSettings.occlusionCulling;

                cameraSettings = copied;
            }

            drawGizmos = false;
            const float initialZoom = 0.12f;
            LookAt(point: _expressionEditor.GetAvatarViewPosition(),
                direction: Quaternion.Euler(-5, 180, 0), newSize: initialZoom, ortho: true, instant: true);
        }

        public override void OnEnable()
        {
            // (Workaround) To avoid an error when the icon is not found in SceneView.OnEnable, generate the icon to handle the situation.
            const string IconDir = "Assets/Editor Default Resources/Icons";
            AV3Utility.CreateFolderRecursively(IconDir);
            var iconPath = IconDir + $"/{typeof(ExpressionPreviewWindow).FullName}.png";
            if (AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath) == null)
            {
                AssetDatabase.CopyAsset($"{DetailConstants.IconDirectory}/sentiment_satisfied_FILL0_wght400_GRAD200_opsz48.png", iconPath);
            }
            minSize = new Vector2(300, 300);
            base.OnEnable();

            // Workaround for the title being changed to "Scene" when restarting Unity.
            titleContent = new GUIContent(DomainConstants.SystemName);
        }

        public void UpdateRenderCache()
        {
            if (camera == null) { return; }

            // When using the camera without copying, the aspect ratio of the SceneView goes wrong
            var cameraRoot = new GameObject();
            try
            {
                var renderCamera = cameraRoot.AddComponent<Camera>();
                renderCamera.CopyFrom(camera);

                var drawScale = position.width / renderCamera.pixelWidth;
                var scaledTextureWidth = (int)Math.Round(position.width * DetailConstants.UiScale, MidpointRounding.AwayFromZero);
                var scaledTextureHeight = (int)Math.Round(renderCamera.pixelHeight * drawScale * DetailConstants.UiScale, MidpointRounding.AwayFromZero);

                _renderCache = DrawingUtility.GetRenderedTexture(scaledTextureWidth, scaledTextureHeight, renderCamera);
            }
            finally
            {
                UnityEngine.GameObject.DestroyImmediate(cameraRoot);
            }
        }

        public void CloseIfNotDocked()
        {
            if (!IsDocked)
            {
                Close();
            }
            else
            {
                // Must be initialized the next time opened from the main window.
                IsInitialized = false;
            }
        }

#if UNITY_2019
        protected override void OnGUI()
#else
        protected override void OnSceneGUI()
#endif
        {
            // When the animation changes are saved with Ctrl-S, the AnimationMode is stopped.
            // Therefore, the following process is performed to resume sampling.
            if (ReferenceEquals(focusedWindow, this) && !AnimationMode.InAnimationMode() && _expressionEditor?.IsDisposed == false)
            {
                _expressionEditor?.StartSampling();
            }

            // If in AnimationMode, draw SceneView.
            if (AnimationMode.InAnimationMode())
            {
#if UNITY_2019
                base.OnGUI();
#else
                base.OnSceneGUI();
#endif
            }
            // If not in AnimationMode, draw the cache.
            else
            {
                if (_renderCache != null)
                {
                    var x = 0;
#if UNITY_2019
                    var y = position.height - _renderCache.height / DetailConstants.UiScale;
#else
                    var y = 0;
#endif
                    var width = _renderCache.width / DetailConstants.UiScale;
                    var height = _renderCache.height / DetailConstants.UiScale;

                    GUI.DrawTexture(new Rect(x, y, width, height), _renderCache, ScaleMode.ScaleToFit, alphaBlend: false);
                }
            }
        }

        private void OnFocus()
        {
            // When base.OnSceneGUI() is called, lastActiveSceneView becomes active, causing problems with viewpoint manipulation.
            // To avoid this problem, change lastActiveSceneView.
            // Due to compatibility issue with Hai AnimationViewer, control lastActiveSceneView in Unity2019 as well.
            // Since reflection is used, limit the Unity version to 2019 and 2022.
            // Even if this operation is not performed, basic expression editing can be performed with only some problems with zooming by dragging, etc.
            if (lastActiveSceneView != null && !ReferenceEquals(lastActiveSceneView, this))
            {
                _lastActiveSceneViewCache = lastActiveSceneView;
            }
            SetLastActiveSceneView(this);

            if (_expressionEditor?.IsDisposed == false)
            {
                _expressionEditor?.StartSampling();
            }
        }

        private void OnLostFocus()
        {
#if UNITY_2019
            try
            {
                UpdateRenderCache();
            }
            finally
            {
                _expressionEditor?.StopSampling();
            }
#else
            // In Unity2022, OnLostFocus() can be executed when wheel-clicking or right-clicking on SceneView.
            // Therefore, do not stop sampling.
#endif
            if (_lastActiveSceneViewCache != null)
            {
                SetLastActiveSceneView(_lastActiveSceneViewCache);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _expressionEditor?.StopSampling();

            if (_lastActiveSceneViewCache != null)
            {
                SetLastActiveSceneView(_lastActiveSceneViewCache);
            }
        }

        private static void SetLastActiveSceneView(SceneView sceneView)
        {
            if (ReferenceEquals(sceneView, lastActiveSceneView)) { return; }
#if UNITY_2019
            Type sceneViewType = typeof(SceneView);
            FieldInfo lastActiveSceneViewInfo = sceneViewType.GetField("s_LastActiveSceneView", BindingFlags.NonPublic | BindingFlags.Static);
            if (lastActiveSceneViewInfo != null)
            {
                lastActiveSceneViewInfo.SetValue(null, sceneView);
            }
            else
            {
                Debug.LogError("s_LastActiveSceneView field not found");
            }
#elif UNITY_2022
            Type sceneViewType = typeof(SceneView);
            PropertyInfo lastActiveSceneViewInfo = sceneViewType.GetProperty("lastActiveSceneView", BindingFlags.Public | BindingFlags.Static);
            if (lastActiveSceneViewInfo != null)
            {
                lastActiveSceneViewInfo.SetValue(null, sceneView, null);
            }
            else
            {
                Debug.LogError("lastActiveSceneView property not found");
            }
#endif
        }
    }
}
