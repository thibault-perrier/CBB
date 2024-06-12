
using UnityEngine;

[System.Serializable]
public struct StatsBotRank
{
    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the enemy weapon")]
    private bool _attackEnemyWeapon;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;
    [SerializeField, Tooltip("if he can fail any attack with probability")]
    private bool _canFailAnyAttack;

    [Space(15)]
    [SerializeField, Tooltip("if he dodge the trap with probability")]
    private bool _dodgeTrap;
    [SerializeField, Tooltip("if he flee the enemy when he cant attack")]
    private bool _canFleeEnemy;
    [SerializeField, Tooltip("if he can ignore the trap when we are enough near the target")]
    private bool _canIgnoreTrap;
    [SerializeField, Tooltip("if he can flee the enmy whith trap")]
    private bool _canFleeWithTrap;

    [Space(15)]
    [SerializeField, Tooltip("if he can make an accidental movement with probability")]
    private bool _canMakeAccidentalMovement;
    [SerializeField, Tooltip("if he can make an accidental direction with probability")]
    private bool _canMakeAccidentalDirection;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100), Tooltip("probability to make an attack when he can do it")]
    private float _attackSuccesProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to get enemy weapons for the current target")]
    private float _attackEnemyWeaponProbabiltiy;
    [SerializeField, Range(0, 100), Tooltip("probability to fail an attack when he cant do any attack")]
    private float _attackFailProbability;

    [Space(15)]
    [SerializeField, Range(0, 100), Tooltip("probability to make a movement every frame")]
    private float _movementProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to turn for make a dodge")]
    private float _dodgeProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to start the flee")]
    private float _fleeProbability;

    [Space(15)]
    [SerializeField, Range(0, 100), Tooltip("probability to make an accidental movement")]
    private float _accidentalMovementProbability;
    [SerializeField, Range(0, 100), Tooltip("probabiltiy to make an accidental direction")]
    private float _accidentalDirectionProbability;

    [Header("Cooldown")]
    [SerializeField, Min(0f), Tooltip("coolDown for the flee failure come back to None")]
    private float _fleeCooldown;
    [SerializeField, Min(0f), Tooltip("cooldown for try to fail any attack")]
    private float _attackFailCooldown;

    [Header("Distance")]
    [SerializeField, Min(0f), Tooltip("min distance for fail an attack")]
    private float _attackFailDistance;

    [Header("Offset variable random")]
    [SerializeField, Tooltip("the offset of attack succes probability when he is gets, Y is Exclusive")]
    private Vector2 _attackSuccesProbabilityRandom;
    [SerializeField, Tooltip("the offset of attack enemy probability when he is gets, Y is exclusive")]
    private Vector2 _attackEnemyWeaponProbabilityRandom;
    [SerializeField, Tooltip("the offset of attack failed probability when he is gets, Y is Exclusive")]
    private Vector2 _attackFailProbabilityRandom;

    [Space(15)]
    [SerializeField, Tooltip("the offset of movement probability when he is gets, Y is Exclusive")]
    private Vector2 _movementProbabilityRandom;
    [SerializeField, Tooltip("the offset of dodge probability when he is gets, Y is Exclusive")]
    private Vector2 _dodgeProbabilityRandom;
    [SerializeField, Tooltip("the offset of flee probability when he is gets, Y is Exclusive")]
    private Vector2 _fleeProbabilityRandom;
    [SerializeField, Tooltip("the offset of accidental movement probabiltiy when he is gets, Y is Exclusive")]
    private Vector2 _accidentalMovementProbabilityRandom;
    [SerializeField, Tooltip("the offset of accidental direction when he is gets, Y is exlusive")]
    private Vector2 _accidentalDirectionProbabilityRandom;

    [Space(15)]
    [SerializeField, Tooltip("the offset of attack cooldown when he is gets, Y is exclusive")]
    private Vector2 _fleeCooldownRandom;
    [SerializeField, Tooltip("the offset of attack failed cooldown when he is gets, Y is exclusive")]
    private Vector2 _attackFailCooldownRandom;

    /// <summary>
    /// if he focus the enemy weapon
    /// </summary>
    public bool AttackEnemyWeapon
    {
        get => _attackEnemyWeapon;
    }
    /// <summary>
    /// if he use her best weapon for attack
    /// </summary>
    public bool AttackWithBestWeapon 
    { 
        get => _attackWithBestWeapon; 
    }
    /// <summary>
    /// if he dodge the trap
    /// </summary>
    public bool CanDodgeTrap
    {
        get => _dodgeTrap;
    }
    /// <summary>
    /// if he flee the enemy when he cant attack
    /// </summary>
    public bool CanFleeEnemy
    {
        get => _canFleeEnemy;
    }
    /// <summary>
    /// if he can fail any attack
    /// </summary>
    public bool CanFailAnyAttack
    {
        get => _canFailAnyAttack;
    }
    /// <summary>
    /// if he can ignore the trap when we are enough near the target
    /// </summary>
    public bool CanIgnoreTrap
    {
        get => _canIgnoreTrap;
    }
    /// <summary>
    /// if he can make an accidental movement with probability
    /// </summary>
    public bool CanMakeAccidentalMovement
    {
        get => _canMakeAccidentalMovement;
    }
    /// <summary>
    /// if he can make an accidental direction with probability
    /// </summary>
    public bool CanMakeAccidentalDirection
    {
        get => _canMakeAccidentalDirection;
    }
    /// <summary>
    /// if he can flee the enmy whith trap
    /// </summary>
    public bool CanFleeEnemyWithTrap
    {
        get => _canFleeWithTrap;
    }

    /// <summary>
    /// Get attack succes probability with random offset value
    /// </summary>
    public float AttackSuccesProbability 
    {
        get => Mathf.Clamp(_attackSuccesProbability + Random.Range(_attackSuccesProbabilityRandom.x, _attackSuccesProbabilityRandom.y), 0f, 100f);
    }
    /// <summary>
    /// Get movement probability with random offset
    /// </summary>
    public float MovementProbability 
    { 
        get => Mathf.Clamp(_movementProbability + Random.Range(_movementProbabilityRandom.x, _movementProbabilityRandom.y), 0f, 100f); 
    }
    /// <summary>
    /// Get dodge probability with random offset
    /// </summary>
    public float DodgeProbability
    { 
        get => Mathf.Clamp(_dodgeProbability + Random.Range(_dodgeProbabilityRandom.x, _dodgeProbabilityRandom.y), 0f, 100f); 
    }
    /// <summary>
    /// Get flee probability with random offset
    /// </summary>
    public float FleeProbability 
    { 
        get => Mathf.Clamp(_fleeProbability + Random.Range(_fleeProbabilityRandom.x, _fleeProbabilityRandom.y), 0f, 100f); 
    }
    /// <summary>
    /// Get attack fail probability with random offset
    /// </summary>
    public float AttackFailProbabiltiy 
    { 
        get => Mathf.Clamp(_attackFailProbability + Random.Range(_attackFailProbabilityRandom.x, _attackFailProbabilityRandom.y), 0f, 100f); 
    }
    /// <summary>
    /// probability to reverse her movement
    /// </summary>
    public float AccidentalMovementProbability
    {
        get => Mathf.Clamp(_accidentalMovementProbability + Random.Range(_accidentalMovementProbabilityRandom.x, _accidentalMovementProbabilityRandom.y), 0f, 100f);
    }
    /// <summary>
    /// the offset of attack enemy probability when he is gets
    /// </summary>
    public float AttackWeaponEnemyProbability
    {
        get => Mathf.Clamp(_attackEnemyWeaponProbabiltiy + Random.Range(_attackEnemyWeaponProbabilityRandom.x, _attackEnemyWeaponProbabilityRandom.y), 0f, 100f);
    }
    /// <summary>
    /// probabiltiy to reverse her direction
    /// </summary>
    public float AccidentalDirectionProbability
    {
        get => Mathf.Clamp(_accidentalDirectionProbability + Random.Range(_accidentalDirectionProbabilityRandom.x, _accidentalDirectionProbabilityRandom.y), 0f, 100f);
    }

    /// <summary>
    /// coolDown for the flee failure come back to None
    /// </summary>
    public float FleeCooldown 
    {
        get => Mathf.Max(_fleeCooldown + Random.Range(_fleeCooldownRandom.x, _fleeCooldownRandom.y), 0f);
    }
    /// <summary>
    /// cooldown for try to fail any attack
    /// </summary>
    public float AttackFailCooldown
    {
        get => Mathf.Max(_attackFailCooldown + Random.Range(_attackFailCooldownRandom.x, _attackFailCooldownRandom.y), 0f);
    }

    /// <summary>
    /// min distance for fail an attack
    /// </summary>
    public float AttackFailDistance
    {
        get => _attackFailDistance;
    }
}

