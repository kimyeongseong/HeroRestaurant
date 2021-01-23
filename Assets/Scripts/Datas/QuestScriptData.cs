using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ScriptData
{
    public string talkerName;
    [TextArea]
    public string script;
}

public struct QuestScriptData {
    public string scriptID;
    public ScriptData[] scriptDatas;
}
