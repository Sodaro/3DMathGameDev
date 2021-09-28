using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    Vector3 velocity;

    Vector3 smoothVelocity;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - _player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //velocity = _player.Velocity;
        //transform.position += velocity * Time.deltaTime;
        //Vector3 targetPosition = _player.transform.TransformPoint(new Vector3(0, 14, 5));
        float horizontal = Input.GetAxisRaw("Horizontal");
        transform.position = _player.transform.TransformPoint(offset);
        transform.up = _player.transform.up;
        //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, 0.3f);
        //transform.LookAt(_player.transform);
    }
}
