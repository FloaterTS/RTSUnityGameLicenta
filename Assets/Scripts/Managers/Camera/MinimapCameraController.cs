using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    public static MinimapCameraController instance;

    public Collider mapBaseCollider; //we keep the terrain base collider here
    public Material cameraFrustumMat; //material used to draw the camera frustum outline

    public Color cameraFrustumColor = Color.black; //outline color


    private Camera minimapCam;

    private Vector3 topLeftFrustumCorner, topRightFrustumCorner, bottomLeftFrustumCorner, bottomRightFrustumCorner;


    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another minimap controller is already active");

        minimapCam = GetComponent<Camera>();

        cameraFrustumMat.color = cameraFrustumColor;
    }

    private void Update()
    {
        //We update the main camera frustum on the minimap
        ShowMainCameraFrustum();
    }

    private void LateUpdate()
    {
        //We rotate the minimap so that it will have the same rotation on the y axis as the main camera
        RotateMinimap();
    }


    private void RotateMinimap()
    {
        //Rotate the minimap camera on the y axis to match the main camera
        if (minimapCam.transform.eulerAngles.y != Camera.main.transform.eulerAngles.y)
        {
            minimapCam.transform.rotation = Quaternion.Euler(
                minimapCam.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, minimapCam.transform.eulerAngles.z
                );
        }
    }

    private void ShowMainCameraFrustum()
    {
        Ray topLeftCornerRay = Camera.main.ScreenPointToRay(new Vector3(0f, 0f));
        Ray topRightCornerRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width, 0f));
        Ray bottomLeftCornerRay = Camera.main.ScreenPointToRay(new Vector3(0, Screen.height));
        Ray bottomRightCornerRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width, Screen.height));

        Vector3 topLeftCornerPoint = new Vector3(0f, 0f, 0f);
        Vector3 topRightCornerPoint = new Vector3(0f, 0f, 0f);
        Vector3 bottomLeftCornerPoint = new Vector3(0f, 0f, 0f);
        Vector3 bottomRightCornerPoint = new Vector3(0f, 0f, 0f);

        if (mapBaseCollider.Raycast(topLeftCornerRay, out RaycastHit topLeftCornerHit, 1000f))
            topLeftCornerPoint = topLeftCornerHit.point;

        if (mapBaseCollider.Raycast(topRightCornerRay, out RaycastHit topRightCornerHit, 1000f))
            topRightCornerPoint = topRightCornerHit.point;

        if (mapBaseCollider.Raycast(bottomLeftCornerRay, out RaycastHit bottomLeftCornerHit, 1000f))
            bottomLeftCornerPoint = bottomLeftCornerHit.point;

        if (mapBaseCollider.Raycast(bottomRightCornerRay, out RaycastHit bottomRightCornerHit, 1000f))
            bottomRightCornerPoint = bottomRightCornerHit.point;

        topLeftFrustumCorner = minimapCam.WorldToViewportPoint(topLeftCornerPoint);
        topRightFrustumCorner = minimapCam.WorldToViewportPoint(topRightCornerPoint);
        bottomLeftFrustumCorner = minimapCam.WorldToViewportPoint(bottomLeftCornerPoint);
        bottomRightFrustumCorner = minimapCam.WorldToViewportPoint(bottomRightCornerPoint);

        topLeftFrustumCorner.z = 0f;
        topRightFrustumCorner.z = 0f;
        bottomLeftFrustumCorner.z = 0f;
        bottomRightFrustumCorner.z = 0f;
    }

    public void OnPostRender()
    {
        GL.PushMatrix();
        {
            if (cameraFrustumMat != null)
                cameraFrustumMat.SetPass(0);
            else
                Debug.LogError("Camera outline material not assigned.");
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            {
                GL.Color(Color.black);
                GL.Vertex(topLeftFrustumCorner);
                GL.Vertex(topRightFrustumCorner);

                GL.Vertex(topRightFrustumCorner);
                GL.Vertex(bottomRightFrustumCorner);

                GL.Vertex(bottomRightFrustumCorner);
                GL.Vertex(bottomLeftFrustumCorner);

                GL.Vertex(bottomLeftFrustumCorner);
                GL.Vertex(topLeftFrustumCorner);
            }
            GL.End();
        }
        GL.PopMatrix();
    }

}
