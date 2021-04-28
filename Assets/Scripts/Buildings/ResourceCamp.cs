using UnityEngine;

public class ResourceCamp : MonoBehaviour
{
    public ResourceType initialCampType = ResourceType.NONE;
    public int initialAmountStored = 0;

    [HideInInspector] public ResourceType campType;
    [HideInInspector] public Vector3 accessLocation;
    [HideInInspector] public float accessDistance;
    [HideInInspector] public int amountStored;

    private readonly string foodStuffName = "FoodStuff";
    private readonly string goldStuffName = "GoldStuff";
    private readonly string woodStuffName = "WoodStuff";

    void Awake()
    {
        campType = ResourceType.NONE;
        AssignCampType(initialCampType);
        amountStored = initialAmountStored;

        accessLocation = transform.position + transform.forward * GetComponent<BoxCollider>().size.z / 2f;
        accessDistance = GetComponent<BoxCollider>().size.x / 2.2f;
    }

    private void Start()
    {
        ResourceManager.instance.resourceCamps.Add(this);
    }

    private void OnDestroy()
    {
        ResourceManager.instance.resourceCamps.Remove(this);
    }

    void Update()
    {
        CheckIfEmpty();
    }

    public void StoreResourceInCamp(int amount, ResourceType resourceType)
    {
        if (amount == 0)
            return;

        if (campType == ResourceType.NONE)
            AssignCampType(resourceType);
        else if (campType != resourceType)
            Debug.LogError("Error: Trying to store resource " + resourceType + " to camp of type: " + campType);

        amountStored += amount;
    }

    public int TakeResourceFromCamp(int amount)
    {
        if (amountStored < amount) // if not enough resources, take what is left
        {
            amount = amountStored;
            amountStored = 0;
        }
        else                       // takes the resources from the camp
            amountStored -= amount;

        return amount;
    }

    void CheckIfEmpty()
    {
        if (campType != ResourceType.NONE && IsEmpty())
            DeAssignCampType();
    }

    bool IsEmpty()
    {
        if (amountStored > 0)
            return false;
        else
            return true;
    }

    void AssignCampType(ResourceType resourceType)
    {
        if (campType != ResourceType.NONE)
        {
            Debug.Log("Camp " + this.gameObject + " is already assigned! Can't assign new type.");
            return;
        }

        switch (resourceType)
        {
            case ResourceType.FOOD:
                transform.Find(foodStuffName).gameObject.SetActive(true);
                break;
            case ResourceType.GOLD:
                transform.Find(goldStuffName).gameObject.SetActive(true);
                break;
            case ResourceType.WOOD:
                transform.Find(woodStuffName).gameObject.SetActive(true);
                break;
        }
        campType = resourceType;
    }

    void DeAssignCampType()
    {
        if (!IsEmpty())
        {
            Debug.Log("Camp " + this.gameObject + " is not empty! Can't deassign type.");
            return;
        }

        switch (campType)
        {
            case ResourceType.FOOD:
                transform.Find(foodStuffName).gameObject.SetActive(false);
                break;
            case ResourceType.GOLD:
                transform.Find(goldStuffName).gameObject.SetActive(false);
                break;
            case ResourceType.WOOD:
                transform.Find(woodStuffName).gameObject.SetActive(false);
                break;
            default:
                return;
        }
        campType = ResourceType.NONE;
    }

}
