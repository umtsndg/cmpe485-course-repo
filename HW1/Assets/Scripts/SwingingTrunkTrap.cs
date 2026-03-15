using System.Collections;
using UnityEngine;

public class SwingingTrunkTrap : MonoBehaviour
{
    [Header("Swing Settings")]
    public float leftAngle = -45f;
    public float rightAngle = 45f;
    public float swingDuration = 1f;
    public float pauseAtEnds = 0.2f;

    [Header("Damage")]
    public bool failOnTouch = true;

    private void Start()
    {
        StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(RotateToAngle(leftAngle, rightAngle, swingDuration));
            yield return new WaitForSeconds(pauseAtEnds);

            yield return StartCoroutine(RotateToAngle(rightAngle, leftAngle, swingDuration));
            yield return new WaitForSeconds(pauseAtEnds);
        }
    }

    IEnumerator RotateToAngle(float startAngle, float endAngle, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float zAngle = Mathf.Lerp(startAngle, endAngle, t);

            transform.localRotation = Quaternion.Euler(0f, 0f, zAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, endAngle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!failOnTouch) return;

        if (other.CompareTag("Player"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.FailGame();
            }
        }
    }
}