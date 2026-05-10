using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 5f;

    private Vector3 moveDirection;
    private bool isReflected = false;
    private float reflectIgnoreTimer = 0f;

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    public void Reflect(Vector3 newDirection)
    {
        isReflected = true;
        reflectIgnoreTimer = 0.1f;

        moveDirection = newDirection.normalized;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (reflectIgnoreTimer > 0f)
        {
            reflectIgnoreTimer -= Time.deltaTime;
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore immediate re-collision with player just after reflect
        if (reflectIgnoreTimer > 0f && other.CompareTag("Player"))
        {
            return;
        }

        // Bullet hits player before reflection
        if (!isReflected && other.CompareTag("Player"))
        {
            PlayerBlock playerBlock = other.GetComponentInParent<PlayerBlock>();
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

            if (playerBlock != null)
            {
                BlockResult result = playerBlock.TryHandleIncomingHit(1, moveDirection);

                if (result.wasBlocked)
                {
                    if (result.wasParried)
                    {
                        Transform playerView = playerBlock.playerView;
                        if (playerView != null)
                        {
                            Reflect(playerView.forward);
                            if (AudioManager.Instance != null)
                            {
                                AudioManager.Instance.PlayParry();
                            }
                            return;
                        }
                    }

                    Destroy(gameObject);
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayBlock();
                    }
                    return;
                }
            }

            if (playerHealth != null)
            {
                playerHealth.Die();
                return;
            }
        }

        // Reflected bullet hits enemy
        if (isReflected && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.Die();
            }

            Destroy(gameObject);
            return;
        }

        // Hit something solid
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}