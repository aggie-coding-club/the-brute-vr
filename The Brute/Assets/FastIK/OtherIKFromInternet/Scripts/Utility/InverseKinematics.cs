using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour
{
    [SerializeField] bool isLeg;

    [SerializeField] int chainLength = 2;

    [SerializeField] Transform hips;
    [SerializeField] Transform target;
    [SerializeField] Transform pole;

    int iterations = 10;
    float delta = 0.001f;

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirectionSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirectionSucc = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        startRotationTarget = target.rotation;
       

        completeLength = 0;

        Transform current = this.transform;
        for(int i = bones.Length - 1; i >= 0; i--)
        { 
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if(i == bones.Length - 1)
            {
                startDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }

    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
            return;

        if (bonesLength.Length != chainLength)
            Init();

        for (int i = 0; i < bones.Length; i++)
            positions[i] = bones[i].position;

        

        if ((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            var direction = (target.position - positions[0]).normalized;

            for (int i = 1; i < positions.Length; i++)
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
        }
        else
        {
            for(int i = 0; i < iterations; i++)
            {
                for(int x = positions.Length - 1; x > 0; x--)
                {
                    if (x == positions.Length - 1)
                        positions[x] = target.position;
                    else
                        positions[x] = positions[x + 1] + (positions[x] - positions[x + 1]).normalized * bonesLength[x];
                }

                for (int y = 1; y < positions.Length; y++)
                    positions[y] = positions[y - 1] + (positions[y] - positions[y - 1]).normalized * bonesLength[y - 1];

                if ((positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                    break;
            }
        }

        if(pole != null)
        {
            for (int i = 1; i < positions.Length - 1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
            {
                //uncomment below to make the feet parallel to ground, even when its lifting 
                //bones[i].rotation = target.rotation * startRotationTarget * startRotationBone[i];
            }
            else
            {
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
                if (isLeg)
                {
                    bones[i].RotateAround(bones[i].position, bones[i].up, -hips.localEulerAngles.y);
                }
            } 
            bones[i].position = positions[i];
        }
    }
}
