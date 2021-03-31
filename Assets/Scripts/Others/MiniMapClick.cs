using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapClick : MonoBehaviour, IPointerClickHandler
{
    public Camera miniMapCam;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out Vector2 localCursorPoint))
        {
            Rect imageRectSize = GetComponent<RawImage>().rectTransform.rect;

            /* localCursorPoint is the distance on x and y axis from the rect center point
            off we add the imageRectSize (by substracting because it's negative) which is the half size
            the rectangle so we can get the local coordinates x and y inside the rectangle
            then we divide them by the rectSize so we can get their ratios (between 0.0 - 1.0) */
            localCursorPoint.x = (localCursorPoint.x - imageRectSize.x) / imageRectSize.width;
            localCursorPoint.y = (localCursorPoint.y - imageRectSize.y) / imageRectSize.height;
           
            CastMiniMapRayToWorld(localCursorPoint);
        }
    }

    private void CastMiniMapRayToWorld(Vector2 localCursor)
    {
        //we multiply the local ratios inside the minimap image rect with the minimap camera's pixelWidth so we can get the right pixel coordinates for the ray
        Ray miniMapRay = miniMapCam.ScreenPointToRay(new Vector2(localCursor.x * miniMapCam.pixelWidth, localCursor.y * miniMapCam.pixelHeight));

        //we cast the ray through the minimap camera, which will hit the world point that it pointed towards
        if (Physics.Raycast(miniMapRay, out RaycastHit miniMapHit, Mathf.Infinity))
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = miniMapHit.point;
            cube.transform.localScale *= 5;
        }
    }
}
