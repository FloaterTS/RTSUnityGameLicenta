using UnityEngine;

[CreateAssetMenu]
public class ResourceInfo : ScriptableObject
{
    public ResourceType resourceType;
    public ResourceRaw resourceRaw;
    public string harvestAnimation;
    public string carryAnimation;
    public string storeAnimation;
    public float harvestTimePerUnit = 1f;
    public float storeDuration = 1f;
}
