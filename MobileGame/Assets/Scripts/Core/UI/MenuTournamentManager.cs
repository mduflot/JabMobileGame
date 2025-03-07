﻿using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Service.Currency;
using Service.Fight;
using Service.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Service.UI
{
    public class MenuTournamentManager : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _playFightButton;
        [SerializeField] private Button _backMenuButton;

        [Header("Tournament Canvas & Parent")]
        [SerializeField] private Canvas _tournamentButtonsCanvas;
        [SerializeField] private Canvas _tournamentQuarterCanvas;
        [SerializeField] private RectTransform _tournamentQuarterParent;
        [SerializeField] private Canvas _tournamentDemiCanvas;
        [SerializeField] private RectTransform _tournamentDemiParent;
        [SerializeField] private Canvas _tournamentFinalCanvas;
        [SerializeField] private RectTransform _tournamentFinalParent;

        [Header("Image Fighters")] 
        [SerializeField] private Image[] _imagePlayerFighter;
        [SerializeField] private Image[] _imageQuarterWinners;
        [SerializeField] private Image[] _imageQuarterLosers;
        [SerializeField] private Image[] _imageDemiWinners;
        [SerializeField] private Image[] _imageDemiLosers;
        [SerializeField] private Image[] _imageFinalWinners;
        [SerializeField] private Image _imageFinalLoser;
        [SerializeField] private Image _imageLogoQuarter;
        [SerializeField] private Image[] _imageLogoDemi;
        [SerializeField] private Image[] _imageLogoFinal;
        [SerializeField] private Image[] _imageLogoQuarterFake;
        [SerializeField] private Image[] _imageLogoDemiFakes;
        [SerializeField] private Sprite _neutralImage;
        [SerializeField] private Sprite _neutralPlayerImage;
        [SerializeField] private Sprite _winnerImage;
        [SerializeField] private Sprite _loserImage;

        [Header("Name Fighters")]
        [SerializeField] private TextMeshProUGUI _quarterName;
        [SerializeField] private TextMeshProUGUI[] _demiNames;
        [SerializeField] private TextMeshProUGUI[] _finalNames;
        [SerializeField] private TextMeshProUGUI[] _quarterFakeNames;
        [SerializeField] private TextMeshProUGUI[] _demiFakeNames;

        [Header("Popups")]
        [SerializeField] private Canvas _pubCanvas;

        [SerializeField] private JuicyPopup _pubPopUp;
        [SerializeField] private Canvas _winTournament;
        [SerializeField] private JuicyPopup _winPopUp;
        [SerializeField] private Canvas _defeatTournament;
        [SerializeField] private JuicyPopup _defeatPopUp;
        [SerializeField] private TextMeshProUGUI _winTournamentText;
        [SerializeField] private Image _rewardImage;

        private Fight.Fight[] _fights;
        private MenuManager _menuManager;
        private IGameService _gameService;
        private ITournamentService _tournamentService;
        private ICurrencyService _currencyService;
        private IItemsService _itemsService;
        private List<FriendUser> _fakes;
        private string _enemyAddressableName;
        private string _environmentAddressableName;

        public void SetupMenu(IGameService gameService, ITournamentService tournamentService, MenuManager menuManager,
            ICurrencyService currencyService, IItemsService itemsService, IFightService fightService)
        {
            _gameService = gameService;
            _tournamentService = tournamentService;
            _currencyService = currencyService;
            _menuManager = menuManager;
            _itemsService = itemsService;
            if (!_tournamentService.GetTournamentIsActive()) _tournamentService.SetTournament();
            _fights = _tournamentService.GetFights();
            _fakes = _tournamentService.GetFakes();
            SetTournamentNames();
            if (!fightService.GetFightTutorial() && !fightService.GetFightDebug())
            {
                UpdateCurrentFightUI();
                if (_tournamentService.CompareState(FightState.DEFEAT)) _defeatTournament.gameObject.SetActive(true);
                else if (_tournamentService.CompareState(FightState.WAITING)) UpdateCurrentFightUI();
                else ActivateWinnerUI();
            }
        }

        private void SetTournamentNames()
        {
            _quarterName.text = _fights[0].EnemyGlobalSO.Name;
            _imageLogoQuarter.sprite = _fights[0].EnemyGlobalSO.IconSprite;
            foreach (var demiName in _demiNames)
            {
                demiName.text = _fights[1].EnemyGlobalSO.Name;
            }
            foreach (var demiName in _imageLogoDemi)
            {
                demiName.sprite = _fights[1].EnemyGlobalSO.IconSprite;
            }

            foreach (var finalName in _finalNames)
            {
                finalName.text = _fights[2].EnemyGlobalSO.Name;
            }
            foreach (var finalName in _imageLogoFinal)
            {
                finalName.sprite = _fights[2].EnemyGlobalSO.IconSprite;
            }

            for (var index = 0; index < _quarterFakeNames.Length; index++)
            {
                var quarterFakeName = _quarterFakeNames[index];
                quarterFakeName.text = _fakes[index].name;
                var quarterFakeImage = _imageLogoQuarterFake[index];
                quarterFakeImage.sprite = _fakes[index].picture;
            }

            foreach (var demiFakeName in _demiFakeNames)
            {
                demiFakeName.text = _fakes.LastOrDefault()?.name;
            }

            foreach (var imageLogoDemiFake in _imageLogoDemiFakes)
            {
                imageLogoDemiFake.sprite = _fakes.LastOrDefault()?.picture;
            }
        }

        public void OpenUITournament()
        {
            _tournamentButtonsCanvas.gameObject.SetActive(true);
            switch (_tournamentService.GetCurrentFightPlayer().TournamentStep)
            {
                case TournamentStep.QUARTER :
                    _tournamentQuarterCanvas.gameObject.SetActive(true);
                    break;
                case TournamentStep.DEMI :
                    _tournamentDemiCanvas.gameObject.SetActive(true);
                    break;
                case TournamentStep.FINAL :
                    _tournamentFinalCanvas.gameObject.SetActive(true);
                    break;
            }
        }

        public void UpdateUITournament()
        {
            _tournamentButtonsCanvas.gameObject.SetActive(true);
            switch (_tournamentService.GetCurrentFightPlayer().TournamentStep)
            {
                case TournamentStep.QUARTER :
                    _tournamentQuarterParent.DOAnchorPos(new Vector2(0, 0), 0f);
                    _tournamentDemiParent.DOAnchorPos(new Vector2(1920, 0), 0f);
                    _tournamentFinalParent.DOAnchorPos(new Vector2(3840, 0), 0f);
                    _tournamentQuarterCanvas.gameObject.SetActive(true);
                    break;
                case TournamentStep.DEMI :
                    _tournamentQuarterParent.DOAnchorPos(new Vector2(-1920, 0), 0f);
                    _tournamentDemiParent.DOAnchorPos(new Vector2(0, 0), 0f);
                    _tournamentFinalParent.DOAnchorPos(new Vector2(1920, 0), 0f);
                    _tournamentDemiCanvas.gameObject.SetActive(true);
                    break;
                case TournamentStep.FINAL :
                    _tournamentQuarterParent.DOAnchorPos(new Vector2(-3840, 0), 0f);
                    _tournamentDemiParent.DOAnchorPos(new Vector2(-1920, 0), 0f);
                    _tournamentFinalParent.DOAnchorPos(new Vector2(0, 0), 0f);
                    _tournamentFinalCanvas.gameObject.SetActive(true);
                    break;
            }
        }

        private async void UpdateCurrentFightUI()
        {
            Fight.Fight currentFight = _tournamentService.GetCurrentFightPlayer();
            switch (currentFight.TournamentStep)
            {
                case TournamentStep.DEMI:
                    _playFightButton.gameObject.SetActive(false);
                    _backMenuButton.gameObject.SetActive(false);
                    foreach (var imageQuarterWinner in _imageQuarterWinners)
                    {
                        imageQuarterWinner.sprite = _winnerImage;
                    }
                    foreach (var imageQuarterLoser in _imageQuarterLosers)
                    {
                        imageQuarterLoser.sprite = _loserImage;
                    }

                    _tournamentQuarterCanvas.gameObject.SetActive(true);
                    _tournamentDemiCanvas.gameObject.SetActive(true);
                    await UniTask.Delay(1000);
                    _tournamentQuarterParent.DOAnchorPos(new Vector2(-1920, 0), 5f)
                        .OnComplete(() => _tournamentQuarterCanvas.gameObject.SetActive(false));
                    _tournamentDemiParent.DOAnchorPos(new Vector2(0, 0), 5f)
                        .OnComplete(ActivateButtons);
                    break;

                case TournamentStep.FINAL:
                    _playFightButton.gameObject.SetActive(false);
                    _backMenuButton.gameObject.SetActive(false);
                    foreach (var imageQuarterWinner in _imageQuarterWinners)
                    {
                        imageQuarterWinner.sprite = _winnerImage;
                    }
                    foreach (var imageQuarterLoser in _imageQuarterLosers)
                    {
                        imageQuarterLoser.sprite = _loserImage;
                    }

                    foreach (var imageDemiWinner in _imageDemiWinners)
                    {
                        imageDemiWinner.sprite = _winnerImage;
                    }
                    foreach (var imageDemiLoser in _imageDemiLosers)
                    {
                        imageDemiLoser.sprite = _loserImage;
                    }

                    _tournamentQuarterCanvas.gameObject.SetActive(true);
                    _tournamentDemiCanvas.gameObject.SetActive(true);
                    _tournamentFinalCanvas.gameObject.SetActive(true);
                    _tournamentQuarterParent.DOAnchorPos(new Vector2(-3840, 0), 0f)
                        .OnComplete(() => _tournamentQuarterCanvas.gameObject.SetActive(false));
                    _tournamentDemiParent.DOAnchorPos(new Vector2(0, 0), 0f);
                    _tournamentFinalParent.DOAnchorPos(new Vector2(1920, 0), 0f);
                    await UniTask.Delay(1000);
                    _tournamentDemiParent.DOAnchorPos(new Vector2(-1920, 0), 5f)
                        .OnComplete(() => _tournamentDemiCanvas.gameObject.SetActive(false));
                    _tournamentFinalParent.DOAnchorPos(new Vector2(0, 0), 5f)
                        .OnComplete(ActivateButtons);
                    break;
            }
        }

        private void ActivateWinnerUI()
        {
            _playFightButton.gameObject.SetActive(false);
            _backMenuButton.gameObject.SetActive(false);
            foreach (var imageQuarterWinner in _imageQuarterWinners)
            {
                imageQuarterWinner.sprite = _winnerImage;
            }
            foreach (var imageQuarterLoser in _imageQuarterLosers)
            {
                imageQuarterLoser.sprite = _loserImage;
            }

            foreach (var imageDemiWinner in _imageDemiWinners)
            {
                imageDemiWinner.sprite = _winnerImage;
            }
            foreach (var imageDemiLoser in _imageDemiLosers)
            {
                imageDemiLoser.sprite = _loserImage;
            }

            foreach (var imageFinalWinner in _imageFinalWinners)
            {
                imageFinalWinner.sprite = _winnerImage;
            }
            _imageFinalLoser.sprite = _loserImage;
            

            _tournamentQuarterCanvas.gameObject.SetActive(true);
            _tournamentDemiCanvas.gameObject.SetActive(true);
            _tournamentFinalCanvas.gameObject.SetActive(true);
            _tournamentQuarterParent.DOAnchorPos(new Vector2(-3840, 0), 0f)
                .OnComplete(() => _tournamentQuarterCanvas.gameObject.SetActive(false));
            _tournamentDemiParent.DOAnchorPos(new Vector2(-1920, 0), 0f)
                .OnComplete(() => _tournamentDemiCanvas.gameObject.SetActive(false));
            _tournamentFinalParent.DOAnchorPos(new Vector2(0, 0), 0f)
                .OnComplete(ActivateWinTournamentCanvas);
        }

        private void ActivateWinTournamentCanvas()
        {
            ActivateButtons();
            _winTournamentText.text = "+" + _tournamentService.GetSettings().CoinsAmountWhenWinTournament;
            _rewardImage.sprite = _tournamentService.GetCurrentFightPlayer().EnemyGlobalSO.ItemSO.SpriteUI;
            _winTournament.gameObject.SetActive(true);
            _winPopUp.ActivatePopUp();
        }

        private void ResetUITournament()
        {
            foreach (var imageQuarterWinner in _imageQuarterWinners)
            {
                imageQuarterWinner.sprite = _neutralImage;
            }

            foreach (var imageQuarterLoser in _imageQuarterLosers)
            {
                imageQuarterLoser.sprite = _neutralImage;
            }

            foreach (var imageDemiWinner in _imageDemiWinners)
            {
                imageDemiWinner.sprite = _neutralImage;
            }

            foreach (var imageDemiLoser in _imageDemiLosers)
            {
                imageDemiLoser.sprite = _neutralImage;
            }

            foreach (var imageFinalWinner in _imageFinalWinners)
            {
                imageFinalWinner.sprite = _neutralImage;
            }

            _imageFinalLoser.sprite = _neutralImage;

            foreach (var imagePlayerFighter in _imagePlayerFighter)
            {
                imagePlayerFighter.sprite = _neutralPlayerImage;
            }
            
            _fights = _tournamentService.GetFights();
            _fakes = _tournamentService.GetFakes();
            SetTournamentNames();
        }

        private void ActivateButtons()
        {
            _playFightButton.gameObject.SetActive(true);
            _backMenuButton.gameObject.SetActive(true);
        }

        private void GainEndTournamentCoins()
        {
            _currencyService.AddCoins(_tournamentService.GetSettings().CoinsAmountWhenWinTournament);
            _itemsService.UnlockItem(_tournamentService.GetFights()[^1].EnemyGlobalSO.ItemSO);
        }

        private void DeactivateUITournament()
        {
            _tournamentButtonsCanvas.gameObject.SetActive(false);
            _tournamentQuarterCanvas.gameObject.SetActive(false);
            _tournamentDemiCanvas.gameObject.SetActive(false);
            _tournamentFinalCanvas.gameObject.SetActive(false);
        }

        public void StartFight()
        {
            _playFightButton.interactable = false;
            Fight.Fight currentFight = _tournamentService.GetCurrentFightPlayer();
            LoadingScreenManager.Instance.gameObject.SetActive(true);
            _gameService.LoadGameScene(currentFight.EnvironmentSO.EnvironmentAddressableName,
                currentFight.EnemyGlobalSO.EnemyAddressableName, false, false);
        }

        public void QuitTournament()
        {
            _defeatTournament.gameObject.SetActive(false);
            _defeatPopUp.gameObject.SetActive(false);
            _winTournament.gameObject.SetActive(false);
            _winPopUp.gameObject.SetActive(false);
            GainEndTournamentCoins();
            _tournamentService.ResetTournament();
            ResetUITournament();
            BackMenu();
        }

        public void BackMenu()
        {
            DeactivateUITournament();
            _menuManager.ActivateHome();
        }

        public void LaunchPub()
        {
            _defeatTournament.gameObject.SetActive(false);
            _defeatPopUp.gameObject.SetActive(false);
            _tournamentService.GetCurrentFightPlayer().FightState = FightState.WAITING;
            _pubCanvas.gameObject.SetActive(true);
            _pubPopUp.ActivatePopUp();
        }

        public void ClosePub()
        {
            _pubCanvas.gameObject.SetActive(false);
            _pubPopUp.gameObject.SetActive(false);
            UpdateUITournament();
        }
    }
}