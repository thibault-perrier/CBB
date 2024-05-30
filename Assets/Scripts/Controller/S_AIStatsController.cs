
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public struct StatsBotRank
{
    [Header("Togles Actions")]
    [SerializeField, Tooltip("if he focus the enemy weapon")]
    private bool _attackEnemyWeapon;
    [SerializeField, Tooltip("if he use her best weapon for attack")]
    private bool _attackWithBestWeapon;
    [SerializeField, Tooltip("if he dodge the trap")]
    private bool _dodgeTrap;
    [SerializeField, Tooltip("if he flee the enemy when he cant attack")]
    private bool _canFleeEnemy;
    [SerializeField, Tooltip("if he can failed any attack")]
    private bool _canFailedAnyAttack;
    [SerializeField, Tooltip("if he can ignore the trap when we are enough near the target")]
    private bool _canIgnoreTrap;
    [SerializeField, Tooltip("if he can reverse her movement")]
    private bool _canReverseMovement;

    [Header("Probability Actions")]
    [SerializeField, Range(0, 100), Tooltip("probability to make an attack when he can do it")]
    private float _attackSuccesProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to make a movement every frame")]
    private float _movementProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to turn for make a dodge")]
    private float _dodgeProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to start the flee")]
    private float _fleeProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to fail an attack when he cant do any attack")]
    private float _attackFailedProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to reverse her movement")]
    private float _reverseMovementProbability;
    [SerializeField, Range(0, 100), Tooltip("probability to get enemy weapons for the current target")]
    private float _attackEnemyWeaponProbabiltiy;

    [Header("Cooldown")]
    [SerializeField, Min(0f), Tooltip("coolDown for the next attack")]
    private float _attackCooldown;
    [SerializeField, Min(0f), Tooltip("coolDown for the flee failure come back to None")]
    private float _fleeCooldown;

    [Header("Offset variable random")]
    [SerializeField, Tooltip("the offset of attack succes probability when he is gets, Y is Exclusive")]
    private Vector2 _attackSuccesProbabilityRandom;
    [SerializeField, Tooltip("the offset of movement probability when he is gets, Y is Exclusive")]
    private Vector2 _movementProbabilityRandom;
    [SerializeField, Tooltip("the offset of dodge probability when he is gets, Y is Exclusive")]
    private Vector2 _dodgeProbabilityRandom;
    [SerializeField, Tooltip("the offset of flee probability when he is gets, Y is Exclusive")]
    private Vector2 _fleeProbabilityRandom;
    [SerializeField, Tooltip("the offset of attack failed probability when he is gets, Y is Exclusive")]
    private Vector2 _attackFailedProbabilityRandom;
    [SerializeField, Tooltip("the offset of reverse movement probabiltiy when he is gets, Y is Exclusive")]
    private Vector2 _reverseMovementProbabilityRandom;
    [SerializeField, Tooltip("the offset of attack enemy probability when he is gets, Y is exclusive")]
    private Vector2 _attackEnemyWeaponProbabilityRandom;

    [Space(15)]
    [SerializeField, Tooltip("the offset of flee cooldown when he is gets, Y is exclusive")]
    private Vector2 _attackCooldownRandom;
    [SerializeField, Tooltip("the offset of attack cooldown when he is gets, Y is exclusive")]
    private Vector2 _fleeCooldownRandom;


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
    /// if he can failed any attack
    /// </summary>
    public bool CanFailedAnyAttack
    {
        get => _canFailedAnyAttack;
    }
    /// <summary>
    /// if he can ignore the trap when we are enough near the target
    /// </summary>
    public bool CanIgnoreTrap
    {
        get => _canIgnoreTrap;
    }
    /// <summary>
    /// if he can reverse her movement
    /// </summary>
    public bool CanReverseMovement
    {
        get => _canReverseMovement;
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
    /// Get attack failed probability with random offset
    /// </summary>
    public float AttackFailedProbabiltiy 
    { 
        get => Mathf.Clamp(_attackFailedProbability + Random.Range(_attackFailedProbabilityRandom.x, _attackFailedProbabilityRandom.y), 0f, 100f); 
    }
    /// <summary>
    /// probability to reverse her movement
    /// </summary>
    public float ReverseMovementProbability
    {
        get => Mathf.Clamp(_reverseMovementProbability + Random.Range(_reverseMovementProbabilityRandom.x, _reverseMovementProbabilityRandom.y), 0f, 100f);
    }
    /// <summary>
    /// the offset of attack enemy probability when he is gets
    /// </summary>
    public float AttackWeaponEnemyProbability
    {
        get => Mathf.Clamp(_attackEnemyWeaponProbabiltiy + Random.Range(_attackEnemyWeaponProbabilityRandom.x, _attackEnemyWeaponProbabilityRandom.y), 0f, 100f);
    }


    /// <summary>
    /// coolDown for the flee failure come back to None
    /// </summary>
    public float FleeCooldown 
    {
        get => Mathf.Clamp(_fleeCooldown + Random.Range(_fleeCooldownRandom.x, _fleeCooldownRandom.y), 0f, 100f);
    }
    /// <summary>
    /// coolDown for the next attack
    /// </summary>
    public float AttackCooldown
    {
        get => Mathf.Clamp(_attackCooldown + Random.Range(_attackCooldownRandom.x, _attackCooldownRandom.y), 0f, 100f);
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

    [SerializeField]
    private bool _updateStatsOnStart;
    [SerializeField]
    private BotRank _botRank = BotRank.Bronze;

    [Header("Bot Ranks")]
    [SerializeField, Tooltip("the current statistique used for tutorial bot")]
    private StatsBotRank _statsBotTutorialRank;
    [SerializeField, Tooltip("the current statistique used for bronze ranked bot")]
    private StatsBotRank _statsBotBronzeRank;
    [SerializeField, Tooltip("the current statistique used for sliver ranked bot")]
    private StatsBotRank _statsBotSliverRank;
    [SerializeField, Tooltip("the current statistique used for gold ranked bot")]
    private StatsBotRank _statsBotGoldRank;
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
    /// set the bot statistique
    /// </summary>
    /// <param name="stats">the statistique at set</param>
    private void SetBotStatistique(StatsBotRank stats)
    {
        // set the toggles actions
        _aiController.AttackEnemyWeapon = stats.AttackEnemyWeapon;
        _aiController.CanFleeEnemy = stats.CanFleeEnemy;
        _aiController.CanFailedAnyAttack = stats.CanFailedAnyAttack;
        _aiController.CanIgnoreTrap = stats.CanIgnoreTrap;
        _aiController.AttackWithBestWeapon = stats.AttackWithBestWeapon;
        _aiController.DodgeTrap = stats.CanDodgeTrap;
        _aiController.CanReverseMovement = stats.CanReverseMovement;

        // set the actions probability
        _aiController.AttackFailedProbability = stats.AttackFailedProbabiltiy;
        _aiController.AttackSuccesProbability = stats.AttackSuccesProbability;
        _aiController.DodgeProbability = stats.DodgeProbability;
        _aiController.FleeProbability = stats.FleeProbability;
        _aiController.MovementProbability = stats.MovementProbability;
        _aiController.ReverseMovementProbability = stats.ReverseMovementProbability;
        _aiController.AttackEnemyWeaponProbability = stats.AttackWeaponEnemyProbability;

        // set the cooldown
        _aiController.AttackCooldown = stats.AttackCooldown;
        _aiController.FleeCooldown = stats.FleeCooldown;
    }
    /// <summary>
    /// Set the all bot statistique randomly
    /// </summary>
    private void SetBotStatistiqueRandom()
    {
        int AiRank = Random.Range(1, 5);
        BotDifficulty = (BotRank)AiRank;
    }
}
