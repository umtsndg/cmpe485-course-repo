using UnityEngine;
using TMPro;

public class ProjectileCountDebug : MonoBehaviour
{
    public TextMeshProUGUI projectileText;

    void Update()
    {
        EnemyProjectile[] projectiles = FindObjectsOfType<EnemyProjectile>();

        if (projectileText != null)
        {
            projectileText.text = "Projectiles: " + projectiles.Length;
        }
    }
}