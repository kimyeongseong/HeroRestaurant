using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetComponentsTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var test = GetComponents<ISavable>();
        print(test);
        print(test.Length);
	}
}
