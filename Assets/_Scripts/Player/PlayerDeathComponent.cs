using UnityEngine;

public class PlayerDeathComponent : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _uiManagerBehaviour;

    private IUIManager _uiManager;

    private void Awake()
    {
        if (_uiManagerBehaviour != null)
            _uiManager = _uiManagerBehaviour as IUIManager;
    }

    private void Start() => PlayerFacade.Instance.Health.OnDeath += OnPlayerDeath;

    private void OnDestroy() => PlayerFacade.Instance.Health.OnDeath -= OnPlayerDeath;

    private void OnPlayerDeath(IHealth _)
    {
        _uiManager.ShowGameOver();

        PlayerFacade.Instance.gameObject.SetActive(false);

        //_player.Movement.DisableMovement();
    }
}
