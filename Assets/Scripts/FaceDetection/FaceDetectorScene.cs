using UnityEngine.UI;

namespace AR
{
    using UnityEngine;
    using OpenCvSharp;
    
    // Taken from OpenCvSharp Demo package and altered

    public class FaceDetectorScene : WebCamera
    {
        public TextAsset faces;
        public TextAsset eyes;
        public TextAsset shapes;
        public GameObject Output;

        public bool faceAligned = false;

        private FaceProcessorLive<WebCamTexture> processor;

        protected void Awake()
        {
            forceFrontalCamera = true; // For macOS: force frontal camera behavior

            processor = new FaceProcessorLive<WebCamTexture>();
#if UNITY_ANDROID
            // On Android, load the cascade XML from bytes (UTF8 conversion) to avoid issues with TextAsset.text
            string facesCascade = System.Text.Encoding.UTF8.GetString(faces.bytes);
            string eyesCascade = System.Text.Encoding.UTF8.GetString(eyes.bytes);
#else
            string facesCascade = faces.text;
            string eyesCascade = eyes.text;
#endif
            processor.Initialize(facesCascade, eyesCascade, shapes.bytes);

            // Data stabilizer parameters
            processor.DataStabilizer.Enabled = true;
            processor.DataStabilizer.Threshold = 2.0;
            processor.DataStabilizer.SamplesCount = 2;

            // Performance parameters
            processor.Performance.Downscale = 256;
            processor.Performance.SkipRate = 0;
        }

        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            processor.ProcessTexture(input, TextureParameters);
            Point[] faceOutline = processor.MarkDetected();

            if (faceOutline != null)
            {
                Mat frame = Unity.TextureToMat(input, TextureParameters);
                OpenCvSharp.Rect boundingRect = Cv2.BoundingRect(faceOutline);
                Mat mask = new Mat(frame.Size(), MatType.CV_8UC1, Scalar.Black);
                Cv2.FillConvexPoly(mask, faceOutline, Scalar.White);
                Mat frameWithAlpha = new Mat();
                Cv2.CvtColor(frame, frameWithAlpha, ColorConversionCodes.BGR2BGRA);
                var channels = Cv2.Split(frameWithAlpha);
                channels[3] = mask;
                Cv2.Merge(channels, frameWithAlpha);
                frameWithAlpha = new Mat(frameWithAlpha, boundingRect);
                Texture2D outputTexture = Unity.MatToTexture(frameWithAlpha);
                Output.GetComponent<RawImage>().texture = outputTexture;
                Output.GetComponent<RawImage>().rectTransform.sizeDelta = new Vector2(frameWithAlpha.Width, frameWithAlpha.Height);
                faceAligned = true;
                Close();
            }

            output = Unity.MatToTexture(processor.Image, output);
            return true;
        }
    }

    
}

