using UnityEngine;

public class WorldToLocalVisualizer : MonoBehaviour
{
    //we assign this
    [SerializeField] Vector3 _sphereWorldPosition;

    //this is based on the assigned world position
    [SerializeField] private Vector3 _spherePosRelative;
    private void OnDrawGizmos()
    {

        Vector3 offset = _sphereWorldPosition - transform.position;


        //use other object transform.right,up,forward instead of Vector3 if you have other object
        _spherePosRelative.x = Vector3.Dot(offset, Vector3.right);
        _spherePosRelative.y = Vector3.Dot(offset, Vector3.up);
        _spherePosRelative.z = Vector3.Dot(offset, Vector3.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, _sphereWorldPosition);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(Vector3.zero, transform.position);
        Gizmos.DrawSphere(_sphereWorldPosition, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + _spherePosRelative);


    }
}
