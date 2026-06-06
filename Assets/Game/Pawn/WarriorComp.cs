using System;
using UnityEngine;
using UnityEngine.AI;


public class WarriorComp : MonoBehaviour
{
    Pawn _pawn;

    public PawnState state { get => _pawn.state; set => _pawn.state = value; }

    public HealthComp healthComp => _pawn.HealthComp;

    float _attackTimer = 0f;

    [SerializeField]
    BuildingComp ogTarget;

    [SerializeField]
    BuildingComp target;

    [SerializeField]
    AudioClip attackSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pawn = GetComponent<Pawn>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{target != null}    {ogTarget != null}");
        if (target == null) return;

        switch (state)
        {
            case PawnState.home:
                GoToTarget();
                break;
            case PawnState.attacking:
                Attack();
                break;
            default:
                break;
        }
    }

    void GoToTarget(bool subscribeToEv = true)
    {

        //Transform HouseGatherSpot = _pawn.CurrentHouse.building.gatherSpot;
        if (!target.GetSpot(this, out Transform WorkGatherSpot)) return;
        //_pawn.transform.position = HouseGatherSpot.position;

        if (subscribeToEv)
            target.healthComp.OnDeath += OnTargetDefeated;

        state = PawnState.walking;
        _pawn.MovementComp.MoveTo(WorkGatherSpot.position, OnLocationArrive);
    }

    void OnLocationArrive(NavMeshAgent agent, bool success)
    {
        Debug.Log("Has movement been succesfull: " + success);
        if(success)
        {
            StartAttacking();
            return;
        }

        Vector3[] corners = agent.path.corners;
        if (corners.Length == 0) return;

        Vector3 lastReachablePoint = corners[corners.Length - 1];

        Vector3 dirTarget = (target.transform.position - lastReachablePoint).normalized;

        RaycastHit hit;
        float checkDistance = 2.0f;
        int buildingLayerMask = 1 << LayerMask.NameToLayer("Building");

        
        if (Physics.Raycast(lastReachablePoint + Vector3.up * 0.5f, dirTarget, out hit, checkDistance, buildingLayerMask))
        {
            Attacked(hit.collider.transform.root.GetComponent<BuildingComp>());
        
            Debug.DrawLine(lastReachablePoint, hit.point, Color.red);
        }
        else
        {
            
            Debug.DrawLine(lastReachablePoint + Vector3.up * 0.5f, lastReachablePoint + dirTarget * checkDistance + Vector3.up * 0.5f, Color.red, 1000000);
        }
    }

    void StartAttacking()
    {
        _attackTimer = 0;

        state = PawnState.attacking;
    }

    void Attack()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        _attackTimer += Time.unscaledDeltaTime;
        if (_attackTimer >= _pawn.data.attackInterval / 1000f)
        {
            _attackTimer = 0;
            target.healthComp.TakeDamage(_pawn.data.attackDamage);
        }
    }

    public void GoHome()
    {
        target = null;
        state = PawnState.home;
    }

    public void OnTargetDefeated(HealthComp destroyed)
    {
        if(destroyed == null) return;
        if (target != null && target.gameObject == destroyed.gameObject)
        {
            state = PawnState.home;
            // 1. Unsubscribe from the dead building immediately
            target.healthComp.OnDeath-= OnTargetDefeated;

            // 2. Pivot to your backup target
            target = ogTarget;
            ogTarget = null; // Clear backup since it's now primary

            // 3. Decide whether to march to the new target or go home
            if (target != null)
            {
                GoToTarget(false); // Subscribe to the new target's destruction event
            }
            else
            {
                GoHome();
            }
            return;
        }

        if (ogTarget != null && ogTarget.gameObject == destroyed.gameObject)
        {
            ogTarget.healthComp.OnDeath -= OnTargetDefeated;
            ogTarget = null;
        }
    }

    public void Attacked(BuildingComp tower)
    {
        if (ogTarget != null || ogTarget == target || target == tower || tower == null) return;

        ogTarget = target;
        target = tower;

        ogTarget.FreeSpot(this);
        GoToTarget();

        enabled = true;
    }

    public void SetTarget(Transform t)
    {
        target = t.GetComponent<BuildingComp>();
    }

    void OnDestroy()
    {
        if (target != null) {
            target.healthComp.OnDeath -= OnTargetDefeated;
        }
        if (ogTarget != null)
        {
            ogTarget.healthComp.OnDeath -= OnTargetDefeated;
        }
    }
}
