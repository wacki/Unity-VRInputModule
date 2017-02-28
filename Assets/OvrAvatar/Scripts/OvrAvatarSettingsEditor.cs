#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(OvrAvatarSettings))]
public class OvrAvatarSettingsEditor : Editor {
    GUIContent appIDLabel = new GUIContent("Oculus Rift App Id [?]", 
      "This AppID will be used for OvrAvatar registration.");

    [UnityEditor.MenuItem("Oculus Avatars/Edit Configuration")]
    public static void Edit()
    {
        var settings = OvrAvatarSettings.Instance;
        UnityEditor.Selection.activeObject = settings;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(appIDLabel);
        GUI.changed = false;
        OvrAvatarSettings.AppID = EditorGUILayout.TextField(OvrAvatarSettings.AppID);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(OvrAvatarSettings.Instance);
            GUI.changed = false;
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
