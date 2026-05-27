using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Tooltip("Basically: Is this attached to the player")]
    [SerializeField] private bool shootWithInput;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPos;

    [Header("Shoot with input")]

    [SerializeField] private KeyCode shootButton;

    [Header("Shooting automatically")]
    [SerializeField] private float shootInterval;
    [SerializeField] private float shootTimer;

    [Header("Other")]
    [SerializeField] private float lastXPos;
    [Tooltip("-1 means left, 1 means right")]
    [SerializeField] private int shootDirection;
    [SerializeField] private bool fixedShootDirection;

    private void Start()
    {
        shootTimer = shootInterval;
    }

    private void Update()
    {
        if (shootWithInput)
        {
            if (Input.GetKeyDown(shootButton))
            {
                Shoot();
            }
        }
        else
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer < 0)
            {
                Shoot();
                shootTimer += shootInterval;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!fixedShootDirection)
        {
            shootDirection = Mathf.RoundToInt(Mathf.Sign(bulletSpawnPos.position.x - lastXPos + 0.001f * shootDirection));
            lastXPos = bulletSpawnPos.position.x;
        }
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.LookRotation(Vector3.forward * shootDirection));
    }

}
