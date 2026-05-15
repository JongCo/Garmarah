using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{

    private List<Hwatoo> hwatooOnHand = new();
    private List<Hwatoo> ownedHwatoos => gwangs.Concat(ddis).Concat(yeols).Concat(pis).ToList();

    private List<Hwatoo> gwangs = new();
    private List<Hwatoo> ddis = new();
    private List<Hwatoo> yeols = new();
    private List<Hwatoo> pis = new();

    [SerializeField] private Transform gwangPivot;
    [SerializeField] private Transform ddiPivot;
    [SerializeField] private Transform yeolPivot;
    [SerializeField] private Transform piPivot;

    [SerializeField] private bool isHuman;
    [SerializeField] private bool isBottomPlayer;

    [SerializeField] private Transform ownedHwatooPivot;
    [SerializeField] private Hwatoo dummyCardPrefab;


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

    public List<Hwatoo> GetSameMonthCardsOnHand(int month)
    {
        return hwatooOnHand.Where(h => h.hwatooData?.month == month).ToList();
    }

    public List<Hwatoo> GetCardsByTypeOnOwned(CardType type)
    {
        return ownedHwatoos.Where(h => h.hwatooData.cardType == type).ToList();
    }

    public Hwatoo GetPiCardOnOwned()
    {
        List<Hwatoo> pis = GetCardsByTypeOnOwned(CardType.Pi);
        if (pis.Count > 0) {return pis[^1];}

        List<Hwatoo> ssangPis = GetCardsByTypeOnOwned(CardType.SSangPi);
        if (ssangPis.Count > 0) {return ssangPis[^1];}

        return null;
    }

    public void AddDummyCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Hwatoo dummy = Instantiate(dummyCardPrefab);
            dummy.InitializeAsDummy();
            AddHwatooToHand(dummy);
        }
    }

    private List<Hwatoo> SelectOwnedHwatooListByType(Hwatoo hwatoo) 
    {
        switch (hwatoo.hwatooData.cardType)
        {
            case CardType.Gwang:
                return gwangs;
            case CardType.Yeol:
                return yeols;
            case CardType.DDi:
                return ddis;
            case CardType.Pi:
            case CardType.SSangPi:
                return pis;
        }

        Debug.LogError("얻을 수 없는 타입의 OwnedHwatoo 리스트에 접근하였습니다.");
        return null;
    }

    public async UniTask AddHwatooToOwned(IEnumerable<Hwatoo> hwatoos)
    {
        List<Hwatoo> hwatooList = hwatoos.ToList();

        foreach (var hwatoo in hwatooList)
        {
            hwatoo.owner = this;
            SelectOwnedHwatooListByType(hwatoo).Add(hwatoo);
        }

        // ownedHwatoos.AddRange(hwatooList);
        await MoveHwatooToOwned();
    }

    public async UniTask RemoveHwatooFromOwned(Hwatoo hwatoo)
    {
        // ownedHwatoos.Remove(hwatoo);
        SelectOwnedHwatooListByType(hwatoo).Remove(hwatoo);
        hwatoo.owner = null;
        await MoveHwatooToOwned();
    }

    private void SortHwatooInHandByMonth() {
        hwatooOnHand.Sort((a, b) => (a.hwatooData?.month ?? int.MaxValue).CompareTo(b.hwatooData?.month ?? int.MaxValue));
    }

    private void MoveHwatooToHand()
    {
        for (int i = 0; i < hwatooOnHand.Count; i++)
        {
            hwatooOnHand[i].MoveTo((Vector2) transform.position + Vector2.right * (i + 1), Vector2.one);
        }
    }

    private UniTask MoveHwatooToOwned()
    {
        List<UniTask> tasks = new();
        void ForEach(List<Hwatoo> e, Transform pivot)
        {
            for (int i = 0; i < e.Count; i++)
            {
                e[i].zIndex = i;
                tasks.Add(e[i].MoveTo( (Vector2) pivot.position + Vector2.right * i * 0.3f, Vector2.one*0.75f) );
            }
        }

        ForEach(gwangs, gwangPivot);
        ForEach(ddis, ddiPivot);
        ForEach(yeols, yeolPivot);
        ForEach(pis, piPivot);
        
        return UniTask.WhenAll(tasks);
    }
}
