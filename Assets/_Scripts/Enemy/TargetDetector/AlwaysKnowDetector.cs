using System.Collections;
using UnityEngine;

public class AlwaysKnowDetector : BaseDetector
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float _detectionInterval = 0.1f;

    private CapsuleCollider _playerCollider;
    private Coroutine _detectionCoroutine;
    private bool _isDetectionActive = true;

    private void Awake()
    {
        _playerCollider = PlayerFacade.Instance.GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        _detectionCoroutine = StartCoroutine(DetectionRoutine());
    }

    private void OnDisable()
    {
        if (_detectionCoroutine != null)
        {
            StopCoroutine(_detectionCoroutine);
            _detectionCoroutine = null;
        }
    }

    private IEnumerator DetectionRoutine()
    {
        while (_isDetectionActive)
        {
            Detect();
            yield return new WaitForSeconds(_detectionInterval);
        }
    }

    protected override void Detect()
    {
        _currentTarget = PlayerFacade.Instance.Target;
        _isVisible = false;

        if (!CheckRaycastHit(out RaycastHit hit))
        {
            Debug.DrawRay(transform.position,
            (PlayerFacade.Instance.transform.position - transform.position).normalized * 10f, Color.red);
            return;
        }

        if (hit.collider == _playerCollider)
        {
            _isVisible = true;
            Debug.DrawLine(transform.position, hit.point, Color.green);
        }
    }

    private bool CheckRaycastHit(out RaycastHit hit)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = PlayerFacade.Instance.transform.position;

        Vector3 direction = targetPos - startPos;

        bool isRaycast = Physics.Raycast(
            startPos, 
            direction.normalized,
            out hit,
            direction.magnitude,
            collisionMask);

        return isRaycast;
    }
}
