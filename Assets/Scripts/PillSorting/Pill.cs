using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    public Color pillColor { get; private set; }

    private void Awake()
    {
        pillColor = GetComponent<Renderer>().material.color;
    }
    
    public void SetColor(Color color)
    {
        pillColor = color;
        GetComponent<Renderer>().material.color = color;
    }
}
