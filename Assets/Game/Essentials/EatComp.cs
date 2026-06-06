using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EatComp : MonoBehaviour
{
    Pawn _pawn;

    [SerializeField]
    [ReadOnly]
    float EatTimer = 0;
    float EatTimeout => _pawn.data.foodTimeout;
    float EatAmount => _pawn.data.foodEat;
    float MaxTimesHungry => _pawn.data.TimesHungry;
    List<ResourceCost> Resources => _pawn.data.eatResources;

    float TimesHoungry = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pawn = GetComponent<Pawn>();

        EatTimer = EatTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        EatTimer -= Time.unscaledDeltaTime * 1000f;
        if(EatTimer < 0)
        {
            EatTimer = EatTimeout;
            Eat();
        }
    }

    void Eat()
    {
        if (!_pawn.orchestrator.ApplyResourceCost(Resources, true))
        {
            TimesHoungry++;

            if (TimesHoungry >= MaxTimesHungry) Destroy(this.gameObject);
            return;
        }

        TimesHoungry = 0;
        return;
    }
}
