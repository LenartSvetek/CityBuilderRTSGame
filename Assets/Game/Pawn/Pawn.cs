using UnityEngine;

[System.Serializable]
public enum PawnAlliance
{
    Player = 0,
    Enemy = 1,
    Neutral = 2
}

[System.Serializable]
public enum PawnState
{
    waitingForWork = 0,
    working = 1,
    walking = 2,
    home = 3,
    attacking = 4
}


public class Pawn : MonoBehaviour
{
    public PawnSO data;
    public int currentHealth;
    [SerializeField]
    private PawnAlliance _alliance;
    public PawnAlliance alliance => _alliance;
    public PawnState state = PawnState.home;

    [SerializeField]
    private WorkerComp _workerComp;

    [SerializeField]
    private CapsuleCollider _capsuleCollider;
    public CapsuleCollider CapsuleCollider => _capsuleCollider;

    [SerializeField]
    private HouseComp _currentHouse;

    [SerializeField]
    private WorkshopComp _workingAt;
    public WorkshopComp WorkingPlace => _workingAt;

    public bool isWorking => _workingAt != null;

    public HouseComp CurrentHouse => _currentHouse;

    [SerializeField]
    private PawnMovement _movementComp;
    public PawnMovement MovementComp => _movementComp;

    protected virtual void Awake()
    {
        currentHealth = data.maxHealth;

        _workerComp = GetComponent<WorkerComp>();
        _movementComp = GetComponent<PawnMovement>();

        _capsuleCollider = GetComponentInChildren<CapsuleCollider>();
    }

    public void SetHome(HouseComp house)
    {
        _currentHouse = house;
    }

    public void SetWorkshop(WorkshopComp workshop)
    {
        _workingAt = workshop;
        _workerComp.enabled = true;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }


    public bool CheckPosition(Transform position)
    {
        Vector3 center = position.TransformPoint(_capsuleCollider.center);

        Debug.DrawRay(center, Vector3.up * 2, Color.red, 2f);

        Vector3 lossyScale = transform.lossyScale;
        float maxRadiusScale = 1f;

        maxRadiusScale = Mathf.Max(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.z));

        float worldRadius = _capsuleCollider.radius * maxRadiusScale;


        float heightScale = Mathf.Abs(lossyScale[_capsuleCollider.direction]);
        float worldHeight = _capsuleCollider.height * heightScale;

        float halfCylinderHeight = Mathf.Max(0, (worldHeight * 0.5f) - worldRadius);

        Vector3 localDirection = Vector3.zero;
        localDirection[_capsuleCollider.direction] = 1f;


        Vector3 worldDirection = transform.TransformDirection(localDirection);


        Vector3 point1 = center + worldDirection * halfCylinderHeight;
        Vector3 point2 = center - worldDirection * halfCylinderHeight;
        bool isHit = Physics.CheckCapsule(point1, point2, worldRadius, LayerMask.GetMask("Units"));

        Debug.Log($"Checking position for {gameObject.name} at {position.position}. Capsule points: {point1}, {point2}, radius: {worldRadius}. Hit: {isHit}");

        return !isHit;
    }
}
