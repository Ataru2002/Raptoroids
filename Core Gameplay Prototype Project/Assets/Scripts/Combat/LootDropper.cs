using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [SerializeField] float dropChance;
    [SerializeField] DropTableEntry<GameObject>[] dropTable;
    
    public void DropLoot()
    {
        if (Random.value >= dropChance)
        {
            return;
        }

        float selector = Random.value;

        for (int i = 0; i < dropTable.Length; i++)
        {
            selector -= dropTable[i].weight;
            if (selector <= 0)
            {
                GameObject droppedItem = Instantiate(dropTable[i].item);
                droppedItem.transform.position = transform.position;
                break;
            }
        }
    }
}

[System.Serializable]
public class DropTableEntry<T>
{
    public T item;
    public float weight;
}
