using UnityEngine;
using System.Collections;

namespace Core.Utilities
{
    public class ScreenShotter : MonoBehaviour
    {
        static Vector2Int m_ssRes = default;
        bool takeHiResShot = false;

        Texture2D ScreenshotTexture;
        RenderTexture _renderTexture;

        public static string ScreenShotName(int width, int height)
        {
            return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                                 Application.dataPath,
                                 width, height,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }

        public void TakeHiResShot()
        {
            takeHiResShot = true;
        }

        private void Awake()
        {
            int w = Screen.width;
            int h = Screen.height;
            _renderTexture = new RenderTexture(w, h, 0);
            ScreenshotTexture = new Texture2D(w, h, TextureFormat.RGB24, true);
        }
        void LateUpdate()
        {
            takeHiResShot |= Input.GetKeyDown("k");
            if (takeHiResShot)
            {
                StartCoroutine(UpdateScreenshotTexture());
            }
        }

        public IEnumerator UpdateScreenshotTexture()
        {
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshotIntoRenderTexture(_renderTexture);
            ScreenshotTexture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
            ScreenshotTexture.Apply();

            byte[] bytes = ScreenshotTexture.EncodeToPNG();
            string filename = ScreenShotName(m_ssRes.x, m_ssRes.y);
            if (System.IO.Directory.Exists(Application.dataPath + "/screenshots"))
            {
                System.IO.File.WriteAllBytes(filename, bytes);
                Debug.Log(string.Format("Took screenshot to: {0}", filename));
                takeHiResShot = false;
            }
            else
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/screenshots");
                System.IO.File.WriteAllBytes(filename, bytes);
                Debug.Log(string.Format("Took screenshot to: {0}", filename));
                takeHiResShot = false;
            }
        }
    }
}