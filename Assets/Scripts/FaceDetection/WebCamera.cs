using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AR
{
    // Taken from OpenCvSharp Demo package
    public abstract class WebCamera : MonoBehaviour
    {
        public GameObject Surface;
        private Nullable<WebCamDevice> webCamDevice = null;
        private WebCamTexture webCamTexture = null;
        private Texture2D renderedTexture = null;

        // Workaround for macOS: force the front camera behavior if needed
        protected bool forceFrontalCamera = false;

        protected OpenCvSharp.Unity.TextureConversionParams TextureParameters { get; private set; }
        

        public string DeviceName
        {
            get { return (webCamDevice != null) ? webCamDevice.Value.name : null; }
            set
            {
                if (value == DeviceName)
                    return;

                if (webCamTexture != null && webCamTexture.isPlaying)
                    webCamTexture.Stop();

                int cameraIndex = -1;
                for (int i = 0; i < WebCamTexture.devices.Length && cameraIndex == -1; i++)
                {
                    if (WebCamTexture.devices[i].name == value)
                        cameraIndex = i;
                }

                if (cameraIndex != -1)
                {
                    webCamDevice = WebCamTexture.devices[cameraIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.Value.name);
                    ReadTextureConversionParameters();
                    webCamTexture.Play();
                }
                else
                {
                    Debug.LogError($"{this.GetType().Name}: provided DeviceName is not a valid device identifier");
                }
            }
        }
        
        private IEnumerator Start()
        {
#if UNITY_ANDROID
            // Request user authorization for the camera on Android
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogError("Camera permission not granted");
                yield break;
            }
#endif
            // After permission, initialize the camera device
            if (WebCamTexture.devices.Length > 0)
            {
                foreach(var camDevice in WebCamTexture.devices){ 
                    if(camDevice.isFrontFacing){
                        DeviceName = camDevice.name;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("No camera devices found");
            }
        }

        private void ReadTextureConversionParameters()
        {
            OpenCvSharp.Unity.TextureConversionParams parameters = new OpenCvSharp.Unity.TextureConversionParams();

            // For a front-facing camera, we flip horizontally to get the mirror image
            parameters.FlipHorizontally = forceFrontalCamera || webCamDevice.Value.isFrontFacing;

            // Apply rotation if needed (most platforms report the correct angle)
            if (webCamTexture.videoRotationAngle != 0)
                parameters.RotationAngle = webCamTexture.videoRotationAngle;

#if UNITY_ANDROID
            // Some Android devices may not report the correct rotation.
            // Uncomment and adjust if necessary:
            // if (parameters.RotationAngle == 0)
            //     parameters.RotationAngle = 90;
#endif

            TextureParameters = parameters;
        }

        void OnDestroy()
        {
            if (webCamTexture != null)
            {
                if (webCamTexture.isPlaying)
                    webCamTexture.Stop();
                webCamTexture = null;
            }
            webCamDevice = null;
            Destroy(gameObject);
        }

        public void Close()
        {
        }

        private void Update()
        {
            if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
            {
                ReadTextureConversionParameters();
                if (ProcessTexture(webCamTexture, ref renderedTexture))
                {
                    RenderFrame();
                }
            }
        }

        protected abstract bool ProcessTexture(WebCamTexture input, ref Texture2D output);

        private void RenderFrame()
        {
            if (renderedTexture != null && Surface != null)
            {
                Surface.GetComponent<RawImage>().texture = renderedTexture;
                Surface.GetComponent<RectTransform>().sizeDelta = new Vector2(renderedTexture.width, renderedTexture.height);
            }
        }
    }
}
