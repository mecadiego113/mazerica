using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class CustomUIButton : MonoBehaviour
{
    private Transform buttonBackground;
    private Vector2 buttonBackgroundPosition;

    private const float ANIMATION_TIME = 0.125f;

    [TitleGroup("SETTINGS")]
    [SerializeField] private bool interactable = true;

    [TitleGroup("COMPONENTS")]
    [SerializeField][DisableInPlayMode] private bool hasIcon;
    [SerializeField][DisableInPlayMode][ShowIf("hasIcon")] private Image icon;
    [SerializeField][DisableInPlayMode] private bool hasText;
    [SerializeField][DisableInPlayMode][ShowIf("hasText")] private TextMeshProUGUI text;
    [SerializeField][DisableInPlayMode] private Button button;

    private AudioClip audioClip;
    private AudioSource audioSource;

    [TitleGroup("TOOLS")]
    [SerializeField] private KeyCode keyboardKey;
    [SerializeField]
    [EnableIf("interactable")]
    [DisableInEditorMode]
    [Button("Click")]
    private void ClickButton()
    {
        button.onClick.Invoke();
    }

    private EventTrigger eventTrigger;

    private const float DISABLED_ALPHA = 0.5f;
    private const float AUDIO_SOURCE_VOLUME = 0.25f;
    private const string AUDIO_CLIP = "Button";
    private const string AUDIO_CLIP_PATH = "Audio/UI/";

    private void Awake()
    {
        eventTrigger = GetComponent<EventTrigger>();

        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = AUDIO_SOURCE_VOLUME;
        audioSource.playOnAwake = false;

        audioClip = Resources.Load<AudioClip>(AUDIO_CLIP_PATH + AUDIO_CLIP);

        buttonBackground = transform.GetChild(0).transform;
        buttonBackgroundPosition = buttonBackground.localPosition;
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyboardKey))
        {
            OnButtonDown();
        }

        if (Input.GetKeyUp(keyboardKey))
        {
            OnButtonUp();

            button.onClick.Invoke();
        }
    }

    private void Initialize()
    {
        if (interactable)
        {
            Enable();
        }
        else
        {
            Disable();
        }

        InitializeEventTrigger();
    }

    private void InitializeEventTrigger()
    {
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((eventData) => { OnButtonDown(); });
        eventTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((eventData) => { OnButtonUp(); });
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    public void OnButtonDown()
    {
        if (interactable)
        {
            buttonBackground.DOLocalMove(Vector2.zero, ANIMATION_TIME);
        }
    }

    public void OnButtonUp()
    {
        if (interactable)
        {
            buttonBackground.DOLocalMove(buttonBackgroundPosition, ANIMATION_TIME);

            PlayClip(audioClip);
        }
    }

    public void Enable()
    {
        interactable = true;
        button.interactable = true;

        if (icon != null)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1);
        }
    }

    public void Disable()
    {
        interactable = false;
        button.interactable = false;

        if (hasIcon && icon != null)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, DISABLED_ALPHA);
        }

        if (hasText && text != null)
        {
            text.DOFade(DISABLED_ALPHA, ANIMATION_TIME);
        }
    }

    private void PlayClip(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public void SetIcon(Sprite _icon)
    {
        if (hasIcon && icon != null)
        {
            icon.sprite = _icon;
        }
    }
}