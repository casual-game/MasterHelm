using System.Collections;
using System.Collections.Generic;
//using Beautify.Universal;
using UnityEditor;
using UnityEngine;

public partial class Manager_Main : MonoBehaviour
{
}
#if UNITY_EDITOR
[InitializeOnLoad]
public static class PlayModeStateChangedExample
{
	static PlayModeStateChangedExample()
	{
		EditorApplication.playModeStateChanged += LogPlayModeState;
	}

	private static void LogPlayModeState(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode)
		{
			/*
			BeautifySettings.settings.tintColor.Override(Color.white);
			BeautifySettings.settings.blurIntensity.Override(0);
			
			BeautifySettings.settings.frameBandHorizontalSize.Override(0);
			BeautifySettings.settings.frameBandVerticalSize.Override(0);
			
			BeautifySettings.settings.depthOfFieldFocalLength.Override(0.03f);
			*/
		}
	}
}
#endif