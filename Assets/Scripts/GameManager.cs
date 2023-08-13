using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using qtools.qmaze;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Undefined, MainMenu, Initializing, Voting, Moving
    }

    public enum Direction
    {
        None, North, South, East, West
    }

    public enum Difficulty
    {
        Easy, Normal, Hard, Impossible
    }

    public MazeEngine baseMazeEngine;
    public MazeEngine childMazeEngine_1;
    public MazeEngine childMazeEngine_2;
    public Material baseMazeMaterialPrefab;
    public Material childMazeMaterialPrefab;
    public CameraController mazeCamera;

    private MazeEngine prevMazeEngine;
    private MazeEngine nextMazeEngine;

    private QRectInt prevRect;
    private QRectInt nextRect;

    private int currentLevel;

    public static GameManager Instance;

    [SerializeField][DisableInPlayMode] private Player player;

    [SerializeField][ReadOnly] private GameState state = GameState.Undefined;
    [SerializeField][ReadOnly] private Difficulty difficulty;

    [SerializeField][Range(5, 30)] private int votingTime;

    [SerializeField][ReadOnly] private List<string> currentVotedChatters = new List<string>();
    [SerializeField][ReadOnly] private List<TimeoutChatter> timeoutChatters = new List<TimeoutChatter>();
    [SerializeField][ReadOnly] private List<int> currentVotedSteps = new List<int>();

    [SerializeField][ReadOnly] private int currentNorthVotes;
    [SerializeField][ReadOnly] private int currentSouthVotes;
    [SerializeField][ReadOnly] private int currentEastVotes;
    [SerializeField][ReadOnly] private int currentWestVotes;

    [SerializeField][ReadOnly] private int currentTotalVotes;

    [SerializeField][ReadOnly] private float currentTime;

    [SerializeField][ReadOnly] private int currentMoves;

    [SerializeField][DisableInPlayMode] private Transform goal;
    [SerializeField][DisableInPlayMode] private Light goalLight;

    private Direction direction;
    private int steps;
    [SerializeField][ReadOnly] private bool goalReached;

    private Vector3 defaultGoalScale;
    private float defaultGoalLightIntensity;

    private int childMazeSize = 0;
    private int mazeSize = 0;

    private const float ANIMATION_TIME = 1;

    private const string DIFFICULTY = "difficulty";

    private const int EASY_CHILD_MAZE_SIZE = 2;
    private const int NORMAL_CHILD_MAZE_SIZE = 4;
    private const int HARD_CHILD_MAZE_SIZE = 5;
    private const int IMPOSSIBLE_CHILD_MAZE_SIZE = 8;

    private const int EASY_MAZE_SIZE = 10;
    private const int NORMAL_MAZE_SIZE = 15;
    private const int HARD_MAZE_SIZE = 25;
    private const int IMPOSSIBLE_MAZE_SIZE = 50;

    private const int DEMO_MAZE_SIZE = 25;

    private const int TIMEOUT_TURNS = 5;

    private const float MAZE_GENERATION_DELAY = 2;

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

        Enum.TryParse(PlayerPrefs.GetString(DIFFICULTY, Difficulty.Normal.ToString()), out difficulty);
    }

    private void Start()
    {
        Initialize();

        VolumeManager.Instance.EnableDOF();
    }

    private void Update()
    {
        if (state == GameState.Voting)
        {
            currentTime -= Time.deltaTime;
            UIManager.Instance.UpdateTimebar(currentTime / votingTime);

            if (currentTime < 0)
            {
                OnTimeout();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            player.SetPosition(nextMazeEngine.transform.TransformPoint(nextMazeEngine.getFinishPositionList()[0].toVector3()));
            StartCoroutine(FinishAnimation());
        }
    }

    private void Initialize()
    {
        defaultGoalScale = goal.localScale;
        defaultGoalLightIntensity = goalLight.intensity;

        goal.localScale = new Vector3(0, defaultGoalScale.y, 0);
        goalLight.intensity = 0;

        baseMazeEngine.transform.position = Vector3.zero;

        baseMazeEngine.setMazeWidth(DEMO_MAZE_SIZE);
        baseMazeEngine.setMazeHeight(DEMO_MAZE_SIZE);

        baseMazeEngine.mazeGenerateProgressEvent.AddListener(OnMazeGenerateProgress);
        baseMazeEngine.mazePieceGeneratedEvent.AddListener(OnMazePieceGenerated);

        baseMazeEngine.GenerateMaze();
    }

    public void IncreaseNorthVotes()
    {
        currentNorthVotes += 1;
        currentTotalVotes += 1;
    }

    public void IncreaseSouthVotes()
    {
        currentSouthVotes += 1;
        currentTotalVotes += 1;
    }

    public void IncreaseEastVotes()
    {
        currentEastVotes += 1;
        currentTotalVotes += 1;
    }

    public void IncreaseWestVotes()
    {
        currentWestVotes += 1;
        currentTotalVotes += 1;
    }

    public void VoteStep(int _steps)
    {
        currentVotedSteps.Add(_steps);
    }

    private void ResetTime()
    {
        currentTime = votingTime;
    }

    private void ResetVotes()
    {
        currentNorthVotes = currentSouthVotes = currentEastVotes = currentWestVotes = 0;
        currentTotalVotes = 0;
    }

    private void ResetCurrentVotedChatters()
    {
        currentVotedChatters.Clear();
    }

    private void ResetCurrentVotedSteps()
    {
        currentVotedSteps.Clear();
    }

    public void AddChatter(string _chatter)
    {
        currentVotedChatters.Add(_chatter);
    }

    public bool ChatterAlreadyVoted(string _chatter)
    {
        if (currentVotedChatters.Contains(_chatter))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTimeout()
    {
        UIManager.Instance.HideTimeBar();

        ResetTime();

        SetState(GameState.Moving);

        player.HideSlots();

        if (currentTotalVotes == 0)
        {
            StartCoroutine(OnTimeoutNoVotesCoroutine());
        }
        else
        {
            int[] numbers = { currentNorthVotes, currentSouthVotes, currentEastVotes, currentWestVotes };

            int max = FindMaxValue(numbers);

            if (max == currentNorthVotes)
            {
                direction = Direction.North;
            }
            else if (max == currentSouthVotes)
            {
                direction = Direction.South;
            }
            else if (max == currentEastVotes)
            {
                direction = Direction.East;
            }
            else
            {
                direction = Direction.West;
            }

            if (currentVotedSteps.Count > 0)
            {
                steps = FindMostVotedStepConut();
            }
            else
            {
                steps = 1;
            }

            StartCoroutine(OnTimeoutCoroutine());
        }
    }

    private IEnumerator OnTimeoutNoVotesCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        UIManager.Instance.ShowNoVotesPanel();
        VolumeManager.Instance.FadeInDOF();
    }

    private IEnumerator OnTimeoutCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        UIManager.Instance.CenterCompassRawImage(direction);
        VolumeManager.Instance.FadeInDOF();
    }

    public void OnCompassAnimationCompleted()
    {
        IncreaseMoves();

        ResetCurrentVotedChatters();
        ResetCurrentVotedSteps();

        player.Move(direction, steps, () =>
        {
            if (!goalReached)
            {
                StartVoting();
            }
        });
    }

    public void StartVoting()
    {
        ResetTime();
        ResetVotes();

        player.ShowSlots();
        SetState(GameState.Voting);

        UIManager.Instance.ShowTimeBar();
    }

    int FindMaxValue(int[] numbers)
    {
        int maxValue = int.MinValue;

        foreach (int num in numbers)
        {
            if (num > maxValue)
            {
                maxValue = num;
            }
        }

        int countOfMaxValue = 0;

        foreach (int num in numbers)
        {
            if (num == maxValue)
            {
                countOfMaxValue++;
            }
        }

        if (countOfMaxValue > 1)
        {
            int[] maxIndices = new int[countOfMaxValue];
            int currentIndex = 0;

            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == maxValue)
                {
                    maxIndices[currentIndex] = i;
                    currentIndex++;
                }
            }

            int randomIndex = UnityEngine.Random.Range(0, countOfMaxValue);
            int selectedIndex = maxIndices[randomIndex];

            return numbers[selectedIndex];
        }
        else
        {
            return maxValue;
        }
    }

    public void OnConnected()
    {
        UIManager.Instance.HideTwitchLogo();

        UIManager.Instance.ShowGeneratingMazePanel();

        baseMazeEngine.mazeGeneratedEvent.AddListener(OnBaseMazeGenerated);

        baseMazeEngine.setMazeWidth(mazeSize);
        baseMazeEngine.setMazeHeight(mazeSize);

        baseMazeEngine.GetComponent<Maze>().Hide(GenerateNextLevel);
    }

    private void OnMazePieceGenerated(MazePieceData _mazePieceData)
    {

    }

    private void OnMazeGenerateProgress(float _progress)
    {

    }

    private void GenerateNextLevel()
    {
        goalReached = false;
        ResetMoves();

        baseMazeEngine.destroyImmediateMazeGeometry();

        List<MazeVector2IntDir> exitPositionList = baseMazeEngine.getExitPositionList();
        if (exitPositionList.Count > 1)
        {
            exitPositionList.RemoveAt(0);
            baseMazeEngine.setExitPositionList(exitPositionList);
        }

        List<MazeVector2Int> obstaclePositionList = new List<MazeVector2Int>();
        if (prevMazeEngine == null)
        {
            prevRect = new QRectInt(MazeMath.getRandom(1, baseMazeEngine.getMazeWidth() - childMazeSize - 2), MazeMath.getRandom(1, baseMazeEngine.getMazeHeight() - childMazeSize - 2), childMazeSize, childMazeSize);
            obstaclePositionList.AddRange(rectToList(prevRect));
            prevMazeEngine = CreateChildMaze(prevRect, childMazeEngine_1);
            prevMazeEngine.GenerateMaze();

            player.SetPosition(prevMazeEngine.transform.TransformPoint(prevMazeEngine.getFinishPositionList()[0].toVector3()));
        }
        else
        {
            prevMazeEngine.destroyImmediateMazeGeometry();
            prevRect = nextRect;
            prevMazeEngine = nextMazeEngine;
            obstaclePositionList.AddRange(rectToList(prevRect));
        }

        nextRect = new QRectInt(MazeMath.getRandom(1, baseMazeEngine.getMazeWidth() - childMazeSize - 2), MazeMath.getRandom(1, baseMazeEngine.getMazeHeight() - childMazeSize - 2), childMazeSize, childMazeSize);
        while (IsRectNear(prevRect, nextRect))
        {
            nextRect.x = MazeMath.getRandom(1, baseMazeEngine.getMazeWidth() - childMazeSize - 2);
            nextRect.y = MazeMath.getRandom(1, baseMazeEngine.getMazeHeight() - childMazeSize - 2);
        }

        obstaclePositionList.AddRange(rectToList(nextRect));

        baseMazeEngine.setObstaclePositionList(obstaclePositionList);
        nextMazeEngine = CreateChildMaze(nextRect, prevMazeEngine == childMazeEngine_1 ? childMazeEngine_2 : childMazeEngine_1);
        nextMazeEngine.GenerateMaze();
        List<MazeVector2IntDir> nextMazeEngineFinishPositionList = nextMazeEngine.getFinishPositionList();

        goal.parent = nextMazeEngine.getMazeData()[nextMazeEngineFinishPositionList[0].x][nextMazeEngineFinishPositionList[0].y].geometry.transform;
        goal.localPosition = new Vector3(0, goal.localPosition.y, 0);

        player.SetGoal(nextMazeEngine.transform.TransformPoint(nextMazeEngineFinishPositionList[0].toVector3()), OnGoalReached);

        baseMazeEngine.GenerateMaze();

        currentLevel++;
    }

    private void OnGoalReached()
    {
        goalReached = true;

        StartCoroutine(FinishAnimation());
    }

    private IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        HideGoal();
        player.Hide();

        mazeCamera.SetFollowMode(false);

        yield return new WaitForSeconds(1.0f);

        baseMazeEngine.GetComponent<Maze>().Hide();
        prevMazeEngine.GetComponent<Maze>().Hide(HideCompleteHandler);
    }

    private void HideCompleteHandler()
    {
        GenerateNextLevel();
    }

    private bool IsRectNear(QRectInt first, QRectInt second, int offset = 1)
    {
        if (first.x < second.x + second.width + offset && first.x + first.width + offset > second.x &&
            first.y < second.y + second.height + offset && first.y + first.height + offset > second.y)
            return true;
        else
            return false;
    }

    private MazeEngine CreateChildMaze(QRectInt rect, MazeEngine mazeEngine)
    {
        mazeEngine.setMazeWidth(rect.width);
        mazeEngine.setMazeHeight(rect.height);
        mazeEngine.transform.position = new Vector3(0, 0, 0);
        mazeEngine.transform.position = mazeEngine.transform.TransformPoint(new Vector3(rect.x, -50, -rect.y));

        List<MazeVector2IntDir> finishPositionList = new List<MazeVector2IntDir>();
        finishPositionList.Add(new MazeVector2IntDir(rect.width / 2, rect.height / 2, MazeOutputDirection.NotSpecified));
        mazeEngine.setFinishPositionList(finishPositionList);

        List<MazeVector2IntDir> exitPositionList = new List<MazeVector2IntDir>();
        MazeVector2IntDir mazeExit = getExitForRect(rect);
        exitPositionList.Add(mazeExit);
        mazeEngine.setExitPositionList(exitPositionList);

        List<MazeVector2IntDir> baseMazeEngineExitPositionlist = baseMazeEngine.getExitPositionList();
        baseMazeEngineExitPositionlist.Add(new MazeVector2IntDir(rect.x + mazeExit.x + MazeOutput.dx[mazeExit.direction],
                                                              rect.y + mazeExit.y + MazeOutput.dy[mazeExit.direction],
                                                              MazeOutput.opposite[mazeExit.direction]));
        baseMazeEngine.setExitPositionList(baseMazeEngineExitPositionlist);

        return mazeEngine;
    }

    private MazeVector2IntDir getExitForRect(QRectInt rect)
    {
        MazeOutputDirection dir;
        int ix = MazeMath.getRandom(0, rect.width);
        int iy;
        if (ix == 0 || ix == rect.width - 1)
        {
            iy = MazeMath.getRandom(0, rect.height);
            dir = (ix == 0 ? MazeOutputDirection.W : MazeOutputDirection.E);
        }
        else
        {
            if (MazeMath.getRandom() > 0.5)
            {
                iy = 0;
                dir = MazeOutputDirection.N;
            }
            else
            {
                iy = rect.height - 1;
                dir = MazeOutputDirection.S;
            }
        }
        return new MazeVector2IntDir(ix, iy, dir);
    }

    public void childMazePieceHandler(MazePieceData pieceData)
    {
        pieceData.geometry.GetComponent<MeshRenderer>().material = childMazeMaterialPrefab;
    }

    private void OnBaseMazeGenerated(MazeEngine _mazeEngine)
    {
        StartCoroutine(OnBaseMazeGeneratedCoroutine());
    }

    private IEnumerator OnBaseMazeGeneratedCoroutine()
    {
        yield return new WaitForSeconds(MAZE_GENERATION_DELAY);
        VolumeManager.Instance.FadeOutDOF();

        UIManager.Instance.HideGeneratingMazePanel();

        goal.parent = null;

        baseMazeEngine.GetComponent<Maze>().Show(() =>
        {
            prevMazeEngine.gameObject.GetComponent<Maze>().Show();
            nextMazeEngine.gameObject.GetComponent<Maze>().Show();
        });

        StartCoroutine(StartAnimation());
    }

    private IEnumerator StartAnimation()
    {
        UIManager.Instance.OnMazeGenerated();

        yield return new WaitForSeconds(2);

        mazeCamera.SetTarget(goal);
        mazeCamera.SetFollowMode(true);

        yield return new WaitForSeconds(2);

        ShowGoal();
    }

    private List<MazeVector2Int> rectToList(QRectInt rect)
    {
        List<MazeVector2Int> result = new List<MazeVector2Int>();
        for (int ix = rect.x; ix < rect.x + rect.width; ix++)
        {
            for (int iy = rect.y; iy < rect.y + rect.height; iy++)
            {
                result.Add(new MazeVector2Int(ix, iy));
            }
        }
        return result;
    }

    private void ShowGoal()
    {
        goal.DOScale(defaultGoalScale, ANIMATION_TIME).OnComplete(() =>
        {
            player.Show(() =>
            {
                StartVoting();
            });

            mazeCamera.SetTarget(player.transform);
            mazeCamera.SetFollowMode(true);
        });

        goalLight.DOIntensity(defaultGoalLightIntensity, ANIMATION_TIME);
    }

    private void HideGoal()
    {
        goal.DOScale(new Vector3(0, defaultGoalScale.y, 0), ANIMATION_TIME);
        goalLight.DOIntensity(0, ANIMATION_TIME);
    }

    private void IncreaseMoves()
    {
        currentMoves += 1;
        UIManager.Instance.UpdateCurrentMovesText(currentMoves);

        for (int i = 0; i < timeoutChatters.Count; i++)
        {
            timeoutChatters[i].timeoutTurns -= 1;

            if (timeoutChatters[i].timeoutTurns == 0)
            {
                timeoutChatters.Remove(timeoutChatters[i]);
            }
        }
    }

    private void ResetMoves()
    {
        currentMoves = 0;
    }

    public void SetState(GameState _state)
    {
        state = _state;
    }

    public GameState GetState()
    {
        return state;
    }

    public void TimeoutChatter(string _chatterName)
    {
        TimeoutChatter timeoutChatter = new TimeoutChatter(_chatterName, TIMEOUT_TURNS);

        timeoutChatters.Add(timeoutChatter);
    }

    private int FindMostVotedStepConut()
    {
        Dictionary<int, int> stepCounts = new Dictionary<int, int>();

        foreach (int number in currentVotedSteps)
        {
            if (stepCounts.ContainsKey(number))
            {
                stepCounts[number]++;
            }
            else
            {
                stepCounts[number] = 1;
            }
        }

        int mostVotedStepCount = 0;
        int maxCount = 0;

        foreach (KeyValuePair<int, int> kvp in stepCounts)
        {
            if (kvp.Value > maxCount)
            {
                mostVotedStepCount = kvp.Key;
                maxCount = kvp.Value;
            }
        }

        return mostVotedStepCount;
    }

    public int GetNorthVotes()
    {
        return currentNorthVotes;
    }

    public int GetSouthVotes()
    {
        return currentSouthVotes;
    }

    public int GetEastVotes()
    {
        return currentEastVotes;
    }

    public int GetWestVotes()
    {
        return currentWestVotes;
    }

    public Difficulty GetDifficulty()
    {
        return difficulty;
    }

    public void SetDifficulty(Difficulty _difficulty)
    {
        difficulty = _difficulty;
        PlayerPrefs.SetString(DIFFICULTY, _difficulty.ToString());

        switch (difficulty)
        {
            case Difficulty.Easy:
                childMazeSize = EASY_CHILD_MAZE_SIZE;
                mazeSize = EASY_MAZE_SIZE;
                break;
            case Difficulty.Normal:
                childMazeSize = NORMAL_CHILD_MAZE_SIZE;
                mazeSize = NORMAL_MAZE_SIZE;
                break;
            case Difficulty.Hard:
                childMazeSize = HARD_CHILD_MAZE_SIZE;
                mazeSize = HARD_MAZE_SIZE;
                break;
            case Difficulty.Impossible:
                childMazeSize = IMPOSSIBLE_CHILD_MAZE_SIZE;
                mazeSize = IMPOSSIBLE_MAZE_SIZE;
                break;
        }
    }
}

public class QRectInt
{
    public int x;
    public int y;
    public int width;
    public int height;

    public QRectInt(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}

[Serializable]
public class TimeoutChatter
{
    public string chatterName;
    public int timeoutTurns;

    public TimeoutChatter(string _chatterName, int _timeoutTurns)
    {
        chatterName = _chatterName;
        timeoutTurns = _timeoutTurns;
    }
}