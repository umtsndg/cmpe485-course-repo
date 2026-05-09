using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead = false;
    private PlayerRespawn playerRespawn;

    private void Awake()
    {
        playerRespawn = GetComponent<PlayerRespawn>();

        if (playerRespawn == null)
        {
            playerRespawn = GetComponentInParent<PlayerRespawn>();
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (playerRespawn != null)
        {
            playerRespawn.RespawnWithSceneReset();
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
