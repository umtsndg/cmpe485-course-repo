using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();

            if (respawn == null)
            {
                respawn = other.GetComponentInParent<PlayerRespawn>();
            }

            if (respawn != null)
            {
                respawn.RespawnWithSceneReset();
                return;
            }

            gameManager.RestartLevel();
        }
    }
}
