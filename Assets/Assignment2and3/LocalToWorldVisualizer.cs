using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalToWorldVisualizer : MonoBehaviour
{
    [SerializeField] private Vector3 _localOffset;

    private Vector3 _targetWorldPos;
	private void OnDrawGizmos()
	{
        Transform tf = transform;
        //Vector3 pos = transform.position;
        //Vector3 rotatedX = _localOffset.x * ;
        //Vector3 rotatedY = _localOffset.y * ;
        //Vector3 rotatedZ = _localOffset.z * transform.forward;

        Vector3 transformedOffset = tf.position + (_localOffset.x * tf.right) + (_localOffset.y * tf.up) + (_localOffset.z * tf.forward);

        _targetWorldPos = tf.position + transformedOffset;


        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, _targetWorldPos);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(Vector3.zero, transform.position);
        Gizmos.DrawSphere(_targetWorldPos, 0.5f);

	}
}
