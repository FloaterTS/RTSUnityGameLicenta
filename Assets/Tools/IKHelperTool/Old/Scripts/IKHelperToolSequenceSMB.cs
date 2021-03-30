///////////////////////////////////////////////////////////////////////////
//  IK Helper Tool 1.1 - Sequence IK StateMachineBehaviour               //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
//  Documentation: 														 //
//  https://www.keviniglesias.com/assets/IKHelperTool/Documentation.pdf  //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

//For custom inspector only
#if UNITY_EDITOR
	using UnityEditor;
#endif

namespace KevinIglesias {

	public class IKHelperToolSequenceSMB : StateMachineBehaviour
	{
		private IKHelperTool iKHTScript;
		
		public int id;
		
		public int selectorIKType;
		public IKType goal;
		
		public bool smoothEntry;

        public bool clearOnExit;
        
        public bool isLoop;
        
		public List<IKSequence> iKSequence = new List<IKSequence>();
		
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if(iKHTScript == null)
			{
				iKHTScript = animator.GetComponent<IKHelperTool>();
			}
			
			if(iKHTScript != null)
			{
                iKHTScript.StartSequence(id, goal, iKSequence, smoothEntry, isLoop);
			}
		}
        
        
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
            if(clearOnExit)
            {
                if(iKHTScript != null)
                {
                    iKHTScript.ClearIK(false, id, 0, false, 0);
                }
            }
        }
	}

