using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject finishText;

    private bool levelCompleted = false;

    void Update()
    {
        if (levelCompleted && Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void CompleteLevel()
    {
        levelCompleted = true;

        if (finishText != null)
        {
            finishText.SetActive(true);
        }

        Debug.Log("Level Complete! Press R to restart.");
    }

    public void RestartLevel()
    {
        PlayerRespawn.ClearSavedCheckpoint();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
