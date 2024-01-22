using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityRoyale.ScreenShot.Editor
{
    public sealed class ScreenshotterWindow : EditorWindow
    {
        public int width = 1920;
        public int height = 1080;

        [MenuItem("Window/ScreenShot")]
        private static void OpenWindow()
        {
            var instance = GetWindow<ScreenshotterWindow>();
            instance.Show();
        }

        private void OnGUI()
        {
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            if (GUILayout.Button("Take Screenshot"))
            {
                TakeScreenshot();
            }
        }

        private void TakeScreenshot()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                Debug.LogError("No main camera found!");
                return;
            }
            if (camera.targetTexture != null)
            {
                Debug.LogError("Camera is rendering to a RenderTexture!");
                return;
            }

            var target = new RenderTexture(width, height, 24);
            RenderPipeline.StandardRequest request = new()
            {
                destination = target,
            };
            RenderPipeline.SubmitRenderRequest(camera, request);

            RenderTexture.active = target;
            var screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            RenderTexture.active = null;
            DestroyImmediate(target);

            // Save the screenshot to file
            var path = EditorUtility.SaveFilePanel("Save Screenshot", "", "screenshot.png", "png");
            if (path.Length != 0)
            {
                var bytes = screenshot.EncodeToPNG();
                System.IO.File.WriteAllBytes(path, bytes);
                Debug.Log($"Saved screenshot to {path}");
            }
        }
    }
}