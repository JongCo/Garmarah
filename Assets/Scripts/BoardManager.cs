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
        card.interactable = true;
        player.AddHwatooToHand(card);
    }

    public void DealToPlayerTop() {
        DealToPlayer(playerTop);
    }
    public void DealToPlayerBottom() {
        DealToPlayer(playerBottom);
    }

    public async void DealToField()
    {
        Hwatoo card = deck.Draw();
        if (card == null) return;
        await field.AddHwatoo(card);
    }

    public void SetDeck(Deck deck)
    {
        this.deck = deck;
    }

    public void Play(Hwatoo hwatoo)
    {
        hwatoo.interactable = false;
        PlayHwatoo(hwatoo).Forget();
    }

    private async UniTask PlayHwatoo(Hwatoo hwatoo)
    {
        PlayContext playContext = CreatePlayContext(hwatoo);

        if (hwatoo.isDummy)
        {
            playContext.player.RemoveHwatooFromHand(hwatoo);
            Destroy(hwatoo.gameObject);
        }
        else
        {
            await PlayHandCard(playContext);
        }

        playContext.drawedCard = DrawForPlay(playContext.player);
        playContext.drawedPlayedSlot = await field.AddHwatoo(playContext.drawedCard, true);

        if (playContext.playedSlot == -1)
        {
            Hwatoo[] gotFromDeck = await field.ResolveCaptured(playContext.drawedCard);

            if (gotFromDeck.Length == 4) { playContext.piTakeCount++; }

            await playContext.player.AddHwatooToOwned(gotFromDeck);
        }
        else if (playContext.playedSlot == playContext.drawedPlayedSlot)
        {
            if (field.GetCardsInSlot(playContext.drawedPlayedSlot).Count == 3)
            {
                // 뻑
                await field.RearrangeSlot(playContext.drawedPlayedSlot);
            }
            else
            {
                // 쪽, 따닥
                Hwatoo[] gotFromSameSlot = await field.ResolveCaptured(playContext.drawedCard);
                if (gotFromSameSlot.Length > 0)
                {
                    await playContext.player.AddHwatooToOwned(gotFromSameSlot);
                    playContext.piTakeCount++;
                }
            }
        }
        else
        {
            Hwatoo[] gotFromHand = await field.ResolveCaptured(playContext.playedCard);
            Hwatoo[] gotFromDeck = await field.ResolveCaptured(playContext.drawedCard);

            if (gotFromHand.Length == 4) playContext.piTakeCount++;
            if (gotFromDeck.Length == 4) playContext.piTakeCount++;

            if (field.CheckAllClear()) playContext.piTakeCount++;

            await playContext.player.AddHwatooToOwned(Enumerable.Concat(gotFromHand, gotFromDeck));
        }

        for(int i = 0; i < playContext.piTakeCount; i++)
        {
            await TakePiFromOpponent(playContext.player, playContext.opponent);
        }
    }

    private async UniTask PlayHandCard(PlayContext playContext)
    {

        if (TryGetBombCards(playContext, out var bombCards))
        {
            int bombTargetMonth = bombCards[0].hwatooData.month;
            foreach (var bombHwatoo in bombCards)
            {
                playContext.player.RemoveHwatooFromHand(bombHwatoo);
                playContext.playedSlot = await field.AddHwatoo(bombHwatoo);
            }

            Vector3 slotPos = field.GetSlotPosition(bombTargetMonth);
            playContext.player.AddDummyCards(3);
            await UniTask.WhenAll(bombCards.Select(c => c.PlayTo(slotPos)));

            playContext.isBomb = true;

            return;
        } 
        else
        {
            playContext.player.RemoveHwatooFromHand(playContext.playedCard);
            playContext.playedSlot = await field.AddHwatoo(playContext.playedCard, true);
        }
    }

    private PlayContext CreatePlayContext(Hwatoo hwatoo)
    {
        Player player = hwatoo.owner;
        Player opponent = player == playerTop ? playerBottom : playerTop;
        return new PlayContext(hwatoo, player, opponent);
    }

    private bool TryGetBombCards(PlayContext context, out List<Hwatoo> bombCards)
    {
        int month = context.playedCard.hwatooData.month;

        // 폭탄 체크: 손패에 같은 월 3장 + 바닥에 같은 월 패 존재
        if (TryGetTripleCardsOnHand(context, out bombCards) && field.HasSlotForMonth(month))
        {
            return true;
        }

        return false;
    }

    private bool TryGetTripleCardsOnHand(PlayContext playContext, out List<Hwatoo> shakableCards)
    {
        shakableCards = null;
        int month = playContext.playedCard.hwatooData.month;

        List<Hwatoo> sameMonthCards = playContext.player.GetSameMonthCardsOnHand(month);
        if (sameMonthCards.Count < 3)
        {
            return false;
        }

        shakableCards = sameMonthCards;
        return true;
    }

    private async UniTask TakePiFromOpponent(Player player, Player opponent)
    {
        Hwatoo takenHwatoo = opponent.GetPiCardOnOwned();
        if (takenHwatoo == null) return;

        UniTask removeFromOpponent = opponent.RemoveHwatooFromOwned(takenHwatoo);
        UniTask addToPlayer = player.AddHwatooToOwned(new Hwatoo[] { takenHwatoo });
        await UniTask.WhenAll(removeFromOpponent, addToPlayer);
    }

    /// <summary>
    /// BoardManager에 등록된 Deck에게서 더미 패를 한장 뽑는 메서드입니다. 뽑은 패의 소유권은 매개변수로 전달한 Player로 지정됩니다.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private Hwatoo DrawForPlay(Player player)
    {
        Hwatoo drawnCard = deck.Draw();
        if (drawnCard != null)
            drawnCard.owner = player;

        return drawnCard;
    }

    private class PlayContext
    {
        public Player player;
        public Player opponent;

        public Hwatoo playedCard;
        public Hwatoo drawedCard;

        public int playedSlot = -1;
        public int drawedPlayedSlot = -1;
        
        public int piTakeCount = 0;

        public bool isBomb = false;

        public PlayContext(Hwatoo playedCard, Player player, Player opponent)
        {
            this.playedCard = playedCard;
            this.player = player;
            this.opponent = opponent;
        }

        public void Reset()
        {
            playedCard = null;
            player = null;
            opponent = null;
            this.piTakeCount = 0;
        }
    }
}
