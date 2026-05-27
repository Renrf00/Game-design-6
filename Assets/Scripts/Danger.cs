using UnityEngine;

public class Danger : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private bool isFriendly;
    [SerializeField] private bool destroyOnHit;

    public int GetDamage()
    {
        // Destroy self?
        if (destroyOnHit) Destroy(gameObject);

        return damage;
    }
    public bool IsFriendly()
    {
        return isFriendly;
    }
}
