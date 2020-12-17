using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias
{

    public class LiftScript : MonoBehaviour {

        public GameObject item;
        public GameObject itemInHand;
        
        public void GetItem(bool dropItem)
        {
            item.SetActive(dropItem);
            itemInHand.SetActive(!dropItem);
        }
    }
}
