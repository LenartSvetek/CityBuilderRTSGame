using System;
using UnityEngine;


public class WarriorComp : MonoBehaviour
{
    Pawn _pawn;

    public PawnState state { get => _pawn.state; set => _pawn.state = value; }

    float _attackTimer = 0f;

    [SerializeField]
    BuildingComp ogTarget;

    [SerializeField]
    BuildingComp target;

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
        Debug.Log($"target: {target} ogTarget: {ogTarget}");
        //Transform HouseGatherSpot = _pawn.CurrentHouse.building.gatherSpot;
        if (!target.GetSpot(this, out Transform WorkGatherSpot)) return;
        //_pawn.transform.position = HouseGatherSpot.position;

        if (subscribeToEv)
            target.OnBuildingDestroyed += OnTargetDefeated;

        state = PawnState.walking;
        _pawn.MovementComp.MoveTo(WorkGatherSpot.position, (bool b) => { StartAttacking(); });
    }

    void StartAttacking()
    {
        Debug.Log($"target: {target} ogTarget: {ogTarget}");
        _attackTimer = 0;

        state = PawnState.attacking;
    }

    void Attack()
    {
        _attackTimer += Time.unscaledDeltaTime;
        if (_attackTimer >= _pawn.data.attackInterval / 1000f)
        {
            _attackTimer = 0;
            target.TakeDamage(_pawn.data.attackDamage);
        }
    }

    public void GoHome()
    {
        target = null;
        state = PawnState.home;
    }

    public void OnTargetDefeated(BuildingComp destroyed)
    {
        Debug.Log(gameObject.name + " handling destruction of " + destroyed.name);

        if (target == destroyed)
        {
            state = PawnState.home;
            // 1. Unsubscribe from the dead building immediately
            target.OnBuildingDestroyed -= OnTargetDefeated;

            // 2. Pivot to your backup target
            target = ogTarget;
            ogTarget = null; // Clear backup since it's now primary

            // 3. Decide whether to march to the new target or go home
            if (target != null)
            {
                GoToTarget(true); // Subscribe to the new target's destruction event
            }
            else
            {
                GoHome();
            }
            return;
        }

        if (ogTarget == destroyed)
        {
            ogTarget.OnBuildingDestroyed -= OnTargetDefeated;
            ogTarget = null;
        }
    }

    public void Attacked(BuildingComp tower)
    {
        Debug.Log("Attacked");
        if (ogTarget != null || ogTarget == target || target == tower || tower == null) return;

        ogTarget = target;
        target = tower;

        ogTarget.FreeSpot(this);
        GoToTarget();

        enabled = true;
    }

    void OnDestroy()
    {
        if (target != null) {
            target.OnBuildingDestroyed -= OnTargetDefeated;
        }
        if (ogTarget != null)
        {
            ogTarget.OnBuildingDestroyed -= OnTargetDefeated;
        }
    }
}
