using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private HwatooData[] hwatooDataArray;
    [SerializeField] private Hwatoo hwatooPrefab;

    private readonly List<Hwatoo> cards = new();

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
}
