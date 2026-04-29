using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private Deck deck;
    private Player playerBottom;
    private Player playerTop;
    private Field field;


    public static BoardManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);

        instance = this;
    }


    public void SetPlayerTop(Player player)
    {
        playerTop = player;
    }

    public void SetPlayerBottom(Player player)
    {
        playerBottom = player;
    }

    public void SetField(Field field)
    {
        this.field = field;
    }

    public void DealToPlayer(Player player) {
        Hwatoo card = deck.Draw();
        if (card == null) return;
        player.AddHwatooToHand(card);
    }

    public void DealToPlayerTop() {
        DealToPlayer(playerTop);
    }
    public void DealToPlayerBottom() {
        DealToPlayer(playerBottom);
    }

    public void DealToField()
    {
        Hwatoo card = deck.Draw();
        if (card == null) return;
        field.AddHwatoo(card);
    }

    public void SetDeck(Deck deck)
    {
        this.deck = deck;
    }

    public async void PlayCard(Hwatoo hwatoo)
    {
        Player player = hwatoo.owner;
        int month = hwatoo.hwatooData.month;

        // 폭탄 체크: 손패에 같은 월 3장 + 바닥에 같은 월 패 존재
        List<Hwatoo> sameMonthCards = player.GetSameMonthCardsOnHand(month);
        if (sameMonthCards.Count >= 3 && field.HasSlotForMonth(month))
        {
            await PlayBomb(player, sameMonthCards);
            return;
        }

        // 플레이어 화투 패 처리
        player.RemoveHwatooFromHand(hwatoo);

        int addedSlotIndex = await field.AddHwatoo(hwatoo, true);

        // 뽑은 화투 패 처리
        Hwatoo drawedHwatoo = deck.Draw();
        drawedHwatoo.owner = player;
        int addedDrawedSlotIndex = await field.AddHwatoo(drawedHwatoo, true);
        
        int afterPlayCount = field.GetCardsInSlot(addedSlotIndex).Count;
        print(afterPlayCount);

        if (addedSlotIndex == addedDrawedSlotIndex)
        {
            if (afterPlayCount == 3)
            {
                // TODO : Field에게 해당 화투는 뻑나서 먹지 않을테니 널부러진 화투 재정리하게 시키기
                return;
            }
            else
            {
                Hwatoo[] gotFromDeck = await field.PlayCard(drawedHwatoo);
                if (gotFromDeck.Length > 0) await player.AddHwatooToOwned(gotFromDeck);
                return;
            }
        } 
        else
        {
            Hwatoo[] gotFromHand = await field.PlayCard(hwatoo);
            Hwatoo[] gotFromDeck = await field.PlayCard(drawedHwatoo);
            
            await player.AddHwatooToOwned(Enumerable.Concat(gotFromHand, gotFromDeck));


            return;
        }
    }

    private async UniTask PlayBomb(Player player, List<Hwatoo> bombCards)
    {
        int month = bombCards[0].hwatooData.month;

        foreach (var card in bombCards)
            player.RemoveHwatooFromHand(card);

        // 3장을 바닥 슬롯 위치로 날림
        Vector3 slotPos = field.GetSlotPosition(month);
        await UniTask.WhenAll(bombCards.Select(c => c.PlayTo(slotPos)));

        // 바닥 해당 월 패 전부 수거
        Hwatoo[] fieldCards = field.TakeAllCardsFromMonth(month);

        await player.AddHwatooToOwned(bombCards.Concat(fieldCards));

        // 공패 3장 지급
        player.AddDummyCards(3);
    }

    public async void PlayDummyCard(Hwatoo dummyCard)
    {
        Player player = dummyCard.owner;
        player.RemoveHwatooFromHand(dummyCard);
        Destroy(dummyCard.gameObject);

        Hwatoo drawnCard = deck.Draw();
        drawnCard.owner = player;
        await field.AddHwatoo(drawnCard, true);
        Hwatoo[] gotten = await field.PlayCard(drawnCard);
        if (gotten.Length > 0) await player.AddHwatooToOwned(gotten);
    }
}
