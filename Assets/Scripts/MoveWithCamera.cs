using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithCamera : MonoBehaviour
{
    public Transform camera;

    // Update is called once per frame
    void Update()
    {
        transform.rotation =  Quaternion.AngleAxis(camera.transform.eulerAngles.y + 90, Vector3.up);
    }
}
