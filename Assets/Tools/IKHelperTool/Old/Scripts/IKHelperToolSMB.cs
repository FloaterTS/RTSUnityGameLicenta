///////////////////////////////////////////////////////////////////////////
//  IK Helper Tool 1.1 - Single IK StateMachineBehaviour                 //
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

	public class IKHelperToolSMB : StateMachineBehaviour
	{
		private IKHelperTool iKHTScript;
		
		public int id;
		
		public int selectorIKType;
		public IKType goal;
		
		public bool smoothEntry;

		public bool defaultState;
		
		public float speed;
        
        public bool clearOnExit;
        
		private int defaultCount = -1;
        
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if(iKHTScript == null)
			{
				iKHTScript = animator.GetComponent<IKHelperTool>();
			}
			
			if(iKHTScript != null)
			{
                if(defaultState && defaultCount == -1)
                {
                    defaultCount = 0;
                    iKHTScript.StartSingleIK(id, goal, false, speed);
                }else{
                    iKHTScript.StartSingleIK(id, goal, smoothEntry, speed);
                }
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
	[CustomEditor(typeof(IKHelperToolSMB))]
    public class IKHelperToolSMBCustomInspector : Editor
    {
		public override void OnInspectorGUI()
		{
			var SMBScript = target as IKHelperToolSMB;

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
			
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Attachment ID: 00", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			bool iBool = EditorGUILayout.Toggle("Smooth Entry", SMBScript.smoothEntry);
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
			
            if(SMBScript.speed < 0.01f)
            {
                SMBScript.speed = 0.01f;
            }
            
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			float iFloat = EditorGUILayout.FloatField("Speed (seconds):", SMBScript.speed);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Change IK Speed");
				SMBScript.speed = iFloat;
			}
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			iBool = EditorGUILayout.Toggle("Skip First Time", SMBScript.defaultState);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Change Skip First Time");
				SMBScript.defaultState = iBool;
			}
			GUILayout.EndHorizontal();
         
			GUI.enabled = true;
			
            IKHelperUtils.DrawUILine(Color.black, 1, 5);
            
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            bool clearExit = EditorGUILayout.Toggle("Clear IK on Exit:", SMBScript.clearOnExit);
            if(EditorGUI.EndChangeCheck()) {
                Undo.RegisterCompleteObjectUndo(target, "Change Clear IK on Exit");
                SMBScript.clearOnExit = clearExit;
            }
            GUILayout.EndHorizontal();
            
			GUILayout.Space(2);
			
			GUILayout.BeginHorizontal();
			GUIContent buttonContent;
			if(EditorApplication.isPlaying)
			{
				if(IKHelperUtils.singleClipboard)
				{
					buttonContent = new GUIContent("Variables copied!", "Copy changes made in Play Mode");
				}else{
					buttonContent = new GUIContent("Copy variables", "Copy changes made in Play Mode");
				}
				if(GUILayout.Button(buttonContent))
				{
					IKHelperUtils.savedSingleSMB.id = SMBScript.id;
					IKHelperUtils.savedSingleSMB.selectorIKType = SMBScript.selectorIKType;
					IKHelperUtils.savedSingleSMB.goal = SMBScript.goal;
					IKHelperUtils.savedSingleSMB.smoothEntry = SMBScript.smoothEntry;
					IKHelperUtils.savedSingleSMB.speed = SMBScript.speed;
					IKHelperUtils.savedSingleSMB.defaultState = SMBScript.defaultState;
					IKHelperUtils.savedSingleSMB.clearOnExit = SMBScript.clearOnExit;
					IKHelperUtils.singleClipboard = true;
				}
			}else{
				
				GUI.enabled = IKHelperUtils.singleClipboard;
				buttonContent = new GUIContent("Paste variables", "Paste changes made in Play Mode");
				if(GUILayout.Button(buttonContent))
				{
					EditorGUI.BeginChangeCheck();
					GUI.changed = true;
					
					if(EditorGUI.EndChangeCheck()) {
						Undo.RegisterCompleteObjectUndo(target, "Pasted variables");
					}
					
					SMBScript.id = IKHelperUtils.savedSingleSMB.id;
					SMBScript.selectorIKType = IKHelperUtils.savedSingleSMB.selectorIKType;
					SMBScript.goal = IKHelperUtils.savedSingleSMB.goal;
					SMBScript.smoothEntry = IKHelperUtils.savedSingleSMB.smoothEntry;
					SMBScript.speed = IKHelperUtils.savedSingleSMB.speed;
					SMBScript.defaultState = IKHelperUtils.savedSingleSMB.defaultState;
                    SMBScript.clearOnExit = IKHelperUtils.savedSingleSMB.clearOnExit;
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