using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(0f, 0.25f)]
    private float moveSmoothing = 0.075f;
    [SerializeField] private float regularMoveSpeed = 27.5f;
    [SerializeField] private float fastMoveFactor = 3f;
    [SerializeField] private float screenPanBorderThickness = 12.5f;
    [SerializeField] private bool movementByMouse = true;

    private Vector3 desiredCameraPosition;
    private float horizontal;
    private float vertical;

    [Header("Rotation Settings")]
    [SerializeField, Range(0f, 0.25f)]
    private float rotationSmoothing = 0.05f;
    [SerializeField] private float regularRotationSpeed = 50f;
    [SerializeField] private float fastRotationFactor = 2f;
    [SerializeField] private float snapDegreeValue = 90f;
    [SerializeField] private bool snapRotation = true;

    private Quaternion desiredCameraRotation;
    private Quaternion facingRotation;

    [Header("Zoom Settings")]
    [SerializeField] private float scrollSensitivity = 250f;

    [Header("Bounding Settings")]
    [SerializeField] private float minHeightFromGround = 5f;
    [SerializeField] private float maxHeightFromGround = 45f;

    private Terrain terrainToHoverOver;
    private Vector3 terrainPosition;
    private Vector3 terrainSize;
    private float minHeight, maxHeight;

    [Header("Other Settings")]
    [SerializeField, Range(45f, 90f)]
    private float camXRotation = 67.5f;
    [SerializeField, Range(45f, 60f)] 
    private float camFieldOfView = 60f;
    [SerializeField] private bool lockMouseCursor = true;


    void Start()
    {
        desiredCameraPosition = transform.position;
        desiredCameraRotation = transform.rotation;

        facingRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        terrainToHoverOver = GameManager.instance.mainTerrain;
        terrainPosition = terrainToHoverOver.transform.position;
        terrainSize = terrainToHoverOver.terrainData.size;

        AdjustXRotation(camXRotation);
        AdjustFieldOfView(camFieldOfView);
        AdjustSnapRotation(snapRotation);

        if(lockMouseCursor)
            Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        CheckMovement();
        CheckZoom();
        MoveCamera();
        CheckRotation();
        CheckBounds();
    }

    void CheckMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if(movementByMouse)
            CheckMovementFromMouse();

        if (horizontal == 0f && vertical == 0f)
            return;

        float moveSpeed = regularMoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed *= fastMoveFactor;

        desiredCameraPosition += facingRotation * Vector3.right * horizontal * moveSpeed * Time.deltaTime;
        desiredCameraPosition += facingRotation * Vector3.forward * vertical * moveSpeed * Time.deltaTime;
    }

    void CheckZoom()
    {
        float scrollValue = Input.mouseScrollDelta.y;

        if (scrollValue == 0f)
            return;

        float scrollValueScaled = scrollValue * scrollSensitivity * Time.deltaTime;

        if (scrollValueScaled > 0f) //Zoom In
        {
            if (desiredCameraPosition.y - 1f > minHeight) //Checking if we're maximum zoomed in
            {
                Ray zoomRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(zoomRay, out RaycastHit zoomHit, 1000f, LayerMask.GetMask("Terrain")))
                    desiredCameraPosition = Vector3.MoveTowards(desiredCameraPosition, zoomHit.point, scrollValueScaled);
            }
            else
                desiredCameraPosition.y -= scrollValueScaled;
        }
        else //Zoom Out
        {
            if (desiredCameraPosition.y + 1f < maxHeight)  //Checking if we're maximum zoomed out
                desiredCameraPosition += transform.forward * scrollValueScaled;
            else
                desiredCameraPosition.y -= scrollValueScaled;
        }
    }

    void CheckRotation()
    {
        if (ConstructionManager.instance.IsPreviewingBuilding())
            return;
        if (snapRotation)
        {
            if (Input.GetKeyDown(KeyCode.E))
                desiredCameraRotation = Quaternion.Euler(desiredCameraRotation.eulerAngles.x,
                    desiredCameraRotation.eulerAngles.y + snapDegreeValue, desiredCameraRotation.eulerAngles.z);

            if (Input.GetKeyDown(KeyCode.Q))
                desiredCameraRotation = Quaternion.Euler(desiredCameraRotation.eulerAngles.x,
                    desiredCameraRotation.eulerAngles.y - snapDegreeValue, desiredCameraRotation.eulerAngles.z);
        }
        else
        {
            float rotationSpeed = regularRotationSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                rotationSpeed *= fastRotationFactor;

            if (Input.GetKey(KeyCode.E))
                desiredCameraRotation = Quaternion.Euler(
                    desiredCameraRotation.eulerAngles.x,
                    desiredCameraRotation.eulerAngles.y + rotationSpeed * Time.deltaTime, 
                    desiredCameraRotation.eulerAngles.z);

            if(Input.GetKey(KeyCode.Q))
                desiredCameraRotation = Quaternion.Euler(
                    desiredCameraRotation.eulerAngles.x,
                    desiredCameraRotation.eulerAngles.y - rotationSpeed * Time.deltaTime,
                    desiredCameraRotation.eulerAngles.z);
        }
        facingRotation = Quaternion.Euler(0f, desiredCameraRotation.eulerAngles.y, 0f);
    }

    void MoveCamera()
    {
        if(transform.position != desiredCameraPosition)
            transform.position = Vector3.Lerp(transform.position, desiredCameraPosition, moveSmoothing);

        if(transform.rotation != desiredCameraRotation)
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredCameraRotation, rotationSmoothing);
    }

    void CheckBounds()
    {
        float currentTerrainHeight = terrainToHoverOver.SampleHeight(transform.position);

        minHeight = currentTerrainHeight + minHeightFromGround;
        maxHeight = currentTerrainHeight + maxHeightFromGround;

        desiredCameraPosition.y = Mathf.Clamp(desiredCameraPosition.y, minHeight, maxHeight);

        desiredCameraPosition.x = Mathf.Clamp(desiredCameraPosition.x, terrainPosition.x, terrainPosition.x + terrainSize.x);
        desiredCameraPosition.z = Mathf.Clamp(desiredCameraPosition.z, terrainPosition.z, terrainPosition.z + terrainSize.z);
    }

    void CheckMovementFromMouse()
    {
        if (SelectionManager.instance.IsDragging())
            return;

        Vector3 currentMousePosition = Input.mousePosition;

        if (currentMousePosition.x <= screenPanBorderThickness)
            horizontal = -1;
        if (currentMousePosition.x >= Screen.width - screenPanBorderThickness)
            horizontal = 1;
        if (currentMousePosition.y <= screenPanBorderThickness)
            vertical = -1;
        if (currentMousePosition.y >= Screen.height - screenPanBorderThickness)
            vertical = 1;
    }

    public void AdjustFieldOfView(float valueFOV)
    {
        camFieldOfView = valueFOV;
        Camera.main.fieldOfView = camFieldOfView;
    }

    public void AdjustXRotation(float valueXRotation)
    {
        camXRotation = valueXRotation;
        desiredCameraRotation = Quaternion.Euler(camXRotation, desiredCameraRotation.eulerAngles.y, desiredCameraRotation.eulerAngles.z);
    }

    public void AdjustSnapRotation(bool snapRotationActive)
    {
        if (snapRotationActive)
        {
            desiredCameraRotation = Quaternion.Euler(desiredCameraRotation.eulerAngles.x, 0f, desiredCameraRotation.eulerAngles.z);
            facingRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        snapRotation = snapRotationActive;
    }

    public void ToggleMovementByMouse(bool movementByMouseActive)
    {
        movementByMouse = movementByMouseActive;
    }   
    
    //void SelectionZoomOut()
}
