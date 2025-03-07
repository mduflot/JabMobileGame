using System;
using Cysharp.Threading.Tasks;
using Enemy;
using Environment.MoveGrid;
using HelperPSR.RemoteConfigs;
using Service;
using Service.Fight;
using Service.Hype;
using Service.UI;
using UnityEngine;
using Tree = BehaviorTree.Trees;

public class EnemyManager : MonoBehaviour, IRemoteConfigurable, IHypeable
{
    public Action CanUltimateEvent;
    public Animator Animator;
    public bool IsBoosted;
    public EnemyEnums.EnemyMobilityState CurrentMobilityState;
    public EnemyEnums.EnemyBlockingState CurrentBlockingState;
    public EnemyInGameSO EnemyInGameSo;
    public EnemyGlobalSO EnemyGlobalSO;

    [SerializeField] private Tree.Tree _tree;
    [SerializeField] private string _remoteConfigAngleBlock;
    [SerializeField] private string _remoteConfigAngleStun;
    [SerializeField] private string _remoteConfigPercentageDamageReductionBoostChimist;
    [SerializeField] private ParticleSystem _startUltimateParticle;
    [SerializeField] private ParticleSystem _ultimateParticle;
    [SerializeField] private ParticleSystem _blockParticle;
    [SerializeField] private GameObject[] _particleToReset;
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    [SerializeField] private int _timeShaderActivate = 500;

