using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class S_StreetFightManager : MonoBehaviour
{
    [Header("paricipants")]
    [SerializeField, Tooltip("the current player bot")]
    private GameObject _playerBot;
    [SerializeField, Tooltip("the current AI bot")]
    private GameObject _AIBot;

    [Header("UI")]
    [SerializeField, Tooltip("it is the 3, 2, 1 on begin of match")]
    private TextMeshPro _timerBeforeStart;
    [SerializeField, Tooltip("the parent of street fight ui")]
    private GameObject _UIStreetFight;

    [Header("Camera")]
    [SerializeField, Tooltip("camera for focus the view on two bots")]
    private S_CameraView _cameraView;

    [Header("Event")]
    [SerializeField]
    private UnityEvent _onStreetFightEnd;

    private S_AIController _AIController;
    private PlayerInput _playerInput;

    private void Start()
    {
        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_playerBot.transform);
        _cameraView.AddObjectToView(_AIBot.transform);

        _AIController = _AIBot.GetComponent<S_AIController>();
        _playerInput = _playerBot.GetComponent<PlayerInput>();
        SetEnableBots(false);
        BindDeadEventForBots();
    }

    /// <summary>
    /// set the enabled of controller component for player and ai
    /// </summary>
    /// <param name="enabled">enabled value</param>
    private void SetEnableBots(bool enabled)
    {
        _AIController.enabled = enabled;
        _playerInput.enabled = enabled;
    }
    private void BindDeadEventForBots()
    {
        var frameAI     = _AIBot.GetComponent<S_FrameManager>();
        var framePlayer = _playerBot.GetComponent<S_FrameManager>();

        frameAI.OnDie       += (_) => BotPlayerVictorie();
        framePlayer.OnDie   += (_) => BotAiVictorie();
    }
    private void BotAiVictorie()
    {

    }
    private void BotPlayerVictorie()
    {

    }

    public void StartStreetFight()
    {
        _UIStreetFight.SetActive(true);
        
        StartCoroutine(TimerBeforeStart(() =>
        {
            _UIStreetFight.SetActive(false);
            SetEnableBots(true);
        }));
    }

    private IEnumerator TimerBeforeStart(System.Action endTimer)
    {
        _timerBeforeStart.text = "3";
        yield return new WaitForSeconds(1f);

        _timerBeforeStart.text = "2";
        yield return new WaitForSeconds(1f);

        _timerBeforeStart.text = "1";
        yield return new WaitForSeconds(1f);

        _timerBeforeStart.text = "0";
        yield return new WaitForSeconds(1f);
        endTimer?.Invoke();
    }
}
