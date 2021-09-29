using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SurfaceAreaCalculator : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private TextMesh _text;

    private Vector3[] vertices;
    private int[] triangles;

    private void OnValidate()
    {
        EditorApplication.delayCall += OnValidationDone;
    }

    private void OnValidationDone()
    {
        GetComponent<MeshFilter>().mesh = _mesh;
        vertices = _mesh.vertices;
        triangles = _mesh.triangles;

        float surfaceAreaOfMesh = 0;
        for (int i = 2; i < triangles.Length; i += 3)
        {
            //every 3 indices in triangle array are connected vertex indices
            Vector3 vertex = vertices[triangles[i]];
            Vector3 vertex1 = vertices[triangles[i - 1]];
            Vector3 vertex2 = vertices[triangles[i - 2]];

            //create edges from vertices
            Vector3 edge = vertex1 - vertex;
            Vector3 edge1 = vertex2 - vertex;

            Vector3 cross = Vector3.Cross(edge, edge1);

            //triangle area is half of rectangle area
            surfaceAreaOfMesh += (cross.magnitude / 2);
        }
        _text.text = $"{surfaceAreaOfMesh}m²";
    }
}
