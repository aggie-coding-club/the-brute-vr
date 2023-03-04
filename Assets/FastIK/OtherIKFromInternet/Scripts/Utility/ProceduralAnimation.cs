using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProceduralAnimation : MonoBehaviour
{ 
    [SerializeField] Transform hips;
    [SerializeField] Transform home;
    [SerializeField] Transform footTarget;

    public bool moving;

    [SerializeField] float maxStepDistance;

    [SerializeField] float moveDuration;
    [SerializeField] float highness;

    [SerializeField] float stepOverhootFraction;

    LayerMask groundMask;

    [SerializeField] bool doGround;

    [SerializeField] float groundCheckDist;

    void Start()
    {
        groundMask = LayerMask.GetMask("Ground");

        StartCoroutine(MoveToHome(footTarget, home));
    }

    void Update()
    {
        GroundHome();
    }

    void GroundHome()
    {
        RaycastHit hit;

        if (doGround) 
        { 
            if (Physics.Raycast(new Vector3(home.position.x, hips.position.y, home.position.z), Vector3.down, out hit, groundCheckDist, groundMask))
            {
                home.position = new Vector3(home.position.x, hit.point.y, home.position.z);
            }
        }
    }

    public void TryMove()
    {
        if (moving) return;

        float distFromHome = Vector3.Distance(footTarget.position, home.position);
        
        if(distFromHome > maxStepDistance)
        {
            StartCoroutine(MoveToHome(footTarget, home));
        }
    }

    IEnumerator MoveToHome(Transform footTarget, Transform footHome)
    {
        moving = true;

        Quaternion startRot = footTarget.rotation;
        Vector3 startPoint = footTarget.position;

        Quaternion endRot = footHome.rotation;

        Vector3 towardHome = (footHome.position - footTarget.position);

        float overshootDistance = maxStepDistance * stepOverhootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;

        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = footHome.position + overshootVector;

        Vector3 centerPoint = (startPoint + endPoint) / 2;

        centerPoint += (footHome.up * Vector3.Distance(startPoint, endPoint) / 2f) * highness;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;

            footTarget.position = Vector3.Lerp(Vector3.Lerp(startPoint, centerPoint, normalizedTime), Vector3.Lerp(centerPoint, endPoint, normalizedTime), normalizedTime);
            footTarget.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        moving = false;

    }
}
    


