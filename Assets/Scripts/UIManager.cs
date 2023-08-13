using DG.Tweening;
using Sirenix.OdinInspector;
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

    [SerializeField][DisableInPlayMode] private Image logo;
    [SerializeField][DisableInPlayMode] private Transform inGameLogoAnchor;
    private RectTransform logoRectTransform;
    private Vector2 scaledLogoRectTransform;

    [SerializeField][DisableInPlayMode] private Transform difficultyTogglesContainer;
    [SerializeField][DisableInPlayMode] private Transform difficultyTogglesContainerHiddenAnchor;
    [SerializeField][DisableInPlayMode] private Toggle easyToggle;
    [SerializeField][DisableInPlayMode] private Toggle normalToggle;
    [SerializeField][DisableInPlayMode] private Toggle hardToggle;
    [SerializeField][DisableInPlayMode] private Toggle impossibleToggle;

    [SerializeField][DisableInPlayMode] private TMP_InputField twitchChannelInputField;
    [SerializeField][DisableInPlayMode] private Button connectButton;
    [SerializeField][DisableInPlayMode] private Image twitchLogo;

    [SerializeField][DisableInPlayMode] private List<Image> connectingDots;

    [SerializeField][DisableInPlayMode] private Image timebar;
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

    private float currentCompassImageRotationSpeed;

    private bool rotateCompassImage;

    private Direction currentDirection;

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

    private const float TOOLTIP_TEXT_ANIMATION_TIME = 30;

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
        difficultyTogglesContainer.localPosition = new Vector2(difficultyTogglesContainerHiddenAnchor.localPosition.x, difficultyTogglesContainer.localPosition.y);
    }

    private void Start()
    {
        defaultLogoPosition = logo.transform.localPosition;
        defaultCompassRawImagePosition = compassRawImage.transform.localPosition;
        defaultCompassRawImageSizeDelta = compassRawImage.GetComponent<RectTransform>().sizeDelta;

        logo.transform.localPosition = Vector2.zero;

        twitchChannelInputField.transform.localScale = new Vector2(0, 1);
        connectButton.transform.localScale = new Vector2(0, 1);

        StartCoroutine(Initialize());

        tooltipText.text = tooltips[Random.Range(0, tooltips.Count)];

        tooltipText.transform.DOLocalMoveX(-1920, TOOLTIP_TEXT_ANIMATION_TIME).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear).OnStepComplete(() =>
        {
            tooltipText.text = tooltips[Random.Range(0, tooltips.Count)];
        });

        switch (GameManager.Instance.GetDifficulty())
        {
            case Difficulty.Easy:
                easyToggle.isOn = true;
                break;
            case Difficulty.Normal:
                normalToggle.isOn = true;
                break;
            case Difficulty.Hard:
                hardToggle.isOn = true;
                break;
            case Difficulty.Impossible:
                impossibleToggle.isOn = true;
                break;
        }
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
        });
    }

    public void Initialize(string _twitchChannel)
    {
        twitchChannelInputField.text = _twitchChannel;
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

        chatterName.transform.DOMoveX(compassRawImage.transform.position.x, CHATTER_NAME_TEXT_ANIMATION_TIME).OnComplete(() =>
        {
            chatterName.transform.DOMoveY(compassRawImage.transform.position.y, CHATTER_NAME_TEXT_ANIMATION_TIME);
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
        }
    }

    public void HandleNormalToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Normal);
        }
    }

    public void HandleHardToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Hard);
        }
    }

    public void HandleImpossibleToggle(bool _value)
    {
        if (_value)
        {
            GameManager.Instance.SetDifficulty(Difficulty.Impossible);
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
}
