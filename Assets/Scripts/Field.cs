using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{

    private Hwatoo[] fieldSlots = new Hwatoo[12];
    [SerializeField] Transform[] slotTransforms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoardManager.instance.SetField(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int[] GetEmptySlotIndices()
    {
        List<int> emptySlotIndices = new List<int>();
        for (int i = 0; i < fieldSlots.Length; i++)
        {
            if (fieldSlots[i] == null) emptySlotIndices.Add(i);
        }

        return emptySlotIndices.ToArray();
    }

    public int GetRandomEmptySlotIndex()
    {
        int[] emptySlotIndices = GetEmptySlotIndices();
        int randomIndex = Random.Range(0, emptySlotIndices.Length);

        return emptySlotIndices[randomIndex];
    }

    public void AddHwatooToSlot(Hwatoo hwatoo, int index)
    {
        fieldSlots[index] = hwatoo;
        hwatoo.MoveTo(slotTransforms[index].position);
    }
}
