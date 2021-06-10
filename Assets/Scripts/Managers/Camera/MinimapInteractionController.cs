using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class MinimapInteractionController : MonoBehaviour, IPointerClickHandler
{
    public static MinimapInteractionController instance;

    public Camera minimapCam; // we keep the minimap camera component here

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another minimap interaction controller is already active");
    }

    private void Start()
    {
        minimapCam = MinimapCameraController.instance.gameObject.GetComponent<Camera>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out Vector2 localCursorPoint))
        {
            Rect imageRectSize = GetComponent<RawImage>().rectTransform.rect;

            /* localCursorPoint is the distance on x and y axis from the rect center point
            off we add the imageRectSize (by subtracting because it's negative) which is the half size
            the rectangle so we can get the local coordinates x and y inside the rectangle
            then we divide them by the rectSize so we can get their ratios (between 0.0 - 1.0) */
            localCursorPoint.x = (localCursorPoint.x - imageRectSize.x) / imageRectSize.width;
            localCursorPoint.y = (localCursorPoint.y - imageRectSize.y) / imageRectSize.height;
           
            CastMinimapRayToWorld(localCursorPoint, eventData.button);
        }
    }

    private void CastMinimapRayToWorld(Vector2 localCursor, InputButton mouseButton)
    {
        //we multiply the local ratios inside the minimap image rect with the minimap camera's pixelWidth so we can get the right pixel coordinates for the ray
        Ray miniMapRay = minimapCam.ScreenPointToRay(new Vector2(localCursor.x * minimapCam.pixelWidth, localCursor.y * minimapCam.pixelHeight));

        //we cast the ray through the minimap camera, which will hit the world point that it pointed towards
        if (Physics.Raycast(miniMapRay, out RaycastHit minimapHit, 1000f, LayerMask.GetMask("Interactable", "Terrain")))
        {
            if (mouseButton == InputButton.Left)
            {
                CameraController.instance.MoveCameraToPosition(minimapHit.point.x, minimapHit.point.z, true);
            }
            else if (mouseButton == InputButton.Right)
            {
                if(SelectionManager.instance.selectedUnits.Count > 0)
                {
                    InteractionManager.instance.PerformInteraction(minimapHit);
                }
            }
        }
    }

}
