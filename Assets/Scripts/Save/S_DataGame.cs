using System.Collections.Generic;
using UnityEngine;

public class S_DataGame : MonoBehaviour
{
    public static S_DataGame instance;
    [SerializeField] public S_TournamentManager _tournamentManager;
    public InventorySaver inventory = new InventorySaver();
    public TournamentSaver tournament = new TournamentSaver();

    private void Awake()
    {
        instance = this;
    }
}

[System.Serializable]
public class InventorySaver // Inventory 
{
    public int _money;
    public List<Frame> frames = new List<Frame>();
    public List<Weapons> weapons = new List<Weapons>();
    public List<Robot> robots = new List<Robot>();
}

[System.Serializable]
public class Weapons
{
    public int _id;
    public string _name;
    public int _number;
}

[System.Serializable]
public class Frame
{
    public int _id;
    public string _name;
    public int _number;
}

[System.Serializable]
public class Robot
{
    public int _id;
    public string _name;
    public Frame _frame;
    public List<Weapons> _weapons;
}

[System.Serializable]
public class TournamentSaver // Tournament
{
    public bool _isRunning;
    public List<S_TournamentManager.Participant> _participants;
    public int _currentMatch;
    public int _currentLevel;
    public S_TournamentManager.Tournament _tournamentInfo;
    public List<S_TournamentManager.Participant> _roundWinners;
}


