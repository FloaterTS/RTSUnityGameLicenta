using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager instance;

    [SerializeField] private GameObject previewResourceCamp;
    [SerializeField] private GameObject underConstructionResourceCamp;
    [SerializeField] private GameObject constructedResourceCamp;
    [SerializeField] private Material validityMaterial;

    private GameObject previewBuildingGO;
    private GameObject underConstructionBuildingPrefab;
    private GameObject constructedBuildingPrefab;
    private float constructionRotation;
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

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                StopPreviewBuilding();

            if (Input.GetMouseButtonDown(0))
                StartConstructionForSelection();

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                isPreviewingBuildingConstruction = false;
        }
    }

    public void StartPreviewResourceCampConstruction()
    {
        previewBuildingGO = Instantiate(previewResourceCamp);
        previewBuildingGO.transform.eulerAngles = new Vector3(0f, FaceCameraInitialPreviewRotation(), 0f);

        constructionRotation = previewBuildingGO.transform.eulerAngles.y;
        underConstructionBuildingPrefab = underConstructionResourceCamp;
        constructedBuildingPrefab = constructedResourceCamp;

        StartCoroutine(StartBuildingPreview());
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
        constructionRotation = previewBuildingGO.transform.eulerAngles.y;
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
        //foreach(Unit unit in SelectionManager.instance.selectedUnits)
            //unit.StartConstruction(underConstructionBuilding, constructedBuilding, constructionRotation);
    }

    public bool IsPreviewingBuilding()
    {
        return isPreviewingBuildingConstruction;
    }
}
