using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public class BucketRoute : MonoBehaviour {

        public GameObject[] buckets;
    
        public GameObject liftedBucket;
    
        public bool drop;
    
        public Transform villager;

        Animator anim;
        
        public bool going;
        
        void Awake()
        {
            anim = GetComponent<Animator>();
            going = true;
        }
        
        public void LiftBucket(float delay, bool lift)
        {
            StartCoroutine(ChangeBucket(delay, lift));
        }
        
        public IEnumerator ChangeBucket(float delay, bool lift)
        {
            float destination = 3.08f;
            float limit = 0f;
            
            
            if(going)
            {
                limit = destination;
            }else{
                limit = -destination;
            }

            yield return new WaitForSeconds(delay);

            
            if(lift)
            {
                liftedBucket.SetActive(true);
                if(going)
                {
                    buckets[0].SetActive(false);
                }else{
                    buckets[1].SetActive(false);
                }

                yield return new WaitForSeconds(1f);
                
                villager.localEulerAngles = new Vector3(villager.localEulerAngles.x, villager.localEulerAngles.y+180f, villager.localEulerAngles.z);
           
                if(going)
                {
                    while(villager.localPosition.z <= limit)
                    {
                        yield return 0;
                    }
                }else{
                    while(villager.localPosition.z >= limit)
                    {
                        yield return 0;
                    }
                }
                
                anim.applyRootMotion = false;

                villager.localPosition = new Vector3(villager.localPosition.x, villager.localPosition.y, limit);
                
                anim.SetTrigger("Stop");
                
                yield return new WaitForSeconds(0.1f);

                anim.applyRootMotion = true;
                
                going = !going;
            }else{
                
                liftedBucket.SetActive(false);
                if(going)
                {
                    buckets[0].SetActive(true);
                }else{
                    buckets[1].SetActive(true);
                }
            }

       }

    }

}
