using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject levelSelectPanel;

    [Header("Scene Names")]
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    [SerializeField] private string level2SceneName = "Level2";

    private void Start()
    {
        ShowMainMenu();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        LoadTutorial();
    }

    public void OpenLevelSelect()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(true);
        }
    }

    public void ShowMainMenu()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }

        if (levelSelectPanel != null)
        {
            levelSelectPanel.SetActive(false);
        }
    }

    public void LoadTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void LoadLevel2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level2SceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}