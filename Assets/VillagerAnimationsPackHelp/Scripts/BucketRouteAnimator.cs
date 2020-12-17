using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KevinIglesias {

    public class BucketRouteAnimator : StateMachineBehaviour {

        BucketRoute bR;
        
        public float delay;
        
        public bool lift;
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            
            if(bR == null)
            {
                bR = animator.GetComponent<BucketRoute>();
            }
            
            if(bR != null)
            {
               bR.LiftBucket(delay, lift);
            }

        }
    }

}
