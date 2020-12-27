using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementValidity : MonoBehaviour
{
    [SerializeField] private Material validityMaterial;
    [SerializeField] private float maxTerrainHeightDifference = 2.25f;

    private BoxCollider boxCollider;
    private Terrain terrain;
    private enum CurrentColor { invalid, valid };
    private CurrentColor currentColor;
    private Color validColor;
    private Color invalidColor;
    private bool overlapping = false;
    private bool invalidTerrainBase = false;


    void Start()
    {
        overlapping = false;
        invalidTerrainBase = false;

        validColor = new Color(0f, 0.55f, 0f, 0.3f); // transparent dark green 
        invalidColor = new Color(1f, 0f, 0f, 0.4f); // transparent red
        validityMaterial.color = validColor;
        currentColor = CurrentColor.valid;

        boxCollider = GetComponent<BoxCollider>();
        terrain = GameManager.instance.mainTerrain;
    }

    void Update()
    {
        CheckTerrainHeightDifference();
        CheckValidity();
    }

    public bool IsValidlyPlaced()
    {
        return !overlapping && !invalidTerrainBase;
    }

    private void CheckValidity()
    {
        if (IsValidlyPlaced())
        {
            if (currentColor != CurrentColor.valid)
            {
                validityMaterial.color = validColor;
                currentColor = CurrentColor.valid;
            }
        }
        else
        {
            if (currentColor != CurrentColor.invalid)
            {
                validityMaterial.color = invalidColor;
                currentColor = CurrentColor.invalid;
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Terrain"))
            overlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Terrain"))
            overlapping = false;
    }

    private void CheckTerrainHeightDifference()
    {
        Vector3[] buildingCorners = new Vector3[4];
        buildingCorners[0] = new Vector3(transform.position.x - (boxCollider.size.x / 2), transform.position.y, transform.position.z - (boxCollider.size.z / 2));
        buildingCorners[1] = new Vector3(transform.position.x + (boxCollider.size.x / 2), transform.position.y, transform.position.z - (boxCollider.size.z / 2));
        buildingCorners[2] = new Vector3(transform.position.x - (boxCollider.size.x / 2), transform.position.y, transform.position.z + (boxCollider.size.z / 2));
        buildingCorners[3] = new Vector3(transform.position.x + (boxCollider.size.x / 2), transform.position.y, transform.position.z + (boxCollider.size.z / 2));

        float diff, maxDiff = 0;
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                diff = Mathf.Abs(terrain.SampleHeight(buildingCorners[i]) - terrain.SampleHeight(buildingCorners[j]));
                if (diff > maxDiff)
                    maxDiff = diff;
            }
        }
        if (maxDiff > maxTerrainHeightDifference)
            invalidTerrainBase = true;
        else
            invalidTerrainBase = false;
    }
}
