using System;
using Addressables;
using Attributes;
using HelperPSR.MonoLoopFunctions;
using HelperPSR.RemoteConfigs;
using Service.Fight;
using Service.UI;
using UnityEngine;

namespace Service.Hype
{
    public class HypeService : IHypeService, IRemoteConfigurable, IUpdatable
    {
        [DependsOnService] private IFightService _fightService;
        private EnemyManager _enemyManager;
        private Hype _hypePlayer;
        private Hype _hypeEnemy;
        private HypeServiceSO _hypeServiceSo;
        private bool _isDecreasePlayerHype;
        private float _decreasePlayerHypeTimer;
        private bool _inCooldown;
        private float _halfHype;

        private float _ultimateAreaReachedHalfHypeSpeed;
        private float _startHypeValuePlayer;

        private float _startHypeValueEnemy;

        public event Action EnableHypeServiceEvent;

        public void OnUpdate()
        {
            if (_hypePlayer.UltimateCurrentValue >= _halfHype)
            {
                _hypePlayer.UltimateCurrentValue -= Time.deltaTime * _ultimateAreaReachedHalfHypeSpeed;
                _hypeEnemy.UltimateCurrentValue -= Time.deltaTime * _ultimateAreaReachedHalfHypeSpeed;
                UltimateAreaIncreaseEvent?.Invoke(_hypePlayer.UltimateCurrentValue);
                TryGainUltimateValue(_hypeEnemy,0);
                TryGainUltimateValue(_hypePlayer,0);
            }
            else
            {
                UpdateManager.UnRegister(this);
            }
        }

        public void PlayUltimateAreasIncreased()
        {
            if(!_fightService.GetFightTutorial())
            {
                UpdateManager.Register(this);
            }
        }

        public void StopUltimateAreasIncreased()
        {
            UpdateManager.UnRegister(this);
        }

        public void IncreaseHypePlayer(float amount)
        {
            IncreaseHype(amount, _hypePlayer, _hypeEnemy);
        }

        public void IncreaseHypeEnemy(float amount)
        {
            IncreaseHype(amount, _hypeEnemy, _hypePlayer);
        }

        public void IncreaseHype(float amount, Hype hype, Hype otherHype)
        {
            if (CheckMaximumHypeIsReached(hype.CurrentValue)) return;
            if (CheckMaximumHypeIsReached(hype.CurrentValue + amount))
            {
                hype.CurrentValue = _hypeServiceSo.MaxHype;
                ReachMaximumHypeEvent?.Invoke(amount);
            }
            else
            {
                if (CheckHypeReachOtherHype(hype, amount, otherHype))
                {
                    TryGainUltimateValue(hype, hype.CurrentValue - _hypeServiceSo.MaxHype - otherHype.CurrentValue);
                    hype.CurrentValue = _hypeServiceSo.MaxHype - otherHype.CurrentValue;
                }
                else
                {
                    TryGainUltimateValue(hype, amount);
                    hype.CurrentValue += amount;
                }
            }

            hype.IncreaseHypeEvent?.Invoke(amount);
        }

        private void TryGainUltimateValue(Hype hype, float amount)
        {
            if (!hype.IsInUltimateArea)
            {
                if (hype.CurrentValue + amount >= hype.UltimateCurrentValue)
                {
                    hype.IsInUltimateArea = true;
                    Vibration.Vibrate(100);
                    hype.GainUltimateEvent?.Invoke(amount);
                }
            }
        }

        private void TryLoseUltimateValue(Hype hype, float amount)
        {
            if (hype.IsInUltimateArea)
            {
                if (hype.CurrentValue - amount < hype.UltimateCurrentValue)
                {
                    hype.IsInUltimateArea = false;
                    hype.LoseUltimateEvent?.Invoke(amount);
                }
            }
        }

        public bool GetUltimateAreaPlayer()
        {
            return _hypePlayer.IsInUltimateArea;
        }

        public bool GetUltimateAreaEnemy()
        {
            return _hypeEnemy.IsInUltimateArea;
        }

        public event Action<float> UltimateAreaIncreaseEvent;

        public void DecreaseHypePlayer(float amount)
        {
            DecreaseHype(amount, _hypePlayer);
        }

