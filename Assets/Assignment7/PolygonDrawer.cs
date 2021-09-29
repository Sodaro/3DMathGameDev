using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolygonDrawer : MonoBehaviour
{
    [SerializeField, Tooltip("The thickness of the debug line")] private float lineThickness = 5f;
    [SerializeField, Tooltip("The amount of sides in the polygon")] private int numberOfSides = 3;
    [SerializeField, Tooltip("The amount of 'lines' per line")] private int vertsPerLine = 4;
    [SerializeField, Tooltip("The length of each side and radius of circle")] private float sideLength = 1;
    [SerializeField, Tooltip("Density of line connections")] private int density = 1;
    [SerializeField, Tooltip("Scales the lines by distance from center")] private bool scaleByDistance = false;
    [SerializeField, Tooltip("Height of the shape")] private float height = 1;

    [SerializeField, Tooltip("The bottom color")] private Color startColor = Color.red;
    [SerializeField, Tooltip("The middle color")] private Color midColor = Color.blue;
    [SerializeField, Tooltip("The top color")] private Color endColor = Color.blue;

    [SerializeField] private bool drawUnitCircle = false;

    [SerializeField, Tooltip("Doubles the edge loops, making it rounded")] private bool makeRounded = false;


    private List<List<Vector3>> outsideLoop;

    private void Start()
    {
        UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
    }

    private void OnValidate()
    {
        if (numberOfSides < 3)
        {
            numberOfSides = 3;
        }
        if (outsideLoop?.Count > 0)
            outsideLoop.Clear();

        if (makeRounded)
        {
            MakeTorus();
        }
        else
        {
            PlacePointsV2();
        }
    }

    //place points using maf
    private void PlacePointsV2()
    {
        outsideLoop = new List<List<Vector3>>();

        if (vertsPerLine == 0)
            vertsPerLine = 1;

        float offsetPerLine = height / vertsPerLine;
        float initialOffset = 0;

        if (vertsPerLine > 1)
            initialOffset = offsetPerLine * (vertsPerLine - 1) / 2;

        for (int j = 0; j < vertsPerLine; j++)
        {
            List<Vector3> ring = new List<Vector3>();
            float yOffset = offsetPerLine * j;

            float dist = Mathf.Abs(-initialOffset + yOffset);

            for (int i = 0; i < numberOfSides; i++)
            {
                float angleRadians = Mathf.PI * 2 / numberOfSides; //the rotation amount in degrees (which is used by Quaternion.Euler)
                Vector3 point = transform.TransformPoint(GetPointOnLoop(angleRadians * i, dist, sideLength, -initialOffset + yOffset));
                ring.Add(point);
            }
            outsideLoop.Add(ring);
        }
    }

    private void MakeTorus()
    {
        outsideLoop = new List<List<Vector3>>();

        if (vertsPerLine == 0)
            vertsPerLine = 2;

        float ringRadius = 1f;

        for (int j = 0; j < numberOfSides; j++)
        {
            List<Vector3> ring = new List<Vector3>();

            for (int i = 0; i < vertsPerLine; i++)
            {
                float centerRadians = Mathf.PI * 2 / numberOfSides; //the rotation amount in degrees (which is used by Quaternion.Euler)
                float ringRadians = Mathf.PI * 2 / vertsPerLine;

                float x = Mathf.Cos(centerRadians*j) * (sideLength + ringRadius * Mathf.Cos(ringRadians*i));
                float y = ringRadius * Mathf.Sin(ringRadians * i);
                float z = Mathf.Sin(centerRadians * j) * (sideLength + ringRadius * Mathf.Cos(ringRadians * i));

                Vector3 point = new Vector3(x, y, z);
                Vector3 transformedPoint = transform.TransformPoint(point);
                
                ring.Add(transformedPoint);
            }
            outsideLoop.Add(ring);
        }
    }


    Vector3 GetPointOnLoop(float angleValue, float distanceFromCenter, float radius, float yOffset)
    {
        Vector3 point;
        if (scaleByDistance)
        {
            //cos and sin of the radians*i would place it along points on a 1-radius unit circle
            //multiplying by the length will make the circle larger, dividing the length by distance will make that circle smaller
            if (distanceFromCenter > 0)
            {
                point = new Vector3(Mathf.Cos(angleValue) * (radius / (1 + distanceFromCenter)), yOffset, Mathf.Sin(angleValue) * (radius / (1 + distanceFromCenter)));
            }
            else
                point = new Vector3(Mathf.Cos(angleValue) * radius, yOffset, Mathf.Sin(angleValue) * (radius));
        }
        else
            point = new Vector3(Mathf.Cos(angleValue) * radius, yOffset, Mathf.Sin(angleValue) * radius);

        return point;
    }


    private void OnDrawGizmos()
    {
        if (drawUnitCircle)
            Handles.DrawWireDisc(Vector3.zero, Vector3.up, sideLength, lineThickness);

        if (outsideLoop == null)
            return;

        if (outsideLoop.Count == 0)
            return;

        if (density >= outsideLoop[0].Count)
            return;

        int midPoint = outsideLoop.Count / 2;

        for (int i = 0; i < outsideLoop.Count; i++)
        {
            List<Vector3> ring = outsideLoop[i];
            List<Vector3> nextRing = outsideLoop[(i + 1) % outsideLoop.Count];

            Color lineColor;
            if (midPoint > 0)
            {
                if (i <= midPoint)
                    lineColor = Color.Lerp(startColor, midColor, (float)i / midPoint);
                else
                    lineColor = Color.Lerp(midColor, endColor, (float)(i - midPoint) / midPoint);
            }
            else
                lineColor = startColor;

            Handles.color = lineColor;

            for (int j = 0; j < ring.Count + density; j++)
            {
                //the final points should connect to the first ones based on density etc, use modulo connect
                int point = j % ring.Count;
                int pointToConnectTo = (j + density) % outsideLoop[i].Count;

                Vector3 r1P1 = ring[point];
                Vector3 r1P2 = ring[pointToConnectTo];

                Handles.DrawLine(r1P1, r1P2, lineThickness);

                if (makeRounded)
                {
                    Vector3 r2P1 = nextRing[point];
                    Handles.DrawLine(r1P1, r2P1, lineThickness);
                }
            }
        }
    }
}