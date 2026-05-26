using System;
using UnityEngine;


public class WarriorComp : MonoBehaviour
{
    Pawn _pawn;

    public PawnState state { get => _pawn.state; set => _pawn.state = value; }

    float _attackTimer = 0f;

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

    void GoToTarget()
    {
        //Transform HouseGatherSpot = _pawn.CurrentHouse.building.gatherSpot;
        if (!target.GetSpot(this, out Transform WorkGatherSpot)) return;
        //_pawn.transform.position = HouseGatherSpot.position;

        state = PawnState.walking;
        _pawn.MovementComp.MoveTo(WorkGatherSpot.position, (bool b) => { Debug.Log("I arrived"); StartAttacking(); });
    }

    void StartAttacking()
    {
        


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
}
