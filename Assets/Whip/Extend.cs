using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extend : MonoBehaviour
{
    public Transform GameObject;
    // I have given up on coding this garbage ima use the visual scripting instead
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vector = getRotation();
        print(vector);
    }

    public Vector3 getRotation(){
        float x_rotation = 0;
        float y_rotation = 0;
        float z_rotation = 0;

        if(this.eulerAngles.x <= 180f)
        {
            x_rotation = this.eulerAngles.x;
        }
        else
        {
            x_rotation = this.eulerAngles.x - 360f;
        }

        if(this.eulerAngles.y <= 180f)
        {
            y_rotation = this.eulerAngles.y;
        }
        else
        {
            y_rotation = this.eulerAngles.y - 360f;
        }

        if(this.eulerAngles.z <= 180f)
        {
            y_rotation = this.eulerAngles.z;
        }
        else
        {
            y_rotation = this.eulerAngles.z - 360f;
        }

        Vector3 output = transform.rotation;
        output.x = x_rotation;
        output.y = y_rotation;
        output.z = z_rotation;
        return output;
    }

}
