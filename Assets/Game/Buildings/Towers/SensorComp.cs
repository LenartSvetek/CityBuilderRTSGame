using System;
using UnityEngine;

public class SensorComp : MonoBehaviour
{
    public event Action<Pawn> OnEnemyPawnEnter;
    public event Action<Pawn> OnEnemyPawnLeave;


    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (!other.transform.root.TryGetComponent<Pawn>(out Pawn pawn) || pawn.alliance != PawnAlliance.Enemy) return;

        OnEnemyPawnEnter?.Invoke(pawn);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent<Pawn>(out Pawn pawn) || pawn.alliance != PawnAlliance.Enemy) return;

        OnEnemyPawnLeave?.Invoke(pawn);
    }
}
