using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private HwatooData[] hwatooDataArray;
    [SerializeField] private Hwatoo hwatooPrefab;

    // For Debugging
    public bool dontReverse;

    /// <summary>
    /// Deck에 있는 카드들을 나타내는 리스트입니다. 리스트의 0번째 요소가 가장 아래에 있는 카드입니다.
    /// </summary>
    private readonly List<Hwatoo> cards = new();

    void Start()
    {
        BoardManager.instance.SetDeck(this);
    }

    public void CreateHwatooOnDeck()
    {
        foreach (var hwatooData in hwatooDataArray)
        {
            var createdHwatoo = Instantiate(
                hwatooPrefab,
                transform.position,
                Quaternion.identity
            );
            createdHwatoo.Initialize(hwatooData);
            createdHwatoo.isReversed = !dontReverse;

            cards.Add(createdHwatoo);
        }
    }

    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].zIndex = i;
            cards[i].PlayShuffleAnimation();
        }
    }

    public Hwatoo Draw()
    {
        if (cards.Count == 0) return null;

        Hwatoo top = cards[^1];
        top.isReversed = false;
        cards.RemoveAt(cards.Count - 1);
        return top;
    }
}