    private IHypeService _hypeService;

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        CurrentMobilityState = EnemyEnums.EnemyMobilityState.INVULNERABLE;
        CurrentBlockingState = EnemyEnums.EnemyBlockingState.VULNERABLE;
    
    }

    private void OnEnable()
    {
        RemoteConfigManager.RegisterRemoteConfigurable(this);
    }

    private void OnDisable()
    {
        RemoteConfigManager.UnRegisterRemoteConfigurable(this);
    }

    public void Setup(Transform playerTransform, ITickeableService tickeableService,
        GridManager gridManager, IPoolService poolService, IHypeService hypeService, IUICanvasSwitchableService uiCanvasSwitchableService, IFightService fightService)
    {
        _hypeService = hypeService;
        _tree.Setup(playerTransform, tickeableService, gridManager, poolService, hypeService, uiCanvasSwitchableService, fightService);
        _hypeService.GetEnemyGainUltimateEvent += ActivateFXUltimate;
        _hypeService.GetEnemyLoseUltimateEvent += DeactivateFXUltimate;
    }

    public SkinnedMeshRenderer[] GetSkinnedMeshRenderers()
    {
        return _skinnedMeshRenderers;
    }

    public void ActivateGhostTrail(Vector2 direction)
    {
        // _ghostTrail.ActivateTrail(direction);
    }

    private void ActivateFXUltimate(float obj)
    {
        _startUltimateParticle.gameObject.SetActive(true);
        _ultimateParticle.gameObject.SetActive(true);
    }

    private void DeactivateFXUltimate(float obj)
    {
        _startUltimateParticle.gameObject.SetActive(false);
        _ultimateParticle.gameObject.SetActive(false);
    }

    public void SetRemoteConfigurableValues()
    {
        EnemyInGameSo.AngleBlock = RemoteConfigManager.Config.GetFloat(_remoteConfigAngleBlock);
        EnemyInGameSo.AngleStun = RemoteConfigManager.Config.GetFloat(_remoteConfigAngleStun);
        EnemyInGameSo.PercentageDamageReductionBoostChimist =
            RemoteConfigManager.Config.GetFloat(_remoteConfigPercentageDamageReductionBoostChimist);
    }

    public void ResetEnemy()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        CurrentMobilityState = EnemyEnums.EnemyMobilityState.INVULNERABLE;
        CurrentBlockingState = EnemyEnums.EnemyBlockingState.VULNERABLE;
        ResetAnimator();
        ResetParticles();
        ResetShaderColor();
        _hypeService.ResetHypeEnemy();
    }

    private void ResetAnimator()
    {
        Animator.Play("Idle");
        Animator.SetInteger("Direction", -1);
        Animator.SetInteger("RecoveryAnimationSpeed", 1);
        Animator.SetInteger("EndMovementSpeed0", 1);
        Animator.SetInteger("EndMovementSpeed1", 1);
        Animator.SetBool("IsTauting", false);
        Animator.SetInteger("Attack", -1);
        Animator.SetBool("IsBlocking", false);
        Animator.SetBool("IsBoosting", false);
        Animator.SetInteger("Dodge", -1);
        Animator.SetBool("IsStun", false);
        Animator.SetBool("IsStartShooting", false);
        Animator.SetInteger("SpeedAttackAnimation", 0);
    }

    private void ResetParticles()
    {
        foreach (var particle in _particleToReset)
        {
            particle.SetActive(false);
        }
    }

    public void StopTree(Action callback)
    {
        ResetAnimatorParameters();
        _tree.StopTree();
        _tree.ResetTree(callback);
    }

    public void ReplayTree()
    {
        ResetAnimatorParameters();
        _tree.ReplayTree();
    }

    private void ResetAnimatorParameters()
    {
        Animator.Play("Idle");
        Animator.SetInteger("Direction", -1);
        Animator.SetInteger("Attack", -1);
        Animator.SetBool("IsTaunting", false);
        Animator.SetBool("IsBlocking", false);
    }

    public bool TryDecreaseHypeEnemy(float amount, Vector3 posToCheck, Transform particleTransform,
        Enums.ParticlePosition particlePosition, bool isStun)
    {
        float damage = amount;
        if (CurrentBlockingState == EnemyEnums.EnemyBlockingState.BLOCKING)
        {
            Vector3 normalizedPos = (posToCheck - transform.position).normalized;
            float angle = Vector3.Angle(normalizedPos, transform.forward);
            if (angle > EnemyInGameSo.AngleBlock)
            {
                if (IsBoosted) damage = (1 - EnemyInGameSo.PercentageDamageReductionBoostChimist) * damage;
                ActivateShaderDamage();
                _hypeService.DecreaseHypeEnemy(damage);
                if (isStun) TakeStun();
                return true;
            }
            _blockParticle.gameObject.transform.position = (1 * (posToCheck - transform.position).normalized + transform.position);
            if (_blockParticle.isPlaying)
            {
                _blockParticle.Clear();
                _blockParticle.Play();
            }
            else
            {
                _blockParticle.gameObject.SetActive(true);
            }
            
            return false;
        }

        if (IsBoosted)
        {
            // damage = (1 - EnemyInGameSo.PercentageDamageReductionBoostChimist) * amount;
            _blockParticle.gameObject.transform.position = (1 * (posToCheck - transform.position).normalized + transform.position);
            if (_blockParticle.isPlaying)
            {
                _blockParticle.Clear();
                _blockParticle.Play();
            }
            else
            {
                _blockParticle.gameObject.SetActive(true);
            }
            return false;
        }
        
        if (isStun) TakeStun();

        Vector3 pos;
        particleTransform.gameObject.SetActive(true);
        switch (particlePosition)
        {
            case Enums.ParticlePosition.Neck:
                pos = new Vector3(transform.position.x, transform.localScale.y + 0.5F, transform.position.z);
                particleTransform.position = pos;
                break;
            case Enums.ParticlePosition.Punch:
                pos = (1 * (posToCheck - transform.position).normalized + transform.position) + new Vector3(0, 1, 0);
                particleTransform.position = pos;
                break;
            case Enums.ParticlePosition.Uppercut:
                pos = (1 * (posToCheck - transform.position).normalized + transform.position) + new Vector3(0, 1, 0);
                pos.y = transform.localScale.y + 0.5F;
                particleTransform.position = pos;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(particlePosition), particlePosition, null);
        }
        
        ActivateShaderDamage();
        _hypeService.DecreaseHypeEnemy(damage);
        return true;
    }

    private void ActivateShaderDamage()
    {
        foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.SetInt("_TakeDamage", 1);
        }

        DeactivateShaderDamage();
    }

    private async void DeactivateShaderDamage()
    {
        await UniTask.Delay(_timeShaderActivate);
        if (_skinnedMeshRenderers is null) return;
        foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.SetInt("_TakeDamage", 0);
        }
    }

    private void ResetShaderColor()
    {
        foreach (var skinnedMeshRenderer in _skinnedMeshRenderers)
        {
            skinnedMeshRenderer.material.SetInt("_TakeDamage", 0);
            skinnedMeshRenderer.material.SetInt("_Vulnerable", 0);
            if (skinnedMeshRenderer.material.HasInt("_BlueTexture"))
            {
                skinnedMeshRenderer.material.SetInt("_BlueTexture", 0);
            }
            if (skinnedMeshRenderer.material.HasInt("_YellowTexture"))
            {
                skinnedMeshRenderer.material.SetInt("_YellowTexture", 0);
            }
            if (skinnedMeshRenderer.material.HasInt("_RedTexture"))
            {
                skinnedMeshRenderer.material.SetInt("_RedTexture", 0);
            }
        }
    }

    private void TakeStun()
    {
        if (CurrentMobilityState is EnemyEnums.EnemyMobilityState.INVULNERABLE or EnemyEnums.EnemyMobilityState.STAGGER) return;
        CurrentMobilityState = EnemyEnums.EnemyMobilityState.STAGGER;
    }
}