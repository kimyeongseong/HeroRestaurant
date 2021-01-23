using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class SpriteAtlasUtility {
    public static Sprite GetSprite(string atlasPath, string spriteName)
    {
        var atlas = Resources.Load<SpriteAtlas>(atlasPath);
        var sprite = atlas.GetSprite(spriteName);

        Resources.UnloadAsset(atlas);

        return sprite;
    }
}
