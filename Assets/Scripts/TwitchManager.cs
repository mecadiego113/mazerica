using System.Text.RegularExpressions;
using TwitchChat;
using UnityEngine;

public class TwitchManager : MonoBehaviour
{
    public static TwitchManager Instance;

    [SerializeField] TwitchSettings customSettings;

    private string twitchChannel;

    private const string NORTH_COMMAND = "!north";
    private const string SOUTH_COMMAND = "!south";
    private const string EAST_COMMAND = "!east";
    private const string WEST_COMMAND = "!west";

    private const string TWITCH_CHANNEL = "twitch_channel";

    private void OnDestroy()
    {
        TwitchController.onTwitchMessageReceived -= OnMessageReceived;
    }

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
        twitchChannel = PlayerPrefs.GetString(TWITCH_CHANNEL, string.Empty);

        if (twitchChannel != string.Empty)
        {
            UIManager.Instance.Initialize(twitchChannel);
        }

        TwitchController.onTwitchMessageReceived += OnMessageReceived;
        TwitchController.onChannelJoined += OnChannelJoined;
    }

    public void Connect(string _twitchChannel)
    {
        twitchChannel = _twitchChannel;

        TwitchController.Login(_twitchChannel, customSettings);
    }

    private void OnChannelJoined()
    {
        PlayerPrefs.SetString(TWITCH_CHANNEL, twitchChannel);

        GameManager.Instance.OnConnected();
    }

    private void OnMessageReceived(Chatter _chatter)
    {
        if (_chatter.IsCommand() && !GameManager.Instance.ChatterAlreadyVoted(_chatter.tags.displayName) && GameManager.Instance.GetState() == GameManager.GameState.Voting)
        {
            GameManager.Direction direction = GameManager.Direction.None;
            string pattern = @"!(north|south|east|west) ([2-5])";
            MatchCollection matches = Regex.Matches(_chatter.message, pattern);

            string command;

            int steps;

            if (matches.Count == 1)
            {
                command = string.Format("!{0}", matches[0].Groups[1].Value);

                steps = int.Parse(matches[0].Groups[2].Value);
            }
            else
            {
                command = _chatter.message;

                steps = 1;
            }

            bool validCommand = false;

            switch (command)
            {
                case NORTH_COMMAND:
                    GameManager.Instance.AddChatter(_chatter.tags.displayName);
                    GameManager.Instance.IncreaseNorthVotes();
                    direction = GameManager.Direction.North;
                    validCommand = true;
                    break;
                case SOUTH_COMMAND:
                    GameManager.Instance.AddChatter(_chatter.tags.displayName);
                    GameManager.Instance.IncreaseSouthVotes();
                    direction = GameManager.Direction.South;
                    validCommand = true;
                    break;
                case EAST_COMMAND:
                    GameManager.Instance.AddChatter(_chatter.tags.displayName);
                    GameManager.Instance.IncreaseEastVotes();
                    direction = GameManager.Direction.East;
                    validCommand = true;
                    break;
                case WEST_COMMAND:
                    GameManager.Instance.AddChatter(_chatter.tags.displayName);
                    GameManager.Instance.IncreaseWestVotes();
                    direction = GameManager.Direction.West;
                    validCommand = true;
                    break;
            }

            if (validCommand)
            {
                GameManager.Instance.VoteStep(steps);

                if (Player.Instance.HasWallInDirection(direction))
                {
                    GameManager.Instance.TimeoutChatter(_chatter.tags.displayName);
                }

                UIManager.Instance.OnChatterVoted(direction, _chatter.tags.displayName, _chatter.GetNameColor());
            }
        }
    }
}
