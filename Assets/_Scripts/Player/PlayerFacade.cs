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
    public static PlayerFacade Instance { get; private set; }

    public HealthComponent Health { get; private set; }
    public TargetComponent Target { get; private set; }
    public PlayerDeathComponent Death { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerTracker Tracker { get; private set; }
    public Transform Transform { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Target = GetComponent<TargetComponent>();
        Health = GetComponent<HealthComponent>();
        Death = GetComponent<PlayerDeathComponent>();
        Movement = GetComponent<PlayerMovement>();
        Tracker = GetComponent<PlayerTracker>();
    }
}
