using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public void Die()
    {
        Destroy(gameObject);
    }
}