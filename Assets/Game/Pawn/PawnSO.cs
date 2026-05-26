using JetBrains.Annotations;
using UnityEngine;



[CreateAssetMenu(fileName = "PawnSO", menuName = "RTS/Pawn")]
public class PawnSO : ScriptableObject
{
    public string pawnName;
    public GameObject prefab;


    [Header("Stats")]
    public int maxHealth;
    public float moveSpeed;
    public float attackRange;
    public int attackDamage;
    [Tooltip("In milliseconds")]
    public int attackInterval; // In milliseconds

    [Header("Economy")]
    public bool canGather;
    public bool canBuild;

    public int gatherPoints;
    public float gatherPerMin;
}