#if UNITY_EDITOR
	//Custom Inspector
	[CustomEditor(typeof(IKHelperToolSequenceSMB))]
    public class IKHelperToolSequenceSMBCustomInspector : Editor
    {
		public override void OnInspectorGUI()
		{
			var SMBScript = target as IKHelperToolSequenceSMB;

			if(SMBScript.iKSequence == null)
			{
				SMBScript.iKSequence = new List<IKSequence>();
			}
			
			GUI.enabled = true;
            GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			EditorStyles.label.fontStyle = FontStyle.Bold;
			int iID = EditorGUILayout.IntField("State IK ID:", SMBScript.id);
			EditorStyles.label.fontStyle = FontStyle.Normal;
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Changed ID");
				SMBScript.id = iID;
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			iID = EditorGUILayout.Popup("IK Type:",  SMBScript.selectorIKType, IKHelperUtils.IKGoalNames, EditorStyles.popup);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Changed IK Goal");
				SMBScript.selectorIKType = iID;
				SMBScript.goal = (IKType)iID;
			}
			GUILayout.EndHorizontal();
			
			IKHelperUtils.DrawUILine(Color.black, 1, 5);
			
			GUIContent buttonContent = null;
			for(int i = 0; i < SMBScript.iKSequence.Count; i++)
			{
				
                    if(!SMBScript.iKSequence[i].useDefault)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        EditorStyles.label.fontStyle = FontStyle.Bold;
                        iID = EditorGUILayout.IntField("#"+i+"- Attachment ID: ", SMBScript.iKSequence[i].attachment);
                        EditorStyles.label.fontStyle = FontStyle.Normal;
                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RegisterCompleteObjectUndo(target, "Changed ID");
                            SMBScript.iKSequence[i].attachment = iID;
                        }
                        GUILayout.EndHorizontal();
                    }else{
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("#"+i+"- Default Animation", EditorStyles.boldLabel);
                        GUILayout.EndHorizontal();
                        
                    }
                    
                    GUI.enabled = true;
                    GUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    bool iDefault = EditorGUILayout.Toggle("Use default animation:", SMBScript.iKSequence[i].useDefault);
                    if(EditorGUI.EndChangeCheck()) {
                        Undo.RegisterCompleteObjectUndo(target, "Change Use Default Animation");
                        SMBScript.iKSequence[i].useDefault = iDefault;
                    }
                    GUILayout.EndHorizontal();
                    
                    if(i == 0)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        bool iBool = EditorGUILayout.Toggle("Smooth Entry:", SMBScript.smoothEntry);
                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RegisterCompleteObjectUndo(target, "Change Smooth Entry");
                            SMBScript.smoothEntry = iBool;
                        }
                        GUILayout.EndHorizontal();
                        
                        if(SMBScript.smoothEntry)
                        {
                            GUI.enabled = true;
                        }else{
                            GUI.enabled = false;
                        }
                        
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        float iFloat = EditorGUILayout.FloatField("Speed (seconds):", SMBScript.iKSequence[i].speed);
                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RegisterCompleteObjectUndo(target, "Change IK Speed");
                            SMBScript.iKSequence[i].speed = iFloat;
                        }
                        GUILayout.EndHorizontal();

                        GUI.enabled = true;
                    }else{
                        
                        
                        if(SMBScript.iKSequence[i].speed < 0.01f)
                        {
                            SMBScript.iKSequence[i].speed = 0.01f;
                        }
                        
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        float iFloat = EditorGUILayout.FloatField("Delay (seconds):", SMBScript.iKSequence[i].time);
                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RegisterCompleteObjectUndo(target, "Change IK Delay");
                            SMBScript.iKSequence[i].time = iFloat;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        iFloat = EditorGUILayout.FloatField("Speed (seconds):", SMBScript.iKSequence[i].speed);
                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RegisterCompleteObjectUndo(target, "Change IK Speed");
                            SMBScript.iKSequence[i].speed = iFloat;
                        }
                        GUILayout.EndHorizontal();
                        
                        GUILayout.BeginHorizontal();
                        buttonContent = new GUIContent("[X] Delete this IK Entry", "Remove this IK Entry");
                        if(GUILayout.Button(buttonContent))
                        {
                            EditorGUI.BeginChangeCheck();
                            GUI.changed = true;
                            if(EditorGUI.EndChangeCheck()) 
                            {
                                Undo.RegisterCompleteObjectUndo(target, "Removed IK Entry");
                            }
                            SMBScript.iKSequence.RemoveAt(i);
                            break;
                        }
                        GUILayout.EndHorizontal();
                    }
				
                IKHelperUtils.DrawUILine(Color.black, 1, 5);
			}
            

            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            bool isLoop = EditorGUILayout.Toggle("Loop:", SMBScript.isLoop);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RegisterCompleteObjectUndo(target, "Change is Loop");
                SMBScript.isLoop = isLoop;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            bool clearExit = EditorGUILayout.Toggle("Clear IK on Exit:", SMBScript.clearOnExit);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RegisterCompleteObjectUndo(target, "Change Clear IK on Exit");
                SMBScript.clearOnExit = clearExit;
            }
            GUILayout.EndHorizontal();
            
            GUI.enabled = true;
            
            IKHelperUtils.DrawUILine(Color.black, 1, 5);
			
			GUILayout.BeginHorizontal();
			List<IKSequence> iIKSequence = new List<IKSequence>(SMBScript.iKSequence);
			buttonContent = new GUIContent("[+] Add IK Entry (IK Attachment)", "Add a new IK entry to link with an IK Attachment");
			if(GUILayout.Button(buttonContent))
			{
				EditorGUI.BeginChangeCheck();
				GUI.changed = true;
				
				if(EditorGUI.EndChangeCheck()) 
				{
					Undo.RegisterCompleteObjectUndo(target, "Added IK Entry");
				}
				
				SMBScript.iKSequence.Add(new IKSequence());
			}
			GUILayout.EndHorizontal();
			
			GUILayout.Space(2);
			
			GUILayout.BeginHorizontal();
			if(EditorApplication.isPlaying)
			{
				if(IKHelperUtils.sequenceClipboard)
				{
					buttonContent = new GUIContent("Variables copied!", "Copy changes made in Play Mode");
				}else{
					buttonContent = new GUIContent("Copy variables", "Copy changes made in Play Mode");
				}
				if(GUILayout.Button(buttonContent))
				{
					IKHelperUtils.savedSequenceSMB.id = SMBScript.id;
					IKHelperUtils.savedSequenceSMB.selectorIKType = SMBScript.selectorIKType;
					IKHelperUtils.savedSequenceSMB.goal = SMBScript.goal;
					IKHelperUtils.savedSequenceSMB.smoothEntry = SMBScript.smoothEntry;
                    IKHelperUtils.savedSequenceSMB.clearOnExit = SMBScript.clearOnExit;
                    IKHelperUtils.savedSequenceSMB.isLoop = SMBScript.isLoop;
					IKHelperUtils.savedSequenceSMB.iKSequence = new List<IKSequence>(SMBScript.iKSequence);
                    
					IKHelperUtils.sequenceClipboard = true;
				}
			}else{
				
				GUI.enabled = IKHelperUtils.sequenceClipboard;
				buttonContent = new GUIContent("Paste variables", "Paste changes made in Play Mode");
				if(GUILayout.Button(buttonContent))
				{
					EditorGUI.BeginChangeCheck();
					GUI.changed = true;
					
					if(EditorGUI.EndChangeCheck()) {
						Undo.RegisterCompleteObjectUndo(target, "Pasted variables");
					}
					
					SMBScript.id = IKHelperUtils.savedSequenceSMB.id;
					SMBScript.selectorIKType = IKHelperUtils.savedSequenceSMB.selectorIKType;
					SMBScript.goal = IKHelperUtils.savedSequenceSMB.goal;
					SMBScript.smoothEntry = IKHelperUtils.savedSequenceSMB.smoothEntry;
                    SMBScript.clearOnExit = IKHelperUtils.savedSequenceSMB.clearOnExit;
                    SMBScript.isLoop = IKHelperUtils.savedSequenceSMB.isLoop;
					SMBScript.iKSequence = new List<IKSequence>(IKHelperUtils.savedSequenceSMB.iKSequence);
                    
                    Debug.Log("Pasted variables.");
				}	
				GUI.enabled = true;
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
#endif	
}