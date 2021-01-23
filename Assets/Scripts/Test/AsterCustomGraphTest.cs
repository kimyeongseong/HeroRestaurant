using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AsterCustomGraphTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var graph = (GridGraph)AstarPath.active.data.graphs[0];
        GridGraph g = (GridGraph)AstarPath.active.data.AddGraph(typeof(GridGraph));
        g.neighbours = NumNeighbours.Four;
        g.SetDimensions(graph.width, graph.depth, graph.nodeSize);
        g.collision.use2D = true;
        g.collision.type = ColliderType.Ray;
        g.rotation = new Vector3(-90f, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
