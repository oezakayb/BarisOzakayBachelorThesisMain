using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementButton : MonoBehaviour
{
    public GameControllerVision GameControllerVision;
    public Vector2Int direction;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameControllerVision.MovePlayer(direction);
        });
    }
}
