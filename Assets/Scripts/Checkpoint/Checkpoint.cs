using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Material gotCheckpointMaterial;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.ToLower() == "player")
        {
            other.GetComponent<PlayerController>().resetPowerUps();

            if (CheckpointManager.Instance.SetCheckpoint(this))
            {
                GetComponentInChildren<MeshRenderer>().material = gotCheckpointMaterial;
            }
        }
    }

    public Transform GetRespawn()
    {
        return respawnPoint;
    }

}
