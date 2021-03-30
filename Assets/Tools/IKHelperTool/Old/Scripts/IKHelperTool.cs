///////////////////////////////////////////////////////////////////////////
//  IK Helper Tool 1.1 - Core Script / MonoBehaviour                     //
//  Kevin Iglesias - https://www.keviniglesias.com/       			     //
//  Contact Support: support@keviniglesias.com                           //
//  Documentation: 														 //
//  https://www.keviniglesias.com/assets/IKHelperTool/Documentation.pdf  //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KevinIglesias {

    [RequireComponent(typeof(Animator))]
    public class IKHelperTool : MonoBehaviour {

		[HideInInspector]
        public Animator animator;
        
        public List<StateIK> stateIKs;
        
		public bool playing = false;
		
		public bool editingIK = false;
		public float animatorSpeed = 0f;
        
		public int[] previousIK = new int[4];
		
		///INITIALIZE
		void Awake()
		{
			animator = GetComponent<Animator>();
			
			//Check humanoid avatar animator
			if(!animator.isHuman)
			{
				Debug.Log("Warning: Animator Avatar is not Human. IK Helper Tool may not work properly.");
			}
			
			editingIK = false;
			
			if(stateIKs == null)
			{
				stateIKs = new List<StateIK>();
			}
			
			previousIK = new int[4] {-1, -1, -1, -1};
			
			for(int i = 0; i < stateIKs.Count; i++)
			{
				for(int j = 0; j < stateIKs[i].IKs.Count; j++)
				{
					if(stateIKs[i].IKs[j].iKAttachment != null)
					{
						stateIKs[i].IKs[j].initPos = stateIKs[i].IKs[j].iKAttachment.localPosition;
						stateIKs[i].IKs[j].initRot = stateIKs[i].IKs[j].iKAttachment.localRotation;
					}
				}
			}
		}

		///IK FUNCTIONS
		//Start playing IK (Single)
		public void StartSingleIK(int id, IKType goal, bool smooth, float speed)
		{
			if(!CheckAvailability(id))
			{
				return;
			}	
			
            if(stateIKs[id].isActive)
            {
                return;
            }
            
			StateIK state = null;
			
            
            
			if(previousIK[(int)goal] > -1 && previousIK[(int)goal] != id)
			{//Used when switching between IK States
				
				if(smooth)
				{
					state = stateIKs[previousIK[(int)goal]];
					
					StopIK(previousIK[(int)goal]);
					previousIK[(int)goal] = id;
					state.stateCoroutine = SmoothStateIKChange(stateIKs[id], state, speed, goal);
					StartCoroutine(state.stateCoroutine);
				}
			}else{
			//Used by default
				
				previousIK[(int)goal] = id;
				
				state = stateIKs[id];
				state.isActive = true;
				state.activeAttachment = 0;
				state.iKGoal = goal;
				
				if(smooth)
				{
					state.IKs[0].weight = 0;
				}else{
					state.IKs[0].weight = 1;
				}
				
				StopIK(id);
				state.stateCoroutine = PerformSingleIK(id, state.IKs[state.activeAttachment], 0, smooth, speed);
				StartCoroutine(state.stateCoroutine);
			}
		}

		//Perform IK (Single)
		IEnumerator PerformSingleIK(int id, IKAttachment ik, float delay, bool smooth, float speed)
		{
            if(delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            
            float initialWeight = 0;
			float finalWeight = 1;

            playing = true;

            
            ik.iKAttachment.localPosition = ik.initPos;
            ik.iKAttachment.localRotation = ik.initRot;
            if(smooth)
            {
				float i = 0;
				while(i < 1)
				{
                    while(animator.speed == 0)
                    {
                        yield return 0;
                    }
                    
					i += Time.deltaTime/speed;
					ik.weight = Mathf.Lerp(initialWeight, finalWeight, i);
					yield return 0;
				}
            }
		}
        
        //Perform smooth state IK change (Single)
		IEnumerator SmoothStateIKChange(StateIK nextState, StateIK previousState, float speed, IKType goal)
		{
			previousState.IKs[0].iKAttachment.localPosition = previousState.IKs[0].initPos;
			previousState.IKs[0].iKAttachment.localRotation = previousState.IKs[0].initRot;
			
			previousState.IKs[0].weight = 1;
					
			Vector3 endPos = nextState.IKs[0].iKAttachment.localPosition;
			Quaternion endRot = nextState.IKs[0].iKAttachment.localRotation;
			float i = 0;
			while(i < 1)
			{
                while(animator.speed == 0)
                {
                    yield return 0;
                }
                
				i += Time.deltaTime/speed;
				previousState.IKs[0].iKAttachment.localPosition = Vector3.Lerp(previousState.IKs[0].initPos, endPos, i);
				previousState.IKs[0].iKAttachment.localRotation = Quaternion.Lerp(previousState.IKs[0].initRot, endRot, i);
				yield return 0;
			}
			
			playing = true;
			
			nextState.isActive = true;
			nextState.activeAttachment = 0;
			nextState.iKGoal = goal;
			nextState.IKs[0].weight = 1;

			previousState.isActive = false;
	
			previousState.IKs[0].iKAttachment.localPosition = previousState.IKs[0].initPos;
			previousState.IKs[0].iKAttachment.localRotation = previousState.IKs[0].initRot;
		}
        
        //Perform IK clear
		IEnumerator PerformIKClear(int id, IKAttachment ik, float delay, bool smooth, float speed)
		{
            if(delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            
            float initialWeight = 1;
			float finalWeight = 0;
            
            
            ik.iKAttachment.localPosition = ik.initPos;
            ik.iKAttachment.localRotation = ik.initRot;
            if(smooth)
            {
               
				float i = 0;
				while(i < 1)
				{
                    while(animator.speed == 0)
                    {
                        yield return 0;
                    }
                    
					i += Time.deltaTime/speed;
					ik.weight = Mathf.Lerp(initialWeight, finalWeight, i);
					yield return 0;
				}
            }

            stateIKs[id].isActive = false;
		}

		//Start playing IK (Sequence)
		public void StartSequence(int id, IKType goal, List<IKSequence> iKSequence, bool smoothEntry, bool isLoop)
		{
			if(!CheckAvailability(id))
			{
				return;
			}
			
			StateIK state = stateIKs[id];
			state.activeAttachment = 0;
			state.iKGoal = goal;

            if(smoothEntry)
            {
                state.IKs[0].weight = 0;
            }else{
                state.IKs[0].weight = 1;
            }
            
			StopIK(id);
			state.isActive = true;
			state.stateCoroutine = PerformSequence(id, iKSequence, smoothEntry, isLoop, 0);
			StartCoroutine(state.stateCoroutine);
		}
		
		//Perform IK (Sequence)
		IEnumerator PerformSequence(int id, List<IKSequence> iKSequence, bool smoothEntry, bool isLoop, int counts)
		{
            
            playing = true;
            
			int activeAttachment = 0; 
			
			IKAttachment ik = stateIKs[id].IKs[activeAttachment];
			
            ik.iKAttachment.localPosition = ik.initPos;
            ik.iKAttachment.localRotation = ik.initRot;
			if(!iKSequence[0].useDefault)
			{
				if(smoothEntry && counts == 0)
				{
					float i = 0;
					while(i < 1)
					{
                        while(animator.speed == 0)
                        {
                            yield return 0;
                        }
                        
						i += Time.deltaTime/iKSequence[activeAttachment].speed;
						ik.weight = Mathf.Lerp(0, 1, i);
						yield return 0;
					}
				}
				ik.weight = 1;
			}else{
				if(smoothEntry && counts == 0)
				{
					float i = 0;
					while(i < 1)
					{
                        while(animator.speed == 0)
                        {
                            yield return 0;
                        }
                        
						i += Time.deltaTime/iKSequence[activeAttachment].speed;
						ik.weight = Mathf.Lerp(1, 0, i);
						yield return 0;
					}
				}
				ik.weight = 0;
			}
			
			Vector3 endPos = Vector3.zero;
			Quaternion endRot = Quaternion.identity;
			for(int j = 1; j < iKSequence.Count; j++)
			{
				
				if(stateIKs[id].IKs.Count <= iKSequence[j].attachment)
				{
					Debug.Log("No IK attachments with ID:"+(iKSequence[j].attachment.ToString("00"))+" found in State IK: "+stateIKs[id].iKName+" - "+this.gameObject.name);
					break;
				}
				
				if(stateIKs[id].IKs[iKSequence[j].attachment].iKAttachment == null )
				{
					Debug.Log("Missing attachment Transform with ID:"+(iKSequence[j].attachment.ToString("00"))+" found in State IK: "+stateIKs[id].iKName+" - "+this.gameObject.name);
					break;
				}

                if(counts == 0)
                {
                    if(j == 1)
                    {
                        yield return new WaitForSeconds(iKSequence[j].time-iKSequence[0].speed);
                    }else{
                        yield return new WaitForSeconds(iKSequence[j].time);
                    }
                }else{
                    yield return new WaitForSeconds(iKSequence[j].time);
                }
                
                
				
		
                if(ik.weight == 0)
                {//Follow default animation without IK (first IK Attachment)
                    activeAttachment = iKSequence[j].attachment;
					stateIKs[id].activeAttachment = activeAttachment;
                    ik = stateIKs[id].IKs[activeAttachment];
                    
                    ik.weight = 0;
                    
                    float h = 0;
					while(h < 1)
					{
                        
                        while(animator.speed == 0)
                        {
                            yield return 0;
                        }
                        
						h += Time.deltaTime/iKSequence[activeAttachment].speed;
						ik.weight = Mathf.Lerp(0, 1, h);
						yield return 0;
					}
                }else{
					if(!iKSequence[j].useDefault)
					{//To next Transform IK attachment
						IKAttachment previousIKAttachment = ik;
						
						endPos = stateIKs[id].IKs[iKSequence[j].attachment].iKAttachment.localPosition;
						endRot = stateIKs[id].IKs[iKSequence[j].attachment].iKAttachment.localRotation;
						float k = 0;
						while(k < 1)
						{
                            
                            while(animator.speed == 0)
                            {
                                yield return 0;
                            }
                            
							k += Time.deltaTime/iKSequence[j].speed;
							ik.iKAttachment.localPosition = Vector3.Lerp(ik.initPos, endPos, k);
							ik.iKAttachment.localRotation = Quaternion.Lerp(ik.initRot, endRot, k);
							yield return 0;
						}

						activeAttachment = iKSequence[j].attachment;
						stateIKs[id].activeAttachment = activeAttachment;
						ik = stateIKs[id].IKs[activeAttachment];
						
						ik.weight = 1;
						
						previousIKAttachment.iKAttachment.localPosition = previousIKAttachment.initPos;
						previousIKAttachment.iKAttachment.localRotation = previousIKAttachment.initRot;
                    }else{
                    //Follow default animation without IK

						ik.weight = 1;
						float h = 0;
						while(h < 1)
						{
                            
                            while(animator.speed == 0)
                            {
                                yield return 0;
                            }
                            
							h += Time.deltaTime/iKSequence[j].speed;
							ik.weight = Mathf.Lerp(1, 0, h);
							yield return 0;
						}
					}
                }
			}

            if(isLoop)
            {

                AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] animClip = animator.GetCurrentAnimatorClipInfo(0);
                float animationTime = animationState.normalizedTime-(1*counts);
                
                if(animationTime < 1)
                {
                    while(animationTime < 1)
                    {
                        animationState = animator.GetCurrentAnimatorStateInfo(0);
                        animClip = animator.GetCurrentAnimatorClipInfo(0);
                        animationTime = animationState.normalizedTime-(1*counts);
                        yield return 0;
                    }
                }
                
                counts++;
                stateIKs[id].isActive = true;
                stateIKs[id].stateCoroutine = PerformSequence(id, iKSequence, smoothEntry, isLoop, counts);
                StartCoroutine(stateIKs[id].stateCoroutine);
            }
            
		}
        
        //Stop playing IK
        public void StopIK(int id)
		{
            if(!CheckAvailability(id))
			{
				return;
			}
            
			if(stateIKs[id].stateCoroutine != null)
			{
				StopCoroutine(stateIKs[id].stateCoroutine);
			}
		}
        
        //Clear current active IK
        public void ClearIK(bool clearAll, int id, float delay, bool smooth, float speed)
		{
			if(clearAll)
			{
				for(int i = 0; i < stateIKs.Count; i++)
				{
					if(!CheckAvailability(i))
					{
						return;
					}
					StopIK(i);
					stateIKs[i].stateCoroutine = PerformIKClear(i, stateIKs[i].IKs[stateIKs[i].activeAttachment], delay, smooth, speed);
					StartCoroutine(stateIKs[i].stateCoroutine);
				}
                playing = false;
			}else{
				if(!CheckAvailability(id))
				{
					return;
				}
				
				StopIK(id);
				stateIKs[id].stateCoroutine = PerformIKClear(id, stateIKs[id].IKs[stateIKs[id].activeAttachment], delay, smooth, speed);
				StartCoroutine(stateIKs[id].stateCoroutine);
			}		
		}
        
		///CORE FUNCTIONS
        void OnAnimatorIK(int layerIndex){

			if(playing)
			{
				for(int i = 0; i < stateIKs.Count; i++)
				{
					if(stateIKs[i].isActive)
					{
						SetIK(stateIKs[i].iKGoal, stateIKs[i].IKs[stateIKs[i].activeAttachment]);
					}
				}
			}
        }

        void SetIK(IKType type, IKAttachment ik)
        {

			Vector3 finalPos = ik.iKAttachment.position;
			Quaternion finalRot = ik.iKAttachment.rotation;
			
            switch(type)
            {
                case IKType.RightHand:
                
                    if(ik.useLocation)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ik.weight);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, finalPos);         
                    }
                    
                    if(ik.useRotation)
                    {
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ik.weight);     
                        animator.SetIKRotation(AvatarIKGoal.RightHand, finalRot);
                    }

                break;
                
                case IKType.LeftHand:
                    
                    if(ik.useLocation)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, ik.weight);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, finalPos);
                    }
                    
                    if(ik.useRotation)
                    {
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, ik.weight);                        
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, finalRot);
                    }
                break;
                
                case IKType.RightFoot:
                   
                    if(ik.useLocation)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, ik.weight);
                        animator.SetIKPosition(AvatarIKGoal.RightFoot, finalPos);         
                    }
                    
                    if(ik.useRotation)
                    {
                        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, ik.weight); 
                        animator.SetIKRotation(AvatarIKGoal.RightFoot, finalRot);                     
                    }
                break;
                
                case IKType.LeftFoot:
                    
                    if(ik.useLocation)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ik.weight);
                        animator.SetIKPosition(AvatarIKGoal.LeftFoot, finalPos);     
                    }
                    
                    if(ik.useRotation)
                    {
						animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ik.weight);  
                        animator.SetIKRotation(AvatarIKGoal.LeftFoot, finalRot);
                    }
                break;
            }
        }
		
		///MISC FUNCTIONS
		bool CheckAvailability(int id)
		{
			if(stateIKs == null || stateIKs.Count <= 0)
			{
				Debug.Log("No State IKs found in "+this.gameObject.name);
				return false;
			}
			
			if(stateIKs.Count <= id)
			{
				Debug.Log("No State IK with ID:"+(id.ToString("00"))+" found in "+this.gameObject.name);
				return false;
			}
			
			if(stateIKs[id].IKs == null || stateIKs[id].IKs.Count <= 0)
			{
				Debug.Log("No IK attachments found in State IK: "+stateIKs[id].iKName+" - "+this.gameObject.name);
				return false;
			}
			
			if(stateIKs[id].IKs[0].iKAttachment == null)
			{
				Debug.Log("IK attachment Transform missing in State IK: "+stateIKs[id].iKName+" - "+this.gameObject.name);
				return false;
			}
			
			return true;
		}
    }
}
