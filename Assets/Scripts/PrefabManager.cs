using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject[] prefabs;

    private void Awake()
    {
        instance = this;
    }

    public GameObject GetPrefab(string s)
    {
        foreach (GameObject prefab in prefabs)
        {
            if (prefab.name.Equals(s))
            {
                return prefab;
            }
        }
        return null;
    }
}
