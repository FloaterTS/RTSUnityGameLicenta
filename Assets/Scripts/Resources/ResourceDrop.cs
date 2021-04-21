using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : MonoBehaviour
{
    public CarriedResource droppedResource;
    public float pickupDistance;

    private void Start()
    {
        GameManager.instance.activeResourceDrops.Add(this);
    }
}
