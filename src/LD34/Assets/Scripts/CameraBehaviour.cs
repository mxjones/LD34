using System;
using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour
{

    public GameObject CameraTarget;
    public Vector3 CameraOffset;

    private AudioSource _audioSource;
    public bool SoundOn = true;

    // Use this for initialization
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
        // Eww Eww Eww Eww Eww
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (SoundOn)
            {
                _audioSource.Stop();
                SoundOn = false;
            }
            else
            {
                _audioSource.Play();
                SoundOn = true;
            }
            
        }
    }
}
