using UnityEngine;

public enum ResourceFieldModel
{
    BERRY_BUSH_SMALL,
    BERRY_BUSH_LARGE,
    LUMBER_TREE,
    GOLD_ORE_MINE
}

public class ResourceField : MonoBehaviour
{
    public ResourceInfo resourceInfo;

    public ResourceFieldModel resourceFieldModel;
    
    public float collectDistance;

    [SerializeField] private int initialAmount = 100;

    [HideInInspector] public int leftAmount;


    private void Awake()
    {
        leftAmount = initialAmount;
    }

    void Start()
    {
        GameManager.instance.activeResourceFields.Add(this);
    }

    private void OnDestroy()
    {
        GameManager.instance.activeResourceFields.Remove(this);
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
