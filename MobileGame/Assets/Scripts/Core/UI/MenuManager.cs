using System;
using DG.Tweening;
using Service.Currency;
using Service.Fight;
using Service.Items;
using Service.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Service.UI
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Menu UI")]
        [SerializeField] private Button _playMatchButton;
        [SerializeField] private Button _playTournamentButton;
        [SerializeField] private GridLayoutGroup _enemySelectionGrid;
        [SerializeField] private GridLayoutGroup _environnementSelectionGrid;
        [SerializeField] private Button _environmentButton;
        [SerializeField] private Button _enemyButton;
        [SerializeField] private MenuTopManager _menuTopManager;
        [SerializeField] private Canvas _topCanvas;
        [SerializeField] private Canvas _botCanvas;
        [SerializeField] private Canvas _backgroundCanvas;

        [SerializeField] private Color _colorBottomButtonWhenNotSelect;
        [SerializeField] private Image _homeButtonImage;
        [SerializeField] private Button _homeButton;
        [SerializeField] private MenuTournamentManager _menuTournamentManager;
        [SerializeField] private GameObject _tournamentCanvas;
        [SerializeField] private GameObject _debugFightCanvas;

        [SerializeField] private MenuShopManager _menuShopManager;
        [SerializeField] private Image _shopButtonImage;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Canvas _shopCanvas;

        [SerializeField] private MenuInventoryManager _menuInventoryManager;
        [SerializeField] private Image _inventoryButtonImage;
        [SerializeField] private Button _inventoryButton;
        [SerializeField] private Canvas _inventoryCanvas;

        [SerializeField] private GameObject[] _tournaments;
        [SerializeField] private Button _leftArrowTournament;
        [SerializeField] private Button _rightArrowTournament;

        [SerializeField] private Canvas _unlockTournament;
        [SerializeField] private JuicyPopup _unlockTournamentPopup;
        [SerializeField] private JuicyPopup _badgesPopup;
        [SerializeField] private JuicyPopup _friendsPanel;
        [SerializeField] private JuicyPopup _friendPopup;
        [SerializeField] private JuicyPopup _dailyPanel;
        [SerializeField] private GameObject _friendsLayout;
        [SerializeField] private Button _friendTag;
        [SerializeField] private Image _friendPopupImage;
        [SerializeField] private TextMeshProUGUI _friendPopupNameText;

        [SerializeField] private DragPlayer _dragPlayer;

        [SerializeField] private Image _rewardImage;

        private TournamentSettingsSO _tournamentSettingsSO;
        private int _actualTournament;
        private IGameService _gameService;
        private ITournamentService _tournamentService;
        private IFightService _fightService;
        private string _nextEnvironmentAddressableName;
        private string _nextEnemyAddressableName;
        private ICurrencyService _currencyService;
        private GameObject _player;

        public void SetupMenu(IGameService gameService, ITournamentService tournamentService,
            ICurrencyService currencyService, IItemsService itemsService, IShopService shopService,
            GameObject player, IFightService fightService)
        {
            LoadingScreenManager.Instance.gameObject.SetActive(false);
            _gameService = gameService;
            _tournamentService = tournamentService;
            _dragPlayer.Init(player);
            _tournamentSettingsSO = _tournamentService.GetSettings();
            _fightService = fightService;
            _menuTopManager.SetUp(currencyService);
            _currencyService = currencyService;
            _player = player;
            _menuInventoryManager.Setup(itemsService, player.GetComponentInChildren<PlayerItemsLinker>());
            _menuShopManager.Setup(itemsService, currencyService, shopService);
            _tournaments[_actualTournament].SetActive(true);
            _leftArrowTournament.gameObject.SetActive(false);
            _homeButton.interactable = false;
            SetImageBottomButtonColor(_inventoryButtonImage);
            SetImageBottomButtonColor(_shopButtonImage);
            ResetImageBottomButtonColor(_homeButtonImage);
            _menuTournamentManager.SetupMenu(_gameService, _tournamentService, this, _currencyService, itemsService, _fightService);
            _rewardImage.sprite = _tournamentService.GetFights()[^1].EnemyGlobalSO.ItemSO.SpriteUI;
            if (!_fightService.GetFightTutorial() && !_fightService.GetFightDebug())
            {
                _playTournamentButton.interactable = false;
                _tournamentCanvas.SetActive(false);
                _topCanvas.gameObject.SetActive(false);
                _botCanvas.gameObject.SetActive(false);
                _menuTournamentManager.OpenUITournament();
            }

            foreach (var enemyGlobalSo in gameService.GlobalSettingsSO.AllEnemyGlobalSO)
            {
                if (enemyGlobalSo.EnemyAddressableName.Contains("Tutorial")) continue;
                var button = Instantiate(_enemyButton, _enemySelectionGrid.transform);
                button.onClick.AddListener(() => SetEnemyAddressableName(enemyGlobalSo.EnemyAddressableName));
                button.transform.GetChild(0).GetComponent<Image>().sprite = enemyGlobalSo.IconSprite;
                button.GetComponentInChildren<TextMeshProUGUI>().text = enemyGlobalSo.Name;
            }

            foreach (var environmentSo in gameService.GlobalSettingsSO.AllEnvironmentsSO)
            {
                var button = Instantiate(_environmentButton, _environnementSelectionGrid.transform);
                button.onClick.AddListener(
                    () => SetEnvironmentAddressableName(environmentSo.EnvironmentAddressableName));
                button.GetComponentInChildren<TextMeshProUGUI>().text = environmentSo.Name;
            }

            foreach (var fakeName in _tournamentSettingsSO.FakeNames)
            {
                var button = Instantiate(_friendTag, _friendsLayout.transform);
                button.onClick.AddListener(() => OpenFriendPopup(fakeName.picture, fakeName.name));
                button.GetComponent<FriendComponent>().SetValue(fakeName.picture, fakeName.name);
            }
        }

        public void SetEnvironmentAddressableName(string value) => _nextEnvironmentAddressableName = value;

        public void SetEnemyAddressableName(string value) => _nextEnemyAddressableName = value;

        public void ActivateHome()
        {
            _player.SetActive(false);
            _tournamentCanvas.SetActive(true);
            _backgroundCanvas.gameObject.SetActive(true);
            _debugFightCanvas.SetActive(false);
            _shopCanvas.gameObject.SetActive(false);
            _inventoryCanvas.gameObject.SetActive(false);
            _topCanvas.gameObject.SetActive(true);
            _botCanvas.gameObject.SetActive(true);
            _playTournamentButton.interactable = true;
            _homeButton.interactable = false;
            _shopButton.interactable = true;
            _inventoryButton.interactable = true;
            SetImageBottomButtonColor(_inventoryButtonImage);
            SetImageBottomButtonColor(_shopButtonImage);
            ResetImageBottomButtonColor(_homeButtonImage);
            _rewardImage.sprite = _tournamentService.GetFights()[^1].EnemyGlobalSO.ItemSO.SpriteUI;
        }

        void ResetImageBottomButtonColor(Image image)
        {
            var imageColor = image.color;
            imageColor = Color.white;
            image.color = imageColor;
        }
        void SetImageBottomButtonColor(Image image)
        {
            var imageColor = image.color;
            imageColor = _colorBottomButtonWhenNotSelect;
            image.color = imageColor;
        }

        public void ActivateDebug()
        {
            _debugFightCanvas.SetActive(true);
            _tournamentCanvas.SetActive(false);
            _homeButton.interactable = true;
            _shopButton.interactable = true;
            _inventoryButton.interactable = true;
            ResetImageBottomButtonColor(_inventoryButtonImage);
            ResetImageBottomButtonColor(_shopButtonImage);
            ResetImageBottomButtonColor(_homeButtonImage);
        }

        public void StartFight()
        {
            _playMatchButton.interactable = false;
            if (_nextEnemyAddressableName == null)
                _nextEnemyAddressableName = _gameService.GlobalSettingsSO.AllEnemyGlobalSO[0].EnemyAddressableName;
            if (_nextEnvironmentAddressableName == null)
                _nextEnvironmentAddressableName =
                    _gameService.GlobalSettingsSO.AllEnvironmentsSO[0].EnvironmentAddressableName;
            LoadingScreenManager.Instance.gameObject.SetActive(true);
            _gameService.LoadGameScene(_nextEnvironmentAddressableName, _nextEnemyAddressableName, true, false);
        }

        private void StartFightTutorial()
        {
            LoadingScreenManager.Instance.gameObject.SetActive(true);
            _playMatchButton.interactable = false;
            _gameService.LoadGameScene("Coliseum", "ArnoldiosTutorialPrefab", false, true);
        }

        public void StartTournament()
        {
            if (_actualTournament == 0)
            {
                StartFightTutorial();
                return;
            }

            if (_actualTournament != 1)
            {
                _unlockTournament.gameObject.SetActive(true);
                _unlockTournamentPopup.ActivatePopUp();
                return;
            }

            OpenTournamentUI();
        }

        private void OpenTournamentUI()
        {
            _playTournamentButton.interactable = false;
            _tournamentCanvas.SetActive(false);
            _topCanvas.gameObject.SetActive(false);
            _botCanvas.gameObject.SetActive(false);
            _menuTournamentManager.UpdateUITournament();
        }

        public void LeftTournament()
        {
            _actualTournament -= 1;
            if (_actualTournament < 0)
            {
                _actualTournament = 0;
            }
            else
            {
                _leftArrowTournament.interactable = false;
                _rightArrowTournament.interactable = false;
                for (int i = _actualTournament - 1; i >= 0; i--)
                {
                    var index = i;
                    _tournaments[index].GetComponent<RectTransform>()
                        .DOAnchorPos(new Vector2(1920 * (index - _actualTournament), 0), 0.5f)
                        .OnComplete(() => MoveLeftTournament(index));
                }

                _tournaments[_actualTournament].SetActive(true);
                _tournaments[_actualTournament].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f);
                for (int i = _actualTournament + 1; i < _tournaments.Length; i++)
                {
                    var index = i;
                    _tournaments[index].GetComponent<RectTransform>()
                        .DOAnchorPos(new Vector2(1920 * (index - _actualTournament), 0), 0.5f)
                        .OnComplete(() => MoveLeftTournament(index));
                }
            }
        }

        private void MoveLeftTournament(int index)
        {
            _tournaments[index].SetActive(false);
            if( _actualTournament == 0)
            _leftArrowTournament.gameObject.SetActive(false);
            else _leftArrowTournament.gameObject.SetActive(true);
            _rightArrowTournament.gameObject.SetActive(true);
            _rightArrowTournament.interactable = true;
            _leftArrowTournament.interactable = true;
        }

        public void RightTournament()
        {
            _actualTournament += 1;
            if (_actualTournament > _tournaments.Length - 1)
            {
                _actualTournament = _tournaments.Length - 1;
            }
            else
            {
                _leftArrowTournament.interactable = false;
                _rightArrowTournament.interactable = false;
                for (int i = _actualTournament - 1; i >= 0; i--)
                {
                    var index = i;
                    _tournaments[index].GetComponent<RectTransform>()
                        .DOAnchorPos(new Vector2(1920 * (index - _actualTournament), 0), 0.5f)
                        .OnComplete(() => MoveRightTournament(index));
                }

                _tournaments[_actualTournament].SetActive(true);
                _tournaments[_actualTournament].GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f);
                for (int i = _actualTournament + 1; i < _tournaments.Length; i++)
                {
                    var index = i;
                    _tournaments[index].GetComponent<RectTransform>()
                        .DOAnchorPos(new Vector2(1920 * (index - _actualTournament), 0), 0.5f)
                        .OnComplete(() => MoveRightTournament(index));
                }
            }
        }

        private void MoveRightTournament(int index)
        {
            _tournaments[index].SetActive(false);
            if (_actualTournament == _tournaments.Length - 1)
                _rightArrowTournament.gameObject.SetActive(false);
            else _rightArrowTournament.gameObject.SetActive(true);
            _leftArrowTournament.gameObject.SetActive(true);
            _rightArrowTournament.interactable = true;
            _leftArrowTournament.interactable = true;
        }

        public void OpenShop()
        {
            _player.SetActive(false);
            _tournamentCanvas.SetActive(false);
            _backgroundCanvas.gameObject.SetActive(true);
            _inventoryCanvas.gameObject.SetActive(false);
            _shopCanvas.gameObject.SetActive(true);
            _homeButton.interactable = true;
            _shopButton.interactable = false;
            _inventoryButton.interactable = true;
            SetImageBottomButtonColor(_inventoryButtonImage);
            ResetImageBottomButtonColor(_shopButtonImage);
            SetImageBottomButtonColor(_homeButtonImage);
        }

        public void OpenInventory()
        {
            _player.SetActive(true);
            _tournamentCanvas.SetActive(false);
            _backgroundCanvas.gameObject.SetActive(false);
            _inventoryCanvas.gameObject.SetActive(true);
            _shopCanvas.gameObject.SetActive(false);
            _homeButton.interactable = true;
            _shopButton.interactable = true;
            _inventoryButton.interactable = false;
            _menuInventoryManager.UpdateScrollRect();
            ResetImageBottomButtonColor(_inventoryButtonImage);
            SetImageBottomButtonColor(_shopButtonImage);
            SetImageBottomButtonColor(_homeButtonImage);
        }

        public void OpenDaily()
        {
            _dailyPanel.ActivatePopUp();
        }

        public void OpenFriends()
        {
            _friendsPanel.ActivatePopUp();
        }

        public void OpenBadges()
        {
            _badgesPopup.ActivatePopUp();
        }

        private void OpenFriendPopup(Sprite picture, string name)
        {
            _friendPopupImage.sprite = picture;
            _friendPopupNameText.text = name;
            _friendPopup.ActivatePopUp();
        }

        public void CloseLockTournament()
        {
            _unlockTournament.gameObject.SetActive(false);
            _unlockTournamentPopup.gameObject.SetActive(false);
        }

        public void ClosePopup()
        {
            _friendsPanel.gameObject.SetActive(false);
            _dailyPanel.gameObject.SetActive(false);
            _badgesPopup.gameObject.SetActive(false);
        }

        public void CloseFriendPopup()
        {
            _friendPopup.gameObject.SetActive(false);
        }
    }
}