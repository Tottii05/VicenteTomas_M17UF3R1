using UnityEditor;
using UnityEngine;

public static class ClearPlayerPrefsOnPlay
{
     [InitializeOnEnterPlayMode]
     private static void OnEnterPlayMode(EnterPlayModeOptions options)
     {
         PlayerPrefs.DeleteAll();
         Debug.Log("ClearPlayerPrefsOnPlay: PlayerPrefs reset - All data cleared before entering Play Mode!");
     }
}