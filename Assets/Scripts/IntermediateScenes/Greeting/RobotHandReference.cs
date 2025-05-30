using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHandReference : MonoBehaviour
{
    public static RobotHandReference Instance { get; private set; }
    
    public Transform hand;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
