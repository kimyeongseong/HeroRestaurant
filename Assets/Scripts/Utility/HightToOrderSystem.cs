using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class HightToOrderSystem : MonoBehaviour {
    [SerializeField]
    private float    multiplier   = 100;
    [SerializeField]
    private Renderer meshRenderer = null;

	void Update () {
        meshRenderer.sortingOrder = (int)(transform.position.y * -multiplier);
    } 
}
