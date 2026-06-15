using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Generator))]
public class GeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Generator myScript = (Generator)target;
        
        GUILayout.BeginHorizontal("Box");
            if(GUILayout.Button("Generate"))
                myScript.Generate();
            
            if(GUILayout.Button("Clear"))
                myScript.ClearRooms();
        GUILayout.EndHorizontal();
    }

}
