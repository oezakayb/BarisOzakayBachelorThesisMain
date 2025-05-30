using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Utilities.Extensions;
using Debug = UnityEngine.Debug;

public class GameController : MonoBehaviour
{
    [Header("Prefabs & Containers")]
    public GameObject bottlePrefab;
    public GameObject pillPrefab;
    public Transform bottleContainer;

    [Header("Level Control")]
    public int currentLevel = 1;
    private const int maxLevel = 3;

    [Header("UI Panels")]
    [Tooltip("Panels shown when each level completes (index 0 = level1, etc.). Index 2 is game-complete panel.")]
    public GameObject[] levelCompletePanels = new GameObject[maxLevel];
    
    [Header("Action Buttons")]
    public Button hintButton;      // "Hinweis"
    public Button undoButton;      // "Zurück"
    public Button cancelButton;    // "Abwählen"

    private bool isTransferring = false;
    private bool gamePaused = false;

    private GameObject firstSelectedBottle;
    private GameObject secondSelectedBottle;
    private List<GameObject> bottles = new List<GameObject>();
    private Stack<(PillBottle giver, PillBottle receiver)> moves = new Stack<(PillBottle, PillBottle)>();

    [Header("Hint & Undo")]
    public GameObject hintArrow;
    private List<GameObject> hintArrows = new List<GameObject>();
    public Button undo;

    [Header("Arm Animations")]
    public GameObject armLeft;
    public GameObject armRight;
    public GameObject handLeft;
    public GameObject handRight;
    private Vector3 armPos = new Vector3(0, -3.24f, -11.06f);

    public AudioManager audioManager;

    void Start()
    {
        foreach (var panel in levelCompletePanels)
            if (panel != null)
                panel.SetActive(false);

        LoadLevel(currentLevel);
    }

