using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Movement settings")]
    [SerializeField] private float moveSpeed = 4;
    public float movementMultiplier = 1;
    [SerializeField] private float jumpSpeed = 5;
    public float jumpMultiplier = 1;
    [SerializeField] private float jumpCooldown = 0.1f;
    private bool canJump = true;
    [SerializeField] private float coyoteJumpTime;

    [Header("Keys")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Tags")]
    [SerializeField] private string groundTag = "Ground";

    [Header("Input")]
    private bool moveInput = false;
    private bool jumpInput = false;

    [Header("Ground check")]
    [Range(0f, 1f)]
    [SerializeField] private float boxLength;

    [SerializeField] private float groundRaycastLength = 2;
    public bool grounded = false;
    public bool groundRay = false;
    public bool groundCollision = false;

    [Header("Extras")]
    [SerializeField] private float coyoteJumpTimer;
    [SerializeField] private bool canCoyoteJump;
    [SerializeField] private bool snapierMovement = false;
    private Dictionary<PowerUp, float> powerUpCooldowns = new();

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerUpCooldowns.Add(PowerUp.Movement, 0f);
        powerUpCooldowns.Add(PowerUp.Jump, 0f);
    }

    private void Update()
    {
        foreach (PowerUp powerUp in new List<PowerUp>(powerUpCooldowns.Keys))
        {
            if (powerUpCooldowns[powerUp] > 0)
            {
                powerUpCooldowns[powerUp] -= Time.deltaTime;
            }
            else
            {
                powerUpCooldowns[powerUp] = 0f;

                switch (powerUp)
                {
                    case PowerUp.Movement:
                        movementMultiplier = 1;
                        break;
                    case PowerUp.Jump:
                        jumpMultiplier = 1;
                        break;
                    default:
                        break;
                }
            }
        }
        // Create a box at [position], edge distance from center as [size] and pick a direction. Move box to that direction, and any new collisions are a [hit]. Don't [rotate] it and define [max distance]
        groundRay = Physics.BoxCast(transform.position, Vector3.one * boxLength * .5f - Vector3.up * boxLength * .45f, Vector3.down, out RaycastHit hit, Quaternion.identity, groundRaycastLength) ? hit.collider.tag == groundTag : false;

        /*if (Physics.BoxCast(transform.position, Vector3.one * boxLength * .5f - Vector3.up * boxLength * .45f, Vector3.down, out RaycastHit twohit, Quaternion.identity, groundRaycastLength))
        {
            Debug.Log(twohit.collider.tag);

        }*/
        //groundRay = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundRaycastLength) ? hit.collider.tag == groundTag : false;

        grounded = groundRay && groundCollision;

        moveInput = Input.GetAxis("Horizontal") != 0;

        jumpInput = Input.GetKey(jumpKey);
    }
    private void OnDrawGizmosSelected()
    {
        // Visualisation of the BoxCast
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundRaycastLength * .5f, Vector3.one * boxLength + (Vector3.up * (groundRaycastLength - boxLength)));

    }

    private void FixedUpdate()
    {
        if (moveInput && !Physics.Raycast(transform.position, new Vector3(Mathf.Round(Input.GetAxis("Horizontal")), 0, 0), 1))
            Move();

        if (grounded && canJump)
        {
            canCoyoteJump = true;
            coyoteJumpTimer = coyoteJumpTime;
        }
        else if (canCoyoteJump)
        {
            coyoteJumpTimer -= Time.fixedDeltaTime;
            if (coyoteJumpTimer <= 0) canCoyoteJump = false;
        }

        if (((grounded && canJump) || canCoyoteJump) && jumpInput)
            StartCoroutine(Jump());
    }

    private void Move()
    {
        if (snapierMovement)
            rb.linearVelocity = new Vector3(
                (Input.GetAxis("Horizontal") > 0 ? Mathf.Ceil(Input.GetAxis("Horizontal")) : Mathf.Floor(Input.GetAxis("Horizontal"))) * moveSpeed * movementMultiplier,
                rb.linearVelocity.y,
                0);
        else
            rb.linearVelocity = new Vector3(
                Input.GetAxis("Horizontal") * moveSpeed * movementMultiplier,
                rb.linearVelocity.y,
                0);
    }
    private IEnumerator Jump()
    {
        jumpInput = false;
        canJump = false;
        canCoyoteJump = false;
        rb.AddForce(Vector3.up * jumpSpeed * jumpMultiplier, ForceMode.Impulse);

        yield return new WaitForSeconds(jumpCooldown);

        canJump = true;
    }

    public void ApplyPowerUp(PowerUp powerUp, float multilpierAmount, float cooldown)
    {
        switch (powerUp)
        {
            case PowerUp.Movement:
                movementMultiplier = multilpierAmount;
                break;
            case PowerUp.Jump:
                jumpMultiplier = multilpierAmount;
                break;
            default:
                break;
        }

        powerUpCooldowns[powerUp] = cooldown;
    }

    public void resetPowerUps()
    {
        foreach (PowerUp powerUp in new List<PowerUp>(powerUpCooldowns.Keys))
        {
            powerUpCooldowns[powerUp] = 0f;
        }
    }

    public void Respawn(Vector3 respawnPos)
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = respawnPos;
    }

    void OnCollisionStay(Collision collision)
    {
        groundCollision = collision.gameObject.tag == groundTag;
    }

    void OnCollisionExit(Collision collision)
    {
        groundCollision = false;
    }

    public enum PowerUp
    {
        Jump,
        Movement
    }
}
