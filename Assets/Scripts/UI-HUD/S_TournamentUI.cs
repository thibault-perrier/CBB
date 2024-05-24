using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_TournamentBracket : MonoBehaviour
{
    [SerializeField] private S_TournamentManager _tournamentManager;

    [Header("Tournament brackets")]
    [SerializeField] private GameObject _eightParticipantsBracket;
    [SerializeField] private GameObject _sixteenParticipantsBracket;

    private GameObject _currentUsedBracket;

    [Header("Match buttons")]
    [SerializeField] private GameObject _botMatchButtons;
    [SerializeField] private GameObject _playerMatchButton;

    [Header("Win / Lost text")]
    [SerializeField] private GameObject _playerLose;

    private int _currentRound = 0;
    private int _currentMatch = 0;

    private void Start()
    {
        InitializeLogo(_eightParticipantsBracket.transform);
        _currentUsedBracket = _eightParticipantsBracket;

        _botMatchButtons.SetActive(false);
        _playerMatchButton.SetActive(false);

        PopUpButton();
    }

    //Put the logo of the participant in the bottom of the bracket
    public void InitializeLogo(Transform bracket)
    {
        List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

        if (_tournamentManager != null && participants != null)
        {
            Transform logos = bracket.transform.GetChild(1).GetChild(0);

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
        else
        {
            Debug.LogWarning("There is maybe no reference to S_TournamentManager, or the list of participants is null.");
        }
    }

    /* After a match it will put the winner's logo at the right place */
    public void UpdateWinnerLogo(Transform bracket, int currentRound, int currentMatch)
    {
        List<S_TournamentManager.Participant> winners = _tournamentManager.GetRoundWinners();
        Transform round = bracket.transform.GetChild(1);

        if (_tournamentManager != null && winners != null)
        {
            if (currentRound < round.childCount - 1)
            {
                Transform winnerLogo = GetNextRoundLogo(bracket, currentRound, currentMatch);

                winnerLogo.GetComponent<Image>().color = winners[currentMatch].logo;
            }
        }
    }

    /* Get a reference of the next winner's logo 
     * depending of the match of the current round */
    public Transform GetNextRoundLogo(Transform bracket, int currentRound, int currentMatch)
    {
        Transform logos = bracket.transform.GetChild(1).GetChild(currentRound + 1);

        return logos.GetChild(currentMatch);
    }

    /* If the player don't want to watch the match it will
     * simulate a match and get a random winner based on rating 
     * Also update automatically the logo in the UI */
    public void SkipBotMatch()
    {
        if (_tournamentManager.IsEven())
        {
            _tournamentManager.SimulateMatch();

            UpdateWinnerLogo(_currentUsedBracket.transform, _currentRound, _currentMatch);
            _tournamentManager.NextMatch();
            UpdateMatchAndRound();

            PopUpButton();
        }
    }

    /* Check which match and round it is to update the correct logos */
    public void UpdateMatchAndRound()
    {
        _currentMatch = _tournamentManager.GetMatchNb();
        _currentRound = _tournamentManager.GetRoundNb();
    }

    public void PopUpButton()
    {
        List<S_TournamentManager.Participant> participants = _tournamentManager.GetParticipants();

        if (participants != null && _tournamentManager.IsEven())
        {
            S_TournamentManager.Participant p1 = participants[_currentMatch * 2]; //need to multiply so the previous participant does not fight again
            S_TournamentManager.Participant p2 = participants[(_currentMatch * 2) + 1];

            if (_tournamentManager.IsPlayerPlaying(p1, p2))
            {
                ActivateUI(_playerMatchButton, true);
                ActivateUI(_botMatchButtons, false);
            }
            else
            {
                ActivateUI(_botMatchButtons, true);
                ActivateUI(_playerMatchButton, false);
            }
        }
    }

    //Activate or deactivate a button
    public void ActivateUI(GameObject button, bool active)
    {
        button.SetActive(active);
    }

    public void PlayerLostScreen()
    {
        ActivateUI(_playerLose, true);
    }
}