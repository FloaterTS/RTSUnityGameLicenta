using UnityEngine;

public enum UnitState
{
    idle,
    moving,
    working,
    attacking
}

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    private SelectionManager selectionManager;


    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another unit interaction manager present.");

        selectionManager = GetComponent<SelectionManager>();
    }

    void Update()
    {
        if (GameManager.instance.IsPaused() || UIManager.instance.IsMouseOverUI() || ConstructionManager.instance.IsPreviewingBuilding())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (selectionManager.selectedUnits.Count > 0)
                PerformInteraction();
        }
    }

    void PerformInteraction()
    {
        Ray rayLocation = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayLocation, out RaycastHit hitLocation, 1000f, LayerMask.GetMask("Interactable", "Terrain")))
        {
            if (hitLocation.collider.gameObject.CompareTag("Resource"))
            {
                ResourceField resource = hitLocation.collider.GetComponent<ResourceField>();
                if (resource != null)
                {
                    foreach (Unit unit in selectionManager.selectedUnits)
                        if (unit.worker != null)
                            unit.worker.CollectResource(resource);
                }
            }
            else if (hitLocation.collider.gameObject.CompareTag("ResourceCamp"))
            {
                ResourceCamp resourceCamp = hitLocation.collider.GetComponent<ResourceCamp>();
                if (resourceCamp != null)
                {
                    foreach (Unit unit in selectionManager.selectedUnits)
                        if (unit.worker != null)
                            unit.worker.StoreResource(resourceCamp);
                }
            }
            else if (hitLocation.collider.gameObject.CompareTag("ResourceDrop"))
            {
                ResourceDrop resourceDrop = hitLocation.collider.GetComponent<ResourceDrop>();
                Unit closestUnit = selectionManager.ClosestUnitToSpot(hitLocation.point, true);
                if(closestUnit != null)
                    closestUnit.worker.PickUpResourceAction(resourceDrop);
            }
            else if(hitLocation.collider.gameObject.CompareTag("UnderConstruction"))
            {
                foreach (Unit unit in selectionManager.selectedUnits)
                    if (unit.worker != null)
                        unit.worker.StartConstruction(hitLocation.collider.gameObject);
            }
            else
            {
                MoveToSpot(hitLocation);
            }
        }
    }


    void MoveToSpot(RaycastHit hitLocation)
    {
        if (selectionManager.selectedUnits.Count == 1)
            selectionManager.selectedUnits[0].MoveToLocation(hitLocation.point);
        else
            MovementManager.instance.MoveInFormation(hitLocation.point);
    }

}
