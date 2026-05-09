using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompletePopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject popupPanel;

    [Header("Scene Flow")]
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Player Control")]
    [SerializeField] private MonoBehaviour[] scriptsToDisableOnComplete;
    [SerializeField] private Rigidbody playerRigidbody;

    private bool isOpen;

    private void Start()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowPopup()
    {
        if (isOpen) return;

        isOpen = true;

        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }

        foreach (MonoBehaviour script in scriptsToDisableOnComplete)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        if (playerRigidbody != null)
        {
#if UNITY_6000_0_OR_NEWER
            playerRigidbody.linearVelocity = Vector3.zero;
#else
            playerRigidbody.velocity = Vector3.zero;
#endif
            playerRigidbody.angularVelocity = Vector3.zero;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}