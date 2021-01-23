using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameResolutionInitializer {
    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        Screen.SetResolution(Screen.width, (Screen.width / 16) * 9, true);

        QualitySettings.vSyncCount  = 0;
        Application.targetFrameRate = 60;
    }
}
