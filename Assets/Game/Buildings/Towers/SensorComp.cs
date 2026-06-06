using System;
using UnityEngine;

public class SensorComp : MonoBehaviour
{
    public event Action<WarriorComp> OnEnemyPawnEnter;
    public event Action<WarriorComp> OnEnemyPawnLeave;

    MeshRenderer meshRenderer;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent<Pawn>(out Pawn pawn) || pawn.alliance != PawnAlliance.Enemy) return;

        OnEnemyPawnEnter?.Invoke(pawn.GetComponent<WarriorComp>());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent<Pawn>(out Pawn pawn) || pawn.alliance != PawnAlliance.Enemy) return;

        OnEnemyPawnLeave?.Invoke(pawn.GetComponent<WarriorComp>());
    }

    public void ShowRange()
    {
        meshRenderer.enabled = true;
    }

    public void HideRange()
    {
        meshRenderer.enabled = false;
    }
}
