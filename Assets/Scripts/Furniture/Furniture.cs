using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;
using Sirenix.OdinInspector;
using SimpleDatabase;

public enum FurnitureState
{
    NotBuy,
    Building,
    Bought
}

public enum Direction
{
    Up,
    Right,
    Down,
    Left,
    Max
}

[System.Serializable]
public class FurnitureEvent : UnityEvent<Furniture>
{
}

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Furniture : SerializedMonoBehaviour, ISavable {
    [SerializeField, TabGroup("Furniture")]
    private FurnitureData furnitureData  = new FurnitureData();

    private Sprite         buildingSprite = null;
    private Sprite[]       sprites        = null;

    private int currentSpriteIndex = 0;

    [Title("Event"), TabGroup("Furniture")]
    public FurnitureEvent onBuy            = new FurnitureEvent();
    [TabGroup("Furniture")]
    public FurnitureEvent onBuildCompleted = new FurnitureEvent();
    [TabGroup("Furniture")]
    public FurnitureEvent onRelocated      = new FurnitureEvent();

    protected SpriteRenderer  SpriteRenderer   { get; private set; }

    public Floor              OwnerFloor       { get; private set; }
    public Vector2Int         MapStartIndex    { get; private set; }
    public FurnitureState     CurrentState     { get; private set; }
    public Direction          CurrentDirection { get; private set; }
    
    public FurnitureData FurnitureData
    {
        get
        {
            FurnitureData readonlyData = furnitureData;
            return readonlyData;
        }
    }

    private void ChangeDirection(Direction direction)
    {
        CurrentDirection = (Direction)currentSpriteIndex;
        SpriteRenderer.sprite = sprites[currentSpriteIndex];
    }

    private IEnumerator Building()
    {
        if (furnitureData.buildCompleteTime > 0f)
        {
            SpriteRenderer.sprite = buildingSprite;
            yield return new WaitForSeconds(furnitureData.buildCompleteTime);
        }

        OnBuildCompleted();
    }

    private void OnBuildCompleted()
    {
        CurrentState = FurnitureState.Bought;
        SpriteRenderer.sprite = sprites[currentSpriteIndex];
        onBuildCompleted.Invoke(this);
    }

    public void Setup()
    {
        string objectID = gameObject.name.Replace("(Clone)", "");
        furnitureData   = Database.Instance.Select<FurnitureData>("FurnitureDataTable").Select(objectID);

        sprites = new Sprite[furnitureData.spritePaths.Length];
        var atlas = Resources.Load<SpriteAtlas>("Atlas/FurnitureAtlas");
        for (int i = 0; i < sprites.Length; i++)
            sprites[i] = atlas.GetSprite(furnitureData.spritePaths[i]);

        buildingSprite = atlas.GetSprite("Construction");

        SpriteRenderer        = GetComponent<SpriteRenderer>();
        SpriteRenderer.sprite = sprites[0];

        Resources.UnloadAsset(atlas);
    }

    public void ArrangeTo(Floor ownerFloor, Vector2Int mapStartIndex)
    {
        OwnerFloor    = ownerFloor;
        MapStartIndex = mapStartIndex;
    }

    public void Buy(GameObject buyer)
    {
        if (CurrentState == FurnitureState.NotBuy)
        {
            CurrentState = FurnitureState.Building;

            StartCoroutine("Building");

            onBuy.Invoke(this);
        }
    }

    public void Relocate(Vector2Int newMapStartIndex)
    {
        MapStartIndex = newMapStartIndex;

        onRelocated.Invoke(this);
    }

    virtual public void Rotate()
    {
        currentSpriteIndex = ++currentSpriteIndex % sprites.Length;
        ChangeDirection((Direction)currentSpriteIndex);
    }

    public void BuildCompleteImmediate()
    {
        if (CurrentState != FurnitureState.Bought)
        {
            StopCoroutine("Building");
            OnBuildCompleted();
        }
    }

    public virtual JSONObject SaveToJson()
    {
        return new JSONObject(JSONObject.Type.OBJECT);
    }

    public virtual void LoadFromJson(JSONObject root)
    { 
    }
}
