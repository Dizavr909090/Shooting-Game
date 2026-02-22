using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
[RequireComponent(typeof(TargetComponent))]
[RequireComponent(typeof(PlayerDeathComponent))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerTracker))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerFacade : MonoBehaviour
{
    public HealthComponent Health { get; private set; }
    public TargetComponent Target { get; private set; }
    public PlayerDeathComponent Death { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerTracker Tracker { get; private set; }

    private void Awake()
    {
        Target = GetComponent<TargetComponent>();
        Health = GetComponent<HealthComponent>();
        Death = GetComponent<PlayerDeathComponent>();
        Movement = GetComponent<PlayerMovement>();
        Tracker = GetComponent<PlayerTracker>();
    }
}
