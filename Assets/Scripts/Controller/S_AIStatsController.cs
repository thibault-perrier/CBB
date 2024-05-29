
using UnityEngine;

[System.Serializable]
public struct StatsBotRank
{
    [Header("Togles Actions")]
    [Tooltip("if he focus the enemy weapon")]
    public bool _attackEnemyWeapon;
    [Tooltip("if he use her best weapon for attack")]
    public bool _attackWithBestWeapon;
    [Tooltip("if he dodge the trap")]
    public bool _dodgeTrap;
    [Tooltip("if he flee the enemy when he cant attack")]
    public bool _canFleeEnemy;
    [Tooltip("if he can failed any attack")]
    public bool _canFailedAnyAttack;
    [Tooltip("if he can ignore the trap when we are enough near the target")]
    public bool _canIgnoreTrap;

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

    [Header("Offset variable random")]
    [SerializeField, Tooltip("the offset of attack succes probability when he is get")]
    private Vector2 _attackSuccesProbabilityRandom;
    [SerializeField, Tooltip("the offset of movement probability when he is get")]
    private Vector2 _movementProbabilityRandom;
    [SerializeField, Tooltip("the offset of dodge probability when he is get")]
    private Vector2 _dodgeProbabilityRandom;
    [SerializeField, Tooltip("the offset of flee probability when he is get")]
    private Vector2 _fleeProbabilityRandom;
    [SerializeField, Tooltip("the offset of attack failed probability when he is get")]
    private Vector2 _attackFailedProbabilityRandom;


    /// <summary>
    /// Get attack succes probability with random offset value
    /// </summary>
    public float AttackSuccesProbability 
    {
        get => Mathf.Clamp(_attackSuccesProbability + Random.Range(_attackSuccesProbabilityRandom.x, _attackSuccesProbabilityRandom.y), 0f, 100f);
        private set => _attackSuccesProbability = value; 
    }
    /// <summary>
    /// Get movement probability with random offset
    /// </summary>
    public float MovementProbability 
    { 
        get => Mathf.Clamp(_movementProbability + Random.Range(_movementProbabilityRandom.x, _movementProbabilityRandom.y), 0f, 100f); 
        private set => _movementProbability = value; 
    }
    /// <summary>
    /// Get dodge probability with random offset
    /// </summary>
    public float DodgeProbability
    { 
        get => Mathf.Clamp(_dodgeProbability + Random.Range(_dodgeProbabilityRandom.x, _dodgeProbabilityRandom.y), 0f, 100f); 
        private set => _dodgeProbability = value; 
    }
    /// <summary>
    /// Get flee probability with random offset
    /// </summary>
    public float FleeProbability 
    { 
        get => Mathf.Clamp(_fleeProbability + Random.Range(_fleeProbabilityRandom.x, _fleeProbabilityRandom.y), 0f, 100f); 
        private set=> _fleeProbability = value; 
    }
    /// <summary>
    /// Get attack failed probability with random offset
    /// </summary>
    public float AttackFailedProbabiltiy 
    { 
        get => Mathf.Clamp(_attackFailedProbability + Random.Range(_attackFailedProbabilityRandom.x, _attackFailedProbabilityRandom.y), 0f, 100f); 
        private set => _attackFailedProbability = value; 
    }
}

[RequireComponent (typeof(S_AIController))]
public class S_AIStatsController : MonoBehaviour
{
    public enum BotRank
    {
        Bronze,
        Sliver,
        Gold,
        Diamond
    }

    [Header("Bot Ranks")]
    [SerializeField, Tooltip("the current statistique for bronze ranked bot")]
    private StatsBotRank _statsBotBronzeRank;
    [SerializeField, Tooltip("the current statistique for sliver ranked bot")]
    private StatsBotRank _statsBotSliverRank;
    [SerializeField, Tooltip("the current statistique for gold ranked bot")]
    private StatsBotRank _statsBotGoldRank;
    [SerializeField, Tooltip("the current statistique for diamond ranked bot")]
    private StatsBotRank _statsBotDiamondRank;

    private BotRank _botRank = BotRank.Bronze;
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
    }

    /// <summary>
    /// Set bot statistique from bit difficulty
    /// </summary>
    /// <param name="rank">the current difficulty</param>
    public void SetBotRank(BotRank rank)
    {
        // select the stats to set in the current bot
        switch (_botRank)
        {
            case BotRank.Bronze:    SetBotStatistique(_statsBotBronzeRank);     break;
            case BotRank.Sliver:    SetBotStatistique(_statsBotSliverRank);     break;
            case BotRank.Gold:      SetBotStatistique(_statsBotGoldRank);       break;
            case BotRank.Diamond:   SetBotStatistique(_statsBotDiamondRank);    break;
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
        _aiController.AttackEnemyWeapon = stats._attackEnemyWeapon;
        _aiController.CanFleeEnemy = stats._canFleeEnemy;
        _aiController.CanFailedAnyAttack = stats._canFailedAnyAttack;
        _aiController.CanIgnoreTrap = stats._canIgnoreTrap;
        _aiController.AttackWithBestWeapon = stats._attackWithBestWeapon;
        _aiController.DodgeTrap = stats._dodgeTrap;

        // set the actions probability
        _aiController.AttackFailedProbability = stats.AttackFailedProbabiltiy;
        _aiController.AttackSuccesProbability = stats.AttackSuccesProbability;
        _aiController.DodgeProbability = stats.DodgeProbability;
        _aiController.FleeProbability = stats.FleeProbability;
        _aiController.MovementProbability = stats.MovementProbability;
    }
}
