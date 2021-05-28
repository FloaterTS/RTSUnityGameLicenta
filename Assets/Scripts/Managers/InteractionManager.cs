using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    private SelectionManager selectionManager;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another unit interaction manager present.");
    }

    void Start()
    {
        selectionManager = SelectionManager.instance;
    }

    void Update()
    {
        if (GameManager.instance.IsPaused() || UIManager.instance.IsMouseOverUI() || ConstructionManager.instance.IsPreviewingBuilding())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            CheckInteraction();
        }
    }

    void CheckInteraction()
    {
        if (selectionManager.selectedUnits.Count > 0)
        {
            Ray rayLocation = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayLocation, out RaycastHit hitLocation, 1000f, LayerMask.GetMask("Interactable", "Terrain")))
            {
                PerformInteraction(hitLocation);
            }
        }
    }

    public void PerformInteraction(RaycastHit hitLocation)
    {

        if (hitLocation.collider.gameObject.CompareTag("Resource"))
        {
            ResourceField resourceField = hitLocation.collider.GetComponent<ResourceField>();
            if (resourceField != null)
            {
                foreach (Unit unit in selectionManager.selectedUnits)
                    if (unit.worker != null)
                        unit.worker.CollectResource(resourceField);
            }
            else
                Debug.LogError("ResourceField script missing from " + hitLocation.collider.gameObject + " tagged as ResourceField");
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
            else
                Debug.LogError("ResourceCamp script missing from " + hitLocation.collider.gameObject + " tagged as ResourceCamp");
        }
        else if (hitLocation.collider.gameObject.CompareTag("ResourceDrop"))
        {
            ResourceDrop resourceDrop = hitLocation.collider.GetComponent<ResourceDrop>();
            if (resourceDrop != null)
            {
                Unit closestUnit = selectionManager.ClosestUnitToSpot(hitLocation.point, true);
                if (closestUnit != null)
                    closestUnit.worker.PickUpResourceAction(resourceDrop);
            }
            else
                Debug.LogError("ResourceDrop script missing from " + hitLocation.collider.gameObject + " tagged as ResourceDrop");
        }
        else if (hitLocation.collider.gameObject.CompareTag("UnderConstruction"))
        {
            if (hitLocation.collider.GetComponent<UnderConstruction>() != null)
            {
                foreach (Unit unit in selectionManager.selectedUnits)
                    if (unit.worker != null)
                        unit.worker.StartConstruction(hitLocation.collider.gameObject);
            }
            else
                Debug.LogError("UnderConstruction script missing from " + hitLocation.collider.gameObject + " tagged as UnderConstruction");
        }
        else if(hitLocation.collider.gameObject.CompareTag("Unit"))
        {
            Unit enemyUnit = hitLocation.collider.GetComponent<Unit>();
            if (enemyUnit != null)
            {
                if (enemyUnit.unitStats.unitTeam != Team.PLAYER)
                {
                    foreach (Unit unit in selectionManager.selectedUnits)
                        if (unit.fighter != null)
                            unit.fighter.AttackCommand(enemyUnit.gameObject, true);
                }
                else
                {
                    MoveToSpot(hitLocation.point);
                }
            }
            else
                Debug.LogError("Unit script missing from " + hitLocation.collider.gameObject + " tagged as Unit");
        }
        else
        {
            MoveToSpot(hitLocation.point);
        }
    }


    void MoveToSpot(Vector3 spot)
    {
        if (selectionManager.selectedUnits.Count == 1)
            selectionManager.selectedUnits[0].MoveToLocation(spot);
        else
            MovementManager.instance.MoveInFormation(spot);
    }

}
