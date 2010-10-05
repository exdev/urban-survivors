 //-----------------------------------------------------------------
//  Copyright 2010 Brady Wright and Above and Beyond Software
//	All rights reserved
//-----------------------------------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;

// Only compile if not using Unity iPhone
#if !UNITY_IPHONE || UNITY_3_0
[CustomEditor(typeof(UIBistateInteractivePanel))]
#endif
public class UIBistateInteractivePanelInspector : UIPanelBaseInspector
{
}