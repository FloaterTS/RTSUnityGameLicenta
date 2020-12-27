using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit))]
public class Worker : MonoBehaviour
{
    private Unit unit;
    private Animator animator;
    private NavMeshObstacle navWorkerObstacle;
    private CarriedResource carriedResource;
    private bool onWayToTask = false;
    private bool constructionSiteReached = false;

    void Start()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        navWorkerObstacle = GetComponent<NavMeshObstacle>();

        carriedResource.amount = 0;
    }

    public bool IsBusy()
    {
        return onWayToTask;
    }

    public void SetConstructionSiteReached(bool reached)
    {
        constructionSiteReached = reached;
    }

    public void CollectResource(ResourceField targetResource)
    {
        StartCoroutine(CollectResourceCo(targetResource));
    }

    public void StoreResource(ResourceCamp resourceCamp)
    {
        StartCoroutine(StoreResourceCo(resourceCamp));
    }

    public void StoreResourceInClosestCamp()
    {
        StartCoroutine(StoreResourceInClosestCampCo());
    }

    public void StartConstruction(GameObject underConstructionBuilding)
    {
        StartCoroutine(StartConstructionCo(underConstructionBuilding));
    }

    public IEnumerator StopTaskCo()
    {
        unit.unitState = UnitState.idle;
        animator.SetBool("working", false);
        navWorkerObstacle.enabled = false;
        yield return null;
        unit.EnableNavAgent(true);
        unit.StopNavAgent();
    }

    private IEnumerator StartConstructionCo(GameObject underConstructionBuilding)
    {
        if (unit.target == underConstructionBuilding.transform.position)
            yield break;

        yield return StartCoroutine(GoToConstructionSite(underConstructionBuilding));

        if (underConstructionBuilding == null || unit.target != underConstructionBuilding.transform.position)
            yield break;

        yield return StartCoroutine(ConstructBuilding(underConstructionBuilding));
    }

    private IEnumerator GoToConstructionSite(GameObject underConstructionBuilding)
    {
        yield return StartCoroutine(unit.MoveToLocationCo(underConstructionBuilding.transform.position));
        onWayToTask = true;

        while (!constructionSiteReached && underConstructionBuilding != null && unit.target == underConstructionBuilding.transform.position)
            yield return null;

        onWayToTask = false;
    }

    private IEnumerator ConstructBuilding(GameObject underConstructionBuilding)
    {
        StartTask();
        Vector3 underConstructionBuildingPosition = underConstructionBuilding.transform.position;
        transform.LookAt(new Vector3(underConstructionBuildingPosition.x, transform.position.y, underConstructionBuildingPosition.z));
        animator.SetTrigger("hammering");
        yield return new WaitForSeconds(0.5f); //animation transition duration

        if (underConstructionBuilding == null) // check if building was finished during animation transition
        {
            if(unit.target == underConstructionBuildingPosition)
                yield return StartCoroutine(StopTaskCo());
            yield break;
        }

        Building building = underConstructionBuilding.GetComponent<Building>();
        UnderConstruction underConstruction = underConstructionBuilding.GetComponent<UnderConstruction>();
        while (underConstructionBuilding != null && underConstruction.BuiltPercentage() < 100f && unit.target == underConstructionBuildingPosition)
        {
            yield return null;
            underConstruction.Construct(Time.deltaTime);
            float percentageConstructedThisFrame = Time.deltaTime * 100f / building.buildingStats.constructionTime;
            building.Repair(percentageConstructedThisFrame * building.buildingStats.maxHitPoints / 100f);
        }
        if (unit.target == underConstructionBuildingPosition)
            yield return StartCoroutine(StopTaskCo());
    }

    private IEnumerator CollectResourceCo(ResourceField resourceToCollect)
    {
        if (unit.target == resourceToCollect.transform.position)
            yield break;

        yield return StartCoroutine(GoToResourceCo(resourceToCollect));

        if (unit.target != resourceToCollect.transform.position)
            yield break;

        if (resourceToCollect.leftAmount == 0)
            yield break; //Change this to search for another field in certain area around former field, and if not found then break

        yield return StartCoroutine(HarvestResourceCo(resourceToCollect));

        if (unit.target != resourceToCollect.transform.position)
            yield break;

        yield return StartCoroutine(StoreResourceInClosestCampCo());

        ResourceCamp campStoredInto = FindClosestResourceCampByType(ResourceManager.ResourceRawToType(resourceToCollect.resourceInfo.resourceRaw));
        if (campStoredInto != null)
            if (unit.target == campStoredInto.accessLocation)
                CollectResource(resourceToCollect);
    }

    private IEnumerator GoToResourceCo(ResourceField resourceToGoTo)
    {
        yield return StartCoroutine(unit.MoveToLocationCo(resourceToGoTo.transform.position));
        onWayToTask = true;

        while (Vector3.Distance(transform.position, resourceToGoTo.transform.position) > resourceToGoTo.collectDistance
            && unit.target == resourceToGoTo.transform.position)
            yield return null;

        onWayToTask = false;
        if (unit.target == resourceToGoTo.transform.position) //not needed - unit will be stopped by HarvestResourceCo anyway
        {
            unit.StopNavAgent();
            unit.unitState = UnitState.idle;
        }
    }

    private IEnumerator HarvestResourceCo(ResourceField resourceToHarvest)
    {
        StartTask();
        transform.LookAt(new Vector3(resourceToHarvest.transform.position.x, transform.position.y, resourceToHarvest.transform.position.z));
        animator.SetTrigger(resourceToHarvest.resourceInfo.harvestAnimation);

        if (carriedResource.resourceInfo == null)
            carriedResource.resourceInfo = resourceToHarvest.resourceInfo;
        else if (carriedResource.resourceInfo.resourceRaw != resourceToHarvest.resourceInfo.resourceRaw)
        {
            carriedResource.resourceInfo = resourceToHarvest.resourceInfo;
            carriedResource.amount = 0;
        }

        while (carriedResource.amount < unit.unitStats.carryCapactity && resourceToHarvest.leftAmount > 0
            && unit.target == resourceToHarvest.transform.position)
        {
            yield return new WaitForSeconds(resourceToHarvest.resourceInfo.harvestTimePerUnit);
            carriedResource.amount += resourceToHarvest.HarvestResourceField();
        }
    }

    private IEnumerator StoreResourceCo(ResourceCamp resourceCamp)
    {
        if (unit.target == resourceCamp.accessLocation)
            yield break;

        yield return StartCoroutine(unit.MoveToLocationCo(resourceCamp.accessLocation));
        onWayToTask = true;

        while (Vector3.Distance(transform.position, resourceCamp.accessLocation) > resourceCamp.accessDistance
            && unit.target == resourceCamp.accessLocation 
            && (carriedResource.amount == 0 || resourceCamp.campType == ResourceType.None || resourceCamp.campType == carriedResource.resourceInfo.resourceType))
            yield return null;

        onWayToTask = false;

        if (unit.target == resourceCamp.accessLocation)
        {
            if (carriedResource.amount == 0)
                unit.StopNavAgent();
            else if (resourceCamp.campType != ResourceType.None && resourceCamp.campType != carriedResource.resourceInfo.resourceType)
                yield return StartCoroutine(StoreResourceInClosestCampCo());
            else
            {
                StartTask();
                float storeTimePerUnit = carriedResource.resourceInfo.storeDuration / carriedResource.amount;
                while (carriedResource.amount > 0 && unit.target == resourceCamp.accessLocation) //MAYBE MAKE IMMOBILE ?
                {
                    yield return new WaitForSeconds(storeTimePerUnit);
                    resourceCamp.StoreResourceInCamp(1, carriedResource.resourceInfo.resourceType);
                    carriedResource.amount--;
                }
                if (unit.target == resourceCamp.accessLocation)
                    yield return StartCoroutine(StopTaskCo());
            }
        }
    }

    private IEnumerator StoreResourceInClosestCampCo()
    {
        ResourceCamp campToStoreInto = FindClosestResourceCampByType(carriedResource.resourceInfo.resourceType);
        if (campToStoreInto != null)
            yield return StartCoroutine(StoreResourceCo(campToStoreInto));
        else
        {
            yield return StartCoroutine(StopTaskCo());
            Debug.Log("No camp to store resource into.");
        }
    }

    private ResourceCamp FindClosestResourceCampByType(ResourceType searchedResourceType)
    {
        ResourceCamp closestResourceCamp = null;
        float minDistanceFromUnit = 1000f;
        float distanceFromUnit;
        foreach (ResourceCamp resourceCamp in ResourceManager.instance.resourceCamps)
        {
            if (resourceCamp.campType == ResourceType.None || resourceCamp.campType == searchedResourceType)
            {
                distanceFromUnit = Vector3.Distance(transform.position, resourceCamp.transform.position);
                if (distanceFromUnit < minDistanceFromUnit)
                {
                    minDistanceFromUnit = distanceFromUnit;
                    closestResourceCamp = resourceCamp;
                }
            }
        }
        return closestResourceCamp;
    }

    private void StartTask()
    {
        unit.unitState = UnitState.working;
        animator.SetBool("working", true);
        unit.EnableNavAgent(false);
        navWorkerObstacle.enabled = true;
    }
 
    
}
