using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool gameEnded = false;
    private bool playerWon = false;

    [Header("Disable on Game End")]
    public MonoBehaviour[] scriptsToDisable;

    public void FailGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        playerWon = false;

        EndGameSetup();
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        playerWon = true;

        EndGameSetup();
    }

    private void EndGameSetup()
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    private void OnGUI()
    {
        if (!gameEnded) return;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        GUI.color = new Color(0f, 0f, 0f, 0.7f);
        GUI.DrawTexture(new Rect(0, 0, screenWidth, screenHeight), Texture2D.whiteTexture);

        float panelWidth = 320f;
        float panelHeight = 220f;
        float panelX = (screenWidth - panelWidth) / 2f;
        float panelY = (screenHeight - panelHeight) / 2f;

        GUI.color = Color.white;
        GUI.Box(new Rect(panelX, panelY, panelWidth, panelHeight), "");

        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 28;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.black;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 18;

        string resultText = playerWon ? "YOU WIN" : "YOU LOSE";

        GUI.Label(new Rect(panelX, panelY + 25f, panelWidth, 40f), resultText, titleStyle);

        if (GUI.Button(new Rect(panelX + 60f, panelY + 90f, 200f, 40f), "Restart", buttonStyle))
        {
            RestartGame();
        }

        if (GUI.Button(new Rect(panelX + 60f, panelY + 145f, 200f, 40f), "Quit", buttonStyle))
        {
            QuitGame();
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}