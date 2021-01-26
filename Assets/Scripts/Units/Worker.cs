using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Unit))]
public class Worker : MonoBehaviour
{
    private Unit unit;
    private Animator animator;
    private Transform thingInHand;
    private NavMeshObstacle navWorkerObstacle;
    private CarriedResource carriedResource;
    private bool onWayToTask = false;
    private bool constructionSiteReached = false;
    private bool immobile = false;

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
        if (unit.unitState != UnitState.working)
            yield break;
        unit.unitState = UnitState.idle;
        animator.SetBool("working", false);
        navWorkerObstacle.enabled = false;
        yield return null;
        unit.EnableNavAgent(true);
        unit.StopNavAgent();
    }

    public IEnumerator CheckIfImmobile()
    {
        yield return null;
        while (immobile)
            yield return null;
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
        if (carriedResource.amount > 0)
        {
            yield return StartCoroutine(LetDownResource());
            DropResource();
        }
        StartTask();
        Vector3 underConstructionBuildingPosition = underConstructionBuilding.transform.position;
        transform.LookAt(new Vector3(underConstructionBuildingPosition.x, transform.position.y, underConstructionBuildingPosition.z));
        animator.SetTrigger("hammering");
        yield return new WaitForSeconds(0.5f); //animation transition duration

        if (underConstructionBuilding == null) // check if building was finished during animation transition
        {
            if (unit.target == underConstructionBuildingPosition)
                yield return StartCoroutine(StopTaskCo());
            yield break;
        }

        Building building = underConstructionBuilding.GetComponent<Building>();
        UnderConstruction underConstruction = underConstructionBuilding.GetComponent<UnderConstruction>();

        if (thingInHand != null)
            thingInHand.gameObject.SetActive(false);
        thingInHand = transform.Find(building.buildingStats.toolConstructionName);
        if (thingInHand != null)
            thingInHand.gameObject.SetActive(true);

        while (underConstructionBuilding != null && underConstruction.BuiltPercentage() < 100f && unit.target == underConstructionBuildingPosition)
        {
            yield return null;
            underConstruction.Construct(Time.deltaTime);
            float percentageConstructedThisFrame = Time.deltaTime * 100f / building.buildingStats.constructionTime;
            building.Repair(percentageConstructedThisFrame * building.buildingStats.maxHitPoints / 100f);
        }
        if (unit.target == underConstructionBuildingPosition)
            yield return StartCoroutine(StopTaskCo());

        if (thingInHand != null)
            thingInHand.gameObject.SetActive(false);
    }

    private IEnumerator CollectResourceCo(ResourceField resourceToCollect)
    {
        Vector3 targetResourcePosition = resourceToCollect.transform.position;

        if (unit.target == targetResourcePosition)
            yield break;

        if (carriedResource.amount > 0 && carriedResource.resourceInfo != resourceToCollect.resourceInfo)
        {
            yield return StartCoroutine(LetDownResource());
            DropResource();
            carriedResource.resourceInfo = resourceToCollect.resourceInfo;
        }

        yield return StartCoroutine(GoToResourceCo(resourceToCollect));

        if (unit.target != targetResourcePosition)
            yield break;

        if (resourceToCollect == null)
            yield break; //Change this to search for another field in certain area around former field, and if not found then break

        yield return StartCoroutine(HarvestResourceCo(resourceToCollect));

        if (unit.target != targetResourcePosition || carriedResource.amount == 0)
            yield break;

        yield return StartCoroutine(StoreResourceInClosestCampCo());

        ResourceCamp campStoredInto = FindClosestResourceCampByType(ResourceManager.ResourceRawToType(resourceToCollect.resourceInfo.resourceRaw));
        if (campStoredInto != null)
            if (unit.target == campStoredInto.accessLocation && resourceToCollect != null)
                CollectResource(resourceToCollect);
    }

    private IEnumerator GoToResourceCo(ResourceField resourceToGoTo)
    {
        yield return StartCoroutine(unit.MoveToLocationCo(resourceToGoTo.transform.position));
        onWayToTask = true;

        if (thingInHand != null)
            thingInHand.gameObject.SetActive(false);
        thingInHand = transform.Find(resourceToGoTo.resourceInfo.toolInHandName);
        if (thingInHand != null)
            thingInHand.gameObject.SetActive(true);

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

        if (carriedResource.amount == 0)
            carriedResource.resourceInfo = resourceToHarvest.resourceInfo;

        transform.LookAt(new Vector3(resourceToHarvest.transform.position.x, transform.position.y, resourceToHarvest.transform.position.z));
        animator.SetTrigger(resourceToHarvest.resourceInfo.harvestAnimation);

        if (thingInHand != null)
            thingInHand.gameObject.SetActive(false);
        thingInHand = transform.Find(resourceToHarvest.resourceInfo.toolHarvestingName);
        if (thingInHand != null)
            thingInHand.gameObject.SetActive(true);

        float timeElapsed = 0f;
        while (carriedResource.amount < unit.unitStats.carryCapactity && resourceToHarvest != null
            && unit.target == resourceToHarvest.transform.position)
        {
            yield return null;
            timeElapsed += Time.deltaTime;
            if (timeElapsed > resourceToHarvest.resourceInfo.harvestTimePerUnit * unit.unitStats.harvestSpeedMultiplier)
            {
                carriedResource.amount += resourceToHarvest.HarvestResourceField();
                timeElapsed = 0f;
            }
        }

        if (thingInHand != null)
            thingInHand.gameObject.SetActive(false);

        if (carriedResource.amount > 0)
            StartCoroutine(LiftResource());
        yield return StartCoroutine(StopTaskCo());
    }

    private IEnumerator StoreResourceCo(ResourceCamp resourceCamp)
    {
        if (unit.target == resourceCamp.accessLocation)
            yield break;

        yield return StartCoroutine(unit.MoveToLocationCo(resourceCamp.accessLocation));

        yield return GoToCamp(resourceCamp);

        if (unit.target == resourceCamp.accessLocation)
        {
            if (carriedResource.amount == 0)
                yield break;
            if (resourceCamp.campType != ResourceType.None && resourceCamp.campType != carriedResource.resourceInfo.resourceType)
                yield return StartCoroutine(StoreResourceInClosestCampCo());
            else
            {
                StartTask();
                yield return StartCoroutine(LetDownResource());
                resourceCamp.StoreResourceInCamp(carriedResource.amount, carriedResource.resourceInfo.resourceType);
                carriedResource.amount = 0;

                if (unit.target == resourceCamp.accessLocation)
                    yield return StartCoroutine(StopTaskCo());
            }
        }
    }

    private IEnumerator GoToCamp(ResourceCamp resourceCamp)
    {
        onWayToTask = true;

        while (Vector3.Distance(transform.position, resourceCamp.accessLocation) > resourceCamp.accessDistance
            && unit.target == resourceCamp.accessLocation
            && (carriedResource.amount == 0 || resourceCamp.campType == ResourceType.None || resourceCamp.campType == carriedResource.resourceInfo.resourceType))
            yield return null;

        onWayToTask = false;
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

    private IEnumerator LiftResource()
    {
        unit.StopNavAgent();

        animator.SetBool(carriedResource.resourceInfo.carryAnimation, true);

        unit.ChangeUnitSpeed(carriedResource.resourceInfo.carrySpeed);

        thingInHand = transform.Find(carriedResource.resourceInfo.carriedResourceName);
        if (thingInHand != null)
            thingInHand.gameObject.SetActive(true);

        immobile = true;
        yield return new WaitForSeconds(carriedResource.resourceInfo.liftAnimationDuration);
        immobile = false;
    }

    private IEnumerator LetDownResource()
    {
        unit.StopNavAgent();

        animator.SetBool(carriedResource.resourceInfo.carryAnimation, false);

        unit.ChangeUnitSpeed(UnitSpeed.run);

        immobile = true;
        yield return new WaitForSeconds(carriedResource.resourceInfo.dropAnimationDuration);
        immobile = false;

        if (thingInHand != null)
            thingInHand.gameObject.SetActive(false);
    }

    private void DropResource()
    {
        //To be fully implemented
        carriedResource.amount = 0;
    }

}
