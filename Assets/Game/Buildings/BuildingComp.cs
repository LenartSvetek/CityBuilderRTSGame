using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingComp : MonoBehaviour
{
    [SerializeField]
    List<GameObject> models = new List<GameObject>();

    [SerializeField]
    private Transform[] _attackSpots;
    public Transform[] attackSpots => _attackSpots;

    private WarriorComp[] _freeSpaces;

    [SerializeField]
    [ReadOnly]
    private Transform _GatherSpot;
    public Transform gatherSpot => _GatherSpot;

    HealthComp _healthComp;
    public HealthComp healthComp => _healthComp;

    private void Awake()
    {
        _healthComp = GetComponent<HealthComp>();

        Transform model = transform.Find("Model");

        if (model == null)
        {
            int rInd = Mathf.FloorToInt(UnityEngine.Random.value * models.Count) % models.Count;

            GameObject obj = Instantiate(models[rInd], transform);
            //obj.transform.localPosition = Vector3.zero;
            obj.name = "Model";
        }
        _GatherSpot = transform.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GatherSpot") && t.name == "main").FirstOrDefault();
        _attackSpots = transform.GetComponentsInChildren<Transform>().Where(t => t.CompareTag("GatherSpot")).ToArray();

        _freeSpaces = Enumerable.Repeat<WarriorComp>(null, _attackSpots.Length).ToArray();
    }

    public bool GetSpot(WarriorComp attacker, out Transform spot)
    {
        for (int i = 0; i < _attackSpots.Length; i++)
        {
            if (_freeSpaces[i] == null)
            {
                spot = _attackSpots[i];
                _freeSpaces[i] = attacker;
                return true;
            }
        }
        spot = null;
        return false;
    }

    public void FreeSpot(WarriorComp attacker)
    {
        for (int i = 0; i < _attackSpots.Length; i++)
        {
            if (_freeSpaces[i] == attacker)
            {
                _freeSpaces[i] = null;
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(_attackSpots == null || _freeSpaces == null || _attackSpots == null) return;

        Gizmos.color = Color.red;
        for(int i = 0; i < _attackSpots.Length; i++)
        {
            if (_freeSpaces[i] == null)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            Gizmos.DrawSphere(_attackSpots[i].position, 0.2f);
        }
      
    }
}
