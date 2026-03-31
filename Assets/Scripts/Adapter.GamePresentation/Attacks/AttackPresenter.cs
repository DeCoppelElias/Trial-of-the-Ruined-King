using Domain.Ports;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackPresenter : MonoBehaviour
{
    [Header("Visual Presentation")]
    [SerializeField] private ArenaShaker _arenaShaker;
    [SerializeField] private TileVisualizer _tileVisualizer;

    [Header("Audio")]
    [SerializeField] private AudioPlayer _audioPlayer;
    [SerializeField] private AudioEvent _commitAudioEvent;
    
    [Header("Attacks")]
    [SerializeField] private List<AttackDefinitionBase> _attackDefinitions = new List<AttackDefinitionBase>();

    private ITimeService _timeService;
    private DomainUpdater _domainUpdator;

    private AttackService _attackService;
    private IArena _arena;
    private Dictionary<Attack, IAttackPresentation> _activeVisuals;
    private Dictionary<Type, IAttackPresentationFactory> _visualBehaviors;
    private IAttackPresentationFactoryBuilder _factoryBuilder;
    private bool _initialized = false;

    public event Action<GameObject> OnAttackSpawned;

    public void Initialize(AttackService attackService, IArena arena, ITimeService timeService, DomainUpdater domainUpdator)
    {
        _attackService = attackService;
        _arena = arena;
        _activeVisuals = new Dictionary<Attack, IAttackPresentation>();
        _factoryBuilder = new AttackPresentationFactoryBuilder();
        _timeService = timeService;
        _domainUpdator = domainUpdator;

        InitializeVisualBehaviors();
        
        _attackService.OnAttackAdded += OnAttackAdded;
        _attackService.OnAttackCompleted += OnAttackCompleted;
        _timeService.OnTick += OnTick;

        _initialized = true;
    }

    private void InitializeVisualBehaviors()
    {
        _visualBehaviors = new Dictionary<Type, IAttackPresentationFactory>();

        foreach (IAttackDefinition attackDefinition in _attackDefinitions)
        {
            IAttackPresentationFactory factory = _factoryBuilder.Build(
                attackDefinition,
                _arena,
                transform,
                _tileVisualizer,
                _audioPlayer,
                _arenaShaker,
                _domainUpdator);
            _visualBehaviors[attackDefinition.AttackType] = factory;
        }
    }

    private void OnDestroy()
    {
        if (_attackService != null)
        {
            _attackService.OnAttackAdded -= OnAttackAdded;
            _attackService.OnAttackCompleted -= OnAttackCompleted;
        }
        
        ClearVisualization();
    }

    private void OnTick(float deltaTime)
    {
        if (_initialized) UpdateAllAttackVisuals();
    }

    private void OnAttackAdded(Attack attack)
    {
        attack.OnStageChanged += OnAttackStageChanged;
        CreateVisualForAttack(attack);
    }

    private void OnAttackCompleted(Attack attack)
    {
        attack.OnStageChanged -= OnAttackStageChanged;
        RemoveVisualForAttack(attack);
    }

    private void OnAttackStageChanged(Attack attack, AttackStage newStage)
    {
        if (_activeVisuals.ContainsKey(attack))
        {
            _activeVisuals[attack].OnStageChanged(newStage);
        }
    }

    private void UpdateAllAttackVisuals()
    {
        foreach (Attack attack in _attackService.GetActiveAttacks())
        {
            if (_activeVisuals.ContainsKey(attack))
            {
                _activeVisuals[attack].UpdatePresentation(attack);
            }
        }
    }

    private void CreateVisualForAttack(Attack attack)
    {
        RemoveVisualForAttack(attack);

        IAttackPresentationFactory factory = GetVisualBehaviorForAttack(attack);
        if (factory == null)
        {
            Debug.LogWarning($"No visual behavior found for attack type {attack.GetType().Name}. Skipping visual creation.");
            return;
        }

        IAttackPresentation presentation = factory.CreatePresentation(attack);
        _activeVisuals[attack] = presentation;

        presentation.OnAttackSpawned += (instance) => OnAttackSpawned?.Invoke(instance);
    }

    private void RemoveVisualForAttack(Attack attack)
    {
        if (!_activeVisuals.ContainsKey(attack)) return;

        IAttackPresentation presentation = _activeVisuals[attack];
        presentation.Destroy();
        _activeVisuals.Remove(attack);
    }

    private IAttackPresentationFactory GetVisualBehaviorForAttack(Attack attack)
    {
        Type attackType = attack.GetType();

        // Try to get specific behavior for this attack type
        if (_visualBehaviors.ContainsKey(attackType))
        {
            return _visualBehaviors[attackType];
        }

        return null;
    }

    public void ClearVisualization()
    {
        foreach (IAttackPresentation controller in _activeVisuals.Values)
        {
            controller.Destroy();
        }
        
        _activeVisuals.Clear();
    }

    /// <summary>
    /// Registers a custom visual behavior for a specific attack type.
    /// Allows runtime customization of attack visuals.
    /// </summary>
    public void RegisterVisualBehavior<TAttack>(IAttackPresentationFactory behavior) where TAttack : Attack
    {
        _visualBehaviors[typeof(TAttack)] = behavior;
    }
}