        public void DecreaseHypeEnemy(float amount)
        {
            DecreaseHype(amount, _hypeEnemy);
        }

        public void DecreaseHype(float amount, Hype hype)
        {
            if (CheckMinimumHypeIsReached(hype.CurrentValue)) return;
            if (CheckMinimumHypeIsReached(hype.CurrentValue - amount))
            {
                hype.CurrentValue = 0;
                ReachMinimumHypeEvent?.Invoke(amount);
            }
            else
            {
                TryLoseUltimateValue(hype, amount);
                hype.CurrentValue -= amount;
            }

            hype.DecreaseHypeEvent?.Invoke(amount);
        }

        public void SetHypePlayer(float value)
        {
            SetHype(value, _hypePlayer);
        }

        public void SetHypeEnemy(float value)
        {
            SetHype(value, _hypeEnemy);
        }

        public void ResetHypePlayer()
        {
            _hypePlayer.UltimateCurrentValue = _hypePlayer.HypeSo.UltimateValue;
            
            SetHypePlayer(_startHypeValuePlayer);
            UltimateAreaIncreaseEvent?.Invoke( _hypePlayer.UltimateCurrentValue);
            _hypePlayer.IsInUltimateArea = false;
            _hypePlayer.LoseUltimateEvent?.Invoke(_startHypeValuePlayer);
        }

        public void ResetHypeEnemy()
        {
            _hypeEnemy.UltimateCurrentValue = _hypeEnemy.HypeSo.UltimateValue;
            SetHypeEnemy(_startHypeValueEnemy);
            _hypeEnemy.IsInUltimateArea = false;
            UltimateAreaIncreaseEvent?.Invoke( _hypePlayer.UltimateCurrentValue);
            _hypeEnemy.LoseUltimateEvent?.Invoke(_startHypeValueEnemy);
        }

        public void SetStartHypePlayer(float value)
        {
            _startHypeValuePlayer = value;
        }

        public void SetStartHypeEnemy(float value)
        {
            _startHypeValueEnemy = value;
        }


        private void SetHype(float value, Hype hype)
        {
            if (!CheckHypeIsClamped(value)) return;
            hype.CurrentValue = value;

            hype.SetHypeEvent?.Invoke(value);
        }

        public float GetCurrentHypePlayer()
        {
            return _hypePlayer.CurrentValue;
        }

        public float GetCurrentHypeEnemy()
        {
            return _hypeEnemy.CurrentValue;
        }

        public float GetUltimateHypeValuePlayer()
        {
            return _hypePlayer.HypeSo.UltimateValue;
        }

        public float GetUltimateHypeValueEnemy()
        {
            return _hypeEnemy.HypeSo.UltimateValue;
        }

        public float GetMaximumHype()
        {
            return _hypeServiceSo.MaxHype;
        }

        public Action<float> GetPlayerIncreaseHypeEvent
        {
            get => _hypePlayer.IncreaseHypeEvent;
            set => _hypePlayer.IncreaseHypeEvent = value;
        }

        public Action<float> GetPlayerDecreaseHypeEvent
        {
            get => _hypePlayer.DecreaseHypeEvent;
            set => _hypePlayer.DecreaseHypeEvent = value;
        }

        public Action<float> GetPlayerSetHypeEvent
        {
            get => _hypePlayer.SetHypeEvent;
            set => _hypePlayer.SetHypeEvent = value;
        }

        public Action<float> GetPlayerGainUltimateEvent
        {
            get => _hypePlayer.GainUltimateEvent;
            set => _hypePlayer.GainUltimateEvent = value;
        }

        public Action<float> GetPlayerLoseUltimateEvent
        {
            get => _hypePlayer.LoseUltimateEvent;
            set => _hypePlayer.LoseUltimateEvent = value;
        }

        public Action<float> GetEnemyIncreaseHypeEvent
        {
            get => _hypeEnemy.IncreaseHypeEvent;
            set => _hypeEnemy.IncreaseHypeEvent = value;
        }

        public Action<float> GetEnemyDecreaseHypeEvent
        {
            get => _hypeEnemy.DecreaseHypeEvent;
            set => _hypeEnemy.DecreaseHypeEvent = value;
        }

        public Action<float> GetEnemySetHypeEvent
        {
            get => _hypeEnemy.SetHypeEvent;
            set => _hypeEnemy.SetHypeEvent = value;
        }

