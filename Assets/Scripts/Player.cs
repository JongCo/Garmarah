using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{

    private List<Hwatoo> hwatooOnHand = new();
    private List<Hwatoo> ownedHwatoos = new();
    [SerializeField] private bool isHuman;
    [SerializeField] private bool isBottomPlayer;

    [SerializeField] private Transform ownedHwatooPivot;


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
        hwatoo.owner = this;
        hwatooOnHand.Add(hwatoo);
        SortHwatooInHandByMonth();
        MoveHwatooToHand();
    }

    public void RemoveHwatooFromHand(Hwatoo hwatoo) {
        hwatooOnHand.Remove(hwatoo);
        hwatoo.owner = null;
        MoveHwatooToHand();
    }

    public async UniTask AddHwatooToOwned(IEnumerable<Hwatoo> hwatoo)
    {
        ownedHwatoos.AddRange(hwatoo);
        await MoveHwatooToOwned();
    }

    private void SortHwatooInHandByMonth() {
        hwatooOnHand.Sort((a, b) => a.hwatooData.month.CompareTo(b.hwatooData.month));
    }

    private void MoveHwatooToHand()
    {
        for (int i = 0; i < hwatooOnHand.Count; i++)
        {
            hwatooOnHand[i].MoveTo((Vector2) transform.position + Vector2.right * (i + 1));
        }
    }

    private UniTask MoveHwatooToOwned()
    {
        var tasks = new UniTask[ownedHwatoos.Count];
        for (int i = 0; i < ownedHwatoos.Count; i++)
        {
            tasks[i] = ownedHwatoos[i].MoveTo((Vector2) ownedHwatooPivot.position + Vector2.right * (i + 1));
        }
        return UniTask.WhenAll(tasks);
    }
}
