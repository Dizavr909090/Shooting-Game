using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SceneLoader), typeof(FadeEffect))]

public class UIManager : MonoBehaviour, IUIManager, IPlayerDeathListener
{
    [SerializeField] private GameObject _gameOverUI;

    [Header("Game Over UI settings")]
    [SerializeField] private Image _fadePlane;
    [SerializeField] private Color _endColorUI = Color.black;
    [SerializeField] private float _timeToChange = 1;

    private IFadeEffect _fader;
    private ISceneLoader _sceneLoader;

    private void Awake()
    {
        _fader = GetComponent<IFadeEffect>();
        _sceneLoader = GetComponent<ISceneLoader>();
    }

    public void ShowGameOver()
    {
        _fader.FadeIn(_fadePlane, _endColorUI, _timeToChange);
        _gameOverUI.SetActive(true);
    }

    //UI Input
    public void StartNewGame()
    {
        _sceneLoader.LoadGameScene();
    }

    public void OnPlayerDied()
    {
        ShowGameOver();
    }
}
