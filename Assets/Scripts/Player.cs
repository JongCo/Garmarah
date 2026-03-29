using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private List<Hwatoo> hwatooOnHand = new();
    [SerializeField] private bool isHuman;
    [SerializeField] private bool isBottomPlayer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (isBottomPlayer) {
            BoardManager.instance.SetPlayerBottom(this);
        } else {
            BoardManager.instance.SetPlayerTop(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddHwatooToHand(Hwatoo hwatoo) {
        hwatooOnHand.Add(hwatoo);
        SortHwatooInHandByMonth();
        // hwatoo.MoveTo((Vector2) transform.position + Vector2.right * hwatooOnHand.Count);
        MoveHwatooInHand();
    }
    
    private void SortHwatooInHandByMonth() {
        hwatooOnHand.Sort((a, b) => a.hwatooData.month.CompareTo(b.hwatooData.month));
    }

    private void MoveHwatooInHand()
    {
        for (int i = 0; i < hwatooOnHand.Count; i++)
        {
            hwatooOnHand[i].MoveTo((Vector2) transform.position + Vector2.right * (i + 1));
        }
    }
}
