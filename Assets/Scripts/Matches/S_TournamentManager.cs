using System.Collections.Generic;
using UnityEngine;

public class S_TournamentManager : MonoBehaviour
{
    [SerializeField] S_TournamentBracket _tournamentBracket;

    public struct Participant
    {
        public string name;
        public int id;
        //public Sprite logo;
        public Color logo;
        public float rating;
        //public Robot robot;
    }

    public enum Rank
    {
        Plastic, //FOR TESTING PURPOSE ONLY
        Bronze,
        Silver,
        Gold,
        Diamond
    }

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

    private Tournament _plastic; //FOR TESTING PURPOSE ONLY
    private Tournament _bronze;
    private Tournament _silver;
    private Tournament _gold;
    private Tournament _diamond;

    private Tournament _currentRank;

    private int _currentMatch = 0;
    private int _currentRound = 0;

    //TEST PARTICIPANTS
    private Participant _participant1; //lets pretend this is the player for testing
    private Participant _participant2;
    private Participant _participant3;
    private Participant _participant4;

    public Event PlayerLose;

    private void Awake()
    {
        _roundWinners = new List<Participant>();

        InitializeParticipationData();

        _participant1.name = "PLAYER";
        _participant2.name = "Participant nb 1";
        _participant3.name = "Participant nb 2";
        _participant4.name = "Participant nb 3";

        _participant1.logo = Color.blue;
        _participant2.logo = Color.red;
        _participant3.logo = Color.yellow;
        _participant4.logo = Color.green;

        InitializeCurrentTournament(_plastic);
        Debug.Log("Initialazing a " + _plastic.rank.ToString() + " tournament");
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

    //Set the data inside dictionnaries to check the cost or the number of participants easily
    private void InitializeParticipationData()
    {
        _bronze = SetRankData(_bronze, Rank.Bronze, 25, 16, 400, 8 - 1);
        _silver = SetRankData(_silver, Rank.Silver, 50, 16, 800, 8 - 1);
        _gold = SetRankData(_gold, Rank.Gold, 150, 8, 1200, 4 - 1);
        _diamond = SetRankData(_diamond, Rank.Diamond, 250, 8, 2000, 4 - 1);

        _plastic = SetRankData(_plastic, Rank.Plastic, 0, 4, 1, 2 - 1); //FOR TESTING PURPOSE ONLY
    }

    private Tournament SetRankData(Tournament tournament, Rank rank, int cost, int participantNb, int prize, int matchNb)
    {
        tournament.rank = rank;
        tournament.cost = cost;
        tournament.participantNb = participantNb;
        tournament.prize = prize;
        tournament.maxMatchNb = matchNb;

        return tournament;
    }

    /* Initialize the number of participant and the rank of the tournament
     * and make the player pay */
    public int InitializeCurrentTournament(Tournament tournament)
    {
        _currentRank = tournament;
        Debug.Log(_currentRank.maxMatchNb);

        _participants = new List<Participant>(tournament.participantNb)
        {
             // TEST
            _participant1, 
            _participant2,
            _participant3,
            _participant4
        };

        //Add random participants in the list with random robots, their strength depend of the difficulty

        if (_participants.Count > 0)
        {
            ShuffleParticipants(_participants);
        }

        return tournament.cost;
    }

    //Shuffle the list of participants so they are randomized 
    public void ShuffleParticipants(List<Participant> participants)
    {
        int n = participants.Count;

        while (n > 1)
        {
            int k = Random.Range(0, n);
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

    //Remove the participants that lost
    public void CheckWinners()
    {
        _participants = new List<Participant>(_roundWinners);

        _roundWinners.Clear();
    }

    //Simulate a match and take in account the strength of the participant's robot
    public void SimulateMatch()
    {
        if (IsEven())
        {
            bool participant1Wins = Random.Range(0f, 1f) >= 0.5f; //replace with cote calculation

            Participant p1 = _participants[_currentMatch * 2]; //need to multiply so the previous participant does not fight again
            Participant p2 = _participants[(_currentMatch * 2) + 1];

            if (participant1Wins)
            {
                _roundWinners.Add(p1);
                if (p2.name == "PLAYER")
                {
                    _tournamentBracket.PlayerLostScreen();
                }
            }
            else
            {
                _roundWinners.Add(p2);
                if (p1.name == "PLAYER")
                {
                    _tournamentBracket.PlayerLostScreen();
                }
            }
        }
    }

    //Check if there is an even number of participants
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

    public void StartMatchWithPlayer(Participant p)
    {
        //put the logic that will make a match start here

        Debug.Log("Player vs " + p.name);
    }

    public void StartRealMatchWithBots(Participant p1, Participant p2)
    {
        //put the logic that will make a match start here

        Debug.Log(p1.name + p2.name);
    }

    public void NextMatch()
    {
        _currentMatch++;

        if (_currentMatch > _currentRank.maxMatchNb)
        {
            _currentMatch = 0;
            _currentRank.maxMatchNb /= 2;
            _currentRound++;
            CheckWinners();
        }
    }

    public int GetMatchNb()
    {
        return _currentMatch;
    }

    public int GetRoundNb()
    {
        return _currentRound;
    }

    public bool IsPlayerPlaying(Participant p1, Participant p2)
    {
        if (p1.name == "PLAYER" ||  p2.name == "PLAYER")
        {
            return true;
        }

        return false;
    }

    public int WinPrize()
    {
        return _currentRank.prize;
    }

    public float GetRating()
    {
        return 0; //need to get a way to get the participant's rating
    }
}