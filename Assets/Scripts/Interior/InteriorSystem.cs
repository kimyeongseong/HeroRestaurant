using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class InteriorSystemEvent : UnityEvent<FurnitureController>
{
}

public struct FurnitureSearchInfo<T> where T : Furniture
{
    public Direction relativeDirection;
    public T furniture;

    public FurnitureSearchInfo(Direction relativeDirection, T furniture)
    {
        this.relativeDirection = relativeDirection;
        this.furniture         = furniture;
    }
}

/* 
 * TODO : 현재 Door는 특수한 가구라고 보고 단순히 셀렉트된 가구의 타입을 가지고 어떤 Map에 가구를 설치할 지 결정하는데,
 * 이후 Door처럼 다른 가구들과 Map을 공유하지 않는 특수 가구들이 더 생기면 혹은 Door의 맵을 공유하는 다른 가구 생기면
 * enum과 Dictionary를 이용해 설치할 맵을 선택하는 것으로 리팩토링 필요.
 */
public class InteriorSystem : Singleton<InteriorSystem> {
    [SerializeField]
    private Transform furnitureBuildButtonTransform = null;

    private Floor             selectedFloor           = null;
    private InteriorTileMap2D interiorTileMap         = null;
    private InteriorTileMap2D doorTileMap             = null;
    private InteriorTileMap2D selectedInteriorTileMap = null;

    private FurnitureController selectedFurnitureController     = null;
    private Vector3             selectedFurnitureOriginPosition = Vector3.zero;
    private TileInfo            prevClosestTileInfo;

    private bool isFurnitureOverlapsObstacle = false;

    public InteriorSystemEvent onFurnitureSelected   = new InteriorSystemEvent();
    public InteriorSystemEvent onComplete            = new InteriorSystemEvent();
    public InteriorSystemEvent onDemolish            = new InteriorSystemEvent();
    public InteriorSystemEvent onCancel              = new InteriorSystemEvent();

    public bool  IsRebuildMode      { get; set; }
    public Floor InteriorTargetFloor
    {
        get
        {
            return selectedFloor;
        }
        set
        {
            selectedFloor   = value;
            interiorTileMap = selectedFloor?.InteriorTileMap;
            doorTileMap     = selectedFloor?.DoorTileMap;
        }
    }

    private Vector3 GetPositionOffset(TileMap2D tileMap)
    {
        Vector2 mapHalfSize  = (Vector2)tileMap.MapSize * 0.5f;
        Vector2 tileHalfSize = tileMap.TileSize * 0.5f;

        return new Vector3((mapHalfSize.x * tileMap.TileSize.x) - tileHalfSize.x,
                           (-mapHalfSize.y * tileMap.TileSize.y) + tileHalfSize.y);
    }

    private void SelectedFurnitureMoveTo(GameObject targetObject)
    {
        if (!selectedFurnitureController)
            return;

        var     previewTileMap         = selectedFurnitureController.PreviewObject.GetComponent<TileMap2D>();
        Vector3 previewTileMapPosition = previewTileMap.GetTilePosition(0, 0);

        if (selectedInteriorTileMap.IsContains(previewTileMapPosition))
        {
            TileInfo currentClosestTileInfo = selectedInteriorTileMap.GetClosestTile(previewTileMapPosition);
            if (currentClosestTileInfo.tile != prevClosestTileInfo.tile)
            {
                var     markTileMap         = selectedFurnitureController.MarkTileMap;
                Vector3 closestTilePosition = currentClosestTileInfo.tile.transform.position;
                selectedFurnitureController.transform.position = closestTilePosition + GetPositionOffset(selectedFurnitureController.MarkTileMap);

                FurnitureOverlapUpdate(currentClosestTileInfo.index, selectedFurnitureController);

                prevClosestTileInfo = currentClosestTileInfo;

                Vector2 furnitureScreenPosition = Camera.main.WorldToScreenPoint(selectedFurnitureController.transform.position);
                furnitureBuildButtonTransform.position = furnitureScreenPosition;
            }
        }
    }

    private void FurnitureOverlapUpdate(Vector2Int startIndex, FurnitureController furniture)
    {
        var        markTileMap         = furniture.MarkTileMap;
        Vector2Int markTileMapSize     = markTileMap.MapSize;
        Vector2Int endIndex            = startIndex + markTileMapSize;
        Vector2Int interiorTileMapSize = selectedInteriorTileMap.MapSize;

        isFurnitureOverlapsObstacle = false;

        for (int column = startIndex.y; column < endIndex.y; column++)
        {
            for (int row = startIndex.x; row < endIndex.x; row++)
            {
                Vector2Int currentMarkTileMapIndex = new Vector2Int(row - startIndex.x, column - startIndex.y);
                if (column >= interiorTileMapSize.y || row >= interiorTileMapSize.x)
                {
                    markTileMap.SetMark(currentMarkTileMapIndex, TileMarkType.Error);
                    isFurnitureOverlapsObstacle = true;
                    continue;
                }

                var interiorTile = selectedInteriorTileMap[column, row];

                if (interiorTile.LinkedFurniture != selectedFurnitureController.Furniture &&
                    interiorTile.CurrentState != InteriorTileState.Opend)
                {
                    markTileMap.SetMark(currentMarkTileMapIndex, TileMarkType.Error);
                    isFurnitureOverlapsObstacle = true;
                }
                else
                {
                    markTileMap.SetMark(currentMarkTileMapIndex, TileMarkType.Normal);
                }
            }
        }
    }

