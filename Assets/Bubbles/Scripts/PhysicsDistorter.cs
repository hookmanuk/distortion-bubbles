using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class PhysicsDistorter : MonoBehaviour
    {
        private bool _thrown;
        private XRGrabInteractable _grabInteractable;
        private Rigidbody _rigidbody;
        private BoxCollider _boxCollider;
        private SphereCollider _sphereCollider;
        public DistorterType DistorterType;

        public void Start()
        {
            PhysicsManager.Instance.PhysicsDistorters.Add(this);
            _grabInteractable = GetComponent<XRGrabInteractable>();

            XRInteractableEvent grabExitEvent = new XRInteractableEvent();
            grabExitEvent.AddListener(OnSelectExit);
            _grabInteractable.onSelectExit = grabExitEvent;

            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();
        }

        private void OnSelectExit(XRBaseInteractor interactor)
        {
            //OutputLogManager.OutputText(name + " select exit " + interactor.gameObject.name);
            _thrown = true;
        }        

        public void OnDestroy()
        {
            PhysicsManager.Instance.PhysicsDistorters.Remove(this);
        }

       
        public void FixedUpdate()
        {            
        }

        private void Update()
        {            
        }

        private void OnCollisionEnter(Collision collision)
        {
            //OutputLogManager.OutputText(name + " hit " + collision.gameObject.name);

            if (_thrown && collision.gameObject.GetComponent<PhysicsSurface>() != null)
            {
                ExpandBubble();
            }
        }

        private void ExpandBubble()
        {
            //OutputLogManager.OutputText(name + " expand bubble");            
            _rigidbody.useGravity = false;
            //_rigidbody.isKinematic = true;
            _boxCollider.enabled = false;
            _sphereCollider.enabled = true;
            _rigidbody.velocity = new Vector3(0, 0, 0);
            _rigidbody.freezeRotation = true;
            transform.localScale = new Vector3(2, 2, 2);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.33f, transform.position.z);
            _grabInteractable.interactionLayerMask = LayerMask.GetMask("Nothing");
        }

        private void OnTriggerEnter(Collider other)
        {
            OutputLogManager.OutputText(name + " triggered " + other.gameObject.name);

            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                if (DistorterType == DistorterType.Slow)
                {
                    OutputLogManager.OutputText(other.gameObject.name + " slowed");
                    other.gameObject.GetComponent<PhysicsObject>().IsSlowed = true;
                }
                else if (DistorterType == DistorterType.Grow)
                {
                    other.gameObject.GetComponent<PhysicsObject>().IsGrown = true;
                }
                else if (DistorterType == DistorterType.Shrink)
                {
                    other.gameObject.GetComponent<PhysicsObject>().IsShrunk = true;
                }
            }            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                if (DistorterType == DistorterType.Slow)
                {
                    //OutputLogManager.OutputText(other.gameObject.name + " sped up");
                    other.gameObject.GetComponent<PhysicsObject>().IsSlowed = false;
                }
                else if (DistorterType == DistorterType.Grow)
                {
                    other.gameObject.GetComponent<PhysicsObject>().IsGrown = false;
                }
                else if (DistorterType == DistorterType.Shrink)
                {
                    other.gameObject.GetComponent<PhysicsObject>().IsShrunk = false;
                }
            }
        }
    }

    public enum DistorterType
    {
        Slow,
        Grow,
        Shrink
    }
}
