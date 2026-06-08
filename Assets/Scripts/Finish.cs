using UnityEngine;
using UnityEngine.Events;

public class Finish : MonoBehaviour
{
    [SerializeField] private UnityEvent EndEvent;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player" && MousePosManager.Instance.GetCursorState() == MousePosManager.CursorState.Play)
        {
            EndEvent.Invoke();
        }
    }
}
