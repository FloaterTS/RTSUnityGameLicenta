using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager instance;

    [SerializeField] private GameObject playerBuildingsParent;
    [SerializeField] private GameObject previewResourceCamp;
    [SerializeField] private GameObject underConstructionResourceCampPrefab;
    [SerializeField] private GameObject constructedResourceCampPrefab;

    private GameObject previewBuildingGO;
    private GameObject underConstructionBuildingPrefab;
    private GameObject constructedBuildingPrefab;
    private float snapRotationDegrees = 45f;
    private bool isPreviewingBuildingConstruction = false;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another unit selection manager present.");
    }

    void Update()
    {
        if (isPreviewingBuildingConstruction)
        {
            if (GameManager.instance.IsPaused())
            {
                StopPreviewBuilding();
                isPreviewingBuildingConstruction = false;
            }

            if (previewBuildingGO != null)
            {
                RotatePreviewBuilding();
                Ray previewRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(previewRay, out RaycastHit previewHit, 1000f, LayerMask.GetMask("Terrain")))
                    previewBuildingGO.transform.position = previewHit.point;
            }

            if (UIManager.instance.IsMouseOverUI())
                return;

            if (Input.GetMouseButtonDown(0))
                if(previewBuildingGO.GetComponent<PlacementValidity>().IsValidlyPlaced())
                    StartConstructionForSelection();

            if (Input.GetMouseButtonDown(1))
                StopPreviewBuilding();

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                if(previewBuildingGO == null)
                    isPreviewingBuildingConstruction = false;
        }
    }

    public void StartPreviewResourceCampConstruction()
    {
        if (previewBuildingGO != null)
            Destroy(previewBuildingGO);

        previewBuildingGO = Instantiate(previewResourceCamp);
        previewBuildingGO.transform.eulerAngles = new Vector3(0f, FaceCameraInitialPreviewRotation(), 0f);

        underConstructionBuildingPrefab = underConstructionResourceCampPrefab;
        constructedBuildingPrefab = constructedResourceCampPrefab;
        isPreviewingBuildingConstruction = true;
        //StartCoroutine(StartBuildingPreview()); //we have to wait a frame so the click on the build button doesn't register as building placement
    }

    private IEnumerator StartBuildingPreview()
    {
        yield return null;
        isPreviewingBuildingConstruction = true;
    }

    private void StopPreviewBuilding()
    {
        Destroy(previewBuildingGO);
    }

    private void RotatePreviewBuilding()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            previewBuildingGO.transform.eulerAngles -= new Vector3(0f, snapRotationDegrees, 0f);
        if (Input.GetKeyDown(KeyCode.E))
            previewBuildingGO.transform.eulerAngles += new Vector3(0f, snapRotationDegrees, 0f);
    }

    private float FaceCameraInitialPreviewRotation()
    {
        float closestSnapValue = 0f;
        float minDifference = 360f;
        float degrees = -180f;
        float difference;

        while(degrees <= 180f)
        {
            difference = Mathf.Abs(Camera.main.transform.eulerAngles.y - degrees);
            if (difference < minDifference)
            {
                minDifference = difference;
                closestSnapValue = degrees;
            }
            degrees += snapRotationDegrees;
        }
        return closestSnapValue + 180f;
    }

    private void StartConstructionForSelection()
    {
        GameObject inConstructionBuildingGO = Instantiate(underConstructionBuildingPrefab, previewBuildingGO.transform.position, previewBuildingGO.transform.rotation, playerBuildingsParent.transform);
        UnderConstruction underConstruction = inConstructionBuildingGO.GetComponent<UnderConstruction>();
        underConstruction.constructedBuildingPrefab = constructedBuildingPrefab;
        underConstruction.parentBuildingsGO = playerBuildingsParent;

        StopPreviewBuilding();

        foreach (Unit unit in SelectionManager.instance.selectedUnits)
            if(unit.worker != null)
                unit.worker.StartConstruction(inConstructionBuildingGO);
    }

    public bool IsPreviewingBuilding()
    {
        return isPreviewingBuildingConstruction;
    }
}
