using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingObject : MonoBehaviour {
    private Queue<GameObject> ownedQueue = null;

    public void Setup(Queue<GameObject> ownedQueue)
    {
        this.ownedQueue = ownedQueue;
    }

    private void OnDisable()
    {
        ownedQueue.Enqueue(gameObject);
    }
}
