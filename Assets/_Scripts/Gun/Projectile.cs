using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _lifeTime = 2;

    private Coroutine _lifeTimeRoutine;

    private ProjectilePool _pool;
    private ProjectilePoolableComponent _poolableComponent;
    private float _skinWidth = .1f;
    private float _speed;
    private Vector3 _direction;

    public ProjectilePoolableComponent PoolableComponent => _poolableComponent;

    private void Awake()
    {
        _poolableComponent = GetComponent<ProjectilePoolableComponent>();
    }

    private void OnEnable()
    {
        _lifeTimeRoutine = StartCoroutine(ReturnToPoolAfterDelay(this));
    }

    private void Start()
    {
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, _collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    private void Update()
    {
        float moveDistance = _speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        Launch();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetPool(ProjectilePool pool)
    {
        _pool = pool;
    }

    public void ResetLogic()
    {
        _speed = 0;
    }

    public void SetSpeedAndDirection(float newSpeed, Vector3 newDirection)
    {
        _speed = newSpeed;
        _direction = newDirection.normalized;
    }

    private void Launch()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, _direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + _skinWidth,_collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    private void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(_damage, hit);
        }
        _pool.Return(this);
    }

    private void OnHitObject(Collider collider)
    {
        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(_damage);
        }
        _pool.Return(this);
    }

    private IEnumerator ReturnToPoolAfterDelay(Projectile proj)
    {
        yield return new WaitForSeconds(_lifeTime);
        _pool.Return(proj);
    }
}