    public void LoadLevel(int level)
    {
        foreach (var arrow in hintArrows) Destroy(arrow);
        hintArrows.Clear();
        foreach (var b in bottles) Destroy(b);
        bottles.Clear();
        moves.Clear();
        isTransferring = false;
        gamePaused = false;
        
        foreach (var panel in levelCompletePanels)
            if (panel != null)
                panel.SetActive(false);
        
        if (hintButton != null) hintButton.gameObject.SetActive(true);
        if (undoButton != null) undoButton.gameObject.SetActive(true);
        if (cancelButton != null) cancelButton.gameObject.SetActive(true);
        
        List<List<Color>> configs = GetLevelConfiguration(level);
        int n = configs.Count;
        
        float[] masterX = new float[] { -14f, -10f, -6f, -2f, 2f };
        float masterCenter = masterX[masterX.Length / 2]; // -6f
        
        int startIndex = Mathf.CeilToInt((masterX.Length - n) / 2f);
        List<float> chosen = new List<float>(n);
        for (int i = 0; i < n; i++)
            chosen.Add(masterX[startIndex + i]);
        
        float sum = 0f;
        foreach (var v in chosen) sum += v;
        float avg = sum / n;
        float offset = masterCenter - avg;

        for (int i = 0; i < n; i++)
        {
            float x = chosen[i] + offset;
            GameObject bottle = Instantiate(bottlePrefab, bottleContainer);
            bottle.transform.localPosition = new Vector3(x, 0f, 0f);
            PillBottle pb = bottle.GetComponent<PillBottle>();
            pb.gameController = this;
            pb.bottleID = i;
            bottles.Add(bottle);
        }
        
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < configs[i].Count; j++)
            {
                GameObject pillObj = Instantiate(pillPrefab);
                Pill pill = pillObj.GetComponent<Pill>();
                pill.SetColor(configs[i][j]);
                pillObj.GetComponent<Renderer>().material.color = pill.pillColor;
                pill.transform.SetParent(bottles[i].transform);
                pillObj.transform.localScale = new Vector3(0.3f, 0.4f, 0.4f);
                bottles[i].GetComponent<PillBottle>().ReceivePill(pill);
            }
        }
    }

    public void SelectBottle(GameObject selectedBottle)
    {
        if (isTransferring || gamePaused)
        {
            Debug.Log("Transfer in progress or game paused, ignoring input.");
            return;
        }
        if (firstSelectedBottle == null)
        {
            firstSelectedBottle = selectedBottle;
            audioManager.PlayButtonClick();
            firstSelectedBottle.transform.position += new Vector3(0, 0.3f, 0);
            firstSelectedBottle.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
            if (firstSelectedBottle.GetComponent<PillBottle>().pillStack.Count == 0)
            {
                Debug.Log("First selected bottle is empty. Move canceled.");
                ResetSelection();
                return;
            }
        }
        else if (secondSelectedBottle == null && selectedBottle != firstSelectedBottle)
        {
            secondSelectedBottle = selectedBottle;
            TryMovePill();
        }
    }

    private void TryMovePill()
    {
        PillBottle firstBottleScript = firstSelectedBottle.GetComponent<PillBottle>();
        PillBottle secondBottleScript = secondSelectedBottle.GetComponent<PillBottle>();

        if (firstBottleScript.pillStack.Count == 0)
        {
            Debug.Log("First selected bottle is empty. Move canceled.");
            ResetSelection();
            return;
        }

        Pill topPill = firstBottleScript.pillStack.Peek();

        if (secondBottleScript.pillStack.Count == secondBottleScript.pillCapacity)
        {
            Debug.Log("Invalid move: Bottle is full!");
            ResetSelection();
            return;
        }

        if (secondBottleScript.pillStack.Count == 0 || secondBottleScript.pillStack.Peek().pillColor == topPill.pillColor)
        {
            foreach (var arrow in hintArrows) Destroy(arrow);
            hintArrows.Clear();
            TransferStarted();

            GameObject armToMove = firstSelectedBottle.transform.position.x < secondSelectedBottle.transform.position.x ? armLeft : armRight;
            GameObject handToAnimate = armToMove == armLeft ? handLeft : handRight;
            armToMove.transform.position = new Vector3(firstSelectedBottle.transform.position.x, armToMove.transform.position.y, armToMove.transform.position.z);
            
            audioManager.PlayArmStart();
            Vector3 targetPos = armToMove.transform.position + firstSelectedBottle.transform.position - handToAnimate.transform.position - new Vector3(armToMove == armLeft ? .9f : -1.4f, 0, 1.5f);
            armToMove.transform.DOMove(targetPos, 2f)
                .OnComplete(() =>
                {
                    string clip = armToMove == armLeft ? "left grab" : "right grab";
                    armToMove.GetComponent<Animation>().Play(clip);
                    IEnumerator AnimateArm()
                    {
                        while (armToMove.GetComponent<Animation>().IsPlaying(clip)) yield return null;
                        armToMove.transform.SetParent(firstBottleScript.transform);
                        firstBottleScript.AnimatePillTransfer(secondBottleScript, moves);
                    }
                    StartCoroutine(AnimateArm());
                });
        }
        else
        {
            Debug.Log("Invalid move: Cannot stack different colored pills.");
        }

        ResetSelection();
    }

    private void ResetSelection()
    {
        if (firstSelectedBottle != null)
        {
            firstSelectedBottle.transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
            firstSelectedBottle.transform.position -= new Vector3(0, 0.3f, 0);
        }
        firstSelectedBottle = null;
        secondSelectedBottle = null;
    }

    public void TransferStarted()
    {
        isTransferring = true;
        if (hintButton != null)
        {
            hintButton.gameObject.GetComponent<Image>().color = Color.gray;
            hintButton.interactable = false;
        }

        if (undoButton != null)
        {
            undoButton.gameObject.GetComponent<Image>().color = Color.gray;
            undoButton.interactable = false;
        }

        if (cancelButton != null)
        {
            cancelButton.gameObject.GetComponent<Image>().color = Color.gray;
            cancelButton.interactable = false;
        }
        
    }

    public void TransferCompleted()
    {
        isTransferring = false;
        if (hintButton != null)
        {
            hintButton.gameObject.GetComponent<Image>().color = Color.white;
            hintButton.interactable = true;
        }

        if (undoButton != null)
        {
            undoButton.gameObject.GetComponent<Image>().color = Color.white;
            undoButton.interactable = true;
        }

        if (cancelButton != null)
        {
            cancelButton.gameObject.GetComponent<Image>().color = Color.white;
            cancelButton.interactable = true;
        }
        ResetArms();
    }

    private void ResetArms()
    {
        GameObject armToMoveBack = armLeft.transform.parent != null && armLeft.transform.position != armPos ? armLeft : armRight;
        string clip = armToMoveBack == armLeft ? "left release" : "right release";
        armToMoveBack.GetComponent<Animation>().Play(clip);
        IEnumerator AnimateArm()
        {
            while (armToMoveBack.GetComponent<Animation>().IsPlaying(clip)) yield return null;
            armToMoveBack.transform.parent = null;
            armToMoveBack.transform.DOMove(armPos, 1f)
                .OnComplete(() =>
                {
                    audioManager.PlayArmEnd();
                });
        }
        StartCoroutine(AnimateArm());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (handLeft != null)
            Gizmos.DrawWireSphere(handLeft.transform.position, 1);
    }

    List<List<Color>> GetLevelConfiguration(int level)
    {
        if (level == 1) return new List<List<Color>>()
        {
            new List<Color> { Color.red, Color.red, Color.blue, Color.blue, Color.red },
            new List<Color> { Color.blue, Color.red, Color.blue, Color.red, Color.blue },
            new List<Color>()
        };
        if (level == 2) return new List<List<Color>>()
        {
            new List<Color> { Color.red, Color.blue, Color.green, Color.red, Color.blue },
            new List<Color> { Color.green, Color.red, Color.blue, Color.green, Color.red },
            new List<Color> { Color.blue, Color.green, Color.red, Color.green, Color.blue },
            new List<Color>()
        };
        if (level == 3) return new List<List<Color>>()
        {
            new List<Color> { Color.red, Color.blue, Color.yellow, Color.green, Color.red },
            new List<Color> { Color.green, Color.red, Color.yellow, Color.blue, Color.yellow },
            new List<Color> { Color.blue, Color.green, Color.red, Color.yellow, Color.blue },
            new List<Color> { Color.yellow, Color.blue, Color.green, Color.red, Color.green },
            new List<Color>()
        };
        return new List<List<Color>>();
    }

    public void CheckGameCompletion()
    {
        if (gamePaused) return;
        foreach (var b in bottles)
            if (!b.GetComponent<PillBottle>().IsSortedCorrectly()) return;

        Debug.Log($"Level {currentLevel} Completed!");
        isTransferring = true;
        gamePaused = true;
        ShowLevelCompletePanel();
    }

    void ShowLevelCompletePanel()
    {
        int idx = currentLevel - 1;
        if (idx >= 0 && idx < levelCompletePanels.Length && levelCompletePanels[idx] != null)
            levelCompletePanels[idx].SetActive(true);
        audioManager.PlayArmEnd();
        audioManager.PlayLevelCompleted();
        
        if (hintButton != null) hintButton.gameObject.SetActive(false);
        if (undoButton != null) undoButton.gameObject.SetActive(false);
        if (cancelButton != null) cancelButton.gameObject.SetActive(false);
    }
    
    public void OnNextLevel()
    {
        int idx = currentLevel - 1;
        if (idx >= 0 && idx < levelCompletePanels.Length && levelCompletePanels[idx] != null)
            levelCompletePanels[idx].SetActive(false);

        armLeft.transform.parent = null;
        armRight.transform.parent = null;

        if (currentLevel < maxLevel)
        {
            currentLevel++;
            LoadLevel(currentLevel);
        }
    }

    public void UndoLastMove()
    {
        if (isTransferring || gamePaused) return;
        if (moves.Count == 0) { Debug.Log("No moves!"); return; }
        var move = moves.Pop();
        move.giver.ReceivePill(move.receiver.pillStack.Pop());
    }

    public void GetHintMove()
    {
        if (isTransferring || gamePaused) return;
        if (hintArrows.Count == 2) return;
        var hint = CalculateHintMove();
        if (hint.HasValue)
        {
            Vector3 pos = new Vector3(0,1.8f,0);
            var arrow1 = Instantiate(hintArrow, hint.Value.giver.transform);
            arrow1.GetComponent<Renderer>().material.color = Color.magenta;
            arrow1.transform.localPosition = pos;
            hintArrows.Add(arrow1);
            var arrow2 = Instantiate(hintArrow, hint.Value.receiver.transform);
            arrow2.transform.localPosition = pos;
            arrow2.transform.Rotate(0,0,180,Space.Self);
            arrow2.GetComponent<Renderer>().material.color = Color.magenta;
            hintArrows.Add(arrow2);
        }
        else
        {
            Color orig = undo.image.material.color;
            undo.image.DOColor(Color.yellow,0.3f)
                .SetLoops(3,LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .OnComplete(()=>undo.image.DOColor(orig,0.3f).SetEase(Ease.InOutSine));
        }
    }

    public void CancelSelection()
    {
        if (firstSelectedBottle == null || isTransferring || gamePaused) return;
        ResetSelection();
    }

    private (PillBottle giver, PillBottle receiver)? CalculateHintMove()
    {
        string[] startState = GetCurrentState();
        var queue = new Queue<(string[] state, List<(int,int)> path)>();
        var visited = new HashSet<string>();
        queue.Enqueue((startState,new List<(int,int)>()));
        visited.Add(StateToKey(startState));
        while(queue.Count>0)
        {
            var (state,path) = queue.Dequeue();
            if (IsSolved(state))
            {
                if (path.Count>0)
                {
                    var first = path[0];
                    return (bottles[first.Item1].GetComponent<PillBottle>(),
                            bottles[first.Item2].GetComponent<PillBottle>());
                }
                break;
            }
            foreach(var move in GetValidMoves(state))
            {
                var newState = ApplyMove(state,move);
                string key = StateToKey(newState);
                if (visited.Contains(key)) continue;
                visited.Add(key);
                var newPath = new List<(int,int)>(path){move};
                queue.Enqueue((newState,newPath));
            }
        }
        return null;
    }

    private string[] GetCurrentState()
    {
        string[] state = new string[bottles.Count];
        for(int i=0;i<bottles.Count;i++)
        {
            var pb = bottles[i].GetComponent<PillBottle>();
            var arr = pb.pillStack.ToArray();
            Array.Reverse(arr);
            string s="";
            foreach(var pill in arr) s += ColorToChar(pill.pillColor);
            state[i]=s;
        }
        return state;
    }

    private char ColorToChar(Color color)
    {
        if (color.Equals(Color.red)) return 'R';
        if (color.Equals(Color.blue)) return 'B';
        if (color.Equals(Color.green)) return 'G';
        if (color.Equals(Color.yellow)) return 'Y';
        return 'X';
    }

    private int CountConsecutive(string bottle)
    {
        if (string.IsNullOrEmpty(bottle)) return 0;
        int cnt=1;
        char top=bottle[bottle.Length-1];
        for(int i=bottle.Length-2;i>=0;i--)
            if(bottle[i]==top) cnt++; else break;
        return cnt;
    }

    private bool IsSolved(string[] state)
    {
        var colorCount=new Dictionary<char,int>();
        for(int i=0;i<state.Length;i++)
        {
            var s=state[i];
            if(string.IsNullOrEmpty(s)) continue;
            foreach(char c in s) if(c!=s[0]) return false;
            if(colorCount.ContainsKey(s[0])) colorCount[s[0]]++; else colorCount[s[0]]=1;
            if(colorCount[s[0]]>1) return false;
        }
        return true;
    }

    private List<(int, int)> GetValidMoves(string[] state)
    {
        var list=new List<(int,int)>();
        for(int i=0;i<state.Length;i++)
        {
            var s=state[i]; if(string.IsNullOrEmpty(s)) continue;
            int count=CountConsecutive(s); char top=s[s.Length-1];
            for(int j=0;j<state.Length;j++)
            {
                if(i==j) continue;
                if(state[j].Length+count>5) continue;
                var r=state[j]; if(!string.IsNullOrEmpty(r)&&r[r.Length-1]!=top) continue;
                list.Add((i,j));
            }
        }
        return list;
    }

    private string[] ApplyMove(string[] state, (int giver, int receiver) move)
    {
        string[] newState = new string[state.Length];
        state.CopyTo(newState, 0);

        int g = move.giver;
        int r = move.receiver;
        
        int count = CountConsecutive(newState[g]);
        if (count == 0) return newState;

        char top = newState[g][newState[g].Length - 1];
        
        newState[g] = newState[g].Substring(0, newState[g].Length - count);
        
        newState[r] = newState[r] + new string(top, count);

        return newState;
    }

    private string StateToKey(string[] state) => string.Join("|",state);
}
