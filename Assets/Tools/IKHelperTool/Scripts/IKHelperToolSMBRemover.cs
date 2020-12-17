///////////////////////////////////////////////////////////////////////////
//  IK Helper Tool 1.1 - IK Remover StateMachineBehaviour                //
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

	public class IKHelperToolSMBRemover : StateMachineBehaviour
	{
		private IKHelperTool iKHTScript;
		
		public int id;
		
		public bool clearAll;
		
		public bool smoothExit;
		
        public float delay;
        
		public float speed;
		
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if(iKHTScript == null)
			{
				iKHTScript = animator.GetComponent<IKHelperTool>();
               
			}
			
			if(iKHTScript != null)
			{
				iKHTScript.ClearIK(clearAll, id, delay, smoothExit, speed);
			}
		}
	}
    
#if UNITY_EDITOR
	//Custom Inspector
	[CustomEditor(typeof(IKHelperToolSMBRemover))]
    public class IKHelperToolSMBRemoverCustomInspector : Editor
    {

		public override void OnInspectorGUI()
		{
			var SMBScript = target as IKHelperToolSMBRemover;

			
			GUI.enabled = true;
            GUILayout.Space(5);
			
			if(!SMBScript.clearAll)
			{
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
			}
			
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			bool iBool = EditorGUILayout.Toggle("Clear All IK States", SMBScript.clearAll);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Clear All IK States");
				SMBScript.clearAll = iBool;
			}
			GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			float iFloat = EditorGUILayout.FloatField("Delay (seconds)", SMBScript.delay);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Change Delay");
				SMBScript.delay = iFloat;
			}
			GUILayout.EndHorizontal();
            
			GUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();
			iBool = EditorGUILayout.Toggle("Smooth Exit", SMBScript.smoothExit);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Change Smooth Exit");
				SMBScript.smoothExit = iBool;
			}
			GUILayout.EndHorizontal();
			
			if(SMBScript.smoothExit)
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
			iFloat = EditorGUILayout.FloatField("Speed (seconds):", SMBScript.speed);
			if(EditorGUI.EndChangeCheck()) {
				Undo.RegisterCompleteObjectUndo(target, "Change IK Speed");
				SMBScript.speed = iFloat;
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
			
			GUILayout.Space(2);
			
			GUILayout.BeginHorizontal();
			GUIContent buttonContent;
			if(EditorApplication.isPlaying)
			{
				if(IKHelperUtils.clearClipboard)
				{
					buttonContent = new GUIContent("Variables copied!", "Copy changes made in Play Mode");
				}else{
					buttonContent = new GUIContent("Copy variables", "Copy changes made in Play Mode");
				}
				if(GUILayout.Button(buttonContent))
				{
					IKHelperUtils.savedRemoverSMB.id = SMBScript.id;
					IKHelperUtils.savedRemoverSMB.clearAll = SMBScript.clearAll;
					IKHelperUtils.savedRemoverSMB.smoothExit = SMBScript.smoothExit;
					IKHelperUtils.savedRemoverSMB.delay = SMBScript.delay;
					IKHelperUtils.savedRemoverSMB.speed = SMBScript.speed;
					
					IKHelperUtils.clearClipboard = true;
				}
			}else{
				
				GUI.enabled = IKHelperUtils.clearClipboard;
				buttonContent = new GUIContent("Paste variables", "Paste changes made in Play Mode");
				if(GUILayout.Button(buttonContent))
				{
					EditorGUI.BeginChangeCheck();
					GUI.changed = true;
					
					if(EditorGUI.EndChangeCheck()) {
						Undo.RegisterCompleteObjectUndo(target, "Pasted variables");
					}
					
					SMBScript.id = IKHelperUtils.savedRemoverSMB.id;
					SMBScript.clearAll = IKHelperUtils.savedRemoverSMB.clearAll;
					SMBScript.smoothExit = IKHelperUtils.savedRemoverSMB.smoothExit;
					SMBScript.delay = IKHelperUtils.savedRemoverSMB.delay;
					SMBScript.speed = IKHelperUtils.savedRemoverSMB.speed;
                    
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
