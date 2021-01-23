using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using SimpleDatabase;

public class WaitingStaffScrollView : MonoBehaviour {
    [SerializeField]
    private GameObject    waitingStaffItemPrefab = null;
    [SerializeField]
    private RectTransform content                = null;

    public void Awake()
    {
        var atlas = Resources.Load<SpriteAtlas>("Atlas/WaitingStaffPreviewAtlas");

        var waitingStaffDatas = Database.Instance.Select<WaitingStaffData>("WaitingStaffDataTable").Rows;
        foreach (var waitingStaffData in waitingStaffDatas)
        {
            var waitingStaffPrefab = Resources.Load<GameObject>("NPC/WaitingStaff/" + waitingStaffData.name);
            var previewSprite      = atlas.GetSprite(waitingStaffData.previewSpritePath);
            var waitingStaffItem   = Instantiate(waitingStaffItemPrefab, content);
            waitingStaffItem.GetComponent<WaitingStaffItem>().Setup(waitingStaffPrefab, previewSprite, waitingStaffData);
        }

        Resources.UnloadAsset(atlas);
    }
}
