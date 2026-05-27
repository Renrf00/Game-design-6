using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private Checkpoint currentCheckpoint;

    public static CheckpointManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void Respawn()
    {
        var player = FindAnyObjectByType<PlayerController>();

        if (currentCheckpoint == null)
        {
            // GameOver.Instance.DoGameOver();
            return;
        }
        player.Respawn(currentCheckpoint.GetRespawn().position);
    }

    public bool SetCheckpoint(Checkpoint pCheckpoint)
    {
        if (pCheckpoint == null || pCheckpoint == currentCheckpoint) { return false; }
        currentCheckpoint = pCheckpoint;
        return true;
    }



}
