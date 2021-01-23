using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public enum TileMarkType
{
    Normal,
    Error
}

public class MarkTileMap2D : TileMap2D {
    [SerializeField]
    private TileMarkSpriteContainer markSpriteSetting = null;

    public override void GenerateMap()
    {
        base.GenerateMap();

        foreach (var tile in Tiles)
        {
            var tileRenderer = tile.GetComponent<SpriteRenderer>();
            if (tileRenderer == null)
                tileRenderer = tile.AddComponent<SpriteRenderer>();

            tileRenderer.sprite = markSpriteSetting[TileMarkType.Normal];

        }
    }

    public void SetMark(int column, int row, TileMarkType tileMarkType)
    {
        var tileRenderer = this[column, row].GetComponent<SpriteRenderer>();
        tileRenderer.sprite = markSpriteSetting[tileMarkType];
    }

    public void SetMark(Vector2Int index, TileMarkType tileMarkType)
    {
        SetMark(index.y, index.x, tileMarkType);
    }
}
