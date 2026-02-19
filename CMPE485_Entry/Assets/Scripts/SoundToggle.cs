using UnityEngine;

public class SoundToggle : MonoBehaviour
{
    public AudioSource audioSource;
    public KeyCode toggleKey = KeyCode.M;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.Play();
        }
    }
}
