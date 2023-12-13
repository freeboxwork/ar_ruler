using UnityEngine;
using UnityEditor;

public class DirectionVectorTool : EditorWindow
{
    private GameObject targetObject;

    [MenuItem("Tools/Direction Vector Tool")]
    public static void ShowWindow()
    {
        GetWindow<DirectionVectorTool>("Direction Vector Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Target GameObject", EditorStyles.boldLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true);

        if (GUILayout.Button("Align to Selected GameObject's Direction"))
        {
            AlignToSelectedDirection();
        }
    }

    private void AlignToSelectedDirection()
    {
        if (targetObject != null && Selection.activeTransform != null)
        {
            Vector3 direction = Selection.activeTransform.forward;
            targetObject.transform.rotation = Quaternion.LookRotation(direction);
            Debug.Log(targetObject.name + " has been aligned to " + Selection.activeTransform.name + "'s direction.");
        }
        else
        {
            Debug.Log("Please select a target GameObject and another GameObject to align to.");
        }
    }
}
