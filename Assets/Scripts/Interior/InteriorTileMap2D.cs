using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class InteriorTileMapEvent : UnityEvent<InteriorTileMap2D>
{
}

public class InteriorTileMap2D : TileMap2D, ISavable {
    [SerializeField]
    private TileData          tileData;
    [SerializeField]
    private InteriorTileState tileInitialState = InteriorTileState.NotBuy;

    public InteriorTileMapEvent onBoughtAllTiles = new InteriorTileMapEvent();

    private InteriorTile2D[,] interiorTiles    = null;
    private int               numOfBoughtTiles = 0;
    private int               tileBuyPrice     = 0;
    private Button            tileBuyButton    = null;

    private InteriorTile2D currentSelectedTile = null;

    public new InteriorTile2D this[int column, int row] { get { return interiorTiles[row, column]; } }

    public bool IsInteractionEnable
    {
        set
        {
            foreach (var interiorTile in interiorTiles)
            {
                if (interiorTile.CurrentState == InteriorTileState.NotBuy)
                    interiorTile.GetComponent<BoxCollider2D>().enabled = value;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        tileBuyPrice  = tileData.initialTilePrice;
        tileBuyButton = GameObject.FindWithTag("TileBuyButton").GetComponent<Button>();

        interiorTiles = new InteriorTile2D[MapSize.x, MapSize.y];
        for (int column = 0; column < MapSize.y; column++)
        {
            for (int row = 0; row < MapSize.x; row++)
            {
                var interiorTile = base[column, row].GetComponent<InteriorTile2D>();
                interiorTile.Setup(this, tileInitialState, new Vector2Int(row, column));
                if (tileInitialState != InteriorTileState.Opend)
                    interiorTile.GetComponent<TouchInteraction>().onClicked.AddListener(OnClickedTile);

                interiorTiles[row, column] = interiorTile;
            }
        }
    }

    private void OnClickedTile(GameObject tile)
    {
        var screenPoint = Camera.main.WorldToScreenPoint(tile.transform.position);

        tileBuyButton.gameObject.SetActive(true);
        tileBuyButton.transform.position = screenPoint;

        tileBuyButton.onClick.RemoveAllListeners();
        tileBuyButton.onClick.AddListener(OnClickedButton);

        currentSelectedTile = tile.GetComponent<InteriorTile2D>();
    }

    private void OnClickedButton()
    {
        var inventory = Inventory.Instance;
        if (inventory.CurrentMoney >= tileBuyPrice)
        {
            inventory.IncreaseMoney(-tileBuyPrice);
            currentSelectedTile.Buy(null);
            currentSelectedTile = null;

            tileBuyButton.GetComponent<RectTransform>().anchoredPosition = Vector2.one * 10000;

            tileBuyPrice += Mathf.CeilToInt(tileData.initialTilePrice * tileData.priceIncreasePercentPerBuy);

            if (++numOfBoughtTiles == interiorTiles.Length)
                onBoughtAllTiles.Invoke(this);
        }
    }

    public override void GenerateMap()
    {
        base.GenerateMap();

        foreach (var tile in Tiles)
        {
            var interiorTile = tile.GetComponent<InteriorTile2D>();
            if (interiorTile == null)
                tile.AddComponent<InteriorTile2D>();
        }
    }

    public JSONObject SaveToJson()
    {
        var root = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var interiorTile in interiorTiles)
        {
            root.Add((int)interiorTile.CurrentState);
        }

        return root;
    }

    public void LoadFromJson(JSONObject jsonObject)
    {
        int index = 0;
        foreach (var interiorTile in interiorTiles)
        {
            if ((InteriorTileState)jsonObject[index++].i != InteriorTileState.NotBuy)
            {
                interiorTile.Buy(null);
                tileBuyPrice += Mathf.CeilToInt(tileData.initialTilePrice * tileData.priceIncreasePercentPerBuy);

                if (++numOfBoughtTiles == interiorTiles.Length)
                    onBoughtAllTiles.Invoke(this);
            }
        }
    }
}
