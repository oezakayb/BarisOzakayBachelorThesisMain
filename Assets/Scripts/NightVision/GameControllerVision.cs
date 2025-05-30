using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Turn { Player, Cat }

public class GameControllerVision : MonoBehaviour
{
    [Header("References")]
    public GameObject player; 
    public GameObject gridGeneratorObject;  
    public GridGenerator gridGen;  
    public Button[] actionButtons;   
    public SoundMeter soundMeter;  
    public CatController catController;   
    public GameObject catFood;
    public AudioManagerVision audioManager;

    [Header("UI Panels")]
    public GameObject gameWonPanel;   
    public GameObject gameLostPanel;   
    public GameObject warningPanel;  
    public Button gameWon;
    

    [Header("Turn Settings")]
    public Turn currentTurn = Turn.Player;
    public int playerMovesPerTurn = 2;
    private int playerMovesLeft;

    [Header("Player Action Sound Changes")]
    public float playerMoveSoundIncrease = 10f;
    public float playerActionSoundDecrease = 10f;

    [Header("Sound Warning Settings")]
    [Tooltip("Normalized threshold (0..1) at which to show a warning.")]
    public float warningThreshold = 0.8f;
    
    public GridSquare playerCurrentSquare;
    public GridSquare catFoodSquare;
    private GameObject catFoodInstance = null;

    private bool lostTriggered = false;
    private bool warningShown = false;

    void Start()
    {
        player = RobotHandReference.Instance.gameObject;
        gridGen = gridGeneratorObject.GetComponent<GridGenerator>();
        SetUpGame();
        BeginPlayerTurn();

        if (gameWonPanel) gameWonPanel.SetActive(false);
        if (gameLostPanel) gameLostPanel.SetActive(false);
        if (warningPanel) warningPanel.SetActive(false);
    }

    public void SetUpGame()
    {
        if (gridGen.gridSquares.ContainsKey(catController.currentGridCoordinate))
        {
            var catCell = gridGen.gridSquares[catController.currentGridCoordinate];
            catController.transform.position = catCell.transform.position;
            catCell.GetComponent<GridSquare>().SetEdgeColor(catController.catCellColor);
        }
        MarkWarningZone();
        Vector2Int playerStart = new Vector2Int(-6, -3);
        if (gridGen.gridSquares.ContainsKey(playerStart))
        {
            var cell = gridGen.gridSquares[playerStart];
            playerCurrentSquare = cell.GetComponent<GridSquare>();
            player.transform.position = cell.transform.position;
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
            playerCurrentSquare.SetEdgeColor(Color.blue);
        }
    }

    void BeginPlayerTurn()
    {
        if (playerCurrentSquare.gridCoordinate == catController.currentGridCoordinate)
        {
            OnGameWon();
            return;
        }
        if (lostTriggered) return;
        currentTurn = Turn.Player;
        playerMovesLeft = playerMovesPerTurn;
        EnableButtons(true);
    }

    void EndPlayerTurn()
    {
        EnableButtons(false);
        if (catController.foodFound)
        {
            BeginPlayerTurn();
        }
        StartCoroutine(CatTurnRoutine());
    }

    IEnumerator CatTurnRoutine()
    {
        yield return new WaitForSeconds(1f);
        catController.TakeTurn(playerCurrentSquare.gridCoordinate, catFoodSquare, soundMeter);
        CheckSoundStatus();
        yield return new WaitForSeconds(1f);
        MarkWarningZone();
        BeginPlayerTurn();
    }

    public void MovePlayer(Vector2Int direction)
    {
        if (currentTurn != Turn.Player || lostTriggered) return;
        Vector2Int targetCoord = playerCurrentSquare.gridCoordinate + direction;
        if (gridGen.gridSquares.ContainsKey(targetCoord))
        {
            EnableButtons(false);
            var targetCell = gridGen.gridSquares[targetCoord];
            playerCurrentSquare = targetCell.GetComponent<GridSquare>();

            // Rotate then move
            Vector3 rot = player.transform.rotation.eulerAngles;
            if (direction == Vector2Int.right) rot.y = 90;
            else if (direction == Vector2Int.left) rot.y = 270;
            else if (direction == Vector2Int.up) rot.y = 0;
            else if (direction == Vector2Int.down) rot.y = 180;

            player.transform.DORotate(rot, 1f)
                .OnComplete(() => player.transform.DOMove(targetCell.transform.position, 1f)
                    .OnComplete(()=>
                    {
                        soundMeter.Increase(playerMoveSoundIncrease);
                        CheckSoundStatus();
                        EnableButtons(true);
                        playerMovesLeft--;
                        if (playerCurrentSquare.gridCoordinate == catController.currentGridCoordinate)
                        {
                            OnGameWon();
                            return;
                        }
                        // After moving, update colors
                        playerCurrentSquare.SetEdgeColor(Color.blue);
                        MarkWarningZone();

                        if (playerMovesLeft <= 0)
                            EndPlayerTurn();
                        
                    }));
        }
    }

