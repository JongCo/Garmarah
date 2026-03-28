using System.Collections;
using TMPro;
using UnityEngine;
using JongCo.Easing;


public class Hwatoo : MonoBehaviour
{
    public enum CardLocation { Deck, PlayerHand, OpponentHand, Field, Captured }

    private HwatooData hwatooData;    

    private bool interactable;
    private bool isSelected;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine moveAnimationCoroutine;

    private int _zIndex = 0;
    public int zIndex { 
        get { return _zIndex;}
        set {
            _zIndex = value;
            Vector3 prevPos = transform.position;
            prevPos.z = 10 - _zIndex*0.01f;
            transform.position = prevPos;
        }
    }

    //Temp member for development
    [SerializeField] private TMP_Text cardText;


    void Start()
    {
        
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (moveAnimationCoroutine != null) StopCoroutine(moveAnimationCoroutine);

        StartCoroutine(CardAnimations.MoveAnimation(transform, targetPosition, Preset.FastInSlowOut2, 0.7f));
    }

    public void PlayShuffleAnimation()
    {
        StartCoroutine(
            CardAnimations.ShuffleAnimation(
                transform, 
                Preset.SlowInSlowOut2,
                duration: 0.2f
            )
        );
    }

    public void Initialize(HwatooData hwatooData)
    {
        this.hwatooData = hwatooData;

        cardText.text = $"{hwatooData.month}월\n{hwatooData.cardName}";
        spriteRenderer.sprite = hwatooData.sprite;
    }
}
