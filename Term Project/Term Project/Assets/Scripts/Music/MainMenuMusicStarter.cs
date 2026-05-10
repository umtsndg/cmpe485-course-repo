using UnityEngine;

public class MainMenuMusicStarter : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }
    }
}