[RequireComponent (typeof(S_AIController))]
public class S_AIStatsController : MonoBehaviour
{
    public enum BotRank
    {
        Tutorial,
        Bronze,
        Sliver,
        Gold,
        Diamond,
        Randomly
    }

    [SerializeField, Tooltip("if the statistique are set in the start")]
    private bool _updateStatsOnStart;
    [SerializeField, Tooltip("current rank for the bot")]
    private BotRank _botRank = BotRank.Bronze;

    [Header("Bot Ranks")]
    [ContextMenuItem("set in all components", nameof(SetTutorialRankInAllComponents))]
    [SerializeField, Tooltip("the current statistique used for tutorial bot")]
    private StatsBotRank _statsBotTutorialRank;

    [ContextMenuItem("set in all components", nameof(SetBronzeRankInAllComponents))]
    [SerializeField, Tooltip("the current statistique used for bronze ranked bot")]
    private StatsBotRank _statsBotBronzeRank;

    [ContextMenuItem("set in all components", nameof(SetSliverRankInAllComponents))]
    [SerializeField, Tooltip("the current statistique used for sliver ranked bot")]
    private StatsBotRank _statsBotSliverRank;

    [ContextMenuItem("set in all components", nameof(SetGoldRankInAllComponents))]
    [SerializeField, Tooltip("the current statistique used for gold ranked bot")]
    private StatsBotRank _statsBotGoldRank;

