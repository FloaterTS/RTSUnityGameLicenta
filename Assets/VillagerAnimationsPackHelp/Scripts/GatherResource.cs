using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {

    public enum ResourceType {Berries, Farm, Wood, Gold, Builder}

    [RequireComponent(typeof(Animator))]
    public class GatherResource : MonoBehaviour {

        public float delayTime;
    
        public ResourceType resource;
        
        public int currentPath = 0;
        
        Animator anim;
        
        public Transform[] paths;
        
        public GameObject[] props;
        
        Vector3 initRotation;
        
        void Awake()
        {
            anim = GetComponent<Animator>();
        }
        
        void Start()
        {
            initRotation = transform.eulerAngles;
            StartCoroutine(StartWorking());
        }
        
        IEnumerator StartWorking()
        {
            yield return new WaitForSeconds(delayTime);
            
            anim.SetTrigger("Work");
            
            switch(resource)
            {
                case ResourceType.Farm:
                   StartCoroutine(FarmCycle());
                break;
                
                case ResourceType.Berries:
                   StartCoroutine(BerryCycle());
                break;
                
                case ResourceType.Wood:
                   StartCoroutine(ChopTree());
                break;
                
                case ResourceType.Gold:
                   StartCoroutine(MiningCycle());
                break;
            }
        }
        
        
        IEnumerator FarmCycle()
        {

            props[1].SetActive(false);
            props[2].SetActive(false);
            props[3].SetActive(false); 
            props[0].SetActive(true);
            
            yield return new WaitForSeconds(4.75f);

            anim.SetTrigger("Walk");
            
            while(true)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    break;
                }
                
                yield return 0;
            }
            
            props[0].SetActive(false);
            props[1].SetActive(true);
            
            transform.LookAt(paths[currentPath]);   

            if(currentPath == 8)
            {
                
     
                props[0].SetActive(false);
                props[1].SetActive(false);
                props[2].SetActive(true);
                
                StartCoroutine(EndFarmCycle());
                
            }else{

                yield return new WaitForSeconds(0.75f);
                
                anim.SetTrigger("Work");
            
                currentPath++;
            
                StartCoroutine(FarmCycle());
            
            }
            
        }
        
        IEnumerator EndFarmCycle()
        {
            
            while(transform.position.z < paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            currentPath++;
            
            transform.LookAt(paths[currentPath]);
             
            props[0].SetActive(false);
            props[1].SetActive(false);
            props[2].SetActive(false); 
            props[3].SetActive(true); 
             
            while(transform.position.z > paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            currentPath = 0;
            
            anim.SetTrigger("Work");
            
            //yield return new WaitForSeconds(0.1f);
            
            while(true)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Farm"))
                {
                    break;
                }
                
                yield return 0;
            }
            
            StartCoroutine(FarmCycle());
            
        }
        
        
        IEnumerator BerryCycle()
        {
            props[0].SetActive(true);
            props[1].SetActive(false);
            props[2].SetActive(false);
            
            
            yield return new WaitForSeconds(8f);
            
            anim.SetTrigger("Walk");
            
            while(true)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    break;
                }
                
                yield return 0;
            }
            
            props[0].SetActive(false);
            props[1].SetActive(true);
            props[2].SetActive(false);
            
            
            currentPath++;
            
            transform.LookAt(paths[currentPath]);

            
            while(transform.position.z > paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            currentPath++;
            
            transform.LookAt(paths[currentPath]);
            
            
            while(transform.position.z < paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            props[0].SetActive(false);
            props[1].SetActive(false);
            props[2].SetActive(true);
            
            
            currentPath--;
            
            transform.LookAt(paths[currentPath]);
            
            while(transform.position.z > paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            currentPath--;
            
            transform.LookAt(paths[currentPath]);
            
            while(transform.position.z < paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            
            anim.SetTrigger("Work");
            
            
            
            StartCoroutine(BerryCycle());
            
        }
        
        
        
        IEnumerator ChopTree()
        {
            yield return new WaitForSeconds(3f);
            
            anim.SetTrigger("Work");
            
            
            StartCoroutine(ChopCycle());
        }
        

        IEnumerator ChopCycle()
        {
            
            transform.eulerAngles = new Vector3(0, 242f, 0);
            
            props[0].SetActive(true);
            props[1].SetActive(false);
            
            yield return new WaitForSeconds(9.6f);
            
            anim.SetTrigger("Walk");
            
            props[2].SetActive(false);
            yield return new WaitForSeconds(0.1f);
            
           
            props[3].SetActive(true);
            
            yield return new WaitForSeconds(0.2f);
            
             
            
            currentPath++;
            
            transform.LookAt(paths[currentPath]);
            
            while(transform.position.z < paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            currentPath++;
            
            transform.LookAt(paths[currentPath]);
            
            
            while(transform.position.z < paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            anim.SetTrigger("Walk");
            
            currentPath--;
            
            transform.LookAt(paths[currentPath]);
            
            while(true)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk Axe"))
                {
                    break;
                }
                
                yield return 0;
            }
            
            
            props[2].SetActive(true);
            props[3].SetActive(false);
            
            
            while(transform.position.z > paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            currentPath--;
            
            transform.LookAt(paths[currentPath]);
            
            while(transform.position.z > paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            
            anim.SetTrigger("Work");
            
            
            StartCoroutine(ChopCycle());
            
        }
        
        IEnumerator MiningCycle()
        {
  
            transform.eulerAngles = initRotation;
  
            yield return new WaitForSeconds(9.33f);
            

            anim.SetTrigger("Walk");
 
            
            props[0].SetActive(false);
            yield return new WaitForSeconds(0.1f);
            

            
            props[1].SetActive(true);
            
            yield return new WaitForSeconds(0.2f);
            
           
            
           
            
            currentPath++;
            
            transform.LookAt(paths[currentPath]);
            
            while(transform.position.z < paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            anim.SetTrigger("Walk");
            
            while(true)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Walk Pickaxe"))
                {
                    break;
                }
                
                yield return 0;
            }
            
            
            props[0].SetActive(true);
            props[1].SetActive(false);
            
            currentPath--;
            
            transform.LookAt(paths[currentPath]);
            
            while(transform.position.z > paths[currentPath].position.z)
            {
                yield return 0;
            }
            
            anim.SetTrigger("Work");

            StartCoroutine(MiningCycle());
            
        }
    
       
    }
}
