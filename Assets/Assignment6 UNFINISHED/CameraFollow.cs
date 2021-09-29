using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _player;
    Vector3 offset;
    void Start()
    {
        offset = transform.position - _player.transform.position;
    }
    void Update()
    {
        transform.position = _player.transform.TransformPoint(offset);
        transform.up = _player.transform.up;
    }
}
