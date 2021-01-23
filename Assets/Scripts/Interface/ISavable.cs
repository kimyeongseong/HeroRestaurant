using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavable {
    JSONObject SaveToJson();
    void LoadFromJson(JSONObject json);
}