        public Action<float> GetEnemyGainUltimateEvent
        {
            get => _hypeEnemy.GainUltimateEvent;
            set => _hypeEnemy.GainUltimateEvent = value;
        }

        public Action<float> GetEnemyLoseUltimateEvent
        {
            get => _hypeEnemy.LoseUltimateEvent;
            set => _hypeEnemy.LoseUltimateEvent = value;
        }

        public float GetMinimumHype()
        {
            return 0;
        }

        private bool CheckHypeIsClamped(float value)
        {
            return value <= _hypeServiceSo.MaxHype && value >= 0;
        }

        private bool CheckHypeReachOtherHype(Hype hype, float amount, Hype otherHype)
        {
            return ((hype.CurrentValue + amount + otherHype.CurrentValue) >= _hypeServiceSo.MaxHype);
        }

        private bool CheckMaximumHypeIsReached(float currentHype)
        {
            return currentHype >= _hypeServiceSo.MaxHype;
        }

        private bool CheckMinimumHypeIsReached(float currentHype)
        {
            return currentHype <= 0;
        }

        public event Action<float> ReachMaximumHypeEvent;

        public event Action<float> ReachMinimumHypeEvent;

        public void EnabledService()
        {
            AddressableHelper.LoadAssetAsyncWithCompletionHandler<HypeServiceSO>("HypeSO", SetHypeSO);
        }

        private void SetHypeSO(HypeServiceSO hypeServiceSo)
        {
            _halfHype = hypeServiceSo.MaxHype / 2;
            _ultimateAreaReachedHalfHypeSpeed = (hypeServiceSo.PlayerHypeSO.UltimateValue-_halfHype)/hypeServiceSo._ultimateAreaReachedHalfHypeTime;
            _hypeServiceSo = hypeServiceSo;
            _hypeEnemy = new Hype();
            _hypePlayer = new Hype();
            _hypeEnemy.HypeSo = _hypeServiceSo.EnemyHypeSO;
            _hypePlayer.HypeSo = _hypeServiceSo.PlayerHypeSO;
            ResetHypePlayer();
            ResetHypeEnemy();
            _isDecreasePlayerHype = false;
            _decreasePlayerHypeTimer = 0;
            _hypePlayer.IncreaseHypeEvent += ResetDecreasePlayerHype;
            _hypeEnemy.DecreaseHypeEvent += ResetDecreasePlayerHype;
            RemoteConfigManager.RegisterRemoteConfigurable(this);
            EnableHypeServiceEvent?.Invoke();
        }

        private void ResetDecreasePlayerHype(float amount) => _isDecreasePlayerHype = false;

        public void DisabledService()
        {
            UnityEngine.AddressableAssets.Addressables.Release(_hypeServiceSo);
            _hypeEnemy.DecreaseHypeEvent = null;
            _hypeEnemy.IncreaseHypeEvent = null;
            _hypeEnemy.GainUltimateEvent = null;
            _hypeEnemy.LoseUltimateEvent = null;
            _hypeEnemy.SetHypeEvent = null;
            _hypePlayer.DecreaseHypeEvent = null;
            _hypePlayer.IncreaseHypeEvent = null;
            _hypePlayer.GainUltimateEvent = null;
            _hypePlayer.LoseUltimateEvent = null;
            _hypePlayer.SetHypeEvent = null;
            ReachMaximumHypeEvent = null;
            ReachMinimumHypeEvent = null;
            UltimateAreaIncreaseEvent = null;
            EnableHypeServiceEvent = null;
            UpdateManager.UnRegister(this);
            RemoteConfigManager.UnRegisterRemoteConfigurable(this);
        }

        public bool GetIsActiveService { get; }

        public void SetRemoteConfigurableValues()
        {
            _hypeServiceSo.MaxHype = RemoteConfigManager.Config.GetFloat("MaxHype");
            _hypeServiceSo.EnemyHypeSO.UltimateValue = RemoteConfigManager.Config.GetFloat("EnemyHypeUltimateValue");
            _hypeServiceSo.PlayerHypeSO.UltimateValue = RemoteConfigManager.Config.GetFloat("PlayerHypeUltimateValue");
      
        }
    }
}