using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

public class Chair : Furniture, IUsable {
    [SerializeField, TabGroup("Chair")]
    private LineRenderer linkLineRenderer = null;
    [SerializeField, TabGroup("Chair")]
    private Transform[]  shitPoints       = null;

    private int   currentShitPointIndex = 0;
    private Table linkedTable           = null;

    public Table LinkedTable
    {
        get
        {
            return linkedTable;
        }
        set
        {
            linkedTable = value;
            if (linkedTable != null)
                linkLineRenderer.SetPositions(new Vector3[2] { transform.position, value.transform.position });
            else
                linkLineRenderer.SetPositions(new Vector3[2] { Vector3.zero, Vector3.zero });
        }
    }
    public GameObject UsingActor  { get; private set; }

    public bool IsUseable { get; set; } = true;

    private Direction ReverseDirection { get { return (Direction)(((int)CurrentDirection + 2) % (int)Direction.Max); } }

    private void Awake()
    {
        onBuildCompleted.AddListener(LinkWithSurroundingTable);
        onRelocated.AddListener(LinkWithSurroundingTable);

        GameMode.Instance.onEditorModeStarted.AddListener(ShowLinkedLine);
        GameMode.Instance.onBusinessModeStarted.AddListener(HideLinkedLine);

    }

    private void OnDestroy()
    {
        var gameMode = GameMode.Instance;
        if (gameMode)
        {
            GameMode.Instance.onEditorModeStarted.RemoveListener(ShowLinkedLine);
            GameMode.Instance.onBusinessModeStarted.RemoveListener(HideLinkedLine);
        }
    }

    private void OnDrawGizmos()
    {
        if (LinkedTable)
            Debug.DrawLine(transform.position, LinkedTable.transform.position, Color.green);
    }

    public override void Rotate()
    {
        base.Rotate();

        currentShitPointIndex = ++currentShitPointIndex % shitPoints.Length;
    }

    private void ShowLinkedLine()
    {
        linkLineRenderer.enabled = true;
    }

    private void HideLinkedLine()
    {
        linkLineRenderer.enabled = false;
    }

    private void LinkWithSurroundingTable(Furniture furnitrue)
    {
        if (LinkedTable)
        {
            LinkedTable.UnlinkChair(this);
            LinkedTable = null;
        }

        var searchInfos = OwnerFloor.SearchSurroundingFurniture<Table>(this);
        if (searchInfos == null)
            return;

        foreach (var searchInfo in searchInfos)
        {
            if (searchInfo.relativeDirection == ReverseDirection &&
                searchInfo.furniture.CurrentState == FurnitureState.Bought)
            {
                LinkedTable = searchInfo.furniture;
                LinkedTable.LinkChair(this);
                break;
            }
        }
    }    

    public void Use(GameObject user)
    {
        IsUseable  = false;
        UsingActor = user; 

        user.GetComponent<HightToOrderSystem>().enabled = false;
        user.GetComponent<AILerp>().enabled = false;

        user.GetComponent<SpriteRenderer>().sortingOrder = CurrentDirection == Direction.Down ?
                                                           SpriteRenderer.sortingOrder - 1 :
                                                           SpriteRenderer.sortingOrder + 1;


        user.transform.position = shitPoints[currentShitPointIndex].position;

        var userAnimator = user.GetComponent<Animator>();
        float vertical   = 0f;
        float horizontal = 0f;
        switch (CurrentDirection)
        {
            case Direction.Up:
                vertical = -1f;
                break;

            case Direction.Right:
                horizontal = -1f;
                break;

            case Direction.Down:
                vertical = 1f;
                break;

            case Direction.Left:
                horizontal = 1f;
                break;
        }

        userAnimator.SetFloat("vertical", vertical);
        userAnimator.SetFloat("horizontal", horizontal);
        userAnimator.SetBool("isWaiting", true);
    }

    public void Unuse(GameObject user)
    {
        UsingActor = null;
        IsUseable = true;

        user.GetComponent<AILerp>().enabled = true;
        user.GetComponent<HightToOrderSystem>().enabled = true;

        user.GetComponent<Animator>().SetBool("isWaiting", false);
        
    }
}
