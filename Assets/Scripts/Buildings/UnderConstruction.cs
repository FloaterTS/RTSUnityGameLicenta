using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class UnderConstruction : MonoBehaviour
{
    public GameObject constructedBuildingPrefab;

    private Building building;
    private float amountConstructed = 0f;

    private void Start()
    {
        building = GetComponent<Building>();
    }

    private void Update()
    {
        if (BuiltPercentage() >= 100f)
            FinishConstruction();
    }

    public void Construct(float amount)
    {
        amountConstructed += amount;
    }

    public float BuiltPercentage()
    {
        return amountConstructed * 100 / building.buildingStats.constructionTime;
    }


    public float GetAmountConstructed()
    {
        return amountConstructed;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Unit"))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit.worker != null)
            {
                if (unit.target == transform.position && unit.unitState == UnitState.moving)
                    unit.worker.SetConstructionSiteReached(true);
                else
                    unit.worker.SetConstructionSiteReached(false);
            }   
        }
    }

    private void FinishConstruction()
    {
        GameObject finishedBuilding = Instantiate(constructedBuildingPrefab, transform.position, transform.rotation, PrefabManager.instance.buildingsTransformParentGO.transform);
        finishedBuilding.GetComponent<Building>().SetInitialHitpoints(building.GetCurrentHitpoints());
        Destroy(gameObject);
    }
}
