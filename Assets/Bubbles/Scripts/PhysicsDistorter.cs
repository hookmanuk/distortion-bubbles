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
        public ExpandType ExpandType;
        public bool Expanded { get; set; }
        public VendingMachine SourceMachine { get; set; }
        
        public void Start()
        {
            PhysicsManager.Instance.PhysicsDistorters.Add(this);
            _grabInteractable = GetComponent<XRGrabInteractable>();

            XRInteractableEvent grabEnterEvent = new XRInteractableEvent();
            grabEnterEvent.AddListener(OnSelectEnter);
            _grabInteractable.onSelectEnter = grabEnterEvent;

            XRInteractableEvent grabExitEvent = new XRInteractableEvent();
            grabExitEvent.AddListener(OnSelectExit);
            _grabInteractable.onSelectExit = grabExitEvent;

            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
            _sphereCollider = GetComponent<SphereCollider>();            
        }

        private void OnSelectEnter(XRBaseInteractor interactor)
        {
            SourceMachine?.PointLight.gameObject.SetActive(false);
            //OutputLogManager.OutputText(name + " select exit " + interactor.gameObject.name);
            _thrown = true;
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
            //if (PlayerController.Instance.ReverseGravity)
            //{
               // _rigidbody.AddForce(Vector3.up * 9.81f * Time.deltaTime);
            //}
        }

        private void Update()
        {            
        }

        private void OnCollisionEnter(Collision collision)
        {
            OutputLogManager.OutputText(name + " hit " + collision.gameObject.name);

            if (ExpandType != ExpandType.None && _thrown && collision.gameObject.GetComponent<PhysicsSurface>() != null)
            {
                Expanded = true;

                if (ExpandType == ExpandType.Disc)
                {
                    ExpandDisc();
                }
                else
                {
                    ExpandBubble();
                }
                this.gameObject.layer = 0;
            }
        }

        private void ExpandDisc()
        {
            _rigidbody.useGravity = false;
            _rigidbody.velocity = new Vector3(0, 0, 0);
            _rigidbody.freezeRotation = true;
            _boxCollider.enabled = false;
            _sphereCollider.enabled = true;
            transform.localScale = new Vector3(1, 0.2f, 1);
            //transform.localPosition = transform.localPosition + new Vector3(0, 0.09f, 0);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            _grabInteractable.interactionLayerMask = LayerMask.GetMask("Nothing");            
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
            StartCoroutine(ScaleOverTime(0.2f, transform, new Vector3(2f, 2f, 2f)));
            //transform.localScale = new Vector3(2, 2, 2);
            //transform.position = new Vector3(transform.position.x, transform.position.y + 0.33f, transform.position.z);
            _grabInteractable.interactionLayerMask = LayerMask.GetMask("Nothing");
        }

        private void OnTriggerEnter(Collider other)
        {
            OutputLogManager.OutputText(name + " triggered " + other.gameObject.name);
            if (_thrown)
            {
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
                if (other.gameObject.CompareTag("Player"))
                {
                    if (DistorterType == DistorterType.Gravity)
                    {
                        other.gameObject.GetComponent<PlayerController>().FlipGravity();
                    }
                    else if (DistorterType == DistorterType.Launch)
                    {
                        other.gameObject.GetComponent<PlayerController>().LaunchPlayer();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_thrown)
            {
                if (other.gameObject.CompareTag("PhysicsObject"))
                {
                    StartCoroutine(ExecuteAfterTime(0.1f, (otherCollider) =>
                    {
                        if (DistorterType == DistorterType.Slow)
                        {
                        //OutputLogManager.OutputText(other.gameObject.name + " sped up");
                        otherCollider.gameObject.GetComponent<PhysicsObject>().IsSlowed = false;
                        }
                        else if (DistorterType == DistorterType.Grow)
                        {
                            otherCollider.gameObject.GetComponent<PhysicsObject>().IsGrown = false;
                        }
                        else if (DistorterType == DistorterType.Shrink)
                        {
                            otherCollider.gameObject.GetComponent<PhysicsObject>().IsShrunk = false;
                        }
                    }, other));
                }
            }
        }

        IEnumerator ExecuteAfterTime(float time, Action<Collider> task, Collider other)
        {
            yield return new WaitForSeconds(time);
            task(other);        
        }

        IEnumerator ScaleOverTime(float time, Transform transform, Vector3 newScale)
        {
            float currentTime = 0f;
            Vector3 originalScale = transform.localScale;

            while (currentTime < time)
            {
                transform.localScale = Vector3.Lerp(originalScale, newScale, currentTime / time);

                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    public enum DistorterType
    {
        Slow,
        Grow,
        Shrink,
        Gravity,
        Launch,        
        Show
    }

    public enum ExpandType
    {
        Bubble,
        Disc,
        None
    }
}
