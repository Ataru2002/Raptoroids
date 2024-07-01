using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolCommons
{
    public static void OnGetFromPool(GameObject item)
    {
        item.SetActive(true);
    }

    public static void OnReleaseToPool(GameObject item)
    {
        item.SetActive(false);
    }

    public static void OnPoolItemDestroy(GameObject item)
    {
        Object.Destroy(item);
    }
}
