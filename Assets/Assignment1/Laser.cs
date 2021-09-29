using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Laser : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    void OnDrawGizmos()
    {
        Vector3 rayStartPos = transform.position;
        Vector3 rayDir = -transform.up;
        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);

        float rayLength = 20f;

        for (int i = 1; i < 100; i++)
        {
            Ray ray = new Ray(rayStartPos, rayDir);
            _lineRenderer.positionCount = i + 1;

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, 1, QueryTriggerInteraction.Ignore))
            {
                //projected the ray onto the normal
                float dot = Vector3.Dot(rayDir * rayLength, hit.normal);

                Vector3 projected = dot * hit.normal;
                Vector3 reflected = (rayDir * rayLength) - 2 * projected;

                _lineRenderer.SetPosition(i, hit.point);

                Debug.DrawLine(hit.point - hit.normal / 2, hit.point + hit.normal, Color.red);

                rayDir = reflected.normalized;
                rayStartPos = hit.point;
            }
            else
            {
                _lineRenderer.SetPosition(i, rayStartPos + (rayLength * rayDir));
                break;
            }

        }

    }
}
