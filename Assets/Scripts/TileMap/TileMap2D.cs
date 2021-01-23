using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public struct TileInfo
{
    public Vector2Int index;
    public GameObject tile;
}

public class TileMap2D : MonoBehaviour {
    [SerializeField, Required]
    private GameObject tilePrefab = null;
    [SerializeField]
    private Vector2Int mapSize    = Vector2Int.zero;
    [SerializeField]
    private Vector2    tileSize   = Vector2.zero;

    private Bounds bounds = new Bounds();

    public GameObject this[int column, int row] { get { return Tiles[(MapSize.x * column) + row]; } }

    public GameObject[]  Tiles    { get; private set; }
    public Vector2Int    MapSize  { get { return mapSize;  } set { mapSize  = value; } }
    public Vector2       TileSize { get { return tileSize; } set { tileSize = value; } }

    protected virtual void Awake()
    {
        LoadChildren();
        RecalculateBounds();
    }

    protected virtual void OnDrawGizmos()
    {
        RecalculateBounds();

        if (Tiles == null)
        {
            LoadChildren();
        }
    }

    private void LoadChildren()
    {
        Tiles = transform
                .Cast<Transform>()
                .ToArray()
                .Select(item => item.gameObject)
                .ToArray();

        if (Tiles.Length == 0)
            Tiles = null;

        RecalculateBounds();
    }

    private Vector3 CalculateLeftTopPosition()
    {
        Vector2 mapHalfSize     = new Vector2(mapSize.x, mapSize.y) * 0.5f;
        Vector3 tileHalfSize    = new Vector3(tileSize.x * 0.5f, -tileSize.y * 0.5f);
        Vector3 centerPosition  = new Vector3(-tileSize.x * mapHalfSize.x, tileSize.y * mapHalfSize.y);
        Vector3 leftTopPosition = transform.position + tileHalfSize + centerPosition;

        return leftTopPosition;
    }

    private void RecalculateBounds()
    {
        bounds.center  = transform.position;
        bounds.extents = tileSize * mapSize;
    }

    [Button]
    public virtual void GenerateMap()
    {
        Clear();

        Tiles = new GameObject[mapSize.y * mapSize.x];
        for (int column = 0; column < mapSize.y; column++)
        {
            for (int row = 0; row < mapSize.x; row++)
            {
                Vector3 spawnPosition = GetTilePosition(column, row);

                GameObject newTile = Instantiate(tilePrefab, transform, false);
                newTile.transform.position = spawnPosition;
                Tiles[(MapSize.x * column) + row] = newTile;
            }
        }
    }

    public void GenerateMap(int column, int row)
    {
        GenerateMap(column, row, tileSize);
    }

    public void GenerateMap(int column, int row, Vector2 tileSize)
    {
        this.tileSize = tileSize;
        mapSize       = new Vector2Int(row, column);
        GenerateMap();
    }

    public void GenerateMap(Vector2Int mapSize)
    {
        GenerateMap(mapSize.y, mapSize.x);
    }

    public void GenerateMap(Vector2Int mapSize, Vector2 tileSize)
    {
        GenerateMap(mapSize.y, mapSize.x, tileSize);
    }

    [Button]
    public void Reverse()
    {
        mapSize = new Vector2Int(mapSize.y, mapSize.x);

        if (Tiles != null)
        {
            for (int column = 0; column < mapSize.y; column++)
            {
                for (int row = 0; row < mapSize.x; row++)
                {
                    Vector3 spawnPosition = GetTilePosition(column, row);
                    this[column, row].transform.position = spawnPosition;
                }
            }
        }
    }

    [Button]
    public void Clear()
    {
        var childs = transform.Cast<Transform>().ToArray();
        for (int i = 0; i < childs.Length; i++)
        {
            var childObj = childs[i].gameObject;
            if (Application.isPlaying)
                Destroy(childObj);
            else
                DestroyImmediate(childObj);
        }

        Tiles = null;
    }

    public bool IsContains(Vector2 point)
    {
        return bounds.Contains(point);
    }

    public TileInfo GetClosestTile(Vector2 point)
    {
        float      minDistance      = float.MaxValue;
        GameObject closestTile      = null;
        Vector2Int closestTileIndex = Vector2Int.zero;

        for (int column = 0; column < mapSize.y; column++)
        {
            for (int row = 0; row < mapSize.x; row++)
            {
                var currentTile = this[column, row];

                float distance = ((Vector3)point - currentTile.transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTile = currentTile;
                    closestTileIndex.Set(row, column);
                }
            }
        }

        return new TileInfo {
            index = closestTileIndex,
            tile  = closestTile
        };
    }

    public Vector3 GetTilePosition(int column, int row)
    {
        Vector3 tilePosition = CalculateLeftTopPosition() + new Vector3(tileSize.x * row, -tileSize.y * column);
        return tilePosition;
    }

    public Vector3 GetTilePosition(Vector2Int index)
    {
        return GetTilePosition(index.y, index.x);
    }

    public Vector2Int GetTileIndex(GameObject tile)
    {
        for (int column = 0; column < mapSize.y; column++)
        {
            for (int row = 0; row < mapSize.x; row++)
            {
                var currentTile = this[column, row];
                if (currentTile == tile)
                    return new Vector2Int(row, column);
            }
        }

        return new Vector2Int(-1, -1);
    }
}
