using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ResourceField : MonoBehaviour
{
    public ResourceInfo resourceInfo;

    [SerializeField] private float collectDistanceFromCollider = 0f;
    [HideInInspector] public float collectDistance;

    [SerializeField] private int initialAmount = 100;
    [HideInInspector] public int leftAmount;

    //[SerializeField] private int maxHarvesters = 5;
    //private int numberOfHarvesters = 0;


    void Start()
    {
        collectDistance = GetComponent<SphereCollider>().radius + collectDistanceFromCollider;

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
