using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Player Scripts To Disable")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableWhilePaused;

    private bool isPaused;

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        foreach (MonoBehaviour script in scriptsToDisableWhilePaused)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllSFX();
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        foreach (MonoBehaviour script in scriptsToDisableWhilePaused)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

}