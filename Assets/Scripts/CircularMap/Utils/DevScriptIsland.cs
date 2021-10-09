using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IslandGenerator))]
public class DevScriptIsland : Editor
{
    public override void OnInspectorGUI()
    {
        IslandGenerator islandGen = (IslandGenerator) target;
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Island"))
        {
            islandGen.DrawMesh();
        }
    }
}
