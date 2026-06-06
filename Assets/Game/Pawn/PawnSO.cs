using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
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

    public int foodEat = 5;
    [Tooltip("In ms")]
    public float foodTimeout = 30 * 1000f;
    [Tooltip("How many times can miss food")]
    public int TimesHungry = 3;

    public List<ResourceCost> eatResources; 

}