    [ContextMenuItem("set in all components", nameof(SetDiamondRankInAllComponents))]
    [SerializeField, Tooltip("the current statistique used for diamond ranked bot")]
    private StatsBotRank _statsBotDiamondRank;

    private S_AIController _aiController;

    /// <summary>
    /// Difficulty of the current Bot for attack and movement
    /// </summary>
    public BotRank BotDifficulty
    {
        get => _botRank;
        set => SetBotRank(value);
    }

    /// <summary>
    /// the current statistique used for tutorial bot
    /// </summary>
    public StatsBotRank StatsBotTutorialRank
    {
        get => _statsBotTutorialRank;
        set => _statsBotTutorialRank = value;
    }
    /// <summary>
    /// the current statistique used for bronze ranked bot
    /// </summary>
    public StatsBotRank StatsBotBronzeRank
    {
        get => _statsBotBronzeRank;
        set => _statsBotBronzeRank = value;
    }
    /// <summary>
    /// the current statistique used for sliver ranked bot
    /// </summary>
    public StatsBotRank StatsBotSliverRank
    {
        get => _statsBotSliverRank;
        set => _statsBotSliverRank = value;
    }
    /// <summary>
    /// the current statistique used for gold ranked bot
    /// </summary>
    public StatsBotRank StatsBotGoldRank
    {
        get => _statsBotGoldRank; 
        set => _statsBotGoldRank = value;
    }
    /// <summary>
    /// the current statistique used for diamond ranked bot
    /// </summary>
    public StatsBotRank StatsBotDiamondRank
    {
        get => _statsBotDiamondRank; 
        set => _statsBotDiamondRank = value;
    }

    private void Start()
    {
        _aiController = GetComponent<S_AIController>();

        if (_updateStatsOnStart)
            SetBotRank(BotDifficulty);
    }

