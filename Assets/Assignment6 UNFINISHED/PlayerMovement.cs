using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 1f;
    private Transform _meshTransform;

    [SerializeField] private Transform _planet;
    private Transform _camera;
    Matrix4x4 matrix;
    private Vector3 _velocity;

    private Vector3 gravityDirection;
    private float gravity = 5f;
    public Vector3 Velocity => _velocity;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.transform;
        _meshTransform = GetComponentInChildren<MeshRenderer>().transform;
        //_offset = _planet.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 projected = Vector3.ProjectOnPlane(_camera.transform.forward, transform.up);

        _velocity = projected * _moveSpeed * vertical;


        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 10f))
		{
			float dot = Vector3.Dot(_velocity, hit.normal);
			if (dot > 0f)
			{
                _velocity = (_velocity - hit.normal * dot).normalized * _moveSpeed;
                transform.rotation = Quaternion.LookRotation(_velocity.normalized, hit.normal);
            }
            
			
		}
        transform.position += _velocity * Time.deltaTime;
        //transform.rotation = Quaternion.LookRotation(velocity.normalized, transform.up);
		
    }

	//private void OnDrawGizmos()
	//{
 //       Handles.matrix = matrix;
 //       Vector4 v1 = matrix.GetColumn(2);
 //       Vector4 v2 = matrix.GetColumn(1);
 //       if (v1 == Vector4.zero || v2 == Vector4.zero)
 //           return;
 //       Handles.PositionHandle(matrix.GetColumn(3), Quaternion.LookRotation(v1, v2));
 //   }
}
