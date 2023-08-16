using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField][DisableInPlayMode] private GameObject mainMenuPanel;
    [SerializeField][DisableInPlayMode] private GameObject gamePanel;

    [SerializeField][DisableInPlayMode] private Image fadeImage;

    [SerializeField][DisableInPlayMode] private Image logo;
    [SerializeField][DisableInPlayMode] private Transform inGameLogoAnchor;
    private RectTransform logoRectTransform;
    private Vector2 scaledLogoRectTransform;

    [SerializeField][DisableInPlayMode] private AudioSource difficultyTogglesAudioSource;

    [SerializeField][DisableInPlayMode] private Transform difficultyTogglesContainer;
    [SerializeField][DisableInPlayMode] private Transform difficultyTogglesContainerHiddenAnchor;
    [SerializeField][DisableInPlayMode] private Toggle easyToggle;
    [SerializeField][DisableInPlayMode] private Toggle normalToggle;
    [SerializeField][DisableInPlayMode] private Toggle hardToggle;
    [SerializeField][DisableInPlayMode] private Toggle impossibleToggle;

    [SerializeField][DisableInPlayMode] private Transform vottingTimeContainer;
    [SerializeField][DisableInPlayMode] private Transform vottingTimeContainerHiddenAnchor;
    [SerializeField][DisableInPlayMode] private Slider vottingTimeSlider;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI votingTimeText;

    [SerializeField][DisableInPlayMode] private TMP_InputField twitchChannelInputField;
    [SerializeField][DisableInPlayMode] private Button connectButton;
    [SerializeField][DisableInPlayMode] private Image twitchLogo;

    [SerializeField][DisableInPlayMode] private List<Image> connectingDots;

    [SerializeField][DisableInPlayMode] private Image timebar;
    [SerializeField][DisableInPlayMode] private Transform currentMovesTextContainer;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI currentMovesText;

    [SerializeField][DisableInPlayMode] private Transform compass;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI compassNorthText;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI compassSouthText;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI compassEastText;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI compassWestText;

    [SerializeField][DisableInPlayMode] private TextMeshProUGUI northVotesText;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI southVotesText;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI eastVotesText;
    [SerializeField][DisableInPlayMode] private TextMeshProUGUI westVotesText;

    [SerializeField][DisableInPlayMode] private RawImage compassRawImage;
    [SerializeField][DisableInPlayMode] private Image compassImage;

    [SerializeField][DisableInPlayMode] private Transform timeBarContainer;

    [SerializeField][DisableInPlayMode] private GameObject chatterNamePrefab;
    [SerializeField][DisableInPlayMode] private Transform chatterNameTextContainer;

    [SerializeField][DisableInPlayMode] private TextMeshProUGUI tooltipText;

    [SerializeField][DisableInPlayMode] private List<string> tooltips;

    [SerializeField][DisableInPlayMode] private Transform generatingMazePanel;
    [SerializeField][DisableInPlayMode] private Transform noVotesPanel;

    private Vector2 defaultLogoPosition;
    private Vector3 defaultCompassRawImagePosition;
    private Vector2 defaultCompassRawImageSizeDelta;

    private Vector2 defaultDifficultyTogglesContainer;
    private Vector2 defaultVottingTimeSliderContainer;

    private float currentCompassImageRotationSpeed;

    private bool rotateCompassImage;

    private Direction currentDirection;

    private const float FADE_DELAY = 1;
    private const float FADE_TIME = 1;

    private const float INITIALIZATITON_DELAY = 1;
    private const float HIDE_NO_VOTES_PANEL_DELAY = 2;

    private const float ANIMATION_TIME = 0.5f;
    private const float SHORT_ANIMATION_TIME = 0.125f;
    private const float COMPASS_ANIMATION_TIME = 0.5f;
    private const float CHATTER_NAME_TEXT_ANIMATION_TIME = 0.5f;

    private const float COMPASS_TEXT_SCALE = 1.5f;
    private const float COMPASS_ROTATION = 5;
    private const float COMPASS_IMAGE_ROTATION_SPEED = 3600;
    private const float COMPASS_DELAY = 0.5f;

    private const float TOOLTIP_TEXT_ANIMATION_TIME = 20;

    private const float CHATTER_NAME_OFFSET = 50;

    private const string TIMEOUT_CHATTERS_TOOLTIP = "TIMEOUT_CHATTERS_TOOLTIP";
    private const string TIMEOUT_CHATTERS_MESSAGE = "Jugadores vetados: ";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        logoRectTransform = logo.GetComponent<RectTransform>();
        scaledLogoRectTransform = logoRectTransform.sizeDelta / 2;

        defaultDifficultyTogglesContainer = difficultyTogglesContainer.localPosition;
        defaultVottingTimeSliderContainer = vottingTimeContainer.localPosition;

        difficultyTogglesContainer.localPosition = new Vector2(difficultyTogglesContainerHiddenAnchor.localPosition.x, difficultyTogglesContainer.localPosition.y);
        vottingTimeContainer.localPosition = new Vector2(vottingTimeContainerHiddenAnchor.localPosition.x, vottingTimeContainer.localPosition.y);
    }

    private void Start()
    {
        defaultLogoPosition = logo.transform.localPosition;
        defaultCompassRawImagePosition = compassRawImage.transform.localPosition;
        defaultCompassRawImageSizeDelta = compassRawImage.GetComponent<RectTransform>().sizeDelta;

        logo.transform.localPosition = Vector2.zero;

        twitchChannelInputField.transform.localScale = new Vector2(0, 1);
        connectButton.transform.localScale = new Vector2(0, 1);

        FadeIn(() =>
        {
            StartCoroutine(Initialize());
        });

        StartCoroutine(InitializeTooltip());

        switch (GameManager.Instance.GetDifficulty())
        {
            case Difficulty.Easy:
                easyToggle.SetIsOnWithoutNotify(true);
                break;
            case Difficulty.Normal:
                normalToggle.SetIsOnWithoutNotify(true);
                break;
            case Difficulty.Hard:
                hardToggle.SetIsOnWithoutNotify(true);
                break;
            case Difficulty.Impossible:
                impossibleToggle.SetIsOnWithoutNotify(true);
                break;
        }

        vottingTimeSlider.value = GameManager.Instance.GetVotingTime();
    }

    private void Update()
    {
        if (rotateCompassImage)
        {
            compassImage.transform.Rotate(Vector3.forward, currentCompassImageRotationSpeed * Time.deltaTime);

            currentCompassImageRotationSpeed -= Time.deltaTime * COMPASS_IMAGE_ROTATION_SPEED;

            if (currentCompassImageRotationSpeed <= 360)
            {
                rotateCompassImage = false;
                currentCompassImageRotationSpeed = 0;

                Vector3 vector = Vector3.zero;

                switch (currentDirection)
                {
                    case Direction.North:
                        vector = new Vector3(0, 0, 0);
                        northVotesText.gameObject.SetActive(true);
                        northVotesText.transform.DOScale(1, ANIMATION_TIME / 2);
                        break;
                    case Direction.South:
                        vector = new Vector3(0, 0, 180);
                        southVotesText.gameObject.SetActive(true);
                        southVotesText.transform.DOScale(1, ANIMATION_TIME / 2);
                        break;
                    case Direction.East:
                        vector = new Vector3(0, 0, 90);
                        eastVotesText.gameObject.SetActive(true);
                        eastVotesText.transform.DOScale(1, ANIMATION_TIME / 2);
                        break;
                    case Direction.West:
                        vector = new Vector3(0, 0, -90);
                        westVotesText.gameObject.SetActive(true);
                        westVotesText.transform.DOScale(1, ANIMATION_TIME / 2);
                        break;
                }

                compassImage.transform.DOLocalRotate(vector, COMPASS_ANIMATION_TIME).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    StartCoroutine(OnCompassAnimationCompletedCoroutine());
                });
            }
        }
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(INITIALIZATITON_DELAY);

        logo.transform.DOLocalMoveY(defaultLogoPosition.y, 0.5f).OnComplete(() =>
        {
            twitchChannelInputField.transform.DOScaleX(1, 0.5f);
            connectButton.transform.DOScaleX(1, 0.5f);

            ShowDifficultyTogglesContainer();
            ShowVotingTimeSliderContainer();
        });
    }

    public void Initialize(string _twitchChannel)
    {
        twitchChannelInputField.text = _twitchChannel;
    }

    private IEnumerator InitializeTooltip()
    {
        tooltipText.GetComponent<RectTransform>().ForceUpdateRectTransforms();

        Vector2 initialPosition;
        Vector2 targetPosition;

        tooltipText.color = Color.clear;

        tooltipText.text = tooltips[UnityEngine.Random.Range(0, tooltips.Count)];
        if (tooltipText.text == TIMEOUT_CHATTERS_TOOLTIP)
        {
            CheckTimeoutChattersTooltip();
        }

        yield return new WaitForSeconds(1);

        tooltipText.GetComponent<RectTransform>().ForceUpdateRectTransforms();

        initialPosition = new Vector2(960 + tooltipText.GetComponent<RectTransform>().sizeDelta.x / 2, tooltipText.transform.localPosition.y);
        targetPosition = new Vector2(-960 - tooltipText.GetComponent<RectTransform>().sizeDelta.x / 2, tooltipText.transform.localPosition.y);

        tooltipText.transform.localPosition = initialPosition;

        tooltipText.color = Color.white;

        tooltipText.transform.DOLocalMoveX(targetPosition.x, TOOLTIP_TEXT_ANIMATION_TIME).SetEase(Ease.Linear).OnComplete(() =>
        {
            StartCoroutine(InitializeTooltip());
        });
    }

    private void CheckTimeoutChattersTooltip()
    {
        if (GameManager.Instance.GetTimeoutChatters().Count == 0)
        {
            do
            {
                tooltipText.text = tooltips[UnityEngine.Random.Range(0, tooltips.Count)];
            }
            while (tooltipText.text == TIMEOUT_CHATTERS_TOOLTIP);
        }
        else
        {
            string tooltip = TIMEOUT_CHATTERS_MESSAGE;
            int index = 0;

            foreach (TimeoutChatter timeoutChatter in GameManager.Instance.GetTimeoutChatters())
            {
                if (index == GameManager.Instance.GetTimeoutChatters().Count - 1)
                {
                    tooltip += string.Format("{0} [{1}]", timeoutChatter.chatterName, timeoutChatter.timeoutTurns);
                }
                else
                {
                    tooltip += string.Format("{0} [{1}], ", timeoutChatter.chatterName, timeoutChatter.timeoutTurns);
                }

                index++;
            }

            tooltipText.text = tooltip;
        }
    }

    public void UpdateTimebar(float _value)
    {
        timebar.fillAmount = _value;
    }

    public void UpdateCurrentMovesText(int _value)
    {
        currentMovesText.text = _value.ToString();
    }

    public void OnChatterVoted(Direction _direction, string _chatterName, Color _color)
    {
        GameObject chatterName = Instantiate(chatterNamePrefab, chatterNameTextContainer);
        chatterName.GetComponentInChildren<TextMeshProUGUI>().text = _chatterName;
        chatterName.GetComponentInChildren<TextMeshProUGUI>().color = _color;

        TextMeshProUGUI compassText = null;

        switch (_direction)
        {
            case Direction.North:
                compassText = compassNorthText;
                break;
            case Direction.South:
                compassText = compassSouthText;
                break;
            case Direction.East:
                compassText = compassEastText;
                break;
            case Direction.West:
                compassText = compassWestText;
                break;
        }

        Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-CHATTER_NAME_OFFSET, CHATTER_NAME_OFFSET), UnityEngine.Random.Range(-CHATTER_NAME_OFFSET, CHATTER_NAME_OFFSET));
        chatterName.transform.DOMove(new Vector2(compassRawImage.transform.position.x + randomOffset.x, chatterName.transform.position.y + randomOffset.y), CHATTER_NAME_TEXT_ANIMATION_TIME).OnComplete(() =>
        {
            chatterName.transform.DOMoveY(compassRawImage.transform.position.y + randomOffset.y, CHATTER_NAME_TEXT_ANIMATION_TIME);
            chatterName.transform.DOScale(0, CHATTER_NAME_TEXT_ANIMATION_TIME).OnComplete(() =>
            {
                Destroy(chatterName.gameObject);
            });

            Vector3 rotation = Vector3.zero;
            TextMeshProUGUI votesText = null;
            int votes = 0;

            switch (_direction)
            {
                case Direction.North:
                    rotation = new Vector3(COMPASS_ROTATION, 0, 0);
                    votesText = northVotesText;
                    votes = GameManager.Instance.GetNorthVotes();
                    break;
                case Direction.South:
                    rotation = new Vector3(-COMPASS_ROTATION, 0, 0);
                    votesText = southVotesText;
                    votes = GameManager.Instance.GetSouthVotes();
                    break;
                case Direction.East:
                    rotation = new Vector3(0, 0, COMPASS_ROTATION);
                    votesText = eastVotesText;
                    votes = GameManager.Instance.GetEastVotes();
                    break;
                case Direction.West:
                    rotation = new Vector3(0, 0, -COMPASS_ROTATION);
                    votesText = westVotesText;
                    votes = GameManager.Instance.GetWestVotes();
                    break;
            }

            compass.DORotate(rotation, SHORT_ANIMATION_TIME).SetDelay(CHATTER_NAME_TEXT_ANIMATION_TIME / 2).OnComplete(() =>
            {
                compass.DORotate(Vector3.zero, SHORT_ANIMATION_TIME);
            });

            compassText.transform.DOScale(COMPASS_TEXT_SCALE, SHORT_ANIMATION_TIME).SetDelay(CHATTER_NAME_TEXT_ANIMATION_TIME / 2).OnComplete(() =>
            {
                votesText.text = votes.ToString();

                compassText.transform.DOScale(1, SHORT_ANIMATION_TIME);
            });
        });
    }

    public void CenterCompassRawImage(Direction _direction)
    {
        currentDirection = _direction;

        compassRawImage.transform.DOLocalMove(Vector3.zero, COMPASS_ANIMATION_TIME);
        compassRawImage.GetComponent<RectTransform>().DOSizeDelta(new Vector2(defaultCompassRawImageSizeDelta.x * 2, defaultCompassRawImageSizeDelta.y * 2), COMPASS_ANIMATION_TIME);

        StartCompassAnimation();
    }

    private void StartCompassAnimation()
    {
        rotateCompassImage = true;

        currentCompassImageRotationSpeed = COMPASS_IMAGE_ROTATION_SPEED;
    }

    public void ResetCompassRawImage()
    {
        switch (currentDirection)
        {
            case Direction.North:
                northVotesText.transform.DOScale(0, ANIMATION_TIME / 2).OnComplete(() =>
                {
                    northVotesText.gameObject.SetActive(false);
                });
                break;
            case Direction.South:
                southVotesText.transform.DOScale(0, ANIMATION_TIME / 2).OnComplete(() =>
                {
                    southVotesText.gameObject.SetActive(false);
                });
                break;
            case Direction.East:
                eastVotesText.transform.DOScale(0, ANIMATION_TIME / 2).OnComplete(() =>
                {
                    eastVotesText.gameObject.SetActive(false);
                });
                break;
            case Direction.West:
                westVotesText.transform.DOScale(0, ANIMATION_TIME / 2).OnComplete(() =>
                {
                    westVotesText.gameObject.SetActive(false);
                });
                break;
        }

        compassRawImage.transform.DOLocalMove(defaultCompassRawImagePosition, COMPASS_ANIMATION_TIME);
        compassRawImage.GetComponent<RectTransform>().DOSizeDelta(defaultCompassRawImageSizeDelta, COMPASS_ANIMATION_TIME).OnComplete(() =>
        {
            ShowCurrentMovesTextContainer();

            GameManager.Instance.OnCompassAnimationCompleted();
        });
    }

    private IEnumerator OnCompassAnimationCompletedCoroutine()
    {
        yield return new WaitForSeconds(COMPASS_DELAY);

        ResetCompassRawImage();
        VolumeManager.Instance.FadeOutDOF();
    }

    public void ShowTimeBar()
    {
        timeBarContainer.DOScaleY(1, SHORT_ANIMATION_TIME * 2);
    }

    public void HideTimeBar()
    {
        timeBarContainer.DOScaleY(0, SHORT_ANIMATION_TIME * 2);
    }

    public void HandleConnectButton()
    {
        connectButton.enabled = false;
        connectButton.transform.DOScale(0, SHORT_ANIMATION_TIME);

        twitchChannelInputField.GetComponent<RectTransform>().DOSizeDelta(new Vector2(-100, twitchChannelInputField.GetComponent<RectTransform>().sizeDelta.y), 0.5f).OnComplete(() =>
        {
            StartCoroutine(StartConnectingDotsCoroutine());

            TwitchManager.Instance.Connect(twitchChannelInputField.text);
        });

        HideDifficultyTogglesContainer();
        HideVotingTimeSliderContainer();
    }

    private IEnumerator StartConnectingDotsCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        connectingDots[0].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        connectingDots[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        connectingDots[2].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        connectingDots[0].gameObject.SetActive(false);
        connectingDots[1].gameObject.SetActive(false);
        connectingDots[2].gameObject.SetActive(false);

        StartCoroutine(StartConnectingDotsCoroutine());
    }

    public void OnMazeGenerated()
    {
        logoRectTransform.DOSizeDelta(scaledLogoRectTransform, ANIMATION_TIME);
        logo.transform.DOLocalMove(inGameLogoAnchor.localPosition, ANIMATION_TIME);
        logo.DOFade(0.25f, ANIMATION_TIME);

        mainMenuPanel.gameObject.GetComponent<CanvasGroup>().DOFade(0, ANIMATION_TIME).OnComplete(() =>
        {
            mainMenuPanel.SetActive(false);
        });

        gamePanel.SetActive(true);
    }

    public void ShowNoVotesPanel()
    {
        noVotesPanel.gameObject.SetActive(true);

        noVotesPanel.DOScaleY(1, 0.5f).OnComplete(() =>
        {
            StartCoroutine(HideNoVotesPanel());
        });
    }

    private IEnumerator HideNoVotesPanel()
    {
        yield return new WaitForSeconds(HIDE_NO_VOTES_PANEL_DELAY);

        VolumeManager.Instance.FadeOutDOF();
        noVotesPanel.DOScaleY(0, 0.5f).OnComplete(() =>
        {
            noVotesPanel.gameObject.SetActive(false);

            GameManager.Instance.StartVoting();
        });
    }

    public void ShowGeneratingMazePanel()
    {
        generatingMazePanel.gameObject.SetActive(true);

        generatingMazePanel.DOScaleY(1, ANIMATION_TIME);
    }

    public void HideGeneratingMazePanel()
    {
        generatingMazePanel.DOScaleY(0, ANIMATION_TIME).OnComplete(() =>
        {
            generatingMazePanel.gameObject.SetActive(false);
        });
    }

    public void HideTwitchLogo()
    {
        twitchLogo.transform.DOScale(0, 0.5f);

        foreach (Image connectingDot in connectingDots)
        {
            connectingDot.transform.DOScale(0, 0.5f);
        }
    }

    public void HandleEasyToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Easy);

            difficultyTogglesAudioSource.Play();
        }
    }

    public void HandleNormalToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Normal);

            difficultyTogglesAudioSource.Play();
        }
    }

    public void HandleHardToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Hard);

            difficultyTogglesAudioSource.Play();
        }
    }

    public void HandleImpossibleToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Impossible);

            difficultyTogglesAudioSource.Play();
        }
    }

    public void ShowDifficultyTogglesContainer()
    {
        difficultyTogglesContainer.gameObject.SetActive(true);
        difficultyTogglesContainer.DOLocalMoveX(defaultDifficultyTogglesContainer.x, ANIMATION_TIME);
    }

    public void HideDifficultyTogglesContainer()
    {
        difficultyTogglesContainer.DOLocalMoveX(difficultyTogglesContainerHiddenAnchor.localPosition.x, ANIMATION_TIME).SetRelative(true).OnComplete(() =>
        {
            difficultyTogglesContainer.gameObject.SetActive(false);
        });
    }

    public void ShowVotingTimeSliderContainer()
    {
        vottingTimeContainer.gameObject.SetActive(true);
        vottingTimeContainer.DOLocalMoveX(defaultVottingTimeSliderContainer.x, ANIMATION_TIME);
    }

    public void HideVotingTimeSliderContainer()
    {
        vottingTimeContainer.DOLocalMoveX(vottingTimeContainerHiddenAnchor.localPosition.x, ANIMATION_TIME).SetRelative(true).OnComplete(() =>
        {
            vottingTimeContainer.gameObject.SetActive(false);
        });
    }

    public void HandleVotingTimeSlider(float _value)
    {
        GameManager.Instance.SetVotingTime(_value);
        votingTimeText.text = _value.ToString();
    }

    public void HandleQuitButton()
    {
        FadeOut(() =>
        {
            GameManager.Instance.Quit();
        });
    }

    public void FadeIn(Action _action = null)
    {
        fadeImage.color = Color.black;

        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(0, FADE_TIME).SetDelay(FADE_DELAY).OnComplete(() =>
        {
            if (_action != null)
            {
                _action.Invoke();
            }
        });
    }

    public void FadeOut(Action _action = null)
    {
        fadeImage.DOFade(1, FADE_TIME).OnComplete(() =>
        {
            if (_action != null)
            {
                fadeImage.gameObject.SetActive(true);
                _action.Invoke();
            }
        });
    }

    private void ShowCurrentMovesTextContainer()
    {
        currentMovesTextContainer.gameObject.SetActive(true);

        currentMovesTextContainer.DOScale(1, ANIMATION_TIME);
    }

    public void HideCurrentMovesTextContainer()
    {
        currentMovesTextContainer.DOScale(0, ANIMATION_TIME / 2).OnComplete(() =>
        {
            currentMovesTextContainer.gameObject.SetActive(false);
        });
    }

    public void HandleMuteMusicToggle(bool _value)
    {
        if (_value)
        {
            AudioManager.Instance.MuteMusic();
        }
        else
        {
            AudioManager.Instance.UnmuteMusic();
        }
    }
}
