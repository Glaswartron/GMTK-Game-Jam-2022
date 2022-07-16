using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public int ZoomOutFactor = 2;
    public static CameraFollow instance;

    private void Start()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }

    public void ZoomOut()
    {
        GetComponent<Camera>().orthographicSize += ZoomOutFactor;
    }
}
