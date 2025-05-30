using System;
using System.Collections;
using System.Collections.Generic;
using AR;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class CheckWorking : MonoBehaviour
{
    public RawImage target;
    void Start()
    {
        string name = "";
        foreach(var camDevice in WebCamTexture.devices){ 
            Debug.Log(camDevice.name);
            if(camDevice.isFrontFacing){
                name = camDevice.name;
                break;
            }
        }
        var w = new WebCamTexture(name);
        target.texture = w;
        target.rectTransform.sizeDelta = new Vector2(300,300);
        w.Play();
        Debug.Log("QuickCamTest started camera");
    }
    
}

