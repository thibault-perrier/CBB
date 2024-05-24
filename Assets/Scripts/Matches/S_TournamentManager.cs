using System.Collections.Generic;
using UnityEngine;

public class S_TournamentManager : MonoBehaviour
{
    public struct Participant
    {
        public string name;
        public int id;
        public bool hasLost;
        //public Sprite logo;
        public Color logo;
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
    }

    private List<Participant> _participants;
    private List<Participant> _roundWinners;

    private Tournament _plastic; //FOR TESTING PURPOSE ONLY
    private Tournament _bronze;
    private Tournament _silver;
    private Tournament _gold;
    private Tournament _diamond;

    private Tournament _currentRank;
    private int _currentBet = 0;
    private float _currentBetRating = 0;

    private int _currentMatch = 0;
    private int _currentRound = 0;

    //TEST PARTICIPANTS
    private Participant _participant1;
    private Participant _participant2;
    private Participant _participant3;
    private Participant _participant4;

    private void Awake()
    {
        InitializeParticipationData();

        _participant1.name = "Participant nb 1";
        _participant2.name = "Participant nb 2";
        _participant3.name = "Participant nb 3";
        _participant4.name = "Participant nb 4";

        _participant1.logo = Color.blue;
        _participant2.logo = Color.red;
        _participant3.logo = Color.yellow;
        _participant4.logo = Color.green;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InitializeCurrentTournament(_plastic);
            Debug.Log("Initialazing a " + _plastic.rank.ToString() + " tournament");
        }
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
        SetRankData(_bronze, Rank.Bronze, 25, 16, 400);
        SetRankData(_silver, Rank.Silver, 50, 16, 800);
        SetRankData(_gold, Rank.Gold, 150, 8, 1200);
        SetRankData(_diamond, Rank.Diamond, 250, 8, 2000);

        SetRankData(_plastic, Rank.Plastic, 0, 4, 1); //FOR TESTING PURPOSE ONLY
    }

    private void SetRankData(Tournament tournament, Rank rank, int cost, int participantNb, int prize)
    {
        tournament.rank = rank;
        tournament.cost = cost;
        tournament.participantNb = participantNb;
        tournament.prize = prize;
    }

    /* Initialize the number of participant and the rank of the tournament
     * and make the player pay */
    public int InitializeCurrentTournament(Tournament tournament)
    {
        _currentRank = tournament;

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

    /* Will probably not be used for the finished game
     * make all the participants fight each other */
    public void SimulateTournament()
    {
        if (_participants.Count > 0)
        {
            for (int i = 0; i < _participants.Count; i += 2)
            {
                SimulateMatch(i, i + 1);
            }

            CheckWinners();
        }
    }

    //Remove the participants that lost
    public void CheckWinners()
    {
        //List<Participant> winners = new List<Participant>();

        //foreach (Participant participant in _participants)
        //{
        //    if (!participant.hasLost)
        //    {
        //        winners.Add(participant);
        //    }
        //}

        _participants = _roundWinners;

        _roundWinners.Clear();
    }

    //Simulate a match and take in account the strength of the participant's robot
    public void SimulateMatch(int p1, int p2)
    {
        bool participant1Wins = Random.Range(0f, 1f) >= 0.5f; //replace with cote calculation

        if (participant1Wins)
        {
            Participant participant2 = _participants[p2];
            participant2.hasLost = true;

            _participants[p2] = participant2;
        }
        else
        {
            Participant participant1 = _participants[p1];
            participant1.hasLost = true;

            _participants[p1] = participant1;
        }
    }

    public void SimulateMatch2()
    {
        bool participant1Wins = Random.Range(0f, 1f) >= 0.5f; //replace with cote calculation

        Participant p1 = _participants[_currentMatch * 2];
        Participant p2 = _participants[(_currentMatch * 2) + 1];

        if (participant1Wins)
        {
            //p2.hasLost = true;
            //_participants[(_currentMatch * 2) + 1] = p2;
            _roundWinners.Add(p2);
        }
        else
        {
            //p1.hasLost = true;
            //_participants[_currentMatch * 2] = p1;
            _roundWinners.Add(p1);
        }

        _currentMatch++;

        if (_currentRound == 0 &&_currentMatch > 1)
        {
            _currentMatch = 0;
            _currentRound++;
            CheckWinners();
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

    public int WinPrize()
    {
        return _currentRank.prize;
    }

    public void BetMatch(int betValue)
    {
        _currentBet = betValue;
    }

    public int WinBet()
    {
        return _currentBet * Mathf.RoundToInt(_currentBetRating);
    }
}