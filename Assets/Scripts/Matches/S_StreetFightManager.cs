using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class S_StreetFightManager : MonoBehaviour
{
    [Header("paricipants")]
    [SerializeField, Tooltip("the current player bot")]
    private GameObject _playerBot;
    [SerializeField, Tooltip("the current AI bot")]
    private GameObject _AIBot;

    [Header("UI")]
    [SerializeField, Tooltip("the parent of street fight ui")]
    private GameObject _UIStreetFight;

    [Space(10)]
    [SerializeField, Tooltip("it is the 3, 2, 1 on begin of match")]
    private Image _timerBeforeStart;
    [SerializeField, Tooltip("the number for the timer in the begin match")]
    private Sprite[] _numberForTimer;

    [Header("Camera")]
    [SerializeField, Tooltip("camera for focus the view on two bots")]
    private S_CameraView _cameraView;

    [Header("Event")]
    [SerializeField, Tooltip("invoke when player or AI die")]
    private UnityEvent _onStreetFightEnd;

    private S_AIController _AIController;
    private PlayerInput _playerInput;
    private S_ImmobileDefeat _immobilePlayer, _immobileAI;

    private void Start()
    {
        _cameraView.ClearObjectToView();
        _cameraView.AddObjectToView(_playerBot.transform);
        _cameraView.AddObjectToView(_AIBot.transform);

        _cameraView.gameObject.SetActive(false);
        _UIStreetFight.SetActive(false);

        _AIController = _AIBot.GetComponent<S_AIController>();
        _playerInput = _playerBot.GetComponent<PlayerInput>();
        BindDeadEventForBots();
        SetEnableBots(false);
    }

    /// <summary>
    /// set the enabled of controller component for player and ai
    /// </summary>
    /// <param name="enabled">enabled value</param>
    private void SetEnableBots(bool enabled)
    {
        _AIController.enabled = enabled;
        _playerInput.enabled = enabled;

        _immobileAI.enabled = enabled;
        _immobilePlayer.enabled = enabled;
    }
    private void BindDeadEventForBots()
    {
        var frameAI     = _AIBot.GetComponent<S_FrameManager>();
        var framePlayer = _playerBot.GetComponent<S_FrameManager>();

        frameAI.OnDie       += (_) => BotPlayerVictorie();
        framePlayer.OnDie   += (_) => BotAiVictorie();

        _immobileAI     = _AIBot.GetComponent<S_ImmobileDefeat>();
        _immobilePlayer = _playerBot.GetComponent<S_ImmobileDefeat>();

        _immobileAI.IsImmobile       += () => BotPlayerVictorie();
        _immobilePlayer.IsImmobile   += () => BotAiVictorie();
    }
    private void BotAiVictorie()
    {
        Debug.Log("AI victory");
        SetEnableBots(false);
        _onStreetFightEnd?.Invoke();
    }
    private void BotPlayerVictorie()
    {
        Debug.Log("Player Victory");
        SetEnableBots(false);
        _onStreetFightEnd?.Invoke();
    }
    [ContextMenu("Start fight")]
    public void StartStreetFight()
    {
        _UIStreetFight.SetActive(true);
        _cameraView.gameObject.SetActive(true);

        StartCoroutine(TimerBeforeStart(() =>
        {
            _UIStreetFight.SetActive(false);
            SetEnableBots(true);
        }));
    }

    private IEnumerator TimerBeforeStart(System.Action endTimer)
    {
        foreach (var number in _numberForTimer)
        {
            _timerBeforeStart.sprite = number;
            yield return new WaitForSeconds(1f);
        }

        endTimer?.Invoke();
    }
}
