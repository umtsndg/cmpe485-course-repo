using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private Transform checkpointPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();

        if (respawn != null)
        {
            Transform point = checkpointPoint != null ? checkpointPoint : transform;
            respawn.SetCheckpoint(point.position, point.rotation);
            Debug.Log("Checkpoint reached: " + gameObject.name);
        }
    }
}