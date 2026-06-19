using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Generator))]
public class GeneratorInspector : Editor
{
    static int seedInput = 404;
    
    public override void OnInspectorGUI()
    {
        Generator myScript = (Generator)target;
        GUILayout.BeginHorizontal("Box");
        seedInput = EditorGUILayout.IntSlider(seedInput, 0, 9999);
            
            if(GUILayout.Button("Generate"))
                myScript.Generate(seedInput);
        GUILayout.EndHorizontal();
        
        DrawDefaultInspector();
        
        GUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("Randomize"))
            {
                seedInput = Random.Range(0, 9999);
                myScript.Generate(seedInput);
            }
                
            if(GUILayout.Button("Clear"))
                myScript.ClearRooms();
        GUILayout.EndHorizontal();
    }

}
