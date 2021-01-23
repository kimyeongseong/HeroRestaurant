using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable {
    void Use(GameObject user);
    void Unuse(GameObject user);
}
