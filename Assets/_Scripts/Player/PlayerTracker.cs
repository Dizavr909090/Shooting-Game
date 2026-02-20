using UnityEngine;

public class PlayerTracker : MonoBehaviour, ICampingProvider
{
    [SerializeField] private float _timeBetweenCampingChecks = 2f;
    [SerializeField] private float _campThresholdDistance = 1.5f;

    private ITargetable _playerTarget;
    private bool _isCamping;
    private Vector3 _oldPosition;
    private float _nextCheckTime;

    public bool IsCamping => _isCamping;

    private void Update()
    {
        if (_playerTarget == null || _playerTarget.IsDead)
        {
            _isCamping = false;
            return;
        }

        CampingCheck();
    }

    public void InitializePlayer(ITargetable player)
    {
        _playerTarget = player;
        _oldPosition = _playerTarget.Transform.position;
        _nextCheckTime = Time.time + _timeBetweenCampingChecks;
    }

    private void CampingCheck()
    {
        if (Time.time > _nextCheckTime)
        {
            _nextCheckTime = Time.time + _timeBetweenCampingChecks;

            float distanceMoved = Vector3.Distance(_playerTarget.Transform.position, _oldPosition);

            if (distanceMoved < _campThresholdDistance)
            {
                _isCamping = true;
            }
            else
            {
                _isCamping = false;
            }

            _oldPosition = _playerTarget.Transform.position;
        }
    }
}

