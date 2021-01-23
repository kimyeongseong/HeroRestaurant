using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

/* TODO : 이후에 따로 Presentor 스크립트를 만들어서 아이템 목록을 DB에서 받아와 아이템들을 동적 생성하게 만들 필요가 있음.
   현재는 테이블에 아이템을 불러올 Path 정보가 없기 때문에 직접 삽입하는 방식으로 구현. 
*/
public class FurnitureItem : MonoBehaviour {
    [SerializeField, Required]
    private Image      previewImage    = null;

    private GameObject furniturePrefab = null;

    private int furniturePrice = 0;

    public void Setup(GameObject furniturePrefab, Sprite previewSprite, FurnitureData furnitrueData)
    {
        this.furniturePrefab = furniturePrefab;
        previewImage.sprite  = previewSprite;
        furniturePrice       = furnitrueData.buyPrice;
    }

    public void OnCliked()
    {
        var furnitureObj        = Instantiate(furniturePrefab);
        var furnitureController = furnitureObj.GetComponent<FurnitureController>();

        InteriorSystem.Instance.Select(furnitureController);
    }
}
