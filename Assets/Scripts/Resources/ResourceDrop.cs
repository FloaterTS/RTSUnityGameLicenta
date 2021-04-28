using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public CarriedResource droppedResource;
    public float pickupDistance;

    private void Start()
    {
        GameManager.instance.activeResourceDrops.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.instance.activeResourceDrops.Remove(this);
    }
}
