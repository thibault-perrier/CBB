using System;
using System.Collections.Generic;
using UnityEngine;

public class S_TournamentManager : MonoBehaviour
{
    [SerializeField] S_TournamentBracket _tournamentBracket;
    [SerializeField] List<Sprite> _participantsLogos;

    [Serializable]
    public struct Participant
    {
        public bool isPlayer;
        public int id;
        public string name;
        public Sprite logoSprite;
        public Color logo;
        public float rating;
        public bool hasLost;
        //public Robot robot;
        public Rank rank;
    }

    public enum Rank
    {
        Plastic, //FOR TESTING PURPOSE ONLY
        Bronze,
        Silver,
        Gold,
        Diamond
    }

    [Serializable]
    public struct Tournament
    {
        public Rank rank;
        public int cost;
        public int participantNb;
        public int prize;
        public int maxMatchNb;
    }

    private List<Participant> _participants = new List<Participant>();
    private List<Participant> _roundWinners;
    private List<Participant> _roundLosers = new List<Participant>();

    private Tournament _plastic; //FOR TESTING PURPOSE ONLY
    private Tournament _bronze;
    private Tournament _silver;
    private Tournament _gold;
    private Tournament _diamond;

    [SerializeField] private Tournament _currentTournament;

    private int _currentMatch = 0;
    private int _currentLevel = 0;

    private bool _isRunning = false;
    public bool IsRunning { get { return _isRunning; } set { _isRunning = value; } }
    public S_TournamentBracket Bracket { get => _tournamentBracket; }
    private S_CustomName _customName;

    private void Awake()
    {
        _customName = GetComponent<S_CustomName>();
        _roundWinners = new List<Participant>();

        InitializeCurrentTournament(_currentTournament);

        Debug.Log("maxMatch" + _currentTournament.maxMatchNb);

        _tournamentBracket.SetBracket(_currentTournament.maxMatchNb * 2);

        _currentTournament.maxMatchNb -= 1; //It's so we can use this as an index for arrays and lists
    }

    /// <summary>
    /// Initialize participants for the upcoming tournament
    /// </summary>
    /// <param name="tournament"></param>
    private void InitializeParticipants(Tournament tournament)
    {
        Participant player = new Participant();
        player.name = S_DataGame.Instance.inventory.GetPlayerName();
        player.logo = InitializePlayerLogo();
        player.logoSprite = _participantsLogos[S_DataGame.Instance.inventory.overlayImageIndex];
        player.isPlayer = true;
        _participants.Add(player);

        for (int i = 1; i < tournament.participantNb; i++)
        {
            Participant participant = new Participant();
            string name = _customName.GetRandomName();
            participant.name = name;
            participant.logoSprite = _participantsLogos[UnityEngine.Random.Range(0, _participantsLogos.Count)];
            participant.logo = ChooseRandomColor();
            participant.rank = tournament.rank;
            participant.isPlayer = false;

            _participants.Add(participant);
        }
    }

    /// <summary>
    /// Load the player's logo from the save Data and give it to the player participant variable
    /// </summary>
    /// <returns></returns>
    private Color InitializePlayerLogo()
    {
        if (S_DataGame.Instance != null)
        {
            S_DataGame.Instance.inventory.LoadOverlayColor();
            string overlayColorHex = S_DataGame.Instance.inventory.overlayColorHex;
            Color overlayColor;

            if (ColorUtility.TryParseHtmlString("#" + overlayColorHex, out overlayColor))
            {
                return overlayColor;
            }
        }

        return Color.white; //failed to load color
    }

    /// <summary>
    /// Return a random color, but not too saturated or dark
    /// </summary>
    /// <returns></returns>
    private Color ChooseRandomColor()
    {
        Color.RGBToHSV(UnityEngine.Random.ColorHSV(), out float hue, out float saturation, out float value);
        saturation = Mathf.Clamp(saturation, 0.4f, 0.8f);
        value = Mathf.Clamp(value, 0.8f, 0.9f);

        return Color.HSVToRGB(hue, saturation, value);
    }

    public void AddParticipant(Participant participant)
    {
        _participants.Add(participant);
    }

    public void RemoveParticipant(Participant participant)
    {
        _participants.Remove(participant);
    }

    public void ClearParticipants()
    {
        _participants.Clear();
    }

    /// <summary>
    /// Initialize the number of participant and the rank of the tournament
    /// and make the player pay
    /// </summary>
    /// <param name="tournament"></param>
    /// <returns></returns>
    public int InitializeCurrentTournament(Tournament tournament)
    {
        _currentTournament = tournament;

        InitializeParticipants(tournament);

        if (_participants.Count > 0)
        {
            ShuffleParticipants(_participants);
        }

        //Give participants ID to know where they are in the bracket
        for (int i = 0; i < _participants.Count; i++)
        {
            Participant participant = _participants[i];

            participant.id = i * 2;
            _participants[i] = participant;
        }

        return tournament.cost;
    }

