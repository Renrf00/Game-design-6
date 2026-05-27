using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class HealthSystem : MonoBehaviour
{
    [SerializeField] private bool gameOverOnDeath;
    [SerializeField] private bool isFriendly;
    [SerializeField] private int maxHealth;

    [Header("Respawing Enemies")]
    [Tooltip("Only for enemies, if true the enemy will respawn after specified time")]
    [SerializeField] private bool respawn;
    [SerializeField] private float respawnTime;

    [Tooltip("What, if anything, should this object spawn upon death?")]
    [SerializeField] private GameObject spawnOnDeath;

    [Header("Other")]
    [Tooltip("Place where the prefab spawns upon death")]
    [SerializeField] private Transform spawnOnDeathLocation;

    [Tooltip("If any child is moved by DOTween, put it in this array")]
    [SerializeField] private Transform[] movedByDotween;

    [Header("Other health stuff")]
    [SerializeField] private float healthMoveDuration;
    [SerializeField] private Slider healthSlider;

    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject enemyHitbox;
    [SerializeField] private Material damagedMaterial;
    [SerializeField] private float blinkDuration;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void GetHit(int pDamage)
    {
        StartCoroutine(DamagedBlink(blinkDuration));
        currentHealth -= pDamage;

        if (healthSlider != null)
        {
            healthSlider.DOValue((float)currentHealth / maxHealth, healthMoveDuration);
        }

        if (currentHealth <= 0) Die();
    }

    private IEnumerator DamagedBlink(float duration)
    {
        MeshRenderer meshRenderer = enemyHitbox == null ? GetComponent<MeshRenderer>() : enemyHitbox.GetComponent<MeshRenderer>();

        Debug.Log("damaged!!");
        Material baseMaterial = meshRenderer.material;

        meshRenderer.material = damagedMaterial;

        yield return new WaitForSeconds(duration);

        meshRenderer.material = baseMaterial;
    }
    private void Die()
    {
        for (int i = 0; i < movedByDotween.Length; i++)
        {
            if (!respawn)
            {
                // If any DOTween movement is still happening, kill it (to prevent possible memory leaks)
                movedByDotween[i].DOKill();
            }
        }
        if (healthSlider != null)
        {
            healthSlider.DOKill();
            healthSlider.value = 0;
        }


        if (spawnOnDeath != null)
        {
            // If there's an object to spawn on death, spawn it at designated position and default rotation
            Instantiate(spawnOnDeath, spawnOnDeathLocation.position, Quaternion.identity);
        }

        if (gameOverOnDeath)
        {
            CheckpointManager.Instance.Respawn();
            ResetHealth();
            //GameOver.Instance.DoGameOver();
            return;
        }

        if (respawn)
        {
            StartCoroutine(RespawnAfter(respawnTime));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator RespawnAfter(float duration)
    {
        bool hasShooter = GetComponent<Shooter>().isActiveAndEnabled;
        enemyHitbox.GetComponent<Collider>().enabled = false;
        enemyHitbox.GetComponent<MeshRenderer>().enabled = false;
        if (hasShooter) GetComponent<Shooter>().enabled = false;
        yield return new WaitForSeconds(duration);
        enemyHitbox.GetComponent<Collider>().enabled = true;
        enemyHitbox.GetComponent<MeshRenderer>().enabled = true;
        if (hasShooter) GetComponent<Shooter>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent(out Danger danger))
        {
            // Friendly doesn't hit friendly (preventing player from hurting themselves with own bullets)
            if (isFriendly && danger.IsFriendly()) return;

            // Hit takes place: one of either is not friendly
            GetHit(danger.GetDamage());
        }
    }

    private void ResetHealth()
    {
        currentHealth = maxHealth;
        healthSlider.value = 1;
    }
}
