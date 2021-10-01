using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiMapGenerator))]
public class DevScriptMulti : Editor 
{
    public override void OnInspectorGUI()
    {
        MultiMapGenerator mapGen = (MultiMapGenerator) target;
        DrawDefaultInspector();
        if (GUILayout.Button("GenerateMultiMap"))
        {
            mapGen.DrawMesh();
        }
    }
}
