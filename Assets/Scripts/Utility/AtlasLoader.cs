using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasLoader : MonoBehaviour {
    private void OnEnable()
    {
        SpriteAtlasManager.atlasRequested += RequestAtlas;
    }

    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= RequestAtlas;
    }

    void RequestAtlas(string tag, System.Action<SpriteAtlas> callback)
    {
        var sa = Resources.Load<SpriteAtlas>("atlas/" + tag);
        callback(sa);
    }
}
