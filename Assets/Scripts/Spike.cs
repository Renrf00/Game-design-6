using UnityEngine;

public class Spike : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            MousePosManager.Instance.SetCursorStatePlay();
        }
    }
}
