using UnityEngine;

[CreateAssetMenu]
public class ResourceInfo : ScriptableObject
{
    public ResourceType resourceType;
    public ResourceRaw resourceRaw;
    public string harvestAnimation;
    public string carryAnimation;
    public string carriedResourceName;
    public string toolInHandName;
    public string toolHarvestingName;
    public float liftAnimationDuration;
    public float dropAnimationDuration;
    public float harvestTimePerUnit = 1f;
    public UnitSpeed carrySpeed;
}
