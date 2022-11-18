using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillyController : MonoBehaviour
{
    [SerializeField] Transform followObj;

    [SerializeField] ProceduralLegsController proceduralLegs;

    [SerializeField] float isAliveGroundCheckDist;
    [SerializeField] float upperGroundCheckDist;
    [SerializeField] float lowerGroundCheckDist;

    Rigidbody hipsRb;

    LayerMask groundMask;

    [SerializeField] float speed;
    [SerializeField] float impactForceThreshold;

    [HideInInspector] public bool isGrounded = false;
    bool isStandingUp = false;
    bool doPushUp = false;

    [SerializeField] ConfigurableJoint[] cjs;
    JointDrive[] jds;
    JointDrive inAirDrive;

    [SerializeField] float cfForce;
    [SerializeField] float rotationForce;
    [SerializeField] float rotationBalanceForce;
    [SerializeField] float inAirForce;

    private void Start()
    {
        jds = new JointDrive[cjs.Length];

        inAirDrive.maximumForce = Mathf.Infinity;
        inAirDrive.positionSpring = 0;

        hipsRb = GetComponent<Rigidbody>();

        //Saves the initial drives of each configurable joint
        for(int i = 0; i < cjs.Length; i++)
        {
            jds[i] = cjs[i].angularXDrive;
        }

        groundMask = LayerMask.GetMask("Ground");
    }
    void Update()
    {
        proceduralLegs.GroundHomeParent();
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (isGrounded)
        {
            StabilizeBody();
            Move();
        }
    }
    void StabilizeBody()
    {
        hipsRb.AddTorque(-hipsRb.angularVelocity * rotationBalanceForce, ForceMode.Acceleration);
        var rot = Quaternion.FromToRotation(-transform.right, Vector3.right);
        hipsRb.AddTorque(new Vector3(rot.x, rot.y, rot.z) * rotationBalanceForce, ForceMode.Acceleration);
    }
    void Move ()
    { 
        if(Vector3.Distance(transform.position, followObj.position) > 1.5)
        {
            Vector3 move = (followObj.position - transform.position).normalized;
            hipsRb.velocity = new Vector3(move.x * speed, hipsRb.velocity.y, move.z * speed);

            float rootAngle = transform.eulerAngles.y;
            float desiredAngle = Quaternion.LookRotation(followObj.position - transform.position).eulerAngles.y;
            float deltaAngle = Mathf.DeltaAngle(rootAngle, desiredAngle);
            hipsRb.AddTorque(Vector3.up * deltaAngle * rotationForce, ForceMode.Acceleration);
        }
        else
        {
            hipsRb.velocity = new Vector3(0, hipsRb.velocity.y, 0);

            float rootAngle = transform.eulerAngles.y;
            float desiredAngle = Quaternion.LookRotation(followObj.position - transform.position).eulerAngles.y;
            float deltaAngle = Mathf.DeltaAngle(rootAngle, desiredAngle);
            hipsRb.AddTorque(Vector3.up * deltaAngle * rotationForce, ForceMode.Acceleration);
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, isAliveGroundCheckDist, groundMask))
        {
            if (!isGrounded)
            {
                StartCoroutine(DelayBeforeStand(3));
            }
            
            if (!isStandingUp)
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, lowerGroundCheckDist, groundMask) && !doPushUp)
                {
                    hipsRb.AddForce(new Vector3(0, cfForce, 0), ForceMode.Acceleration);
                    doPushUp = true;
                }else if(Physics.Raycast(transform.position, Vector3.down, out hit, upperGroundCheckDist, groundMask) && doPushUp)
                {
                    hipsRb.AddForce(new Vector3(0, cfForce, 0), ForceMode.Acceleration);
                }
                else
                {
                    doPushUp = false;
                    hipsRb.AddForce(new Vector3(0, inAirForce, 0));
                }
            }
        }
        else
        {
            if (isGrounded)
            {
                Die();
            }
        } 
    }

    public void Die()
    {
        proceduralLegs.DisableIk();
        isGrounded = false;

        foreach (ConfigurableJoint cj in cjs)
        {
            cj.angularXDrive = inAirDrive;
            cj.angularYZDrive = inAirDrive;
        }
        
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

    IEnumerator DelayBeforeStand(float delay)
    {
        isStandingUp = true;
        yield return new WaitForSeconds(delay);

        SetDrives();
        isStandingUp = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Projectile")
        {
            if(collision.relativeVelocity.magnitude > impactForceThreshold)
            {
                Die();
            }
        }
    }
}



