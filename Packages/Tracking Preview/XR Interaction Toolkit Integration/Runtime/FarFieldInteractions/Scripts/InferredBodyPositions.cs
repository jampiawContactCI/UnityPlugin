/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2021.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Preview.FarFieldInteractions
{
#pragma warning disable 0618
    /// <summary>
    /// Infers neck and shoulder positions using real world head data.
    /// Used by the 'FarFieldDirection', and requires a 'RotationDeadzone' component
    /// </summary>
    [RequireComponent(typeof(RotationDeadzone))]
    public class InferredBodyPositions : MonoBehaviour
    {
        [Header("Inferred Shoulder Settings")]
        /// <summary>
        /// Should the Neck Position be used to predict shoulder positions?
        /// If true, the shoulders are less affected by head roll & pitch rotation (z & x rotation)
        /// </summary>
        [Tooltip("Should the Neck Position be used to predict shoulder positions?\n" +
            "If true, the shoulders are less affected by head roll & pitch rotation (z & x rotation)")]
        public bool useNeckPositionForShoulders = true;

        /// <summary>
        /// The shoulder's horizontal offset from the neck.
        /// </summary>
        [Tooltip("The shoulder's horizontal offset from the neck.")]
        public float shoulderOffset = 0.1f;

        [Header("Inferred Neck Settings")]
        /// <summary>
        /// The neck's vertical offset from the head
        /// </summary>
        [Tooltip("The neck's vertical offset from the head")]
        public float neckOffset = -0.1f;

        /// <summary>
        /// Use a deadzone for a neck's y-rotation.
        /// If true, the neck's Yaw is not affected by the head's Yaw 
        /// until the head's Yaw has moved over a certain threshold - this has the
        /// benefit of keeping the neck's rotation fairly stable.
        /// </summary>
        [Tooltip("Use a deadzone for a neck's y-rotation.\n" +
            "If true, the neck's Yaw is not affected by the head's Yaw until the head's" +
            " Yaw has moved over a certain threshold - this has the benefit of keeping the" +
            " neck's rotation fairly stable.")]
        public bool useNeckYawDeadzone = true;

        /// <summary>
        /// Blends between the NeckPositionLocalOffset and the NeckPositionWorldOffset.
        /// At 0, only the local position is into account.
        /// At 1, only the world position is into account.
        /// A blend between the two stops head roll & pitch rotation (z & x rotation) 
        /// having a large effect on the neck position
        /// </summary>
        [Tooltip("Blends between the NeckPositionLocalOffset and the NeckPositionWorldOffset.\n" +
            " - At 0, only the local position is into account.\n" +
            " - At 1, only the world position is into account.\n" +
            " - A blend between the two stops head roll & pitch rotation (z & x rotation) " +
            "having a large effect on the neck position")]
        [Range(0f, 1)] 
        public float WorldLocalNeckPositionBlend = 0.5f;

        /// <summary>
        /// How quickly the neck rotation updates
        /// Used to smooth out sudden large rotations
        /// </summary>
        [Tooltip("How quickly the neck rotation updates\n" +
            "Used to smooth out sudden large rotations")]
        [Range(0.01f, 30)] 
        public float NeckRotationLerpSpeed = 22;

        // Debug gizmo settings
        [Header("Debug Gizmos")]
        [Tooltip("If true, draw any enabled debug gizmos")]
        public bool drawDebugGizmos = false;
        public Color debugGizmoColor = Color.green;
        public bool drawHeadPosition = true;
        public bool drawNeckPosition = true;
        public bool drawShoulderPositions = true;
        
        private float headGizmoRadius = 0.09f;
        private float neckGizmoRadius = 0.02f;
        private float shoulderGizmoRadius = 0.02f;

        private RotationDeadzone neckYawDeadzone;
        private Transform head;
        private Transform transformHelper;

        //Inferred Body Positions

        /// <summary>
        /// Inferred Neck position
        /// </summary>
        public Vector3 NeckPosition { get; private set; }
        
        /// <summary>
        /// Inferred neck position, based purely off of a local space offset to the head
        /// </summary>
        public Vector3 NeckPositionLocalSpace { get; private set; }
        
        /// <summary>
        /// Inferred neck position, based purely off of a world space offset to the head
        /// </summary>
        public Vector3 NeckPositionWorldSpace { get; private set; }
        
        /// <summary>
        /// Inferred neck rotation
        /// </summary>
        public Quaternion NeckRotation { get; private set; }

        /// <summary>
        /// Inferrerd shoulder position
        /// </summary>
        public Vector3[] ShoulderPositions { get; private set; }

        /// <summary>
        /// Inferred shoulder position, based purely off of a local space offset to the head
        /// </summary>
        public Vector3[] ShoulderPositionsLocalSpace { get; private set; }

        private void Start()
        {
            head = Camera.main.transform;
            transformHelper = new GameObject("InferredBodyPositions_TransformHelper").transform;
            transformHelper.SetParent(transform);

            ShoulderPositions = new Vector3[2];
            ShoulderPositionsLocalSpace = new Vector3[2];

            if (neckYawDeadzone == null)
            {
                neckYawDeadzone = GetComponent<RotationDeadzone>();
            }
        }

        private void Update()
        {
            neckYawDeadzone.UpdateDeadzone(head.rotation.eulerAngles.y);
            UpdateBodyPositions();
        }

        private void UpdateBodyPositions()
        {
            UpdateNeckPositions();
            UpdateNeckRotation();
            UpdateHeadOffsetShoulderPositions();
            UpdateShoulderPositions();
        }

        private void UpdateNeckPositions()
        {
            UpdateLocalOffsetNeckPosition();
            UpdateWorldOffsetNeckPosition();
            UpdateNeckPosition();
        }

        private void UpdateNeckPosition()
        {
            NeckPosition = Vector3.Lerp(NeckPositionLocalSpace, NeckPositionWorldSpace, WorldLocalNeckPositionBlend);
        }

        private void UpdateNeckRotation()
        {
            float neckYRotation = useNeckYawDeadzone ? neckYawDeadzone.DeadzoneCentre : head.rotation.eulerAngles.y;
            NeckRotation = Quaternion.Lerp(NeckRotation, Quaternion.Euler(0, neckYRotation, 0), Time.deltaTime * NeckRotationLerpSpeed);
        }

        private void UpdateLocalOffsetNeckPosition()
        {
            Vector3 localNeckOffset = new Vector3()
            {
                x = 0,
                y = neckOffset,
                z = 0
            };

            transformHelper.position = head.position;
            transformHelper.rotation = head.rotation;
            NeckPositionLocalSpace = transformHelper.TransformPoint(localNeckOffset);
        }

        private void UpdateWorldOffsetNeckPosition()
        {
            Vector3 worldNeckOffset = new Vector3()
            {
                x = 0,
                y = neckOffset,
                z = 0
            };

            Vector3 headPosition = head.position;
            NeckPositionWorldSpace = headPosition + worldNeckOffset;
        }

        private void UpdateHeadOffsetShoulderPositions()
        {
            ShoulderPositionsLocalSpace[0] = head.TransformPoint(-shoulderOffset, neckOffset, 0);
            ShoulderPositionsLocalSpace[1] = head.TransformPoint(shoulderOffset, neckOffset, 0);
        }

        private void UpdateShoulderPositions()
        {
            if (useNeckPositionForShoulders)
            {
                ShoulderPositions[0] = GetShoulderPosAtRotation(true, NeckRotation);
                ShoulderPositions[1] = GetShoulderPosAtRotation(false, NeckRotation);
            }
            else
            {
                ShoulderPositions = ShoulderPositionsLocalSpace;
            }
        }

        private Vector3 GetShoulderPosAtRotation(bool isLeft, Quaternion neckRotation)
        {
            transformHelper.position = NeckPosition;
            transformHelper.rotation = neckRotation;

            Vector3 shoulderNeckOffset = new Vector3
            {
                x = isLeft ? -shoulderOffset : shoulderOffset,
                y = 0,
                z = 0
            };

            return transformHelper.TransformPoint(shoulderNeckOffset);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !drawDebugGizmos)
            {
                return;
            }
            Gizmos.color = debugGizmoColor;

            if (drawHeadPosition)
            {
                // Draw head
                Gizmos.matrix = Matrix4x4.TRS(head.position, head.rotation, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, Vector3.one * headGizmoRadius);
                Gizmos.matrix = Matrix4x4.identity;
            }

            //Draw deadzoned shoulder positions 
            if (drawShoulderPositions)
            {
                Gizmos.DrawSphere(ShoulderPositions[0], shoulderGizmoRadius);
                Gizmos.DrawSphere(ShoulderPositions[1], shoulderGizmoRadius);

                //Draw a line between both stable shoulder positions
                Gizmos.DrawLine(ShoulderPositions[0], ShoulderPositions[1]);
            }

            if (drawNeckPosition)
            {
                //Draw the neck position
                Gizmos.DrawSphere(NeckPosition, neckGizmoRadius);
                Gizmos.DrawLine(head.position, NeckPosition);
            }
        }
    }
#pragma warning restore 0618
}