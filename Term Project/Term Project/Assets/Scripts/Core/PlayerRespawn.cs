using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    private static bool hasSavedCheckpoint = false;
    private static int savedCheckpointScene = -1;
    private static Vector3 savedCheckpointPosition;
    private static Quaternion savedCheckpointRotation;

    private Vector3 checkpointPosition;
    private Quaternion checkpointRotation;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (hasSavedCheckpoint && savedCheckpointScene == currentScene)
        {
            checkpointPosition = savedCheckpointPosition;
            checkpointRotation = savedCheckpointRotation;
            Respawn();
            return;
        }

        checkpointPosition = transform.position;
        checkpointRotation = transform.rotation;
    }

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        checkpointPosition = position;
        checkpointRotation = rotation;

        hasSavedCheckpoint = true;
        savedCheckpointScene = SceneManager.GetActiveScene().buildIndex;
        savedCheckpointPosition = position;
        savedCheckpointRotation = rotation;
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
        transform.rotation = checkpointRotation;

        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void RespawnWithSceneReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void ClearSavedCheckpoint()
    {
        hasSavedCheckpoint = false;
        savedCheckpointScene = -1;
    }
}
