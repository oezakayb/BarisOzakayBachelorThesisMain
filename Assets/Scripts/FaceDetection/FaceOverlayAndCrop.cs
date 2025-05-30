using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Feeds the device camera into a UI RawImage, draws a red/green convex hull
/// of the entire ARCore face mesh, and continuously crops the face region
/// into another RawImage (e.g., for RoBody) during gameplay.
/// </summary>
[RequireComponent(typeof(ARCameraManager), typeof(ARFaceManager))]
public class FaceOverlayAndCrop : MonoBehaviour
{
    [Header("AR Components")]
    public ARCameraManager cameraManager;
    public ARFaceManager faceManager;

    [Header("UI Elements")]
    [Tooltip("Full-screen RawImage to show camera feed during detection")]    
    public RawImage cameraRawImage;
    [Tooltip("LineRenderer for face outline")]    
    public LineRenderer faceOutlineLine;
    [Tooltip("RawImage where the cropped face is streamed live")]    
    public RawImage robodyFaceRawImage;

    [Header("Alignment Thresholds")]
    [Tooltip("Max angle between face normal and camera forward to count as orthogonal")]    
    public float angleThreshold = 10f;

    Texture2D cameraTexture;
    bool faceDetected = false;
    RectInt lastFaceRect;

    void OnEnable()
    {
        cameraManager.frameReceived += OnCameraFrame;
        faceManager.facesChanged  += OnFacesChanged;
    }

    void OnDisable()
    {
        cameraManager.frameReceived -= OnCameraFrame;
        faceManager.facesChanged  -= OnFacesChanged;
    }

    void OnCameraFrame(ARCameraFrameEventArgs args)
    {
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cpuImage))
            return;

        var convParams = new XRCpuImage.ConversionParams
        {
            inputRect       = new RectInt(0, 0, cpuImage.width, cpuImage.height),
            outputDimensions= new Vector2Int(cpuImage.width, cpuImage.height),
            outputFormat    = TextureFormat.RGBA32,
            transformation  = XRCpuImage.Transformation.None
        };

        int w = convParams.outputDimensions.x;
        int h = convParams.outputDimensions.y;
        if (cameraTexture == null || cameraTexture.width != w || cameraTexture.height != h)
            cameraTexture = new Texture2D(w, h, convParams.outputFormat, false);

        var buffer = cameraTexture.GetRawTextureData<byte>();
        cpuImage.Convert(convParams, buffer);
        cpuImage.Dispose();
        cameraTexture.Apply();
        
        cameraRawImage.texture = cameraTexture;
        
        if (faceDetected)
            CropAndAssignFace();
    }

    void OnFacesChanged(ARFacesChangedEventArgs args)
    {
        ARFace face = args.updated.FirstOrDefault() ?? args.added.FirstOrDefault();
        if (face == null)
        {
            faceOutlineLine.positionCount = 0;
            faceDetected = false;
            return;
        }
        faceDetected = true;
        
        var verts = face.vertices;
        Camera cam = Camera.main;
        List<Vector2> screenPts = new List<Vector2>(verts.Length);
        foreach (var v in verts)
        {
            Vector3 worldPt = face.transform.TransformPoint(v);
            screenPts.Add(cam.WorldToScreenPoint(worldPt));
        }
        
        var hull = ConvexHull(screenPts);
        DrawOutline(hull);
        
        Vector3 faceNormal = face.transform.forward;
        float angle = Vector3.Angle(faceNormal, cam.transform.forward);
        faceOutlineLine.material.color = (angle <= angleThreshold)
            ? Color.green : Color.red;
        
        float minX = hull.Min(p => p.x), maxX = hull.Max(p => p.x);
        float minY = hull.Min(p => p.y), maxY = hull.Max(p => p.y);
        lastFaceRect = new RectInt(
            Mathf.FloorToInt(minX / Screen.width  * cameraTexture.width),
            Mathf.FloorToInt(minY / Screen.height * cameraTexture.height),
            Mathf.CeilToInt((maxX-minX)/Screen.width  * cameraTexture.width),
            Mathf.CeilToInt((maxY-minY)/Screen.height * cameraTexture.height)
        );
    }

    void CropAndAssignFace()
    {
        if (cameraTexture == null) return;
        
        lastFaceRect.x      = Mathf.Clamp(lastFaceRect.x, 0, cameraTexture.width);
        lastFaceRect.y      = Mathf.Clamp(lastFaceRect.y, 0, cameraTexture.height);
        lastFaceRect.width  = Mathf.Clamp(lastFaceRect.width, 0, cameraTexture.width - lastFaceRect.x);
        lastFaceRect.height = Mathf.Clamp(lastFaceRect.height, 0, cameraTexture.height - lastFaceRect.y);

        var pix = cameraTexture.GetPixels(
            lastFaceRect.x, lastFaceRect.y,
            lastFaceRect.width, lastFaceRect.height
        );
        var faceTex = new Texture2D(
            lastFaceRect.width, lastFaceRect.height,
            TextureFormat.RGBA32, false
        );
        faceTex.SetPixels(pix);
        faceTex.Apply();
        robodyFaceRawImage.texture = faceTex;
    }

    void DrawOutline(List<Vector2> pts)
    {
        int n = pts.Count;
        faceOutlineLine.positionCount = n;
        var arr = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                cameraRawImage.rectTransform,
                pts[i], null, out Vector2 localPt
            );
            arr[i] = localPt;
        }
        faceOutlineLine.SetPositions(arr);
    }
    
    List<Vector2> ConvexHull(List<Vector2> pts)
    {
        var sorted = pts.Distinct()
            .OrderBy(p => p.x).ThenBy(p => p.y)
            .ToList();
        if (sorted.Count <= 1) return new List<Vector2>(sorted);

        List<Vector2> lower = new List<Vector2>();
        foreach (var p in sorted)
        {
            while (lower.Count >= 2 && Cross(lower[lower.Count-2], lower[lower.Count-1], p) <= 0)
                lower.RemoveAt(lower.Count-1);
            lower.Add(p);
        }
        List<Vector2> upper = new List<Vector2>();
        for (int i = sorted.Count-1; i >= 0; i--)
        {
            var p = sorted[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count-2], upper[upper.Count-1], p) <= 0)
                upper.RemoveAt(upper.Count-1);
            upper.Add(p);
        }
        lower.RemoveAt(lower.Count-1);
        upper.RemoveAt(upper.Count-1);
        lower.AddRange(upper);
        return lower;
    }

    float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x)*(c.y - a.y) - (b.y - a.y)*(c.x - a.x);
    }
}
