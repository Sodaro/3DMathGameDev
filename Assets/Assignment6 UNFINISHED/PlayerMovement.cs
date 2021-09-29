using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Transform _camera;
    private Vector3 _velocity;

    public Vector3 Velocity => _velocity;
    void Start()
    {
        _camera = Camera.main.transform;
    }

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");

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
    }
}
