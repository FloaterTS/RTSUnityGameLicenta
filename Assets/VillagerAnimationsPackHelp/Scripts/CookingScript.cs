using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KevinIglesias {
    
    public class CookingScript : MonoBehaviour {

        public GameObject salt;
        public GameObject saltInHand;
        
        public GameObject tube;
        public GameObject tubeInHand;
        
        public void GetSalt(bool dropSalt)
        {
            salt.SetActive(dropSalt);
            saltInHand.SetActive(!dropSalt);
        }
        
        public void GetTube(bool dropTube)
        {
            tube.SetActive(dropTube);
            tubeInHand.SetActive(!dropTube);
        }
    }
}
