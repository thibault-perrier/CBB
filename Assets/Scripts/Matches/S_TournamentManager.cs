using System;
using System.Collections.Generic;
using UnityEngine;

public class S_TournamentManager : MonoBehaviour
{
    [SerializeField] S_TournamentBracket _tournamentBracket;

    [Serializable]
    public struct Participant
    {
        public bool isPlayer;
        public int id;
        public string name;
        //public Sprite logo;
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

    [SerializeField] private List<Participant> _participants;
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

    //TEST PARTICIPANTS
    private Participant _participant1; //lets pretend this is the player for testing
    private Participant _participant2;
    private Participant _participant3;
    private Participant _participant4;
    private Participant _participant5;
    private Participant _participant6;
    private Participant _participant7;
    private Participant _participant8;
    private Participant _participant9;
    private Participant _participant10;
    private Participant _participant11;
    private Participant _participant12;
    private Participant _participant13;
    private Participant _participant14;
    private Participant _participant15;
    private Participant _participant16;

    private void Awake()
    {
        _roundWinners = new List<Participant>();

        //InitializeParticipationData();

        _participant1.isPlayer = true;

        _participant1.name = "PARTICIPANT NB 0";
        _participant2.name = "PARTICIPANT NB 1";
        _participant3.name = "PARTICIPANT NB 2";
        _participant4.name = "PARTICIPANT NB 3";
        _participant5.name = "PARTICIPANT NB 4";
        _participant6.name = "PARTICIPANT NB 5";
        _participant7.name = "PARTICIPANT NB 6";
        _participant8.name = "PARTICIPANT NB 7";
        _participant9.name = "PARTICIPANT NB 8";
        _participant10.name = "PARTICIPANT NB 9";
        _participant11.name = "PARTICIPANT NB 10";
        _participant12.name = "PARTICIPANT NB 11";
        _participant13.name = "PARTICIPANT NB 12";
        _participant14.name = "PARTICIPANT NB 13";
        _participant15.name = "PARTICIPANT NB 14";
        _participant16.name = "PARTICIPANT NB 15";

        _participant1.logo = Color.blue;
        _participant2.logo = Color.red;
        _participant3.logo = Color.yellow;
        _participant4.logo = Color.green;
        _participant5.logo = Color.white;
        _participant6.logo = Color.cyan;
        _participant7.logo = Color.black;
        _participant8.logo = Color.magenta;
        _participant9.logo = Color.blue;
        _participant10.logo = Color.red;
        _participant11.logo = Color.yellow;
        _participant12.logo = Color.green;
        _participant13.logo = Color.white;
        _participant14.logo = Color.cyan;
        _participant15.logo = Color.black;
        _participant16.logo = Color.magenta;

        _participant1.rank = _currentTournament.rank;
        _participant2.rank = _currentTournament.rank;
        _participant3.rank = _currentTournament.rank;
        _participant4.rank = _currentTournament.rank;
        _participant5.rank = _currentTournament.rank;
        _participant6.rank = _currentTournament.rank;
        _participant7.rank = _currentTournament.rank;
        _participant8.rank = _currentTournament.rank;
        _participant9.rank = _currentTournament.rank;
        _participant10.rank = _currentTournament.rank;
        _participant11.rank = _currentTournament.rank;
        _participant12.rank = _currentTournament.rank;
        _participant13.rank = _currentTournament.rank;
        _participant14.rank = _currentTournament.rank;
        _participant15.rank = _currentTournament.rank;
        _participant16.rank = _currentTournament.rank;

        InitializeCurrentTournament(_currentTournament);
        Debug.Log("Initialazing a " + _currentTournament.rank.ToString() + " tournament");

        _tournamentBracket.SetBracket(_currentTournament.maxMatchNb * 2);

        _currentTournament.maxMatchNb -= 1; //It's so we can use this as an index for arrays and lists

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
    /// Initialize the data for each rank
    /// </summary>
    private void InitializeParticipationData()
    {
        _bronze = SetRankData(_bronze, Rank.Bronze, 25, 16, 400, 8 - 1);
        _silver = SetRankData(_silver, Rank.Silver, 50, 16, 800, 8 - 1);
        _gold = SetRankData(_gold, Rank.Gold, 150, 8, 1200, 4 - 1);
        _diamond = SetRankData(_diamond, Rank.Diamond, 250, 8, 2000, 4 - 1);

        _plastic = SetRankData(_plastic, Rank.Plastic, 0, 4, 1, 2 - 1); //FOR TESTING PURPOSE ONLY
    }

    /// <summary>
    /// Set the data for a rank, wich rank, cost, the number of participant, the prize at the end, the number of match there will be
    /// </summary>
    /// <param name="tournament"></param>
    /// <param name="rank"></param>
    /// <param name="cost"></param>
    /// <param name="participantNb"></param>
    /// <param name="prize"></param>
    /// <param name="matchNb"></param>
    /// <returns></returns>
    private Tournament SetRankData(Tournament tournament, Rank rank, int cost, int participantNb, int prize, int matchNb)
    {
        tournament.rank = rank;
        tournament.cost = cost;
        tournament.participantNb = participantNb;
        tournament.prize = prize;
        tournament.maxMatchNb = matchNb;

        return tournament;
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

        if (_currentTournament.rank == Rank.Bronze || _currentTournament.rank == Rank.Silver)
        {
            _participants = new List<Participant>(tournament.participantNb)
        {
             // TEST
            _participant1,
            _participant2,
            _participant3,
            _participant4,
            _participant5,
            _participant6,
            _participant7,
            _participant8,
            _participant9,
            _participant10,
            _participant11,
            _participant12,
            _participant13,
            _participant14,
            _participant15,
            _participant16,
        };
        }
        else
        {
            _participants = new List<Participant>(tournament.participantNb)
        {
             // TEST
            _participant1,
            _participant2,
            _participant3,
            _participant4,
            _participant5,
            _participant6,
            _participant7,
            _participant8,
        };
        }


        //Add random participants in the list with random robots, their strength depend of the difficulty

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
    public void SimulateMatch()
    {
        if (IsEven())
        {
            bool participant1Wins = UnityEngine.Random.Range(0f, 1f) >= 0.5f; //replace with cote calculation

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