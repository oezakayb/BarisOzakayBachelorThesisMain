using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Timeline;

public class CatController : MonoBehaviour 
{
    [Header("Cat Settings")]
    [Tooltip("Starting grid coordinate for the cat.")]
    public Vector2Int currentGridCoordinate;
    [Tooltip("Warning zone radius within which the cat becomes evasive.")]
    public float warningZoneRadius = 3f;
    [Tooltip("Warning zone radius within which the cat notices the cat food.")]
    public float foodDetectionRadius = Mathf.Sqrt(2);
    [Tooltip("Color for the cat's cell.")]
    public Color catCellColor = Color.red;

    [Header("Movement Settings")]
    [Tooltip("Time it takes the cat to move one tile.")]
    public float moveDuration = 1;
    [Tooltip("Amount to increase the sound meter when the cat moves.")]
    public float catMoveSoundIncrease = 20f;

    public AudioClip meowClip;
    public AudioSource catAudioSource;

    public bool foodFound = false;
    
    public GridGenerator gridGenerator;
    
    /// <param name="playerCell">Player's current grid coordinate.</param>
    /// <param name="catFoodCell">Cat food's current grid coordinate. </param>
    /// <param name="soundMeter">Reference to the SoundMeter to update sound levels.</param>
    public void TakeTurn(Vector2Int playerCell, GridSquare catFoodCell, SoundMeter soundMeter)
    {
        if(foodFound) return;
        if (catFoodCell != null && (catFoodCell.gridCoordinate - currentGridCoordinate).magnitude < foodDetectionRadius)
        {
            foodFound = true;
            MoveCatTo(catFoodCell.gridCoordinate, soundMeter);
            return;
        }
        
        if(warningZoneRadius <= (playerCell - currentGridCoordinate).magnitude) return;
        
        List<Vector2Int> candidates = new List<Vector2Int>();
        
        BoxCollider myCol = GetComponent<BoxCollider>();
        Collider[] overlaps = Physics.OverlapBox(transform.position, myCol.bounds.extents, Quaternion.identity);
        bool bedDetected = false;
        float bedRelativeX = 0f;
        foreach (Collider col in overlaps)
        {
            if(col.gameObject == gameObject) continue;
            if(col.CompareTag("Bed"))
            {
                bedDetected = true;
                bedRelativeX = col.bounds.center.x - transform.position.x;
                break;
            }
        }

        if (bedDetected)
        {
            int sign = bedRelativeX < 0 ? -1 : 1;

            if (!gridGenerator.gridSquares.ContainsKey(currentGridCoordinate + new Vector2Int(sign, 0)))
            {
                Vector2Int destination = currentGridCoordinate;
                for (int i = 1; i < 10; i++)
                {
                    if (gridGenerator.gridSquares.ContainsKey(currentGridCoordinate + new Vector2Int(sign * i, 0)))
                    {
                        destination = currentGridCoordinate + new Vector2Int(sign * i, 0);
                        break;
                    }
                }

                candidates.Add(destination);
            }
        }

        
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int candidate = currentGridCoordinate + dir;
            if (gridGenerator.gridSquares.ContainsKey(candidate))
            {
                candidates.Add(candidate);
            }
        }

        if(candidates.Count == 0)
        {
            return;
        }

        Vector2Int chosenDestination = currentGridCoordinate;
        
        foreach (var candidate in candidates)
        {
            chosenDestination = (playerCell - chosenDestination).magnitude >= (playerCell - candidate).magnitude
                ? chosenDestination
                : candidate;
        }
        
        if(chosenDestination != currentGridCoordinate)
        {
            MoveCatTo(chosenDestination, soundMeter);
        }
    }
    
    void MoveCatTo(Vector2Int target, SoundMeter soundMeter)
    {
        currentGridCoordinate = target;
        GameObject targetCell = gridGenerator.gridSquares[target];
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.LookAt(targetCell.transform);
        Vector3 targetDirection = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotation);
        catAudioSource.PlayOneShot(meowClip);
        transform.DORotate(targetDirection, moveDuration)
            .OnComplete(() =>
            {
                transform.DOMove(targetCell.transform.position, moveDuration);
                if (!foodFound) soundMeter.Increase(catMoveSoundIncrease);
            });
    }
}
