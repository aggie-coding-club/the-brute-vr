using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLegsController : MonoBehaviour
{
    [SerializeField] Transform homesParent;
    [SerializeField] Transform polesParent;

    [SerializeField] bool alternateLegs;

    [SerializeField] InverseKinematics leftIk;
    [SerializeField] InverseKinematics rightIk;
    [SerializeField] ProceduralAnimation leftAnim;
    [SerializeField] ProceduralAnimation rightAnim;

    void Start()
    {  
        if (!alternateLegs)
        {
            StartCoroutine(LegUpdate());
        }
        else
        {
            StartCoroutine(AlternatingLegUpdate());
        }
    }
    public void GroundHomeParent()
    {
        homesParent.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        homesParent.eulerAngles = new Vector3(homesParent.eulerAngles.x, transform.eulerAngles.y, homesParent.eulerAngles.z);

        polesParent.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        polesParent.eulerAngles = new Vector3(polesParent.eulerAngles.x, transform.eulerAngles.y, polesParent.eulerAngles.z);
    }

    public void EnableIk()
    {
        rightIk.enabled = true;
        leftIk.enabled = true;
    }
    public void DisableIk()
    {
        rightIk.enabled = false;
        leftIk.enabled = false;
    }

    IEnumerator AlternatingLegUpdate()
    {
        while (true)
        {
            do
            {
                leftAnim.TryMove();
                yield return null;
            } while (leftAnim.moving);

            do
            {
                rightAnim.TryMove();
                yield return null;

            } while (rightAnim.moving);
        }
    }
    IEnumerator LegUpdate()
    {
        while (true)
        {
            do
            {
                leftAnim.TryMove();
                rightAnim.TryMove();
                yield return null;
            } while (leftAnim.moving && rightAnim.moving);
        }
    }
}
