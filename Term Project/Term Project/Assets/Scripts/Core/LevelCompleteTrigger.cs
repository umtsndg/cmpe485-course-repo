using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private LevelCompletePopup levelCompletePopup;

    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed) return;
        if (!other.CompareTag("Player")) return;

        completed = true;

        if (levelCompletePopup != null)
        {
            levelCompletePopup.ShowPopup();
        }
    }
}