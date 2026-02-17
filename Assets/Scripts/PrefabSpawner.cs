using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab to spawn")]
    public GameObject prefab;

    [Header("Spawn settings")]
    public Transform spawnPoint;
    public KeyCode spawnKey = KeyCode.Space;

    public bool randomOffset = true;
    public Vector3 offsetRange = new Vector3(1.5f, 0f, 1.5f);

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            if (prefab == null || spawnPoint == null)
            {
                Debug.LogWarning("Spawner missing prefab or spawnPoint assignment.");
                return;
            }

            Vector3 pos = spawnPoint.position;

            if (randomOffset)
            {
                pos += new Vector3(
                    Random.Range(-offsetRange.x, offsetRange.x),
                    Random.Range(-offsetRange.y, offsetRange.y),
                    Random.Range(-offsetRange.z, offsetRange.z)
                );
            }

            Instantiate(prefab, pos, spawnPoint.rotation);
        }
    }
}
