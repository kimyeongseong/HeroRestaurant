using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<T>();

            return instance;
        }
    }
}
