using UnityEngine;

public enum ResourceFieldModel
{
    BerryBushSmall,
    BerryBushLarge,
    LumberTree,
    GoldOreMine
}

public class ResourceField : MonoBehaviour
{
    public ResourceInfo resourceInfo;

    public ResourceFieldModel resourceFieldModel;
    
    public float collectDistance;

    [SerializeField] private int initialAmount = 100;

    [HideInInspector] public int leftAmount;


    void Start()
    {
        leftAmount = initialAmount;

        GameManager.instance.activeResourceFields.Add(this);
    }

    void Update()
    {
        if (leftAmount <= 0)
            Destroy(this.gameObject);
    }

    public int HarvestResourceField()
    {
        if (leftAmount == 0)
            return 0;
        leftAmount--;
        return 1;
    }

}
