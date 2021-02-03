using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    //Selected Units
    public string[] rankList;

    [HideInInspector] public List<Unit> selectedUnits;
    [HideInInspector] public Building selectedBuilding;
    [HideInInspector] public bool heroSelected = false;

    //Selection Box
    [Space] public RectTransform selectionArea;

    private bool isDragging = false; //Also used by CameraController
    private readonly float minDragSelectValue = 5f;
    private Vector3 startMousePosition;
    private Vector3 upperLeftCorner;
    private Vector3 upperRightCorner;
    private Vector3 lowerLeftCorner;
    private Vector3 lowerRightCorner;


    //DoubleClickSelect
    private readonly float doubleClickDelay = 0.4f; //About half a second delay
    private float timeSinceLastClick = 0f;
    private Unit lastClickedUnit = null;
    private bool doubleClicked = false;


    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another unit selection manager present.");

        selectedUnits = new List<Unit>();
    }

    void Update()
    {
        if (GameManager.instance.IsPaused())
            return;

        timeSinceLastClick += Time.deltaTime;

        if (ConstructionManager.instance.IsPreviewingBuilding())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (UIManager.instance.IsMouseOverUI())
                return;
            startMousePosition = Input.mousePosition;
            selectionArea.position = startMousePosition;
            selectionArea.gameObject.SetActive(true);
            isDragging = true;
        }

        if (Input.GetMouseButton(0)  && isDragging)
        {
            UpdateSelectionCorners();
            AdjustSelectionPivot();
            AdjustSelectionSize();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            selectionArea.gameObject.SetActive(false);
            isDragging = false;

            if (!Input.GetKey(KeyCode.LeftControl))
                DeselectEverything();

            if (Mathf.Abs(startMousePosition.x - Input.mousePosition.x) < minDragSelectValue ||
                Mathf.Abs(startMousePosition.y - Input.mousePosition.y) < minDragSelectValue) 
            {
                SingleSelect(); //Single Select
            }
            else   
            {
                DragSelect();  //Drag Select
            }
        }
    }

    void UpdateSelectionCorners()
    {
        Vector3 currentMousePosition = Input.mousePosition;

        lowerLeftCorner = new Vector3(
                Mathf.Min(startMousePosition.x, currentMousePosition.x),
                Mathf.Min(startMousePosition.y, currentMousePosition.y),
                0f);

        upperRightCorner = new Vector3(
            Mathf.Max(startMousePosition.x, currentMousePosition.x),
            Mathf.Max(startMousePosition.y, currentMousePosition.y),
            0f);

        lowerRightCorner = new Vector3(upperRightCorner.x, lowerLeftCorner.y, 0f);
        upperLeftCorner = new Vector3(lowerLeftCorner.x, upperRightCorner.y, 0f);
    }

    void AdjustSelectionPivot()
    {
        if (startMousePosition == lowerLeftCorner)
            selectionArea.pivot = new Vector2(0f, 0f);
        else if (startMousePosition == lowerRightCorner)
            selectionArea.pivot = new Vector2(1f, 0f);
        else if (startMousePosition == upperRightCorner)
            selectionArea.pivot = new Vector2(1f, 1f);
        else if (startMousePosition == upperLeftCorner)
            selectionArea.pivot = new Vector2(0f, 1f);
    }

    void AdjustSelectionSize()
    {
        selectionArea.sizeDelta = new Vector2(upperRightCorner.x - lowerLeftCorner.x,
                                                  upperRightCorner.y - lowerLeftCorner.y);
    }

    void SingleSelect()
    {
        Ray selectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(selectionRay, out RaycastHit selectionHit, 1000f))
        {
            Unit unit = selectionHit.collider.GetComponent<Unit>();
            Building building = selectionHit.collider.GetComponent<Building>();
            if (unit != null)
            {
                if (CheckDoubleClick(unit))
                    return;
                if(Input.GetKey(KeyCode.LeftControl) && selectedUnits.Contains(unit)) // Deselect the unit if ctrl is pressed while single selecting
                {
                    selectedUnits.Remove(unit);
                    unit.SetSelected(false);
                }
                else if (!selectedUnits.Contains(unit) && unit.unitStats.unitTeam == Team.Player)
                {
                    selectedUnits.Add(unit);
                    unit.SetSelected(true);

                    if(selectedUnits.Count > 1)
                        ArrangeSelectionByRank();
                }
            }
            else if(building != null && selectedUnits.Count == 0)
            {
                selectedBuilding = building;
                building.SetSelected(true);
            }
            else //If we didn't hit a unit
            {
                 // do I need this???
            }
        }
        UIManager.instance.UpdateSelectedInteractionUI();
    }

    void DragSelect()
    {
        Ray selectionCornerRay;
        RaycastHit selectionCornerHit;

        Vector3[] selectionCorners = { upperLeftCorner, upperRightCorner, lowerLeftCorner, lowerRightCorner }; //The order matters
        Vector3[] selectionMeshVertices = new Vector3[5];

        selectionMeshVertices[0] = Camera.main.transform.position; //First vertice is at camera's position

        for (int i = 0; i < 4; i++)
        {
            selectionCornerRay = Camera.main.ScreenPointToRay(selectionCorners[i]);

            if (Physics.Raycast(selectionCornerRay, out selectionCornerHit, 1000f, LayerMask.GetMask("TerrainBase")))
            {
                selectionMeshVertices[i + 1] = selectionCornerHit.point;
                //Debug.DrawLine(selectionRay.origin, selectionHit.point, Color.red, 7f);
            }
        }

        //We generate a mesh with 5 vertices: one from the camera and the others being our selection corners
        Mesh selectionMesh = GenerateSelectionMesh(selectionMeshVertices);

        //We add a collider to out generated mesh so we can detect the units that are inside it
        MeshCollider selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;

        //We destroy the collider after a very short time so it will only detect 
        //the units that are currently present inside it
        StartCoroutine(DestroySelectionBoxMesh(selectionBox));
    }

    void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();
        if (unit != null)
        {
            if (!selectedUnits.Contains(unit) && unit.unitStats.unitTeam == Team.Player)
            {
                if (doubleClicked && unit.unitStats.unitName != lastClickedUnit.unitStats.unitName) //Double click selection check
                    return;

                selectedUnits.Add(unit);
                unit.SetSelected(true);
            }
        }
        else
        {
            Building building = other.gameObject.GetComponent<Building>();
            if (building != null)
                selectedBuilding = building;
        }
    }

    IEnumerator DestroySelectionBoxMesh(MeshCollider selectionBox)
    {
        //Wait for the next fixed update so the trigger collisions will be detected and then destroy the selection box collider
        yield return new WaitForFixedUpdate();

        Destroy(selectionBox);

        doubleClicked = false;

        if (selectedUnits.Count > 1)
            ArrangeSelectionByRank();

        if (selectedBuilding != null)
        {
            if (selectedUnits.Count == 0)
                selectedBuilding.SetSelected(true);
            else
                selectedBuilding = null;
        }

        UIManager.instance.UpdateSelectedInteractionUI();
    }

    Mesh GenerateSelectionMesh(Vector3[] verts)
    {
        int[] tris = { 1, 3, 2, 2, 3, 4, 0, 2, 4, 3, 1, 0, 0, 1, 2, 4, 3, 0 }; // The order matters
        //Each group of 3 numbers represent the vertice indexes for a corresponding triangle
        //Meshes are made of triangles which are only visible from one side
        //The order of the triangles' vertices has to be clockwise for a side to be drawn

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    void ArrangeSelectionByRank()
    {
        List<Unit> selectedUnitsRanked = new List<Unit>(selectedUnits.Count);
        bool[] unitsRanked = new bool[selectedUnits.Count];

        for (int r = 0; r < rankList.Length; r++)
        {
            for(int i = 0; i < selectedUnits.Count; i++)
            {
                if (!unitsRanked[i] && selectedUnits[i].unitStats.unitName == rankList[r])
                {
                    selectedUnitsRanked.Add(selectedUnits[i]);
                    unitsRanked[i] = true;
                }
            }
        }
        if (selectedUnitsRanked.Count < selectedUnits.Count) //If there are any units not defined in rankList we put them at the end
        {
            for(int i = 0; i < selectedUnits.Count; i++)
            {
                if (!unitsRanked[i])
                { 
                    selectedUnitsRanked.Add(selectedUnits[i]);
                    unitsRanked[i] = true;
                }
            }
        }

        selectedUnits.Clear();
        selectedUnits = selectedUnitsRanked;
    }

    bool CheckDoubleClick(Unit clickedUnit)
    {
        if(timeSinceLastClick < doubleClickDelay && clickedUnit == lastClickedUnit)
        {
            doubleClicked = true;

            upperLeftCorner = new Vector3(0f, Screen.height, 0f);
            upperRightCorner = new Vector3(Screen.width, Screen.height, 0f);
            lowerLeftCorner = new Vector3(0f, 0f, 0f);
            lowerRightCorner = new Vector3(Screen.width, 0f, 0f);

            DragSelect();

            return true;
        }
        else
        {
            timeSinceLastClick = 0f;
            lastClickedUnit = clickedUnit;
            return false;
        }
    }

    private void DeselectEverything()
    {
        foreach (Unit unitToClear in selectedUnits)
            unitToClear.SetSelected(false);
        selectedUnits.Clear();

        if (doubleClicked)
        {
            lastClickedUnit.SetSelected(true);
            selectedUnits.Add(lastClickedUnit);
        }

        if (selectedBuilding != null)
        {
            selectedBuilding.SetSelected(false);
            selectedBuilding = null;
        }
    }

    public bool IsDragging()
    {
        return isDragging;
    }

    public Unit ClosestUnitToSpot(Vector3 spot, bool workersOnly = false)
    {
        if (selectedUnits.Count == 0)
            return null;
        Unit closestUnit = selectedUnits[0];
        float minDistance = Vector3.Distance(closestUnit.transform.position, spot);
        for (int i = 1; i < selectedUnits.Count; i++)
        {
            if (workersOnly && selectedUnits[i].worker == null)
                continue;
            float distance = Vector3.Distance(selectedUnits[i].transform.position, spot);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestUnit = selectedUnits[i];
            }
        }
        if (workersOnly && closestUnit.worker == null)
            return null;
        else
            return closestUnit;
    }
}
