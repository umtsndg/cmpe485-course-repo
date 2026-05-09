using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TMP_Text tutorialText;

    [Header("Settings")]
    [SerializeField] private bool hideAfterDelay = false;
    [SerializeField] private float displayDuration = 4f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ClearTutorial();
    }

    public void ShowTutorial(string message)
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        ClearTutorial();

        tutorialText.text = message;
        tutorialPanel.SetActive(true);

        if (hideAfterDelay)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    public void ClearTutorial()
    {
        if (tutorialText != null)
        {
            tutorialText.text = "";
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        ClearTutorial();
        hideCoroutine = null;
    }
}