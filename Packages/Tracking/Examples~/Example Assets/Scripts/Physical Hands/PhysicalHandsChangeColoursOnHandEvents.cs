/******************************************************************************
 * Copyright (C) Ultraleap, Inc. 2011-2023.                                   *
 *                                                                            *
 * Use subject to the terms of the Apache License 2.0 available at            *
 * http://www.apache.org/licenses/LICENSE-2.0, or another agreement           *
 * between Ultraleap and you, your company or other organization.             *
 ******************************************************************************/

using UnityEngine;


namespace Leap.Unity.PhysicalHands.Examples
{
    public class PhysicalHandsChangeColoursOnHandEvents : MonoBehaviour
    {
        Renderer objectRenderer;

        [SerializeField]
        Material baseMaterial;
        [SerializeField]
        Material hoverMaterial;
        [SerializeField]
        Material contactMaterial;
        [SerializeField]
        Material grabMaterial;

        bool leftHover = false;
        bool leftContact = false;
        bool leftGrab = false;

        bool rightHover = false;
        bool rightContact = false;
        bool rightGrab = false;

        private void Start()
        {
            objectRenderer = GetComponent<Renderer>();
        }

        public void HoverEnter(ContactHand hand)
        {
            switch (hand.Handedness)
            {
                case Leap.Unity.Chirality.Left:
                    leftHover = true;
                    break;
                case Leap.Unity.Chirality.Right:
                    rightHover = true;
                    break;
            }

            HandleStateChange();
        }
        public void HoverExit(ContactHand hand)
        {
            switch (hand.Handedness)
            {
                case Leap.Unity.Chirality.Left:
                    leftHover = false;
                    break;
                case Leap.Unity.Chirality.Right:
                    rightHover = false;
                    break;
            }

            HandleStateChange();
        }
        public void ContactEnter(ContactHand hand)
        {
            switch (hand.Handedness)
            {
                case Leap.Unity.Chirality.Left:
                    leftContact = true;
                    break;
                case Leap.Unity.Chirality.Right:
                    rightContact = true;
                    break;
            }

            HandleStateChange();
        }
        public void ContactExit(ContactHand hand)
        {
            switch (hand.Handedness)
            {
                case Leap.Unity.Chirality.Left:
                    leftContact = false;
                    break;
                case Leap.Unity.Chirality.Right:
                    rightContact = false;
                    break;
            }

            HandleStateChange();
        }
        public void GrabEnter(ContactHand hand)
        {
            switch (hand.Handedness)
            {
                case Leap.Unity.Chirality.Left:
                    leftGrab = true;
                    break;
                case Leap.Unity.Chirality.Right:
                    rightGrab = true;
                    break;
            }

            HandleStateChange();
        }
        public void GrabExit(ContactHand hand)
        {
            switch (hand.Handedness)
            {
                case Leap.Unity.Chirality.Left:
                    leftGrab = false;
                    break;
                case Leap.Unity.Chirality.Right:
                    rightGrab = false;
                    break;
            }

            HandleStateChange();
        }

        void HandleStateChange()
        {
            if (leftGrab || rightGrab)
            {
                objectRenderer.material = grabMaterial;
            }
            else if (leftContact || rightContact)
            {
                objectRenderer.material = contactMaterial;
            }
            else if (leftHover || rightHover)
            {
                objectRenderer.material = hoverMaterial;
            }
            else
            {
                objectRenderer.material = baseMaterial;
            }
        }
    }
}