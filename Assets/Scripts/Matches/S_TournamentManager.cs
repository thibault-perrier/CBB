using System;
using System.Collections.Generic;
using UnityEngine;

public class S_TournamentManager : MonoBehaviour
{
    public struct Participant
    {
        public string name;
        public int id;
        public bool hasLost;
        //public Robot robot;
    }

    public enum Rank
    {
        Plastic,//test !!
        Bronze,
        Silver,
        Gold,
        Diamond
    }

    public List<Participant> _participants;
    private Dictionary<Rank, int> _cost = new Dictionary<Rank, int>();
    private Dictionary<Rank, int> _participantsNb = new Dictionary<Rank, int>();

    private Rank _currentRank;
    private int _currentMatchNb = 1;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            InitializeCurrentTournament(Rank.Plastic);
            Debug.Log("Initialazing a " + Rank.Plastic.ToString() + " tournament");
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
        _cost.Add(Rank.Bronze, 25);
        _cost.Add(Rank.Silver, 50);
        _cost.Add(Rank.Gold, 150);
        _cost.Add(Rank.Diamond, 250);

        _cost.Add(Rank.Plastic, 0);//test

        _participantsNb.Add(Rank.Bronze, 16);
        _participantsNb.Add(Rank.Silver, 16);
        _participantsNb.Add(Rank.Gold, 8);
        _participantsNb.Add(Rank.Diamond, 8);

        _participantsNb.Add(Rank.Plastic, 4);//test
    }

    /* Initialize the number of participant and the rank of the tournament
     * and make the player pay */
    public int InitializeCurrentTournament(Rank rank)
    {
        _currentRank = rank;

        _participants = new List<Participant>(_participantsNb[rank])
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

        return _cost[_currentRank];
    }

    //Shuffle the list of participants so they are randomized 
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

    /* Will probably not be used for the finished game
     * make all the participants fight each other */
    public void SimulateTournament()
    {
        if (_participants.Count > 0)
        {
            for (int i = 0; i < _participants.Count; i += 2)
            {
                SimulateMatch(_participants[i], _participants[i + 1]);
            }

            //Remove the participants that lost
            foreach (Participant participant in _participants)
            {
                if (participant.hasLost)
                {
                    _participants.Remove(participant);
                }
            }
        }
    }

    //Simulate a match and take in account the strength of the participant's robot
    public void SimulateMatch(Participant p1, Participant p2)
    {
        bool participant1Wins = UnityEngine.Random.Range(0f, 1f) >= 0.5f; //replace with cote calculation

        if (participant1Wins)
        {
            p2.hasLost = true;
        }
        else
        {
            p1.hasLost = true;
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
}