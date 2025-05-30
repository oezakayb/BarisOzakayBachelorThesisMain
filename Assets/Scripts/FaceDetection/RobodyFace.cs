using System;
using System.Collections;
using System.Collections.Generic;
using AR;
using UnityEngine;
using UnityEngine.UI;

public class RobodyFace : MonoBehaviour
{
    public RawImage input;
    public RawImage robodyFace;

    void Awake()
    {
        if (input == null)
        {
            input = GameObject.FindWithTag("facedetection").GetComponent<RawImage>();
        }
    }

    void Update()
    {
        if (input == null)
        {
            input = GameObject.FindWithTag("facedetection").GetComponent<RawImage>();
        }
        if (input != null && input.texture != null)
        {
            ApplyFaceTexture(input.texture);
        }
    }

    private void ApplyFaceTexture(Texture faceTexture)
    {
        robodyFace.texture = faceTexture;
    }
}
