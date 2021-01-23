using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : Furniture {
    private HashSet<Chair> linkedChairs = new HashSet<Chair>();

    private void Awake()
    {
        onBuildCompleted.AddListener(LinkWithSurroundingChairs);
        onRelocated.AddListener(LinkWithSurroundingChairs);
    }

    private void LinkWithSurroundingChairs(Furniture furnitrue)
    {
        UnlinkAllChairs();

        var searchInfos = OwnerFloor.SearchSurroundingFurniture<Chair>(this);
        if (searchInfos == null)
            return;

        foreach (var searchInfo in searchInfos)
        {
            var chair = searchInfo.furniture;
            if (chair.CurrentState == FurnitureState.Bought && chair.CurrentDirection == searchInfo.relativeDirection)
            {
                LinkChair(chair);
                chair.LinkedTable = this;
            }
        }
    }

    public void LinkChair(Chair chair)
    {
        linkedChairs.Add(chair);
    }

    public void UnlinkChair(Chair chair)
    {
        linkedChairs.Remove(chair);
    }

    public void UnlinkAllChairs()
    {
        foreach (var chair in linkedChairs)
            chair.LinkedTable = null;

        linkedChairs.Clear();
    }
}
