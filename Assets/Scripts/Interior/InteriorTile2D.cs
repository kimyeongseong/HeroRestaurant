using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TileEvent : UnityEvent<InteriorTile2D>
{
}

public enum InteriorTileState
{
    NotBuy,
    Filled,
    Opend,
}

[RequireComponent(typeof(SpriteRenderer), typeof(TouchInteraction), typeof(BoxCollider2D))]
public class InteriorTile2D : MonoBehaviour, IBuyable {
    [SerializeField]
    private InteriorTileSpriteContainer interiorTileSpriteConatiner = null;

    private Furniture      linkedFurniture = null;
    private SpriteRenderer spriteRenderer  = null;

    public TileEvent onBuy = new TileEvent();

    public InteriorTileMap2D OwnerTileMap  { get; private set; }
    public Vector2Int        MapStartIndex { get; private set; }

    public Furniture LinkedFurniture
    {
        get
        {
            return linkedFurniture;
        }
        set
        {
            if (value != null)
                ChangeState(InteriorTileState.Filled);
            else
                ChangeState(InteriorTileState.Opend);

            linkedFurniture = value;
        }
    }

    public InteriorTileState CurrentState { get; private set; }

    public void ChangeState(InteriorTileState newState)
    {
        CurrentState          = newState;
        spriteRenderer.sprite = interiorTileSpriteConatiner[CurrentState];

        if (newState == InteriorTileState.Opend)
            GetComponent<BoxCollider2D>().enabled = false;
    }

    public void Setup(InteriorTileMap2D ownerTileMap, InteriorTileState tileState, Vector2Int mapStartIndex)
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;

        spriteRenderer = GetComponent<SpriteRenderer>();

        OwnerTileMap  = ownerTileMap;
        MapStartIndex = mapStartIndex;
        ChangeState(tileState);
    }

    public void Buy(GameObject buyer)
    {
        ChangeState(InteriorTileState.Opend);
        GetComponent<BoxCollider2D>().enabled = false;
        onBuy.Invoke(this);
    }
}
