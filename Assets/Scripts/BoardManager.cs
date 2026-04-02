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
        player.RemoveHwatooFromHand(hwatoo);

        Hwatoo[] gotHwatoos = await field.PlayCard(hwatoo);
        if (gotHwatoos == null) return;

        player.AddHwatooToOwned(gotHwatoos);
    }
}
