using UnityEngine;

public class PlayerTracker : MonoBehaviour, ICampingProvider
{
    [SerializeField] private float _timeBetweenCampingChecks = 1f;
    [SerializeField] private float _campThresholdDistance = 1.5f;

    private Entity _playerEntity;
    private bool _isCamping;
    private Vector3 _oldPosition;
    private float _nextCheckTime;

    public bool IsCamping => _isCamping;

    private void Update()
    {
        if (_playerEntity == null || _playerEntity.IsDead)
        {
            _isCamping = false;
            return;
        }

        CampingCheck();
    }

    public void InitializePlayer(Entity player)
    {
        _playerEntity = player;
        _oldPosition = _playerEntity.transform.position;
    }

    private void CampingCheck()
    {
        if (Time.time > _nextCheckTime)
        {
            _nextCheckTime = Time.time + _timeBetweenCampingChecks;

            float distanceMoved = Vector3.Distance(_playerEntity.transform.position, _oldPosition);

            if (distanceMoved < _campThresholdDistance)
            {
                _isCamping = true;
            }
            else
            {
                _isCamping = false;
            }

            _oldPosition = _playerEntity.transform.position;
        }
    }
}

