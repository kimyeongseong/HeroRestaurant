using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using Sirenix.OdinInspector;

[RequireComponent(typeof(TouchInteraction))]
public class FurnitureController : MonoBehaviour, IBuyable {
    [SerializeField, Required]
    private GameObject       previewObject     = null;
    [SerializeField, Required]
    private Furniture        furniture         = null;
    [SerializeField, Required]
    private MarkTileMap2D    markTileMap       = null;
    [SerializeField, Required]
    private TouchInteraction touchInteraction  = null;
    [SerializeField, Required]
    private BoxCollider2D    touchCollider     = null;
    [SerializeField]
    private Vector2          colliderOffset    = Vector2.zero;
    [SerializeField]
    private float            colliderDirectionExtend = 0f;

    public GameObject       PreviewObject { get { return previewObject; } }
    public MarkTileMap2D    MarkTileMap   { get { return markTileMap;   } }
    public Furniture        Furniture     { get { return furniture;     } }
    
    public bool IsTouchEnable
    {
        set
        {
            touchCollider.enabled = value;
        }
    }

    public bool IsInteriorMode
    {
        set
        {
            PreviewObject.SetActive(value);
            MarkTileMap.gameObject.SetActive(value);
            touchCollider.enabled = !value;
        }
    }

    private void Awake() {
        IsInteriorMode = false;
        touchCollider.isTrigger = true;

        furniture.Setup();

        MarkTileMap.GenerateMap(furniture.FurnitureData.tileSize);

        PreviewObjectSetup();
        ResizingColliders();

        touchInteraction.onClicked.AddListener(OnClicked);
    }

    private void OnDestroy()
    {
        if (PreviewObject != null)
            Destroy(PreviewObject);
    }

    private void PreviewObjectSetup()
    {
        var previewRenderer = PreviewObject.GetComponent<SpriteRenderer>();
        previewRenderer.sprite = GetComponent<SpriteRenderer>().sprite;

        var draggableObject = PreviewObject.GetComponent<DraggableObject>();
        draggableObject.onDragStarted.AddListener(OnDragStarted);
        draggableObject.onDragEnded.AddListener(OnDragEnded);

        var draggableTileMap = PreviewObject.GetComponent<TileMap2D>();
        draggableTileMap.MapSize = MarkTileMap.MapSize;
    }

    private void ResizingColliders()
    {
        Vector2 colliderSize = (MarkTileMap.MapSize * MarkTileMap.TileSize) + colliderOffset;
        colliderSize /= transform.lossyScale;

        var draggableCollider = PreviewObject.GetComponent<BoxCollider2D>();

        draggableCollider.size = colliderSize;
        touchCollider.size = colliderSize;

        switch (Furniture.CurrentDirection)
        {
            case Direction.Up:
                touchCollider.offset = new Vector2(0f, -colliderDirectionExtend);
                break;

            case Direction.Right:
                touchCollider.offset = new Vector2(-colliderDirectionExtend, 0f);
                break;

            case Direction.Down:
                touchCollider.offset = new Vector2(0f, colliderDirectionExtend);
                break;

            case Direction.Left:
                touchCollider.offset = new Vector2(colliderDirectionExtend, 0f);
                break;
        }
        
        touchCollider.size += new Vector2(Mathf.Abs(touchCollider.offset.x), Mathf.Abs(touchCollider.offset.y));
    }

    private void OnDragStarted(GameObject target)
    {
        var previewObject = PreviewObject;
        previewObject.transform.parent = null;
        previewObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void OnDragEnded(GameObject target)
    {
        var previewObject = PreviewObject;
        previewObject.transform.SetParent(furniture.transform);
        previewObject.transform.localPosition = Vector3.zero;
        previewObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnClicked(GameObject target)
    {
        if (furniture.CurrentState == FurnitureState.Bought &&
            InteriorSystem.Instance.IsRebuildMode)
        {
            InteriorSystem.Instance.Select(this);
        }
    }

    public void BuildCompleteImmediate()
    {
        furniture.BuildCompleteImmediate();
    }

    public void ArrangeTo(Floor onwerFloor, Vector2Int mapStartIndex)
    {
        Furniture.ArrangeTo(onwerFloor, mapStartIndex);
    }

    public void Buy(GameObject buyer)
    {
        furniture.Buy(buyer);
    }

    public void Relocate(Vector2Int newMapStartIndex)
    {
        furniture.Relocate(newMapStartIndex);
    }

    public void Rotate()
    {
        Furniture.Rotate();

        var draggableTileMap = PreviewObject.GetComponent<TileMap2D>();
        draggableTileMap.Reverse();
        MarkTileMap.Reverse();

        ResizingColliders();

        PreviewObject.GetComponent<SpriteRenderer>().sprite = Furniture.GetComponent<SpriteRenderer>().sprite;
    }
}
