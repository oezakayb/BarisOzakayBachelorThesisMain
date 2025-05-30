using UnityEngine;

[ExecuteAlways]
public class BlendShapeLister : MonoBehaviour
{
    public SkinnedMeshRenderer faceRenderer;

    void Start()
    {
        if (faceRenderer == null) faceRenderer = GetComponent<SkinnedMeshRenderer>();
        var mesh = faceRenderer.sharedMesh;
        Debug.Log($"--- {mesh.name} has {mesh.blendShapeCount} blend-shapes: ---");
        for (int i = 0; i < mesh.blendShapeCount; i++)
            Debug.Log($"[{i}] {mesh.GetBlendShapeName(i)}");
    }
}

