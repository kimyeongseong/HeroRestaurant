using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSpan : MonoBehaviour {
    [SerializeField]
    private float lifeTime = 0f;

	void Start () {
        Destroy(gameObject, lifeTime);
	}
}