    /// <summary>
    /// Shuffle the list of participants so they are randomized
    /// </summary>
    /// <param name="participants"></param>
    public void ShuffleParticipants(List<Participant> participants)
    {
        int n = participants.Count;

        while (n > 1)
        {
            int k = UnityEngine.Random.Range(0, n);
            n--;

            Participant tmp = participants[k];
            participants[k] = participants[n];
            participants[n] = tmp;
        }
    }

    public List<Participant> GetParticipants()
    {
        return _participants;
    }

    public List<Participant> GetRoundWinners()
    {
        return _roundWinners;
    }

    /// <summary>
    /// Remove the participants that lost from the list of participants
    /// </summary>
    public void CheckWinners()
    {
        _participants = new List<Participant>(_roundWinners);
        _tournamentBracket.RefreshWinnerLogo();

        _roundWinners.Clear();
        _roundLosers.Clear();
    }

    /// <summary>
    /// Simulate a match and take in account the strength of the participant's robot and rating
    /// </summary>
    public void SimulateMatch(float victoryPercent)
    {
        if (IsEven())
        {
            bool participant1Wins = UnityEngine.Random.Range(0f, 1f) >= victoryPercent; //replace with cote calculation

            if (participant1Wins)
            {
                MakeWinForParticipantOne();
            }
            else
            {
                MakeWinForParticipantTwo();
            }
        }
    }
    /// <summary>
    /// Set the win for the participant number one
    /// </summary>
    public void MakeWinForParticipantOne()
    {
        Participant p1 = _participants[_currentMatch * 2]; //need to multiply so the previous participant does not fight again
        Transform logo1 = _tournamentBracket.GetLogos()[_currentMatch * 2];

        Participant p2 = _participants[(_currentMatch * 2) + 1];
        Transform logo2 = _tournamentBracket.GetLogos()[(_currentMatch * 2) + 1];

        p2.hasLost = true;
        _roundWinners.Add(p1);
        _roundLosers.Add(p2);
        _tournamentBracket.AddWinnerLogo(logo1, logo2);
        if (p2.isPlayer)
        {
            _tournamentBracket.PlayerLostScreen();
            Debug.Log("Player lost screen");
        }
    }
    /// <summary>
    /// Set the win for the participant number two
    /// </summary>
    public void MakeWinForParticipantTwo()
    {
        Participant p1 = _participants[_currentMatch * 2]; //need to multiply so the previous participant does not fight again
        Transform logo1 = _tournamentBracket.GetLogos()[_currentMatch * 2];

        Participant p2 = _participants[(_currentMatch * 2) + 1];
        Transform logo2 = _tournamentBracket.GetLogos()[(_currentMatch * 2) + 1];

        p1.hasLost = true;
        _roundWinners.Add(p2);
        _roundLosers.Add(p1);
        _tournamentBracket.AddWinnerLogo(logo2, logo1);
        if (p1.isPlayer)
        {
            _tournamentBracket.PlayerLostScreen();
            Debug.Log("Player lost screen");
        }
    }

    /// <summary>
    /// Check if there is an even number of participants
    /// </summary>
    /// <returns></returns>
    public bool IsEven()
    {
        if (_participants != null)
        {
            return _participants.Count % 2 == 0;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Start a real match (and not a simulation)
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public void StartRealMatch(Participant p1, Participant p2)
    {
        //put the logic that will make a match start here

        Debug.Log(p1.name + p2.name);
    }

    /// <summary>
    /// Update the index of the currently played match and current level (or round) of the tournament
    /// </summary>
    public void NextMatch()
    {
        _currentMatch++;

        if (_currentMatch > _currentTournament.maxMatchNb)
        {
            _currentMatch = 0;
            _currentTournament.maxMatchNb /= 2;
            _currentLevel++;
            CheckWinners();
        }
    }

    public int GetMatchNb()
    {
        return _currentMatch;
    }

    public int GetRoundNb()
    {
        return _currentLevel;
    }

    /// <summary>
    /// Check if the player has to play the current match or not
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public bool IsPlayerPlaying(Participant p1, Participant p2)
    {
        if (p1.isPlayer || p2.isPlayer)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Give the player his reward
    /// </summary>
    /// <returns></returns>
    public int WinPrize()
    {
        _isRunning = false;
        return _currentTournament.prize;
    }

    public float GetRating()
    {
        return 0; //need to get a way to get the participant's rating
    }

    public Participant GetCurrentLoser()
    {
        return _roundLosers[_currentMatch];
    }

    public int GetTournamentPrize()
    {
        return _currentTournament.prize;
    }
}