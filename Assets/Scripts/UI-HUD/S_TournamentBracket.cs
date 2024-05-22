using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_TournamentBracket : MonoBehaviour
{
    [SerializeField] private S_TournamentManager _tournamentManager;
    [SerializeField] private GameObject _matchBracket;

    [Header("Finals")]
    [SerializeField] private Transform _eightFinal;
    [SerializeField] private Transform _quarterFinal;
    [SerializeField] private Transform _final;

    private Transform[] _finals;
    private int _rounds = 0;
    private int _currentRound = 0;

    private void Awake()
    {
        _finals = new Transform[] { _eightFinal, _quarterFinal, _final };
    }

    private void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.Q) && _tournamentManager.GetParticipants() != null)
        {
            if (_currentRound < _finals.Length)
            {
                Debug.Log("Update brackets and simulating next match.");
                InitializeBracket(_finals[_currentRound]);
                _tournamentManager.SimulateTournament();
                _currentRound++;
            }
            else
            {
                Debug.Log("We have a winner : " + _tournamentManager.GetParticipants()[0].name + " !");
            }
        }
    }

    public void InitializeBracket(Transform bracket)
    {
        if (_tournamentManager != null && _tournamentManager.GetParticipants() != null)
        {
            List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

            if (participants.Count > 0)
            {
                int i = 0;
                foreach (Transform participantName in bracket)
                {
                    participantName.GetComponent<TextMeshProUGUI>().text = participants[i].name;

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