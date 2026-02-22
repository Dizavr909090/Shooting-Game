using UnityEngine;

public class PlayerDeathComponent : MonoBehaviour
{
    [SerializeField] private PlayerFacade _player;
    [SerializeField] private MonoBehaviour _uiManagerBehaviour;

    private IUIManager _uiManager;

    private void Awake()
    {
        if ( _player == null )
            _player = GetComponent<PlayerFacade>();

        if (_uiManagerBehaviour != null)
            _uiManager = _uiManagerBehaviour as IUIManager;
    }

    private void Start() => _player.Health.OnDeath += OnPlayerDeath;

    private void OnDestroy() => _player.Health.OnDeath -= OnPlayerDeath;

    private void OnPlayerDeath(IHealth _)
    {
        _uiManager.ShowGameOver();

        _player.gameObject.SetActive(false);

        //_player.Movement.DisableMovement();
    }
}
