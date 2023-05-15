﻿using System;
using Addressables;
using Attributes;
using Service.Currency;
using Service.Fight;
using Service.Hype;
using Service.Items;
using Service.Shop;
using UnityEngine;
using static UnityEngine.AddressableAssets.Addressables;
using Object = UnityEngine.Object;

namespace Service.UI
{
    public class UICanvasService : IUICanvasSwitchableService
    {
        [DependsOnService] private IGameService _gameService;
        [DependsOnService] private ITournamentService _tournamentService;
        [DependsOnService] private IFightService _fightService;
        [DependsOnService] private IHypeService _hypeService;
        [DependsOnService] private IItemsService _itemsService;
        [DependsOnService] private ICurrencyService _currencyService;
        [DependsOnService] private IShopService _shopService;
        private GameObject _mainMenu;
        private GameObject _inGameMenu;
        private InGameMenuTutorialManager _inGameTutorialMenu;

        public void LoadMainMenu()
        {
            AddressableHelper.LoadAssetAsyncWithCompletionHandler<GameObject>("MainMenu", GenerateMainMenu);
        }

        private void GenerateMainMenu(GameObject gameObject)
        {
            _mainMenu = Object.Instantiate(gameObject);
            _mainMenu.GetComponent<MenuManager>().SetupMenu(_gameService, _tournamentService, _fightService,
                _currencyService, _itemsService, _shopService);
            Release(gameObject);
        }

        public void LoadInGameMenu()
        {
            AddressableHelper.LoadAssetAsyncWithCompletionHandler<GameObject>("InGameMenu", GenerateInGameMenu);
        }

        public void OpenFightTutoPanel()
        {
            _inGameTutorialMenu.OpenFightPopup();
        }

        public void OpenMoveTutoPanel()
        {
            _inGameTutorialMenu.OpenMovePopup();
        }

        public void OpenTauntTutoPanel()
        {
            _inGameTutorialMenu.OpenTauntPopup();
        }

        public void OpenUltimateTutoPanel()
        {
            _inGameTutorialMenu.OpenUltimatePopup();
        }

        private void GenerateInGameMenu(GameObject gameObject)
        {
            _inGameMenu = Object.Instantiate(gameObject);
            var inGameMenu = _inGameMenu.GetComponent<InGameMenuManager>();
            inGameMenu.SetupMenu(_fightService, _hypeService, _tournamentService, _currencyService);
            _inGameTutorialMenu = inGameMenu.InGameMenuTutorialManager;
            InitCanvasEvent?.Invoke();
            Release(gameObject);
        }

        public event Action InitCanvasEvent;

        public void EnabledService()
        {
        }

        public void DisabledService()
        {
        }

        public bool GetIsActiveService { get; }
    }
}