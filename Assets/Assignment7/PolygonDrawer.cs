using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolygonDrawer : MonoBehaviour
{
    [SerializeField, Tooltip("The thickness of the debug line")] private float lineThickness = 5f;
    [SerializeField, Tooltip("The amount of sides in the polygon")] private int numberOfSides = 3;
    [SerializeField, Tooltip("The length of each side and radius of circle")] private int sideLength = 1;
    [SerializeField, Tooltip("Delay between each rotation")] private float rotationDelay = 0.25f;
    [SerializeField, Tooltip("Density of line connections")] private int density = 1;
    [SerializeField] private List<Vector3> points;

    [SerializeField, Tooltip("The starting color")] private Color startColor = Color.blue;
    [SerializeField, Tooltip("The ending color")] private Color endColor = Color.red;

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

	private IEnumerator PlacePoints()
	{
        while (true)
		{
            points = new List<Vector3>(numberOfSides);
            float rotationIncrement = 360 / numberOfSides; //the rotation amount in degrees (which is used by Quaternion.Euler)

            for (int i = 0; i < numberOfSides; i++)
			{
                //if the rotation would exceed 180 it would become -180, which resulted in weird placements
                if (rotationIncrement * i > 180) 
                    transform.rotation = Quaternion.Euler(0, -(360 - rotationIncrement * i), 0);
                else
                    transform.rotation = Quaternion.Euler(0, rotationIncrement * i, 0);

                Vector3 point = transform.right * sideLength;
                points.Add(point);
                yield return new WaitForSeconds(rotationDelay);
            }
        }
	}

    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, sideLength, lineThickness);

        if (points.Count == 0)
            return;
        
        if (density >= points.Count)
            return;

        for (int i = 0; i < points.Count + density; i++)
        {
            Vector3 pos1;
            Vector3 pos2;
            //the final points should connect to the first ones based on density etc, use modulo connect
            int pointToConnectTo = (i + density) % points.Count;
            pos1 = points[i % points.Count];
            pos2 = points[pointToConnectTo];

            Handles.color = Color.Lerp(startColor, endColor, (float)i / (points.Count + density));
            Handles.DrawLine(pos1, pos2, lineThickness);
        }

    }
}