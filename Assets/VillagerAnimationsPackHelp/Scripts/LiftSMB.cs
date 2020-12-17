using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class LiftSMB : StateMachineBehaviour {


        public float timePoint;
            
        public bool dropItem;
        
        LiftScript liftComponent;
        
        bool changeDone;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {	
        
            if(liftComponent == null)
            {
                liftComponent = animator.GetComponent<LiftScript>();
            }
            
            changeDone = false;
            
            if(liftComponent == null)
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
                    
                    liftComponent.GetItem(dropItem);
                    
                    changeDone = true;
                }
            }

        }

    }
}
