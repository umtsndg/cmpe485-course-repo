using UnityEngine;

public class GameMusicStarter : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayNormalGameMusic();
        }
    }
}