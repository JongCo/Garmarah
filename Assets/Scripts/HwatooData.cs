using UnityEngine;

public enum CardType { Gwang, Yeol, DDi, Pi, SSangPi }
public enum DanType { None, Hongdan, Chodan, Cheongdan }

[CreateAssetMenu(fileName = "Card_", menuName = "Garmarah/HwatooCard")]
public class HwatooData : ScriptableObject
{
    public int month;          // 1 ~ 12
    public CardType cardType;
    public DanType danType;    // Ddi 카드일 때만 유효
    public string cardName;    // 예: "학광", "매조", "홍단"
    public Sprite sprite;

    private void OnValidate()
    {
        if (cardType != CardType.DDi)
            danType = DanType.None;
    }
}
