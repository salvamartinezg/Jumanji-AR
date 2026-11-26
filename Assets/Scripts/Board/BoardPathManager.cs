using UnityEngine;

public class BoardPathManager : MonoBehaviour
{
    [Header("Board Path")]
    [SerializeField] private Transform[] pathTiles;

    public int TotalTiles => pathTiles.Length;

    public Transform GetTile(int index)
    {
        if (index < 0 || index >= pathTiles.Length)
        {
            Debug.LogWarning("Invalid tile index: " + index);
            return null;
        }

        return pathTiles[index];
    }

    public TileType GetTileType(int index)
    {
        Transform tile = GetTile(index);
        if (tile == null)
            return TileType.Normal;

        TileData data = tile.GetComponent<TileData>();
        return data != null ? data.Type : TileType.Normal;
    }

    private void Start()
    {
        Debug.Log("Board initialized with " + TotalTiles + " tiles.");

        for (int i = 0; i < TotalTiles; i++)
        {
            Debug.Log("Tile " + (i + 1) + ": " + GetTile(i).name + " -> " + GetTileType(i));
        }
    }
}