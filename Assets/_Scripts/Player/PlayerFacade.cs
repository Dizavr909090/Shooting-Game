using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(TargetComponent))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTracker))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerFacade : MonoBehaviour
{
    public HealthComponent Health { get; private set; }
    public TargetComponent Target { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerTracker Tracker { get; private set; }

    private void Awake()
    {
        Target = GetComponent<TargetComponent>();
        Health = GetComponent<HealthComponent>();
        Movement = GetComponent<PlayerMovement>();
        Tracker = GetComponent<PlayerTracker>();
    }
}
