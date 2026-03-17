using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;

    public void OpenDoor()
    {
        animator.SetTrigger("Open");
    }
}