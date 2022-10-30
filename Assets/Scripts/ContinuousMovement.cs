using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;


public class ContinuousMovement : MonoBehaviour
{
    public XRNode inputSource;
    public float speed = 1;
    public float gravity = -9.81f;
    public LayerMask groundLayer;

    private float fallingSpeed;
    private XROrigin rig;
    private Vector2 inputAxis;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
        Vector3 direction = headYaw *  new Vector3(inputAxis.x, 0, inputAxis.y);

        character.Move(direction * Time.fixedDeltaTime * speed);

        // Gravity
        bool isGrounded = CheckIfGrounded();
        if(isGrounded)  
            fallingSpeed = 0;
        else 
            fallingSpeed += gravity * Time.fixedDeltaTime;

        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
        
    }

    bool CheckIfGrounded() {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}


