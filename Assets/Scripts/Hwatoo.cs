using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using JongCo.Easing;
using UnityEngine.EventSystems;


public class Hwatoo : MonoBehaviour, IPointerDownHandler
{
    public enum CardLocation { Deck, PlayerHand, OpponentHand, Field, Captured }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite reversedSprite;

    public HwatooData hwatooData {get; private set;}
    public Player owner { get; set; }

    private bool interactable;
    private bool _isReversed;
    public bool isReversed{
        get {return _isReversed;}
        set
        {
            _isReversed = value;
            if (_isReversed)
            {
                spriteRenderer.sprite = reversedSprite;
            } else
            {
                spriteRenderer.sprite = hwatooData.sprite;
            }
        }
    }
    private Coroutine animationCoroutine;
    private UniTaskCompletionSource _animationTcs;
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

    private UniTask PlayAnimation(IEnumerator animation)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            _animationTcs?.TrySetResult();
        }
        _animationTcs = new UniTaskCompletionSource();
        animationCoroutine = StartCoroutine(RunAnimation(animation, _animationTcs));
        return _animationTcs.Task;
    }

    private IEnumerator RunAnimation(IEnumerator animation, UniTaskCompletionSource tcs)
    {
        yield return animation;
        tcs.TrySetResult();
    }

    public UniTask MoveTo(Vector2 targetPosition, Vector2 targetScale)
    {
        _targetPosition = targetPosition;
        return PlayAnimation(CardAnimations.MoveAnimation(transform, targetPosition, targetScale, Preset.FastInSlowOut2, 0.7f));
    }

    public UniTask PlayTo(Vector2 targetPosition)
    {
        _targetPosition = targetPosition;
        return PlayAnimation(CardAnimations.MoveAndSlapAnimation(
            transform,
            targetPosition,
            Preset.FastInSlowOut2,
            0.7f,
            0.4f
        ));
    }

    public void PlayShuffleAnimation()
    {
        PlayAnimation(CardAnimations.ShuffleAnimation(transform, Preset.SlowInSlowOut2, _targetPosition, duration: 0.2f));
    }

    public bool isDummy { get; private set; }

    public void Initialize(HwatooData hwatooData)
    {
        this.hwatooData = hwatooData;

        cardText.text = $"{hwatooData.month}월\n{hwatooData.cardName}";
        spriteRenderer.sprite = hwatooData.sprite;
        _targetPosition = transform.position;
    }

    public void InitializeAsDummy()
    {
        isDummy = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BoardManager.instance.Play(this);
    }
}
