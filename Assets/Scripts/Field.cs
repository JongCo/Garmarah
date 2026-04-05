using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Field : MonoBehaviour
{

    private List<Hwatoo>[] fieldSlots;
    [SerializeField] Transform[] slotTransforms;
    [SerializeField] float stackOffset = 0.15f;
    [SerializeField] CardSelectionUI selectionUI;

    void Awake()
    {
        fieldSlots = new List<Hwatoo>[12];
        for (int i = 0; i < fieldSlots.Length; i++)
            fieldSlots[i] = new List<Hwatoo>();
    }

    void Start()
    {
        BoardManager.instance.SetField(this);
    }

    void Update()
    {

    }

    /// <summary>
    /// 바닥 패 slot 중 비어있는 slot의 index array를 반환합니다.
    /// </summary>
    public int[] GetEmptySlotIndices()
    {
        List<int> emptySlotIndices = new List<int>();
        for (int i = 0; i < fieldSlots.Length; i++)
        {
            if (fieldSlots[i].Count == 0) emptySlotIndices.Add(i);
        }

        return emptySlotIndices.ToArray();
    }

    /// <summary>
    /// 비어있는 slot 중 랜덤한 index 번호를 반환합니다.
    /// </summary>
    public int GetRandomEmptySlotIndex()
    {
        int[] emptySlotIndices = GetEmptySlotIndices();
        int randomIndex = Random.Range(0, emptySlotIndices.Length);

        return emptySlotIndices[randomIndex];
    }

    public async UniTask<int> AddHwatoo(Hwatoo hwatoo, int index, bool isPlaying = false)
    {
        int stackCount = fieldSlots[index].Count;
        fieldSlots[index].Add(hwatoo);

        hwatoo.zIndex = stackCount;
        Vector3 targetPos = slotTransforms[index].position + Vector3.right * (stackOffset * stackCount);
        if (isPlaying) {
            await hwatoo.PlayTo(targetPos);
        } else {
            await hwatoo.MoveTo(targetPos);
        }
        return index;
    }

    public async UniTask<int> AddHwatoo(Hwatoo hwatoo, bool isPlaying = false)
    {
        int slotIndex = FindSlotByMonth(hwatoo.hwatooData.month);
        if (slotIndex == -1) { slotIndex = GetRandomEmptySlotIndex(); }
        return await AddHwatoo(hwatoo, slotIndex, isPlaying);
    }

    /// <summary>
    /// 특정 월 패를 가진 slot의 index 번호를 반환합니다.
    /// </summary>
    private int FindSlotByMonth(int month)
    {
        for (int i = 0; i < fieldSlots.Length; i++)
        {
            if (fieldSlots[i].Count > 0 && fieldSlots[i][0].hwatooData.month == month)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 특정 월 패를 가진 slot의 index 번호를 반환합니다.
    /// </summary>
    private int FindSlotByMonth(Hwatoo hwatoo)
    {
        return FindSlotByMonth(hwatoo.hwatooData.month);
    }

    public List<Hwatoo> GetCardsInSlot(int index)
    {
        return fieldSlots[index];
    }

    public void RemoveCardsFromSlot(int index)
    {
        fieldSlots[index].Clear();
    }

    /// <summary>
    /// 패를 냅니다. 먹은 패 배열을 반환하며, 먹은 패가 없으면 빈 배열을 반환합니다.
    /// </summary>
    public async UniTask<Hwatoo[]> PlayCard(Hwatoo playedHwatoo)
    {
        print("PlayedCard");
        int matchedSlotIndex = FindSlotByMonth(playedHwatoo);
        if (fieldSlots[matchedSlotIndex].Count == 1)
        {
            // await AddHwatoo(playedHwatoo);
            print("no match");
            return System.Array.Empty<Hwatoo>();
        }

        print($"matched : {matchedSlotIndex}");
        playedHwatoo.zIndex = fieldSlots[matchedSlotIndex].Count;
        // await playedHwatoo.PlayTo(slotTransforms[matchedSlotIndex].position);

        List<Hwatoo> slotCards = fieldSlots[matchedSlotIndex];

        if (slotCards.Count == 2)
        {
            // 필드에 패가 한 개 -> 같이 먹음
            Hwatoo taken = slotCards[0];
            RemoveCardsFromSlot(matchedSlotIndex);
            return new[] { playedHwatoo, taken };
        }
        else if (slotCards.Count == 3)
        {
            // 필드에 패가 두 개 -> 유저가 하나 선택
            Hwatoo selected = await selectionUI.AskSelection(slotCards);
            Hwatoo remaining = selected == slotCards[0] ? slotCards[1] : slotCards[0];
            RemoveCardsFromSlot(matchedSlotIndex);
            // await AddHwatoo(remaining, matchedSlotIndex);
            return new[] { playedHwatoo, selected };
        }
        else
        {
            // 필드에 패가 세 개 -> 다 먹음
            Hwatoo[] taken = slotCards.ToArray();
            RemoveCardsFromSlot(matchedSlotIndex);
            return new[] { playedHwatoo, taken[0], taken[1], taken[2] };
        }
    }
}
