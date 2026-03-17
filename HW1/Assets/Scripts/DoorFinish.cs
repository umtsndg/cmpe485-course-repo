using System.Collections;
using UnityEngine;

public class DoorFinish : MonoBehaviour
{
    public DoorController doorController;
    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.transform.root.CompareTag("Key"))
        {
            activated = true;
            StartCoroutine(OpenThenWin());
        }
    }

    IEnumerator OpenThenWin()
    {
        doorController.OpenDoor();

        yield return new WaitForSeconds(2f);

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.WinGame();
        }
    }
}