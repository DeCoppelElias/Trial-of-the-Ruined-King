using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain.Ports;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    [SerializeField] private float _timeBetweenAttacks = 3.0f;
    [SerializeField] private float _attackTimingMultiplier = 1f;

    [SerializeField] private float _initialDelay = 2.0f;
    [SerializeField] private int _minSimultaneousAttacks = 2;
    [SerializeField] private int _maxSimultaneousAttacks = 4;
    [SerializeField] private float _minAttackOffset = 0.1f;
    [SerializeField] private float _maxAttackOffset = 0.5f;
    [SerializeField] private List<AttackDefinitionBase> _attackDefinitions;
    [SerializeField, Range(0.5f, 1.0f)] private float _minFreePercentage = 0.5f;

    [SerializeField] private float _difficultyRampDuration = 120f; 
    [SerializeField] private float _minTimeBetweenAttacks = 0.7f; 
    [SerializeField] private float _minAttackTimingMultipier = 0.3f;

    [Header("Attack Sequences")]
    [SerializeField, Tooltip("List of pre-defined attack sequences")]
    private List<AttackSequenceDefinition> _attackSequences;
    
    [SerializeField, Range(0f, 1f), Tooltip("Probability of spawning random attacks (0 = only sequences, 1 = only random)")]
    private float _randomAttackWeight = 0.7f;

    private AttackService _attackService;
    private IArena _arena;
    private ITimeService _timeService;
    private float _timeSinceLastAttack;
    private bool _initialized;
    private bool _isSpawning;
    private Coroutine _currentAttackWave;

    private HashSet<GridPosition> _attackedTiles;

    private float _baseAttackTimingMultiplier;
    private float _baseTimeBetweenAttacks;
    private float _difficultyElapsed;

    private AttackFactoryRegistry _attackFactory;

    public void Initialize(AttackService attackService, IArena arena, ITimeService timeService, IGameStateService gameStateService)
    {
        _attackService = attackService;
        _arena = arena;
        _timeService = timeService;
        _initialized = true;
        _isSpawning = false;

        _attackedTiles = new HashSet<GridPosition>();

        _timeService.OnTick += OnTimeTick;
        _attackService.OnAttackAdded += OnAttackAdded;
        _attackService.OnAttackCompleted += OnAttackCompleted;
        gameStateService.OnStateChanged += OnGameStateChanged;

        _baseTimeBetweenAttacks = _timeBetweenAttacks;
        _baseAttackTimingMultiplier = _attackTimingMultiplier;

        InitializeAttackFactories();
        ValidateAttackSequences();
    }

    private void InitializeAttackFactories()
    {
        _attackFactory = new AttackFactoryRegistry();
        _attackFactory.Register(typeof(SpearRowAttack), new SpearRowAttackFactory(_arena));
        _attackFactory.Register(typeof(SpearColumnAttack), new SpearColumnAttackFactory(_arena));
        _attackFactory.Register(typeof(HammerAttack), new HammerAttackFactory(_arena));
    }

    private void ValidateAttackSequences()
    {
        if (_attackSequences == null || _attackSequences.Count == 0)
            return;

        for (int i = _attackSequences.Count - 1; i >= 0; i--)
        {
            var sequence = _attackSequences[i];
            if (sequence == null)
            {
                Debug.LogWarning($"Attack sequence at index {i} is null, removing from list");
                _attackSequences.RemoveAt(i);
                continue;
            }

            if (!sequence.Validate(_arena, out string errorMessage))
            {
                Debug.LogError($"Attack sequence '{sequence.sequenceName}' is invalid: {errorMessage}. Removing from list.");
                _attackSequences.RemoveAt(i);
            }
        }
    }

    public void StartSpawning()
    {
        _isSpawning = true;
        _timeSinceLastAttack = -_initialDelay;
        _difficultyElapsed = 0f;

        _timeBetweenAttacks = _baseTimeBetweenAttacks;
        _attackTimingMultiplier = _baseAttackTimingMultiplier;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        
        if (_currentAttackWave != null)
        {
            StopCoroutine(_currentAttackWave);
            _currentAttackWave = null;
        }
    }

    private void OnDestroy()
    {
        if (_timeService != null)
        {
            _timeService.OnTick -= OnTimeTick;
        }

        if (_attackService != null)
        {
            _attackService.OnAttackAdded -= OnAttackAdded;
            _attackService.OnAttackCompleted -= OnAttackCompleted;
        }

        if (_currentAttackWave != null)
        {
            StopCoroutine(_currentAttackWave);
        }
    }

    private void OnAttackAdded(Attack attack)
    {
        foreach (var pos in GetAttackedTiles(attack))
        {
            _attackedTiles.Add(pos);
        }
    }

    private void OnAttackCompleted(Attack attack)
    {
        foreach (var pos in GetAttackedTiles(attack))
        {
            _attackedTiles.Remove(pos);
        }
    }

    private IEnumerable<GridPosition> GetAttackedTiles(Attack attack)
    {
        foreach (var pos in attack.Pattern.GetAffectedArea())
            yield return pos;
    }

    private void OnTimeTick(float deltaTime)
    {
        if (!_initialized || !_isSpawning) return;

        // Update difficulty progression
        _difficultyElapsed += deltaTime;
        float t = Mathf.Clamp01(_difficultyElapsed / _difficultyRampDuration);

        // Lerp time between attacks
        _timeBetweenAttacks = Mathf.Lerp(_baseTimeBetweenAttacks, _minTimeBetweenAttacks, t);
        _attackTimingMultiplier = Mathf.Lerp(_baseAttackTimingMultiplier, _minAttackTimingMultipier, t);

        _timeSinceLastAttack += deltaTime;

        if (_timeSinceLastAttack >= _timeBetweenAttacks)
        {
            if (_currentAttackWave != null)
            {
                StopCoroutine(_currentAttackWave);
            }
            _currentAttackWave = StartCoroutine(SpawnAttackWave());
            _timeSinceLastAttack = 0f;
        }
    }

    private IEnumerator SpawnAttackWave()
    {
        // Decide whether to spawn random attacks or a sequence
        bool shouldSpawnSequence = ShouldSpawnSequence();

        if (shouldSpawnSequence)
        {
            yield return StartCoroutine(SpawnAttackSequence());
        }
        else
        {
            yield return StartCoroutine(SpawnMultipleAttacksWithDelay());
        }

        _currentAttackWave = null;
    }

    private bool ShouldSpawnSequence()
    {
        // If no sequences available, always spawn random
        if (_attackSequences == null || _attackSequences.Count == 0)
            return false;

        // Use weighted random to decide
        float roll = Random.value;
        return roll > _randomAttackWeight; // Higher weight = more random, lower chance of sequence
    }

    private IEnumerator SpawnAttackSequence()
    {
        if (_attackSequences == null || _attackSequences.Count == 0)
        {
            yield break;
        }

        // Select random sequence
        var sequence = _attackSequences[Random.Range(0, _attackSequences.Count)];
        if (sequence == null || sequence.steps == null || sequence.steps.Count == 0)
        {
            yield break;
        }

        // Spawn each step with timing
        float sequenceStartTime = Time.time;
        int stepIndex = 0;

        foreach (var step in sequence.steps)
        {
            // Wait until it's time for this step
            float targetTime = sequenceStartTime + (step.delayFromStart * _attackTimingMultiplier);
            while (Time.time < targetTime)
            {
                yield return null;
            }

            // Spawn the attack
            SpawnSequenceAttack(step, sequence);
            stepIndex++;
        }
    }

    private void SpawnSequenceAttack(AttackSequenceStep step, AttackSequenceDefinition sequence)
    {
        // Determine timing: use step override or sequence base timing
        AttackTiming timing = step.overrideTiming ? step.timingOverride : sequence.baseAttackTiming;
        
        // Apply difficulty multiplier
        timing = timing.Multiply(_attackTimingMultiplier);

        // Create attack using factory
        var attack = _attackFactory.CreateSpecificAttackByType(step.attackType, step.parameters, timing);

        if (attack == null)
        {
            Debug.LogWarning($"Failed to create attack of type {step.attackType} in sequence '{sequence.sequenceName}'");
            return;
        }

        _attackService.SpawnAttack(attack);
    }

    private IEnumerator SpawnMultipleAttacksWithDelay()
    {
        int attackCount = Random.Range(_minSimultaneousAttacks, _maxSimultaneousAttacks + 1);

        for (int i = 0; i < attackCount; i++)
        {
            SpawnRandomAttack();

            if (i < attackCount - 1)
            {
                float delay = Random.Range(_minAttackOffset, _maxAttackOffset);
                yield return new WaitForSeconds(delay);
            }
        }
    }

    private void SpawnRandomAttack()
    {
        if (_attackDefinitions == null || _attackDefinitions.Count == 0)
            return;

        var definition = _attackDefinitions[Random.Range(0, _attackDefinitions.Count)];
        var attack = _attackFactory.CreateRandomAttack(definition, _attackTimingMultiplier, _attackedTiles, _minFreePercentage);

        if (attack == null)
            return;

        _attackService.SpawnAttack(attack);
    }

    private void OnGameStateChanged(GameState from, GameState to)
    {
        if (to == GameState.Gameplay)
        {
            StartSpawning();
        }
        else if (to == GameState.MainMenu)
        {
            StopSpawning();
        }
    }
}