    private void SetInteriorMode(bool isEnable, FurnitureController furnitrueConroller = null)
    {
        var prevFurnitureController = selectedFurnitureController == null ? furnitrueConroller : selectedFurnitureController;
        prevFurnitureController.IsInteriorMode = isEnable;

        selectedInteriorTileMap = prevFurnitureController.Furniture.GetType() == typeof(Door) ? doorTileMap : interiorTileMap;

        selectedFurnitureController = furnitrueConroller;

        doorTileMap.IsInteractionEnable     = !isEnable;
        interiorTileMap.IsInteractionEnable = !isEnable;

        var draggableObject = prevFurnitureController.PreviewObject.GetComponent<DraggableObject>();
        if (!isEnable)
            draggableObject.onDragging.RemoveListener(SelectedFurnitureMoveTo);
        else
            draggableObject.onDragging.AddListener(SelectedFurnitureMoveTo);

        selectedFloor.SetArragnedFurnituresInteractionEnable(!isEnable);
        furnitureBuildButtonTransform.gameObject.SetActive(isEnable);
    }

    public void LinkFurnitureToTiles(InteriorTileMap2D tileMap, Furniture furnitrue, Vector2Int linkStartTileIndex, Vector2Int furnitrueTileSzie)
    {
        Vector2Int tileEndIndex = linkStartTileIndex + furnitrueTileSzie;

        for (int column = linkStartTileIndex.y; column < tileEndIndex.y; column++)
        {
            for (int row = linkStartTileIndex.x; row < tileEndIndex.x; row++)
            {
                var interiorTile = tileMap[column, row];
                interiorTile.LinkedFurniture = furnitrue;
            }
        }
    }

    public void Select(FurnitureController furnitureController)
    {
        SetInteriorMode(true, furnitureController);

        if (selectedFurnitureController.Furniture.CurrentState == FurnitureState.NotBuy)
        {
            Vector2Int mapSize = selectedInteriorTileMap.MapSize;
            Vector2Int centerIndex = new Vector2Int(mapSize.x / 2, mapSize.y / 2);
            var centerPosition = selectedInteriorTileMap.GetTilePosition(centerIndex.y, centerIndex.x);

            selectedFurnitureController.transform.position = centerPosition + GetPositionOffset(selectedFurnitureController.MarkTileMap);
            FurnitureOverlapUpdate(centerIndex, selectedFurnitureController);

            prevClosestTileInfo = new TileInfo
            {
                index = centerIndex,
                tile = selectedInteriorTileMap[centerIndex.y, centerIndex.x].gameObject
            };
        }
        else
        {
            var startIndex = selectedFurnitureController.Furniture.MapStartIndex;
            prevClosestTileInfo = new TileInfo
            {
                index = startIndex,
                tile = selectedInteriorTileMap[startIndex.y, startIndex.x].gameObject
            };

            selectedFurnitureOriginPosition = selectedFurnitureController.transform.position;

            LinkFurnitureToTiles(selectedInteriorTileMap,
                     null,
                     furnitureController.Furniture.MapStartIndex,
                     furnitureController.MarkTileMap.MapSize);
        }

        Vector2 furnitureScreenPosition = Camera.main.WorldToScreenPoint(selectedFurnitureController.transform.position);
        furnitureBuildButtonTransform.position = furnitureScreenPosition;

        onFurnitureSelected.Invoke(selectedFurnitureController);
    }

    public void OnComplete()
    {
        int furniturePrice = selectedFurnitureController.Furniture.FurnitureData.buyPrice;

        if (!isFurnitureOverlapsObstacle &&
            Inventory.Instance.CurrentMoney >= furniturePrice)
        {
            Inventory.Instance.IncreaseMoney(-furniturePrice);

            if (selectedFurnitureController.Furniture.CurrentState == FurnitureState.NotBuy)
            {
                selectedFurnitureController.ArrangeTo(selectedFloor, prevClosestTileInfo.index);
                selectedFurnitureController.Buy(null);

                selectedFloor.InsertFurniture(selectedFurnitureController.Furniture);
            }
            else
                selectedFurnitureController.Relocate(prevClosestTileInfo.index);

            LinkFurnitureToTiles(selectedInteriorTileMap,
                                 selectedFurnitureController.Furniture,
                                 prevClosestTileInfo.index,
                                 selectedFurnitureController.MarkTileMap.MapSize);

            onComplete.Invoke(selectedFurnitureController);
            SetInteriorMode(false);

            selectedFloor.GraphScan();
        }
    }

    public void OnDemolish()
    {
        if (selectedFurnitureController.Furniture.CurrentState == FurnitureState.Bought)
        {
            var furnitureData = selectedFurnitureController.Furniture.FurnitureData;
            Inventory.Instance.IncreaseMoney(Mathf.CeilToInt(furnitureData.buyPrice * furnitureData.demolitionCompensationPercent));

            selectedFloor.RemoveFurniture(selectedFurnitureController.Furniture);

            onDemolish.Invoke(selectedFurnitureController);
            Destroy(selectedFurnitureController.gameObject);

            selectedFloor.GraphScan();

            SetInteriorMode(false);
        }
    }

    public void OnCancel()
    {
        if (selectedFurnitureController.Furniture.CurrentState == FurnitureState.NotBuy)
            Destroy(selectedFurnitureController.gameObject);
        else
            selectedFurnitureController.transform.position = selectedFurnitureOriginPosition;

        onCancel.Invoke(selectedFurnitureController);
        SetInteriorMode(false);
    }

    public void OnRotate()
    {
        selectedFurnitureController.Rotate();
        selectedFurnitureController.transform.position = prevClosestTileInfo.tile.transform.position + GetPositionOffset(selectedFurnitureController.MarkTileMap);
        FurnitureOverlapUpdate(prevClosestTileInfo.index, selectedFurnitureController);
    }
}