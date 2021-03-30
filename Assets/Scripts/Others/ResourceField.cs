using UnityEngine;

public class ResourceField : MonoBehaviour
{
    public ResourceInfo resourceInfo;

    public float collectDistance;

    [SerializeField] private int initialAmount = 100;
    [HideInInspector] public int leftAmount;


    void Start()
    {
        leftAmount = initialAmount;
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
