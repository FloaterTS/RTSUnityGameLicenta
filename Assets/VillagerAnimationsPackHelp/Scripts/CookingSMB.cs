using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class CookingSMB : StateMachineBehaviour {


        public float timePoint;
            
        public bool salt;
        public bool dropSalt;
        public bool tube;
        public bool dropTube;
        
        CookingScript cookingComponent;
        
        bool changeDone;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {	
        
            if(cookingComponent == null)
            {
                cookingComponent = animator.GetComponent<CookingScript>();
            }
            
            changeDone = false;
            
            if(cookingComponent == null)
            {
                changeDone = true;
            }
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {
            if(!changeDone)
            {
                if(stateInfo.normalizedTime >= timePoint)
                {
                    if(salt)
                    {
                        cookingComponent.GetSalt(dropSalt);
                        
                    }else{
                        cookingComponent.GetTube(dropTube);
                    }
                    changeDone = true;
                }
            }

        }

    }
}
