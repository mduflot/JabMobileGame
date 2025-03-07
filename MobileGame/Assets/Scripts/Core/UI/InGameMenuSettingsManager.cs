using DG.Tweening;
using Service;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuSettingsManager : MonoBehaviour
{
    [Header("Settings UI")] 
    [SerializeField] private Button _openSettingsButton;
    [SerializeField] private Image _settingsMenu;
    [SerializeField] private Button _closeSettingsButton;
    [SerializeField] private Button _reportButton;
    [SerializeField] private Button _leaveGame;

    private ISceneService _sceneService;

    private void Awake()
    {
        _settingsMenu.transform.DOScale(0f, 0f).SetEase(Ease.OutBack);
    }

    public void Init()
    {
      
        _openSettingsButton.onClick.AddListener(OpenSettings);
        _closeSettingsButton.onClick.AddListener(CloseSettings);
        // _reportButton.onClick.AddListener();
       // _leaveGame.onClick.AddListener(() => _sceneService.LoadScene("MenuScene", null));
    }

    
    
    private void OpenSettings()
    {
        _openSettingsButton.transform.DOKill();
        _openSettingsButton.transform.DOScale(0f, 0.25f).SetEase(Ease.OutBack);
        _settingsMenu.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    private void CloseSettings()
    {
        _settingsMenu.transform.DOKill();
        _settingsMenu.transform.DOScale(0f, 0.25f).SetEase(Ease.OutBack);
        _openSettingsButton.transform.DOKill();
        _openSettingsButton.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }
}