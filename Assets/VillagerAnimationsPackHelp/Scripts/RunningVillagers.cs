using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class RunningVillagers : MonoBehaviour {

    
        Transform villager;
        
        public float destination = 7;
    
        public bool going;
    
        void Awake()
        {
            villager = transform;
        }
    
        // Update is called once per frame
        void Update () {
            if(!going)
            {
                if(villager.position.x >= destination)
                {
                    going = true;
                    villager.localEulerAngles = new Vector3(villager.localEulerAngles.x, villager.localEulerAngles.y+180, villager.localEulerAngles.z);
                }
            }else{
                if(villager.position.x <= -destination)
                {
                    villager.localEulerAngles = new Vector3(villager.localEulerAngles.x, villager.localEulerAngles.y-180, villager.localEulerAngles.z);
                    going = false;
                }
            }
        }
    }
}
