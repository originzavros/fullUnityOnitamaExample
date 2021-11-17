using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSpawner
{
    public static GameObject SpawnTextAtPosition(Vector3 pos, string obname)
    {
        GameObject temp = new GameObject(obname);
        temp.AddComponent<TextMeshPro>();
        temp.transform.position = pos;
        return temp;
    }
}
