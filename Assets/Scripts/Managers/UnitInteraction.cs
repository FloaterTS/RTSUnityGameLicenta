using UnityEngine;

public enum UnitState
{
    idle,
    moving,
    working,
    attacking
}

public class UnitInteraction : MonoBehaviour
{
    public static UnitInteraction instance;

    private UnitSelection unitSelection;


    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another unit interaction manager present.");

        unitSelection = GetComponent<UnitSelection>();
    }

    void Update()
    {
        if (GameManager.instance.isPaused || UIManager.instance.IsMouseOverUI())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            if (unitSelection.selectedUnits.Count > 0)
                PerformInteraction();
        }
    }

    void PerformInteraction()
    {
        Ray rayLocation = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayLocation, out RaycastHit hitLocation, 1000f, LayerMask.GetMask("Interactable", "Terrain")))
        {
            ResourceField resource = hitLocation.collider.GetComponent<ResourceField>();
            ResourceCamp resourceCamp = hitLocation.collider.GetComponent<ResourceCamp>();
            if (resource != null)
            {
                foreach (Unit unit in unitSelection.selectedUnits)
                    if(unit.worker != null)
                        unit.worker.CollectResource(resource);
            }
            else if (resourceCamp != null)
            {
                foreach (Unit unit in unitSelection.selectedUnits)
                    if(unit.worker != null)
                        unit.worker.StoreResource(resourceCamp);
            }
            else
            {
                MoveToSpot(hitLocation);
            }
        }
    }


    void MoveToSpot(RaycastHit hitLocation)
    {
        if (unitSelection.selectedUnits.Count == 1)
            unitSelection.selectedUnits[0].MoveToLocation(hitLocation.point);
        else
            UnitMovement.instance.MoveInFormation(hitLocation.point);
    }

}
