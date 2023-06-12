using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(LookAt))]
    public class LookAtEditor : UnityEditor.Editor
    {
        private LookAt lookAt;
        private static bool showRotationLimitsIndicatorX = true;
        private static bool showRotationLimitsIndicatorY = true;
        private static float radius = 1f;

        private void OnEnable()
        {
            lookAt = target as LookAt;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);
            EditorExtensions.DrawUILine(Color.grey);
            GUILayout.Space(5);
            GUILayout.Label("Debug Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);
            showRotationLimitsIndicatorX = GUILayout.Toggle(showRotationLimitsIndicatorX, " Show rotation Limits X");
            showRotationLimitsIndicatorY = GUILayout.Toggle(showRotationLimitsIndicatorY, " Show rotation Limits Y");
            GUILayout.BeginHorizontal();
            radius = EditorGUILayout.FloatField(radius, GUILayout.Width(30));
            GUILayout.Label("DisplayRadius");
            GUILayout.EndHorizontal();
        }

        public void OnSceneGUI()
        {
            if (lookAt == null) return;
            // Debug the lookAt target
            if (lookAt.lookAtTarget != null)
            { 
                Handles.color = Color.blue;
                if (lookAt.rotationTargetTransform != null)
                {
                    Handles.DrawAAPolyLine(lookAt.lookAtTarget.position,  lookAt.rotationTargetTransform.position);
                }
                else
                {
                    Handles.DrawAAPolyLine(lookAt.lookAtTarget.position,  lookAt.transform.position);
                }
            }
            
            // Debug the max range
            Handles.color = Color.blue.With(a:0.2f);
            Handles.SphereHandleCap(0, lookAt.transform.position, Quaternion.identity, lookAt.maxRange * 2f, EventType.Repaint);

            // Debug the allowed angles
            if (lookAt.useMaximumTurnAngle)
            {
                float maxRotation = lookAt.turnAngleMax;

                float maxRotationXMin = -maxRotation;
                float maxRotationXMax = maxRotation;
                
                float maxRotationYMin = -maxRotation;
                float maxRotationYMax = maxRotation;


                if (lookAt.useAxisClamp)
                {
                    if (lookAt.axisToClamp == LookAt.Axis.X)
                    {
                        maxRotationXMin = Mathf.Clamp(maxRotationXMin, lookAt.clampMin, 0f);
                        maxRotationXMax = Mathf.Clamp(maxRotationXMax, 0f, lookAt.clampMax);
                    }
                    else
                    {
                        maxRotationYMin = Mathf.Clamp(maxRotationYMin, lookAt.clampMin, 360f);
                        maxRotationYMax = Mathf.Clamp(maxRotationYMax, 0f, lookAt.clampMax);
                    }
                }

                if (lookAt.lockAxis)
                {
                    if (lookAt.axisToLock == LookAt.Axis.X)
                    {
                        maxRotationXMax = 1f;
                        maxRotationXMin = -1f;
                        
                    }
                    else
                    {
                        maxRotationYMin = -1f;
                        maxRotationYMax = 1f;
                    }
                }
                
                Vector3 startDirection = Quaternion.AngleAxis(maxRotationYMin, lookAt.transform.up) * lookAt.transform.forward;

                Handles.color = Color.green.With(a:0.1f);
                if(showRotationLimitsIndicatorY) 
                    Handles.DrawSolidArc(lookAt.transform.position, lookAt.transform.up, startDirection.normalized,
                        Mathf.Abs(maxRotationYMin) + maxRotationYMax, radius);

                startDirection = Quaternion.AngleAxis(maxRotationXMin, lookAt.transform.right) * lookAt.transform.forward;
                
                if(showRotationLimitsIndicatorX) 
                Handles.color = Color.red.With(a:0.1f);
                Handles.DrawSolidArc(lookAt.transform.position, lookAt.transform.right, startDirection.normalized,
                    Mathf.Abs(maxRotationXMin) + maxRotationXMax, radius);
            }
        }
    }
}
