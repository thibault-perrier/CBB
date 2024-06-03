using System;
using System.Collections.Generic;
using UnityEngine;

public class S_DataGame : MonoBehaviour
{
    public static S_DataGame Instance;
    [SerializeField] public S_TournamentManager _tournamentManager;

    [Serializable]
    public enum Load
    {
        Nothing,
        Inventory,
        Tournament,
        InventoryAndTournament
    }

    public Load OnSceneLoad;

    public InventorySaver inventory = new InventorySaver();
    public TournamentSaver tournament = new TournamentSaver();

    private void Awake()
    {
        Instance = this;
        if (OnSceneLoad == Load.Inventory || OnSceneLoad == Load.InventoryAndTournament)
        {
            LoadInventory();
        }
        else if (OnSceneLoad == Load.Tournament || OnSceneLoad == Load.InventoryAndTournament)
        {
            LoadTournament();
        }
    }

    public void SaveInventory()
    {
        S_FileHandler.Instance.SaveInventory();
    }

    public void LoadInventory()
    {
        S_FileHandler.Instance.LoadInventory();
    }

    public void SaveTournament()
    {
        S_FileHandler.Instance.SaveTournament();
    }

    public void LoadTournament()
    {
        S_FileHandler.Instance.LoadTournament();
    }
}

[System.Serializable]
public class InventorySaver // Inventory 
{
    private int _currentMoney;
    public int PlayerMoney
    { 
        get => _currentMoney; 
        set => _currentMoney = value; 
    }
    public List<Frame> frames = new List<Frame>();
    public List<Weapons> weapons = new List<Weapons>();
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
public class TournamentSaver // Tournament
{
    public bool _isRunning;
    public List<S_TournamentManager.Participant> _participants;
    public int _currentMatch;
    public int _currentLevel;
    public S_TournamentManager.Tournament _tournamentInfo;
    public List<S_TournamentManager.Participant> _roundWinners;
}


