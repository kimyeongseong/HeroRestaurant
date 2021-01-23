using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour {
    [SerializeField]
    private Vector3 arrivePostion = Vector3.zero;

    public void Move()
    {
        transform.position = arrivePostion;
    }
}