    /// <summary>
    /// Set bot statistique from bit difficulty
    /// </summary>
    /// <param name="rank">the current difficulty</param>
    public void SetBotRank(BotRank rank)
    {
        _botRank = rank;

        // select the stats to set in the current bot
        switch (_botRank)
        {
            case BotRank.Tutorial:  SetBotStatistique(_statsBotTutorialRank);   break;
            case BotRank.Bronze:    SetBotStatistique(_statsBotBronzeRank);     break;
            case BotRank.Sliver:    SetBotStatistique(_statsBotSliverRank);     break;
            case BotRank.Gold:      SetBotStatistique(_statsBotGoldRank);       break;
            case BotRank.Diamond:   SetBotStatistique(_statsBotDiamondRank);    break;
            case BotRank.Randomly:  SetBotStatistiqueRandom();                  break;
            default: break;
        }
    }
    /// <summary>
    /// set the bot statistique for toggles actions, Probability action and cooldown actions
    /// </summary>
    /// <param name="stats">the statistique at set</param>
    private void SetBotStatistique(StatsBotRank stats)
    {
        if (!_aiController)
            _aiController = GetComponent<S_AIController>();

        // set the toggles actions
        _aiController.AttackEnemyWeapon = stats.AttackEnemyWeapon;
        _aiController.CanFleeEnemy = stats.CanFleeEnemy;
        _aiController.CanFailAnyAttack = stats.CanFailAnyAttack;
        _aiController.CanIgnoreTrap = stats.CanIgnoreTrap;
        _aiController.AttackWithBestWeapon = stats.AttackWithBestWeapon;
        _aiController.DodgeTrap = stats.CanDodgeTrap;
        _aiController.CanMakeAccidentalMovement = stats.CanMakeAccidentalMovement;
        _aiController.CanMakeAccidentalDirection = stats.CanMakeAccidentalDirection;
        _aiController.CanFleeEnemyWithTrap = stats.CanFleeEnemyWithTrap;

        // set the actions probability
        _aiController.AttackFailProbability = stats.AttackFailProbabiltiy;
        _aiController.AttackSuccesProbability = stats.AttackSuccesProbability;
        _aiController.DodgeProbability = stats.DodgeProbability;
        _aiController.FleeProbability = stats.FleeProbability;
        _aiController.MovementProbability = stats.MovementProbability;
        _aiController.AccidentalMovementProbability = stats.AccidentalMovementProbability;
        _aiController.AttackEnemyWeaponProbability = stats.AttackWeaponEnemyProbability;
        _aiController.AccidentalDirectionProbability = stats.AccidentalDirectionProbability;

        // set the cooldown
        _aiController.FleeCooldown = stats.FleeCooldown;
        _aiController.AttackFailCooldown = stats.AttackFailCooldown;

        // set the distance
        _aiController.AttackFailDistance = stats.AttackFailDistance;
    }
    /// <summary>
    /// Set the all bot statistique randomly
    /// </summary>
    private void SetBotStatistiqueRandom()
    {
        int AiRank = Random.Range(1, 5);
        BotDifficulty = (BotRank)AiRank;
    }

    [ContextMenu("Set all rank in all components")]
    private void SetAllRankInAllComponents()
    {
        SetTutorialRankInAllComponents();
        SetBronzeRankInAllComponents();
        SetSliverRankInAllComponents();
        SetGoldRankInAllComponents();
        SetDiamondRankInAllComponents();
    }
    /// <summary>
    /// Set tutorial stats in all compoents in scene
    /// </summary>
    private void SetTutorialRankInAllComponents()
    {
        var components = FindObjectsOfType(typeof(S_AIStatsController)) as S_AIStatsController[];

        foreach (var item in components)
            item.StatsBotTutorialRank = _statsBotTutorialRank;
    }
    /// <summary>
    /// Set bronze stats in all compoents in scene
    /// </summary>
    private void SetBronzeRankInAllComponents()
    {
        var components = FindObjectsOfType(typeof(S_AIStatsController)) as S_AIStatsController[];

        foreach (var item in components)
            item.StatsBotBronzeRank = _statsBotBronzeRank;
    }
    /// <summary>
    /// Set sliver stats in all compoents in scene
    /// </summary>
    private void SetSliverRankInAllComponents()
    {
        var components = FindObjectsOfType(typeof(S_AIStatsController)) as S_AIStatsController[];

        foreach (var item in components)
            item.StatsBotSliverRank = _statsBotSliverRank;
    }
    /// <summary>
    /// Set gold stats in all compoents in scene
    /// </summary>
    private void SetGoldRankInAllComponents()
    {
        var components = FindObjectsOfType(typeof(S_AIStatsController)) as S_AIStatsController[];

        foreach (var item in components)
            item.StatsBotGoldRank = _statsBotGoldRank;
    }
    /// <summary>
    /// Set diamond stats in all compoents in scene
    /// </summary>
    private void SetDiamondRankInAllComponents()
    {
        var components = FindObjectsOfType(typeof(S_AIStatsController)) as S_AIStatsController[];

        foreach (var item in components)
            item.StatsBotDiamondRank = _statsBotDiamondRank;
    }
}
