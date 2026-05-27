using UnityEngine;
using DG.Tweening;

public class MovementBehaviour : MonoBehaviour
{
    [Tooltip("How long should it take to move from one place to another? (seconds)")]
    [SerializeField] private float moveDuration;
    [Tooltip("In means it's applied at the start, Out means it's applied at the end")]
    [SerializeField] private Ease easeType;
    [SerializeField] private bool destroyOnCompletion;
    [Tooltip("Hiding the nodes that indicate the movement")]
    [SerializeField] private bool hideNodes = true;

    [Header("Transforms")]

    [SerializeField] private Transform objectToMove;

    [SerializeField] private Transform startPos, endPos;


    private void Start()
    {
        if (hideNodes)
        {
            // Hiding the objects 
            startPos.gameObject.SetActive(false);
            endPos.gameObject.SetActive(false);
        }

        // LoopType.Yoyo makes it move back and forth
        if (!destroyOnCompletion)
        {
            objectToMove.DOLocalMove(endPos.localPosition, moveDuration).SetEase(easeType).From(startPos.localPosition, true, false).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            objectToMove.DOLocalMove(endPos.localPosition, moveDuration).SetEase(easeType).From(startPos.localPosition, true, false).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        

    }

    private void OnDestroy()
    {
        objectToMove.DOKill();
    }

}
