///////////////////////////////////////////////////////////////////////////
//  IK Helper Tool 1.1 - Custom Inspector                                //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
//  Documentation: 														 //
//  https://www.keviniglesias.com/assets/IKHelperTool/Documentation.pdf  //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;

namespace KevinIglesias {

    [CustomEditor(typeof(IKHelperTool))]
    public class IKHelperToolCustomInspector : Editor
    {
        [System.Obsolete]
#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        public override void OnInspectorGUI()
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
        {
            var iKScript = target as IKHelperTool;
       
			if(iKScript.stateIKs == null)
			{
				iKScript.stateIKs = new List<StateIK>();
			}
			List <StateIK> iStateIKs = new List<StateIK>(iKScript.stateIKs);
	   
			GUILayout.Space(5);
	   
			GUIContent buttonContent = null;
			
			//Edit Mode Button
			GUILayout.BeginHorizontal();
			GUILayout.Space(30);
			GUI.enabled = EditorApplication.isPlaying;
			if(iKScript.editingIK)
			{
				buttonContent = new GUIContent("[Editing Mode ON]\nAnimation stopped.", "Click to return to normal speed.");
				if(GUILayout.Button(buttonContent))
				{
					iKScript.animator.speed = iKScript.animatorSpeed;
					iKScript.editingIK = false;
				}
			}else{
				buttonContent = new GUIContent("[Editing Mode OFF]\n(Only in Play mode)", "Easily edit your attachment (Play mode only).");
				if(GUILayout.Button(buttonContent))
				{
					iKScript.animatorSpeed = iKScript.animator.speed;
					iKScript.animator.speed = 0f;
					iKScript.editingIK = true;
				}
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			
			//Separator line
			GUILayout.BeginHorizontal();
			GUILayout.Space(30);
			IKHelperUtils.DrawUILine(Color.black, 1, 5);
			GUILayout.EndHorizontal();
	   
			//IK States
			for(int i = 0; i < iKScript.stateIKs.Count; i++)
			{
				//[X] Remove IK Button
				GUILayout.BeginHorizontal();
				GUILayout.Space(30);
				buttonContent = new GUIContent("[X]", "Remove this IK State");
				if(GUILayout.Button(buttonContent, GUILayout.Width(30)))
				{
					EditorGUI.BeginChangeCheck();
					GUI.changed = true;
					if(EditorGUI.EndChangeCheck()) 
					{
						Undo.RegisterCompleteObjectUndo(target, "Removed State IK");
					}
					iKScript.stateIKs.RemoveAt(i);
					break;
				}

				//Open State Button (ID and Name)
				string stateName = "ID: "+i.ToString("00")+" - "+iKScript.stateIKs[i].iKName;
				Color previousColor = GUI.color;
				
				if(iKScript.playing && iKScript.stateIKs[i].isActive)
				{
					stateName = stateName+" (PLAYING)";

                    if(iKScript.stateIKs[i].incomplete)
                    {
                        GUI.color = IKColors.incompleteIKColor;
                    }else{
                        GUI.color = IKColors.workingIKColor;
                    }
				}
				
				if(iKScript.stateIKs[i].open)
				{
					GUI.skin.button.alignment = TextAnchor.MiddleLeft;
					if(GUILayout.Button("▼ "+stateName))
					{
						iKScript.stateIKs[i].open = !iKScript.stateIKs[i].open;
					}
                    GUI.color = previousColor;
					GUILayout.EndHorizontal();
					GUI.skin.button.alignment = TextAnchor.MiddleCenter;
					
					//Rename field
					GUILayout.BeginHorizontal();
					GUILayout.Space(60);
					EditorGUI.BeginChangeCheck();
					string iName = EditorGUILayout.TextField("", iKScript.stateIKs[i].iKName);
					if(EditorGUI.EndChangeCheck()) {
						Undo.RegisterCompleteObjectUndo(target, "IK State rename");
						iKScript.stateIKs[i].iKName = iName;
					}  
					GUILayout.EndHorizontal();
							
					//Single IK / Sequence IK Buttons
					GUILayout.BeginHorizontal();
					GUILayout.Space(60);
					GUI.enabled = iKScript.stateIKs[i].isSequence;
					buttonContent = new GUIContent("Single IK", "Use only 1 IK per animation (default)");
					if(GUILayout.Button(buttonContent))
					{
						iKScript.stateIKs[i].isSequence = !iKScript.stateIKs[i].isSequence;
					}
					
					if(iKScript.stateIKs[i].IKs.Count <= 0)
					{
						iKScript.stateIKs[i].IKs.Add(new IKAttachment());
					}
					
					GUI.enabled = !iKScript.stateIKs[i].isSequence;
					buttonContent = new GUIContent("Sequence of IK", "Use more than 1 IK in the same animation");
					if(GUILayout.Button(buttonContent))
					{
						iKScript.stateIKs[i].isSequence = !iKScript.stateIKs[i].isSequence;

					}
					GUILayout.EndHorizontal();
					
					//IK Attachments
					GUI.enabled = true;
					List<IKAttachment> iIKAttachments = new List<IKAttachment>(iKScript.stateIKs[i].IKs);
					for(int k = 0; k < iKScript.stateIKs[i].IKs.Count; k++)
					{
                        int incompleteFields = 0;
                        
						//Open State Button (ID and Name)
						string ikName = "Attachment ID: "+k.ToString("00");
						if(iKScript.playing && iKScript.stateIKs[i].isActive && iKScript.stateIKs[i].activeAttachment == k)
						{
							ikName = ikName+" [PLAYING] "+(iKScript.stateIKs[i].IKs[k].weight*100f).ToString("00")+"%";
						}
						
						GUILayout.BeginHorizontal();
						GUILayout.Space(60);
						EditorGUILayout.LabelField(ikName, EditorStyles.boldLabel);
						GUILayout.EndHorizontal();
						
						
						GUILayout.BeginHorizontal();
						GUILayout.Space(60);
						
						//Remove IK Attachment (IK Sequence only)
						if(iKScript.stateIKs[i].isSequence)
						{
							if(k > 0)
							{
								buttonContent = new GUIContent("[X]", "Remove this IK Attachment");
								if(GUILayout.Button(buttonContent, GUILayout.Width(30)))
								{
									EditorGUI.BeginChangeCheck();
									GUI.changed = true;
									if(EditorGUI.EndChangeCheck()) 
									{
										Undo.RegisterCompleteObjectUndo(target, "Removed IK Attachment");
									}
									iKScript.stateIKs[i].IKs.RemoveAt(k);
									break;
								}
							}
						}
									
                        if(iKScript.stateIKs[i].IKs[k].iKAttachment == null)
                        {
                            GUI.color = IKColors.incompleteIKColor;
                            incompleteFields = 2;
                        }
                                
						//IK Attachment Transform field (Game Object)
						EditorGUI.BeginChangeCheck();
						Transform iIKAttachment = EditorGUILayout.ObjectField("", iKScript.stateIKs[i].IKs[k].iKAttachment, typeof(Transform)) as Transform;
                        if(EditorGUI.EndChangeCheck()) {
							Undo.RegisterCompleteObjectUndo(target, "Change IK Attachment");
							iKScript.stateIKs[i].IKs[k].iKAttachment = iIKAttachment;
						}
						GUILayout.EndHorizontal();
						GUI.color = previousColor;
                        
                        if(!iKScript.stateIKs[i].IKs[k].useLocation)
                        {
                            if(!iKScript.stateIKs[i].IKs[k].useRotation)
                            {
                                GUI.color = IKColors.incompleteIKColor;
                            }
                            incompleteFields++;
                        }
                        
						//Enable follow IK Attachment Location
						GUILayout.BeginHorizontal();
						GUILayout.Space(60);
						EditorGUI.BeginChangeCheck();
						bool iUseLocation = EditorGUILayout.Toggle("Use Location", iKScript.stateIKs[i].IKs[k].useLocation);
						if(EditorGUI.EndChangeCheck()) {
							Undo.RegisterCompleteObjectUndo(target, "Change IK Use Location");
							iKScript.stateIKs[i].IKs[k].useLocation = iUseLocation;
						}
						GUILayout.EndHorizontal();
						GUI.color = previousColor;
                        
                        
                        if(!iKScript.stateIKs[i].IKs[k].useRotation)
                        {
                            if(!iKScript.stateIKs[i].IKs[k].useLocation)
                            {
                                GUI.color = IKColors.incompleteIKColor;
                            }
                            incompleteFields++;
                        }
                        
						//Enable follow IK Attachment Rotation
						GUILayout.BeginHorizontal();
						GUILayout.Space(60);
						EditorGUI.BeginChangeCheck();
						bool iUseRotation = EditorGUILayout.Toggle("Use Rotation", iKScript.stateIKs[i].IKs[k].useRotation);
						if(EditorGUI.EndChangeCheck()) {
							Undo.RegisterCompleteObjectUndo(target, "Change IK Use Rotation");
							iKScript.stateIKs[i].IKs[k].useRotation = iUseRotation;
						}
						GUILayout.EndHorizontal();
						GUI.color = previousColor;

                        if(incompleteFields >= 2)
                        {
                            iKScript.stateIKs[i].incomplete = true;
                        }else{
                            iKScript.stateIKs[i].incomplete = false;
                        }
                        
						//Copy IK Attachment Transform values in Play Mode
						GUI.enabled = true;
						GUILayout.BeginHorizontal();
						GUILayout.Space(60);
						if(EditorApplication.isPlaying)
						{
							if(IKHelperUtils.IKClipboard && IKHelperUtils.IKClipBoardId == k)
							{
								buttonContent = new GUIContent("IK "+k.ToString("00")+" copied!", "Copy changes made in Play Mode");
							}else{
								buttonContent = new GUIContent("Copy IK Position/Rotation", "Copy changes made in Play Mode");
							}
							if(GUILayout.Button(buttonContent))
							{
								IKHelperUtils.copiedPos = iKScript.stateIKs[i].IKs[k].iKAttachment.localPosition;
								IKHelperUtils.copiedRot = iKScript.stateIKs[i].IKs[k].iKAttachment.localEulerAngles;
								
								IKHelperUtils.IKClipboard = true;
								IKHelperUtils.IKClipBoardId = k;
							}
						}else{
						//Paste IK Attachment Transform values from Play Mode
							GUI.enabled = IKHelperUtils.IKClipboard;

                            buttonContent = new GUIContent("Paste IK Position/Rotation", "Paste changes made in Play Mode");
                            
							if(GUILayout.Button(buttonContent))
							{
								EditorGUI.BeginChangeCheck();
								GUI.changed = true;
								
								if(EditorGUI.EndChangeCheck()) {
									Undo.RegisterCompleteObjectUndo(target, "Pasted "+k.ToString("00")+" IK Pos/Rot");
								}
								
								iKScript.stateIKs[i].IKs[k].iKAttachment.localPosition = IKHelperUtils.copiedPos;
								iKScript.stateIKs[i].IKs[k].iKAttachment.localEulerAngles = IKHelperUtils.copiedRot;

                                Debug.Log("Pasted IK Position/Rotation on IK Attachment "+k.ToString("00")+" in State "+i.ToString("00")+".");
							}	
							GUI.enabled = true;
						}
						GUILayout.EndHorizontal();
						
						if(iKScript.stateIKs[i].isSequence)
						{
							GUILayout.BeginHorizontal();
							GUILayout.Space(60);
							IKHelperUtils.DrawUILine(Color.black, 1, 10);
							GUILayout.EndHorizontal();
						}else{
							break;
						}
					}

					//Add IK Attachment (IK Sequence only)
					if(iKScript.stateIKs[i].isSequence)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Space(60);
						
						iIKAttachments = new List<IKAttachment>(iKScript.stateIKs[i].IKs);
						buttonContent = new GUIContent("[+] Add IK Attachment", "Add a new IK attachment");
						if(GUILayout.Button(buttonContent))
						{
							EditorGUI.BeginChangeCheck();
							GUI.changed = true;
							
							if(EditorGUI.EndChangeCheck()) 
							{
								Undo.RegisterCompleteObjectUndo(target, "Added IK Attachment");
							}
							
							iKScript.stateIKs[i].IKs.Add(new IKAttachment());
						}
						GUILayout.EndHorizontal();
					}
				}else{
					GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    if(GUILayout.Button("► "+stateName))
                    {
                        iKScript.stateIKs[i].open = !iKScript.stateIKs[i].open;
                    }
                    GUI.color = previousColor;
                    GUILayout.EndHorizontal();
					GUI.skin.button.alignment = TextAnchor.MiddleCenter;
				}
				GUILayout.BeginHorizontal();
				GUILayout.Space(30);
				IKHelperUtils.DrawUILine(Color.black, 1, 5);
				GUILayout.EndHorizontal();
			}
				
			//[+] Add State IK Button	
			GUILayout.BeginHorizontal();
			GUILayout.Space(30);
			iStateIKs = new List<StateIK>(iKScript.stateIKs);
			buttonContent = new GUIContent("[+] Add State IK", "Add a new IK to take effect while in a state");
			if(GUILayout.Button(buttonContent))
			{
				EditorGUI.BeginChangeCheck();
				GUI.changed = true;
				
				if(EditorGUI.EndChangeCheck()) 
				{
					Undo.RegisterCompleteObjectUndo(target, "Added State IK");
				}
				
				iKScript.stateIKs.Add(new StateIK());
			}
			GUILayout.EndHorizontal();
	   
            if(GUI.changed)
            {
                if(!EditorApplication.isPlaying)
				{
					EditorUtility.SetDirty(target);
					EditorSceneManager.MarkAllScenesDirty();
                }
            }
        }
    }
}
#endif