    public void PlaceCatFood()
    {
        if (currentTurn != Turn.Player || lostTriggered) return;
        if (catFoodInstance != null && playerCurrentSquare == catFoodSquare)
        {
            Destroy(catFoodInstance);
            catFoodSquare = null;
        }
        else
        {
            catFoodInstance = Instantiate(catFood, playerCurrentSquare.transform.position, Quaternion.identity);
            catFoodSquare = playerCurrentSquare;
        }
        soundMeter.Decrease(playerActionSoundDecrease);
        CheckSoundStatus();
        playerMovesLeft--;
        if (playerMovesLeft <= 0)
            EndPlayerTurn();
    }

    public void PassTurn()
    {
        if (currentTurn != Turn.Player || lostTriggered) return;
        soundMeter.Decrease(playerActionSoundDecrease);
        CheckSoundStatus();
        playerMovesLeft--;
        if (playerMovesLeft <= 0)
            EndPlayerTurn();
    }

    void EnableButtons(bool enable)
    {
        foreach (var btn in actionButtons)
        {
            btn.interactable = enable;
            var img = btn.GetComponent<Image>();
            if (img != null)
                img.color = enable ? Color.white : Color.gray;
        }
    }

    void CheckSoundStatus()
    {
        if (soundMeter.currentValue >= soundMeter.maxValue)
        {
            OnGameLost();
            return;
        }
        
        if (soundMeter.currentValue >= warningThreshold * soundMeter.maxValue)
        {
           
            if (warningPanel != null && !warningPanel.activeSelf)
            {
                audioManager.PlaySoundWarning();
                warningPanel.SetActive(true);
                warningPanel.transform.SetAsLastSibling();
            }
        }
        else
        {
            if (warningPanel != null && warningPanel.activeSelf)
            {
                warningPanel.SetActive(false);
            }
        }
    }

    void OnGameWon()
    {
        currentTurn = Turn.Player;
        audioManager.PlayGameWon();
        EnableButtons(false);
        if (gameWonPanel != null)
        {
            gameWonPanel.SetActive(true);
            gameWonPanel.transform.SetAsLastSibling();
            gameWon.onClick.AddListener(() =>
            {
                GameFlowManager.Instance.LoadNextScene();
            });
        }
    }

    void OnGameLost()
    {
        lostTriggered = true;
        EnableButtons(false);
        audioManager.PlayGameLost();
        if (gameLostPanel != null)
        {
            gameLostPanel.SetActive(true);
        }
    }

    void MarkWarningZone()
    {
        foreach (var kvp in gridGen.gridSquares)
        {
            var cell = kvp.Value.GetComponent<GridSquare>();
            if (cell != null)
                cell.SetEdgeColor(Color.green);
        }
        if (!catController.foodFound)
        {
            foreach (var kvp in gridGen.gridSquares)
            {
                var cell = kvp.Value.GetComponent<GridSquare>();
                if (cell != null)
                {
                    float dist = (cell.gridCoordinate - catController.currentGridCoordinate).magnitude;
                    if (dist < catController.warningZoneRadius)
                        cell.SetEdgeColor(Color.yellow);
                }
            }
            foreach (var kvp in gridGen.gridSquares)
            {
                var cell = kvp.Value.GetComponent<GridSquare>();
                if (cell != null)
                {
                    float dist = (cell.gridCoordinate - catController.currentGridCoordinate).magnitude;
                    if (dist < catController.foodDetectionRadius)
                        cell.SetEdgeColor(Color.cyan);
                }
            }
        }
        if (gridGen.gridSquares.ContainsKey(catController.currentGridCoordinate))
        {
            var catCell = gridGen.gridSquares[catController.currentGridCoordinate].GetComponent<GridSquare>();
            if (catCell != null)
                catCell.SetEdgeColor(catController.catCellColor);
        }
        if (catFoodSquare != null && catFoodSquare != playerCurrentSquare)
            catFoodSquare.SetEdgeColor(Color.magenta);
        if (playerCurrentSquare != null)
            playerCurrentSquare.SetEdgeColor(Color.blue);
    }

    public void GameLostButton()
    {
        
        catController.currentGridCoordinate = new Vector2Int(0, 0);
        catFoodSquare = null;
        Destroy(catFoodInstance);
        soundMeter.Decrease(100);
        if (gridGen.gridSquares.ContainsKey(catController.currentGridCoordinate))
        {
            var catCell = gridGen.gridSquares[catController.currentGridCoordinate];
            catController.transform.position = catCell.transform.position;
            catCell.GetComponent<GridSquare>().SetEdgeColor(catController.catCellColor);
        }
        Vector2Int playerStart = new Vector2Int(-6, -3);
        if (gridGen.gridSquares.ContainsKey(playerStart))
        {
            var cell = gridGen.gridSquares[playerStart];
            playerCurrentSquare = cell.GetComponent<GridSquare>();
            player.transform.position = cell.transform.position;
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
            playerCurrentSquare.SetEdgeColor(Color.blue);
        }
        MarkWarningZone();
        playerMovesLeft = 2;
        currentTurn = Turn.Player;
        lostTriggered = false;
        EnableButtons(true);
        if (gameLostPanel) gameLostPanel.SetActive(false);
    }
}

