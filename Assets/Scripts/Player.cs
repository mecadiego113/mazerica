using UnityEngine;
using DG.Tweening;
using static GameManager;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public delegate void GoalHandler();
    private GoalHandler goalHandler;

    [SerializeField][Range(0, 10)] private float lerp = 10;

    private Vector3 goalPosition;

    [SerializeField] private float playerY = 0.5f;
    [SerializeField] private Transform raycastEmitter;

    [SerializeField][DisableInPlayMode] private GameObject animal;
    [SerializeField][DisableInPlayMode] private Animator animator;

    [SerializeField][DisableInPlayMode] private List<GameObject> northSlots;
    [SerializeField][DisableInPlayMode] private List<GameObject> southSlots;
    [SerializeField][DisableInPlayMode] private List<GameObject> eastSlots;
    [SerializeField][DisableInPlayMode] private List<GameObject> westSlots;

    private const float ANIMATION_TIME = 0.5f;
    private const float MOVE_TIME = 0.5f;
    private const float ANIMAL_FALL_TIME = 1;

    private const string JUMP = "Jump";
    private const string WALK = "Walk";

    private const string TESTING_CHATTER = "TestingChatter";
    private Vector3 defaultSlotScale = new Vector3(0.05f, 1, 0.05f);

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
    }

    private void Start()
    {
        animal.transform.localPosition = new Vector3(animal.transform.localPosition.x, 100, animal.transform.localPosition.z);
        animal.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Direction.North);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Direction.South);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Direction.East);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Direction.West);
        }

        if (GameManager.Instance.GetState() == GameState.Voting)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                GameManager.Instance.IncreaseNorthVotes();
                UIManager.Instance.OnChatterVoted(Direction.North, TESTING_CHATTER, Color.white);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                GameManager.Instance.IncreaseSouthVotes();
                UIManager.Instance.OnChatterVoted(Direction.South, TESTING_CHATTER, Color.white);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.Instance.IncreaseEastVotes();
                UIManager.Instance.OnChatterVoted(Direction.East, TESTING_CHATTER, Color.white);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager.Instance.IncreaseWestVotes();
                UIManager.Instance.OnChatterVoted(Direction.West, TESTING_CHATTER, Color.white);
            }
        }

        if (goalHandler != null && Vector3.SqrMagnitude(goalPosition - transform.position) < 0.5f)
        {
            goalHandler();
            goalHandler = null;
        }
    }

    public void Move(Direction _direction, int _steps = 1, Action _action = null)
    {
        Vector3 rotation = Vector3.zero;

        switch (_direction)
        {
            case Direction.North:
                rotation = new Vector3(0, 45, 0);
                break;
            case Direction.South:
                rotation = new Vector3(0, -135, 0);
                break;
            case Direction.East:
                rotation = new Vector3(0, -45, 0);
                break;
            case Direction.West:
                rotation = new Vector3(0, 135, 0);
                break;
        }

        if (transform.eulerAngles != rotation)
        {
            animator.SetTrigger(JUMP);
        }

        transform.DORotate(rotation, ANIMATION_TIME).OnComplete(() =>
        {
            if (!Physics.Raycast(new Ray(raycastEmitter.position, transform.forward), _steps))
            {
                animator.SetBool(WALK, true);

                transform.DOMove(transform.forward * _steps, MOVE_TIME * _steps).SetEase(Ease.Linear).SetRelative(true).OnComplete(() =>
                {
                    animator.SetBool(WALK, false);

                    if (_action != null)
                    {
                        _action.Invoke();
                    }
                });
            }
        });
    }

    public bool HasWallInDirection(Direction _direction, int _steps = 1)
    {
        bool wall = false;

        switch (_direction)
        {
            case Direction.North:
                if (Physics.Raycast(new Ray(raycastEmitter.position, transform.right), _steps))
                {
                    wall = true;
                }
                break;
            case Direction.South:
                if (Physics.Raycast(new Ray(raycastEmitter.position, -transform.right), _steps))
                {
                    wall = true;
                }
                break;
            case Direction.East:
                if (Physics.Raycast(new Ray(raycastEmitter.position, transform.forward), _steps))
                {
                    wall = true;
                }
                break;
            case Direction.West:
                if (Physics.Raycast(new Ray(raycastEmitter.position, -transform.forward), _steps))
                {
                    wall = true;
                }
                break;
        }

        return wall;
    }

    public void SetPosition(Vector3 _position)
    {
        transform.position = transform.position = new Vector3(_position.x, playerY, _position.z);
    }

    public void SetGoal(Vector3 _goalPosition, GoalHandler _handler)
    {
        goalHandler = _handler;
        goalPosition = _goalPosition;
        goalPosition.y = playerY;
    }

    public void Show(Action _action = null)
    {
        animal.SetActive(true);

        animal.transform.localScale = new Vector3(0.625f, 0.625f, 0.625f);
        animal.transform.DOLocalMoveY(0, ANIMAL_FALL_TIME).OnComplete(() =>
         {
             if (_action != null)
             {
                 _action.Invoke();
             }
         });
    }

    public void Hide()
    {
        animal.transform.DOScale(new Vector3(0, 10, 0), 1).OnComplete(() =>
        {
            animal.transform.localPosition = new Vector3(animal.transform.localPosition.x, 100, animal.transform.localPosition.z);
            animal.SetActive(false);
        });
    }

    public void ShowSlots()
    {
        bool wall;

        for (int i = 0; i < northSlots.Count; i++)
        {
            wall = HasWallInDirection(Direction.North, i + 1);

            if (!wall)
            {
                northSlots[i].transform.DOScale(defaultSlotScale, ANIMATION_TIME);
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < southSlots.Count; i++)
        {
            wall = HasWallInDirection(Direction.South, i + 1);

            if (!wall)
            {
                southSlots[i].transform.DOScale(defaultSlotScale, ANIMATION_TIME);
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < eastSlots.Count; i++)
        {
            wall = HasWallInDirection(Direction.East, i + 1);

            if (!wall)
            {
                eastSlots[i].transform.DOScale(defaultSlotScale, ANIMATION_TIME);
            }
            else
            {
                break;
            }
        }

        for (int i = 0; i < westSlots.Count; i++)
        {
            wall = HasWallInDirection(Direction.West, i + 1);

            if (!wall)
            {
                westSlots[i].transform.DOScale(defaultSlotScale, ANIMATION_TIME);
            }
            else
            {
                break;
            }
        }
    }

    public void HideSlots()
    {
        for (int i = 0; i < northSlots.Count; i++)
        {
            northSlots[i].transform.DOScale(new Vector3(0, 1, 0), ANIMATION_TIME);
        }

        for (int i = 0; i < southSlots.Count; i++)
        {
            southSlots[i].transform.DOScale(new Vector3(0, 1, 0), ANIMATION_TIME);
        }

        for (int i = 0; i < eastSlots.Count; i++)
        {
            eastSlots[i].transform.DOScale(new Vector3(0, 1, 0), ANIMATION_TIME);
        }

        for (int i = 0; i < westSlots.Count; i++)
        {
            westSlots[i].transform.DOScale(new Vector3(0, 1, 0), ANIMATION_TIME);
        }
    }
}