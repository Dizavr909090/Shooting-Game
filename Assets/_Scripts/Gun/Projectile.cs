using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _damage = 1;
    [SerializeField] private float _lifeTime = 2;

    private ProjectilePool _pool;
    private ProjectilePoolableComponent _poolableComponent;
    private float _skinWidth = .1f;
    private float _speed;

    public ProjectilePoolableComponent PoolableComponent => _poolableComponent;

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

    public void ResetLogic()
    {
        _speed = 0;
    }

    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }

    private void Launch()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
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
        GameObject.Destroy(gameObject);
    }

    private void OnHitObject(Collider collider)
    {
        IDamageable damageableObject = collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(_damage);
        }
        GameObject.Destroy(gameObject);
    }
}
