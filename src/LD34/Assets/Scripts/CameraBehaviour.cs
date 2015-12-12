using System;
using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{

    public GameObject CameraTarget;
    public Vector3 CameraOffset;

    // Use this for initialization
    void Start()
    {
    }

    void FixedUpdate()
    {
        var playerSize = CameraTarget.GetComponent<PlayerBehaviour>().Size;
        CameraOffset = new Vector3(0.0f, 0.0f, -playerSize / 2);
        transform.position = CameraTarget.transform.position + CameraOffset;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
