using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class PillBottle : MonoBehaviour, IPointerDownHandler
{
    public int bottleID;
    public GameController gameController;
    public int pillCapacity = 5; 
    public Stack<Pill> pillStack = new Stack<Pill>(); 
    
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    
    private List<Pill> transferringPills = new List<Pill>();

    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        gameController.SelectBottle(gameObject);
    }
    
    public void AnimatePillTransfer(PillBottle targetBottle, Stack<(PillBottle giver, PillBottle receiver)> moves)
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        transform.DOMoveY(targetBottle.transform.position.y + 4, 1)
            .OnComplete(() =>
            {
                Vector3 targetPos = targetBottle.transform.position;
                bool targetOnRight = targetPos.x > transform.position.x;
                float targetX = targetOnRight ? targetPos.x - 1.5f : targetPos.x + 1.5f;
                float targetRotationZ = targetOnRight ? -140 : 140;

                transform.DOMoveX(targetX, 1)
                    .OnComplete(() =>
                    {
                        transform.DORotate(transform.rotation.eulerAngles + new Vector3(0, 0, targetRotationZ), 1)
                            .OnComplete(() =>
                            {
                                AnimatePillTransferRecursive(targetBottle, moves);
                                StartCoroutine(WaitForTransferComplete());
                            });
                    });
            });
    }
    public void AnimatePillTransferRecursive(PillBottle targetBottle, Stack<(PillBottle giver, PillBottle receiver)> moves)
    {
        if (pillStack.Count == 0)
            return;

        moves.Push((this, targetBottle));
        Pill pillToMove = pillStack.Pop();
        transferringPills.Add(pillToMove);
        targetBottle.ReceivePillAnimate(pillToMove);
        if (pillStack.Count > 0 && targetBottle.pillStack.Count < targetBottle.pillCapacity &&
            pillToMove.pillColor == pillStack.Peek().pillColor)
        {
            StartCoroutine(DelayedRecursiveTransfer(targetBottle, 0.3f, moves));
        }
    }

    private IEnumerator DelayedRecursiveTransfer(PillBottle targetBottle, float delay, Stack<(PillBottle giver, PillBottle receiver)> moves)
    {
        yield return new WaitForSeconds(delay);
        AnimatePillTransferRecursive(targetBottle, moves);
    }
    private IEnumerator WaitForTransferComplete()
    {
        float threshold = 2f; // Set this based on your bottle size
        while (transferringPills.Count > 0)
        {
            for (int i = transferringPills.Count - 1; i >= 0; i--)
            {
                Pill p = transferringPills[i];
                if (Vector3.Distance(p.transform.position, transform.position) > threshold)
                {
                    gameController.audioManager.PlayPillDrop();
                    transferringPills.RemoveAt(i);
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        transform.DORotate(originalRotation.eulerAngles, 1)
            .OnComplete(() =>
            {
                transform.DOMoveX(originalPosition.x, 1)
                    .OnComplete(() =>
                    {
                        transform.DOMoveY(originalPosition.y, 1)
                            .OnComplete(() =>
                            {
                                gameController.CheckGameCompletion();
                                gameController.TransferCompleted();
                            });
                    });
            });
    }
    public void ReceivePillAnimate(Pill pill)
    {
        pill.transform.parent = transform;
        
        Rigidbody rb = pill.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
        pillStack.Push(pill);
        StartCoroutine(WaitForPillToSettle(pill));
    }
    private IEnumerator WaitForPillToSettle(Pill pill)
    {
        int stackIndex = pillStack.Count - 1;
        Rigidbody rb = pill.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.3f);
        while (rb.velocity != Vector3.zero)
        {
            yield return null;
        }
        rb.isKinematic = true;
        rb.useGravity = false;

        
        Vector3 targetLocalPosition = new Vector3(0, -0.789f + stackIndex * 0.359f, 0);
        Vector3 targetLocalRotation = new Vector3(0, 0, 90);
        float tweenDuration = 0.6f;
        pill.transform.DOLocalMove(targetLocalPosition, tweenDuration)
            .OnUpdate(() =>
            {
                pill.transform.localScale = new Vector3(0.3f, 0.4f, 0.4f);
            });


        pill.transform.DOLocalRotate(
                Mathf.Abs(pill.transform.localRotation.eulerAngles.z - 90) <
                Mathf.Abs(pill.transform.localRotation.eulerAngles.z - 270)
                    ? targetLocalRotation
                    : new Vector3(0, 0, 270), tweenDuration, RotateMode.Fast)
            .OnUpdate(() =>
            {
                pill.transform.localScale = new Vector3(0.3f, 0.4f, 0.4f);
            })
            .OnComplete(() =>
            {
                pill.transform.localRotation = Quaternion.Euler(targetLocalRotation);
            });
        yield return new WaitForSeconds(tweenDuration);
    }
    public void ReceivePill(Pill pill)
    {
        pill.transform.parent = transform;
        pill.transform.localPosition = new Vector3(0, -0.789f + pillStack.Count * 0.359f, 0);
        pillStack.Push(pill);
    }
    
    public bool IsSortedCorrectly()
    {
        if (pillStack.Count == 0) return true;
        if (pillStack.Count != 5) return false;
    
        Color firstColor = pillStack.Peek().pillColor;
        foreach (var pill in pillStack)
        {
            if (pill.pillColor != firstColor)
                return false;
        }
        return true;
    }
}
