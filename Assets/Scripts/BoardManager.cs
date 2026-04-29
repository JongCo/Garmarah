using System;
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
        // 플레이어 화투 패 처리
        Player player = hwatoo.owner;
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
}
