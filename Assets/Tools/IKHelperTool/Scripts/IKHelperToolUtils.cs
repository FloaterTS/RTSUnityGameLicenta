///////////////////////////////////////////////////////////////////////////
//  IK Helper Tool 1.1 - Utilities		                                 //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
//  Documentation: 														 //
//  https://www.keviniglesias.com/assets/IKHelperTool/Documentation.pdf  //
///////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace KevinIglesias {

    public class IKColors
    {
        public static Color workingIKColor = Color.green;
        
        public static Color incompleteIKColor = Color.yellow;
    }

    public enum IKType {RightHand, LeftHand, RightFoot, LeftFoot}
	
    [System.Serializable]
    public class StateIK{
        
		public string iKName;
		
		public List<IKAttachment> IKs = new List<IKAttachment>();
		
        public Color color;
        
		public bool open;
		
		public bool isSequence;
		
        public bool incomplete;
        
		public bool isActive;
		
		public int activeAttachment;
		
		public IKType iKGoal;
		
		public IEnumerator stateCoroutine;
		
        public StateIK()
        {
            IKs = new List<IKAttachment>();
            color = Color.white;
			iKName = "Unnamed";
        }
        
    }
	
	[System.Serializable]
    public class IKAttachment{
        
        public Transform iKAttachment;
        
        public bool useLocation;
        public bool useRotation;
        
		public Vector3 initPos;
		public Quaternion initRot;
		
        public float weight;

    }
	
	[System.Serializable]
    public class IKSequence{
        
		public int attachment;
		
		public float speed;
		
        public float time;
		
		public bool useDefault;
    }
	
#if UNITY_EDITOR
	
	public static class IKHelperUtils
	{
		public static IKHelperToolSMB savedSingleSMB = new IKHelperToolSMB();
		public static bool singleClipboard = false;
		
		public static IKHelperToolSequenceSMB savedSequenceSMB = new IKHelperToolSequenceSMB();
		public static bool sequenceClipboard = false;

		public static IKHelperToolSMBRemover savedRemoverSMB = new IKHelperToolSMBRemover();
		public static bool clearClipboard = false;

		public static Vector3 copiedPos;
		public static Vector3 copiedRot;
		public static bool IKClipboard = false;
		
		public static int IKClipBoardId = 0;
		
		//FUNCTION FOR DRAWING A SEPARATOR
		public static void DrawUILine(Color color, int thickness = 1, int padding = 2)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
			r.height = thickness;
			r.y+=padding/2;
			EditorGUI.DrawRect(r, color);
		}
		public static string[] IKGoalNames = new string[] {"Right Hand", "Left Hand", "Right Foot", "Left Foot"};
	}
	
#endif
}
