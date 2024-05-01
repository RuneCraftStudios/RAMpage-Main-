using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectDistributor))]
public class ObjectDistributorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectDistributor distributor = (ObjectDistributor)target;

        if (GUILayout.Button("Distribute Objects"))
        {
            distributor.DistributeObjects();
        }
    }

    // Custom scene GUI to handle object distribution
    void OnSceneGUI()
    {
        ObjectDistributor distributor = (ObjectDistributor)target;

        // Display a button in scene view for manual object distribution
        Handles.BeginGUI();
        if (GUILayout.Button("Distribute Objects in Scene"))
        {
            distributor.DistributeObjects();
        }
        Handles.EndGUI();
    }
}
