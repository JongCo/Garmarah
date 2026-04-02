using System.Collections;
using TMPro;
using UnityEngine;
using JongCo.Easing;
using UnityEngine.EventSystems;


public class Hwatoo : MonoBehaviour, IPointerDownHandler
{
    public enum CardLocation { Deck, PlayerHand, OpponentHand, Field, Captured }

    public HwatooData hwatooData {get; private set;}

    private bool interactable;
    private bool isSelected;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;
    private Vector2 _targetPosition;

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

    private void PlayAnimation(IEnumerator animation)
    {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(animation);
    }

    public void MoveTo(Vector2 targetPosition)
    {
        _targetPosition = targetPosition;
        PlayAnimation(CardAnimations.MoveAnimation(transform, targetPosition, Preset.FastInSlowOut2, 0.7f));
    }

    public void PlayShuffleAnimation()
    {
        PlayAnimation(CardAnimations.ShuffleAnimation(transform, Preset.SlowInSlowOut2, _targetPosition, duration: 0.2f));
    }

    public void Initialize(HwatooData hwatooData)
    {
        this.hwatooData = hwatooData;

        cardText.text = $"{hwatooData.month}월\n{hwatooData.cardName}";
        spriteRenderer.sprite = hwatooData.sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BoardManager.instance.PlayCard(this);
    }
}
