using UnityEngine;
using UnityEngine.UI;

public class ReloadUI : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0.5f, 2f, 0f);
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AmmoHandler _currentAmmoHandler;
    [SerializeField] private GunEventChannelSO _gunEquipChannel;

    private void Awake()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0;
        fillImage.fillAmount = 0;
    }

    private void OnEnable()
    {
        _gunEquipChannel.EventRaised += HandleGunChanged;

        if (_gunEquipChannel.CurrentGun != null)
        {
            HandleGunChanged(_gunEquipChannel.CurrentGun);
        }
    }

    private void OnDisable()
    {
        _gunEquipChannel.EventRaised -= HandleGunChanged;

        if (_currentAmmoHandler != null)
        {
            _currentAmmoHandler.ReloadProgressChanged -= UpdateProgress;
        }
    }

    private void LateUpdate()
    {
        transform.position = PlayerFacade.Instance.transform.position + _offset;
        transform.rotation = Camera.main.transform.rotation;
    }

    public void UpdateProgress(float progress)
    {
        fillImage.fillAmount = progress;

        if (canvasGroup != null) canvasGroup.alpha = progress > 0 && progress < 1 ? 1 : 0;
    }

    private void HandleGunChanged(Gun gun)
    {
        if (_currentAmmoHandler != null)
        {
            _currentAmmoHandler.ReloadProgressChanged -= UpdateProgress;
        }

        _currentAmmoHandler = gun.GetComponent<AmmoHandler>();

        if (_currentAmmoHandler != null)
        {
            _currentAmmoHandler.ReloadProgressChanged += UpdateProgress;

            UpdateProgress(0f);
        }
    }
}