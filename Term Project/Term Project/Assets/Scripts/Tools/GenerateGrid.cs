using UnityEngine;

[ExecuteAlways]
public class GridGroundGenerator : MonoBehaviour
{
    public GameObject groundTilePrefab;
    public int width = 5;
    public int height = 5;
    public float tileSize = 1f;
    public bool centerGrid = true;

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        ClearGrid();

        Vector3 offset = Vector3.zero;

        if (centerGrid)
        {
            offset = new Vector3(
                -(width - 1) * tileSize / 2f,
                0f,
                -(height - 1) * tileSize / 2f
            );
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = new Vector3(
                    x * tileSize,
                    0f,
                    z * tileSize
                ) + offset;

                GameObject tile = Instantiate(
                    groundTilePrefab,
                    transform.position + position,
                    Quaternion.identity,
                    transform
                );

                tile.name = $"GroundTile_{x}_{z}";
            }
        }
    }

    [ContextMenu("Clear Grid")]
    public void ClearGrid()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}