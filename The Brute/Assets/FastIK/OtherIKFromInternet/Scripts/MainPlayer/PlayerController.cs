using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform cam;

    [SerializeField] Transform leftFoot;
    [SerializeField] Transform rightFoot;

    [SerializeField] ProceduralLegsController proceduralLegs;

    [SerializeField] Rigidbody headRb;

    [SerializeField] float feetGroundCheckDist;

    ConfigurableJoint hipsCj;
    Rigidbody hipsRb;

    LayerMask groundMask;

    [SerializeField] float moveSpeed;
    [SerializeField] float rotationForce;
    [SerializeField] float balanceForce;
    [SerializeField] float jumpForce;
    
    [SerializeField] float maxVelocityChange;

    bool isGrounded;
    bool isDead = false;

    float horizontal, vertical;

    [SerializeField] ConfigurableJoint[] cjs;
    JointDrive[] jds;
    JointDrive inAirDrive;
    JointDrive hipsInAirDrive;

    [SerializeField] float airSpring;
    private void Start()
    {
        jds = new JointDrive[cjs.Length];

        inAirDrive.maximumForce = Mathf.Infinity;
        inAirDrive.positionSpring = airSpring;

        hipsInAirDrive.maximumForce = Mathf.Infinity;
        hipsInAirDrive.positionSpring = 0;

        hipsRb = GetComponent<Rigidbody>();
        hipsCj = GetComponent<ConfigurableJoint>();

        //Saves the initial drives of each configurable joint
        for(int i = 0; i < cjs.Length; i++)
        {
            jds[i] = cjs[i].angularXDrive;
        }

        groundMask = LayerMask.GetMask("Ground");
        
    }
    void Update()
    {
        if (!isDead)
        {
            proceduralLegs.GroundHomeParent();
            CheckGrounded();
            SetPlayerInputs();

            if (isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    Jump();
            }
        }
    }
    void FixedUpdate()
    {
        if (isGrounded && !isDead)
        {
            StabilizeBody();
            Move();
        }    
    }

    void SetPlayerInputs()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    void StabilizeBody()
    {
        headRb.AddForce(Vector3.up * balanceForce);
        hipsRb.AddForce(Vector3.down * balanceForce);
    }

    void Move()
    {
        Vector3 move = new Vector3(horizontal, 0f, vertical);
        move = cam.TransformDirection(move);

        Vector3 targetVelocity = new Vector3(move.x, 0, move.z);
        targetVelocity *= moveSpeed;

        Vector3 velocity = hipsRb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        hipsRb.AddForce(velocityChange, ForceMode.VelocityChange);

        float desiredAngle = 0;
        float rootAngle = transform.eulerAngles.y;

        if(targetVelocity.normalized != Vector3.zero)
        {
             desiredAngle = Quaternion.LookRotation(targetVelocity.normalized).eulerAngles.y;
        }

        float deltaAngle = Mathf.DeltaAngle(rootAngle, desiredAngle);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            hipsRb.AddTorque(Vector3.up * deltaAngle * rotationForce, ForceMode.Acceleration);
        }
    }

    void CheckGrounded()
    {
        bool leftCheck = false;
        bool rightCheck = false;
        RaycastHit hit;

        if (Physics.Raycast(leftFoot.position, Vector3.down, out hit, feetGroundCheckDist, groundMask))
            leftCheck = true;

        if (Physics.Raycast(rightFoot.position, Vector3.down, out hit, feetGroundCheckDist, groundMask))
            rightCheck = true;

        if ((rightCheck || leftCheck) && !isGrounded)
        {
            SetDrives();
        }
        else if((!rightCheck && !leftCheck) && isGrounded)
        {
            Die(true);
        }
    }

    public void Die(bool respawn)
    {
        foreach (ConfigurableJoint cj in cjs)
        {
            cj.angularXDrive = inAirDrive;
            cj.angularYZDrive = inAirDrive;
        }

        hipsCj.angularYZDrive = hipsInAirDrive;
        hipsCj.angularXDrive = hipsInAirDrive;

        proceduralLegs.DisableIk();
        isGrounded = false;

        if (!respawn)
            isDead = true;

    }
    void SetDrives()
    {
        for(int i = 0; i < cjs.Length; i++)
        {
            cjs[i].angularXDrive = jds[i];
            cjs[i].angularYZDrive = jds[i];

        }

        proceduralLegs.EnableIk();
        isGrounded = true;
    }
    void Jump()
    {
        hipsRb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        hipsRb.AddTorque(new Vector3(750, 0));
    }
}



