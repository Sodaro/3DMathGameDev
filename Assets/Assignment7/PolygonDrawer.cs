using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolygonDrawer : MonoBehaviour
{
    [SerializeField, Tooltip("The thickness of the debug line")] private float lineThickness = 5f;
    [SerializeField, Tooltip("The amount of sides in the polygon")] private int numberOfSides = 3;
    [SerializeField, Tooltip("The amount of 'lines' per line")] private int vertsPerLine = 4;
    [SerializeField, Tooltip("The length of each side and radius of circle")] private int sideLength = 1;
    [SerializeField, Tooltip("Delay between each rotation")] private float rotationDelay = 0.25f;
    [SerializeField, Tooltip("Density of line connections")] private int density = 1;
    [SerializeField, Tooltip("Scales the lines by distance from center")] private bool scaleByDistance = false;

    [SerializeField, Tooltip("The starting color")] private Color startColor = Color.blue;
    [SerializeField, Tooltip("The ending color")] private Color endColor = Color.red;


    private List<List<Vector3>> points;
    private Coroutine coroutine;

    private void Start()
    {
        coroutine = StartCoroutine(PlacePoints());
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
    }

    private void OnValidate()
    {
        if (numberOfSides < 3)
        {
            numberOfSides = 3;
        }

        //if the coroutine is active, reset and restart it
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            transform.rotation = Quaternion.identity;
            coroutine = StartCoroutine(PlacePoints());
        }
    }

    //place points by rotating
    private IEnumerator PlacePoints()
    {
        while (true)
        {
            points = new List<List<Vector3>>();
            points.Add(new List<Vector3>());

            float rotationIncrement = 360 / numberOfSides; //the rotation amount in degrees (which is used by Quaternion.Euler)

            for (int i = 0; i < numberOfSides; i++)
            {
                //if the rotation would exceed 180 it would become -180, which resulted in weird placements
                if (rotationIncrement * i > 180)
                    transform.rotation = Quaternion.Euler(0, -(360 - rotationIncrement * i), 0);
                else
                    transform.rotation = Quaternion.Euler(0, rotationIncrement * i, 0);

                Vector3 point = transform.right * sideLength;
                points[0].Add(point);
                yield return new WaitForSeconds(rotationDelay);
            }
        }
    }

    //place points using maf
    private void PlacePointsV2()
    {
        points = new List<List<Vector3>>();

        for (int j = 0; j < vertsPerLine; j++)
        {
            List<Vector3> ring = new List<Vector3>();
            float vertAngleRadians = Mathf.PI * 2 / vertsPerLine; //the rotation amount in degrees (which is used by Quaternion.Euler)
            float yOffset = Mathf.Sin(vertAngleRadians * j) * sideLength;

            for (int i = 0; i < numberOfSides; i++)
            {
                float angleRadians = Mathf.PI * 2 / numberOfSides; //the rotation amount in degrees (which is used by Quaternion.Euler)
                Vector3 point = transform.TransformPoint(new Vector3(Mathf.Cos(angleRadians * i) * sideLength, yOffset, Mathf.Sin(angleRadians * i) * sideLength));
                ring.Add(point);
            }
            points.Add(ring);
        }
    }


    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, sideLength, lineThickness);

        //draw the shape without coroutine in scene view
        #if UNITY_EDITOR
        PlacePointsV2();
        #endif

        if (points.Count == 0)
            return;

        if (density >= points[0].Count)
            return;

        for (int j = 0; j < points.Count; j++)
        {
            for (int i = 0; i < points[j].Count + density; i++)
            {
                //the final points should connect to the first ones based on density etc, use modulo connect
                int ringToConnectTo = (j + 1) % points.Count;
                int pointToConnectTo = (i + density) % points[j].Count;
                Vector3 pos1 = points[j][i % points[j].Count];
                Vector3 pos2 = points[j][pointToConnectTo];

                Handles.color = Color.Lerp(startColor, endColor, (float)i / (points[j].Count + density));
                Handles.DrawLine(pos1, pos2, lineThickness);

                Handles.color = Color.Lerp(startColor, endColor, (float)i / points.Count);
                Handles.DrawLine(pos1, points[ringToConnectTo][i % points[j].Count], lineThickness);
            }
        }
    }
}