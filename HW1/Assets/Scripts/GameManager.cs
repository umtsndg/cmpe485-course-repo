using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void FailGame()
    {
        Debug.Log("FAIL!");
        Time.timeScale = 0f;
    }

    public void WinGame()
    {
        Debug.Log("YOU WIN!");
        Time.timeScale = 0f;
    }
}