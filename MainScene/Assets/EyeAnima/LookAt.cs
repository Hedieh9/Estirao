using System;
using UnityEngine;

[Serializable]
public class LookAt : MonoBehaviour
{
    // Variables are public for easier Custom Inspector.
    public Transform lookAtTarget = null;
    public Transform rotationTargetTransform;
    public float trackingSpeed = 10f;
    public bool returnToRestingIfOutOfRange = false;
    public float returnSpeed = 10f;
    public float maxRange = 100f;
    public bool lockAxis = false;
    public Axis axisToLock = Axis.Y;
    public bool useMaximumTurnAngle = false;
    [Range(0f, 180f)] public float turnAngleMax = 180;

    [Tooltip("Clamps an axis - should not be used together with maximum turn angle.")]
    public bool useAxisClamp = false;

    public Axis axisToClamp = Axis.X;
    [Range(-180f, 0)] public float clampMin = -180f;
    [Range(0f, 180f)] public float clampMax = 180;

    private Vector3 startDirectionForward;
    private Vector3 localStartEulers;

    private void Awake()
    {
        if (rotationTargetTransform == null) rotationTargetTransform = transform;
        startDirectionForward = rotationTargetTransform.forward;
    }

    private void LateUpdate()
    {
        Vector3 lookTarget;

        if (lookAtTarget != null)
        {
            lookTarget = lookAtTarget.position;
            if (Vector3.Distance(rotationTargetTransform.position, lookAtTarget.transform.position) > maxRange)
            {
                if (returnToRestingIfOutOfRange)
                {
                    rotationTargetTransform.localRotation = Quaternion.Slerp(rotationTargetTransform.localRotation,
                        Quaternion.identity, 10f * Time.deltaTime * returnSpeed / 10f);
                }

                return;
            }
        }
        else
        {
            rotationTargetTransform.localRotation = Quaternion.Slerp(rotationTargetTransform.localRotation,
                Quaternion.identity, 10f * Time.deltaTime);
            return;
        }

        // Updating rotations in late update so all movements of targets are done.

        // Store the current head rotation since we will be resetting it
        Quaternion currentLocalRotation = rotationTargetTransform.localRotation;
        // Reset the head rotation so our world to local space transformation will use the head's zero rotation. 
        // Note: Quaternion.Identity is the quaternion equivalent of "zero"
        rotationTargetTransform.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = lookTarget - rotationTargetTransform.position;
        Vector3 targetLocalLookDir = rotationTargetTransform.InverseTransformDirection(targetWorldLookDir);

        if (lockAxis)
        {
            if (axisToLock == Axis.X)
            {
                targetLocalLookDir.y = 0;
            }
            else
            {
                targetLocalLookDir.x = 0;
            }
        }

        // Apply angle limit
        if (useMaximumTurnAngle)
        {
            targetLocalLookDir = Vector3.RotateTowards(
                Vector3.forward,
                targetLocalLookDir,
                Mathf.Deg2Rad * turnAngleMax, // Note we multiply by Mathf.Deg2Rad here to convert degrees to radians
                0 // We don't care about the length here, so we leave it at zero
            );
        }

        // Get the local rotation by using LookRotation on a local directional vector
        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);

        // Apply smoothing
        rotationTargetTransform.localRotation = Quaternion.Slerp(
            currentLocalRotation,
            targetLocalRotation,
            1 - Mathf.Exp(-trackingSpeed * Time.deltaTime)
        );

        // Apply Clamping
        if (useAxisClamp)
        {
            if (axisToClamp == Axis.X)
            {
                float currentRotationX = rotationTargetTransform.localEulerAngles.x;

                if (currentRotationX > 180)
                {
                    // Move the rotation to a -180 ~ 180 range
                    currentRotationX -= 360;
                }

                // Clamp the rotation and apply it.
                rotationTargetTransform.localEulerAngles =
                    rotationTargetTransform.localEulerAngles.With(x: Mathf.Clamp(currentRotationX, clampMin, clampMax));
            }
            else
            {
                float currentRotationY = rotationTargetTransform.localEulerAngles.y;

                if (currentRotationY > 180)
                {
                    // Move the rotation to a -180 ~ 180 range
                    currentRotationY -= 360;
                }

                // Clamp the rotation and apply it.
                rotationTargetTransform.localEulerAngles =
                    rotationTargetTransform.localEulerAngles.With(y: Mathf.Clamp(currentRotationY, clampMin, clampMax));
            }
        }
    }

    public enum Axis : byte
    {
        X = 0,
        Y = 1,
    }
}