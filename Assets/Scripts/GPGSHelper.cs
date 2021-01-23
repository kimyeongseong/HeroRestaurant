using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GPGSLoginEvent : UnityEvent
{
}

public class GPGSHelper : Singleton<GPGSHelper> {
    public GPGSLoginEvent    onLoginned            = new GPGSLoginEvent();

    public void Login()
    {
#if UNITY_EDITOR
        onLoginned.Invoke();
#else
        GPGSManager.Login(() => onLoginned.Invoke());
#endif
    }
}