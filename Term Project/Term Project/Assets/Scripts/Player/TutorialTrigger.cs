using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    [SerializeField] private string tutorialMessage;

    [SerializeField] private bool showOnlyOnce = true;
    [SerializeField] private bool clearOnExit = false;

    private bool hasShown;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (showOnlyOnce && hasShown) return;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ShowTutorial(tutorialMessage);
        }

        hasShown = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!clearOnExit) return;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ClearTutorial();
        }
    }
}