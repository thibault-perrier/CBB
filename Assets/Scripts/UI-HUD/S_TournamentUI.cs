using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//It's just for the display, for the tournament logic check S_TournamentManager
public class S_TournamentBracket : MonoBehaviour
{
    [SerializeField] private S_TournamentManager _tournamentManager;

    [Header("Tournament brackets")]
    [SerializeField] private GameObject _eightParticipantsBracket;
    [SerializeField] private GameObject _sixteenParticipantsBracket;

    private int _currentRound = 0;
    private int _currentMatch = 0;

    public void PlayRound()
    {
        if (_tournamentManager.GetParticipants() != null && _tournamentManager.IsEven())
        {
            InitializeLogo(_eightParticipantsBracket.transform);
            _tournamentManager.SimulateTournament();
            _currentRound++;
        }
        else //there is a winner
        {
            InitializeLogo(_eightParticipantsBracket.transform);
        }
    }

    public void PlayMatch()
    {
        if (_tournamentManager.IsEven())
        {
            InitializeLogo(_eightParticipantsBracket.transform);
            _tournamentManager.SimulateMatch2();
            _currentMatch++;

            if (_currentMatch > 1)
            {
                _currentMatch = 0;
                _currentRound++;
            }
        }
        else
        {
            _tournamentManager.CheckWinners();
            InitializeLogo(_eightParticipantsBracket.transform);
        }
    }

    public void InitializeLogo(Transform bracket)
    {
        List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

        if (_tournamentManager != null && participants != null)
        {
            Transform logos = bracket.transform.GetChild(1).transform.GetChild(0);

            if (participants.Count > 0)
            {
                int i = 0;
                foreach (Transform participantLogo in logos)
                {
                    participantLogo.GetComponent<Image>().color = participants[i].logo;
                    Debug.Log(participants[i].name);
                    i++;
                }
            }
            else
            {
                Debug.LogWarning("There is no participants !");
            }
        }
    }
}