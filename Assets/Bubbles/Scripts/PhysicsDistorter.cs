﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Random = System.Random;

namespace BubbleDistortionPhysics
{
    public class PhysicsDistorter : MonoBehaviour
    {
        private bool _thrown;
        private bool _grabbed;
        private XRGrabInteractable _grabInteractable;
        private Rigidbody _rigidbody;
        private BoxCollider _boxCollider;
        private SphereCollider _sphereCollider;
        public DistorterType DistorterType;        
        public ExpandType ExpandType;
        public bool DisolvedByForcefield = false;
        private GameObject _blackHoleExplodeObject;
        private Material _material;
        private Color _emissiveColor;
        private Random _r;

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

            _blackHoleExplodeObject = GameObject.FindGameObjectWithTag("BlackHoleExplodeObject");

            MeshRenderer meshRenderer;
            if (TryGetComponent(out meshRenderer))
            {
                _material = meshRenderer.material;
                _emissiveColor = _material.GetColor("_EmissiveColor");
            }

            _r = new System.Random(Convert.ToInt32(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)));
        }

        private void OnSelectEnter(XRBaseInteractor interactor)
        {
            SourceMachine?.PointLight.gameObject.SetActive(false);
            //OutputLogManager.OutputText(name + " select exit " + interactor.gameObject.name);
            _thrown = false;
            _grabbed = true;
        }

        private void OnSelectExit(XRBaseInteractor interactor)
        {
            //OutputLogManager.OutputText(name + " select exit " + interactor.gameObject.name);
            _thrown = true;
            _grabbed = false;
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

            if (Expanded && DistorterType == DistorterType.BlackHole)
            {
                //draw all objects nearby in
                Collider[] hitObjects = Physics.OverlapSphere(transform.position, 10f);

                foreach (Collider item in hitObjects)
                {
                    PhysicsObject physicsObject = item.GetComponent<PhysicsObject>();
                    if (physicsObject != null)
                    {                        
                        if (physicsObject.Path?.Length > 0)
                        {
                            physicsObject.Path = null;
                        }
                        Vector3 diff = transform.position - item.transform.position;
                        if (diff.magnitude <= 0.15f)
                        {
                            item.gameObject.SetActive(false);
                            GameObject blackHoleExplode = UnityEngine.Object.Instantiate(_blackHoleExplodeObject);
                            blackHoleExplode.transform.position = item.gameObject.transform.position;
                            blackHoleExplode.GetComponent<BlackHoleExplode>().Explode();
                        }
                        else
                        {
                            item.attachedRigidbody.AddForce(diff * (20f/ ((float)(Math.Pow(diff.magnitude,3)))), ForceMode.Acceleration);
                            if (item.attachedRigidbody.useGravity)
                            {
                                item.attachedRigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration); //counteract gravity
                            }
                        }                        
                    }
                }
            }

            if (DistorterType == DistorterType.Light)
            {
                if (_grabbed)
                {
                    if (PlayerController.Instance.TriggerPercentage > 0)
                    {
                        if (_material.GetColor("_EmissiveColor") != _emissiveColor)
                        {
                            _material.SetColor("_EmissiveColor", _emissiveColor);
                        }
                    }
                    else
                    {
                        if (_material.GetColor("_EmissiveColor") != _emissiveColor * 0.1f)
                        {
                            _material.SetColor("_EmissiveColor", _emissiveColor * 0.1f);
                        }
                    }
                }
                //draw all objects nearby in
                Collider[] hitObjects = Physics.OverlapSphere(transform.position, 5f);

                foreach (Collider item in hitObjects)
                {
                    PhysicsObject physicsObject = item.GetComponent<PhysicsObject>();
                    if (physicsObject != null)
                    {
                        if (physicsObject.FollowsLight)
                        {
                            if (_grabbed)
                            {
                                //Vector3 diff = transform.position - item.transform.position;

                                //item.attachedRigidbody.AddForce(diff * (20f / ((float)(Math.Pow(diff.magnitude, 3))) * PlayerController.Instance.TriggerPercentage), ForceMode.Acceleration);
                                //if (item.attachedRigidbody.useGravity)
                                //{
                                item.attachedRigidbody.AddForce(Physics.gravity / 1, ForceMode.Acceleration); //stop them flying
                                                                                                              //}
                                Vector3 diff = transform.position - item.transform.position;
                                item.attachedRigidbody.AddForce(diff.normalized * PlayerController.Instance.TriggerPercentage * Math.Max(Math.Min(diff.magnitude,2),1) / 500f, ForceMode.Impulse);
                                Debug.Log(diff.magnitude);
                                if (PlayerController.Instance.TriggerPercentage > 0 && _r.Next(1, 90 * 1) == 1) //~every 1 secs
                                {
                                    item.GetComponents<AudioSource>()[1].volume = 0.02f * PlayerController.Instance.TriggerPercentage * Math.Max(Math.Min(diff.magnitude, 5), 1);
                                    item.GetComponents<AudioSource>()[1].Play();
                                }
                            }
                            
                            if (_r.Next(1, 90 * 5) == 1) //~every 5 secs
                            {
                                item.attachedRigidbody.AddForce((Vector3.up + Vector3.right * _r.Next(-100,100) / 300 + Vector3.forward * _r.Next(-100, 100) / 300) / 40f, ForceMode.Impulse);
                                item.GetComponents<AudioSource>()[0].Play();
                            };

                            if (_r.Next(1, 90 * 1) == 1) //~every 1 secs
                            {
                                item.attachedRigidbody.AddForce((Vector3.up * 0.1f + Vector3.right * _r.Next(-100, 100) / 300 + Vector3.forward * _r.Next(-100, 100) / 300) / 40f, ForceMode.Impulse);                                
                            };
                        }
                    }
                }                                 
            }            
        }

        private void Update()
        {            
        }

        private void OnCollisionEnter(Collision collision)
        {
            PhysicsSurface surface;
            HintArea hintArea;
            OutputLogManager.OutputText(name + " hit " + collision.gameObject.name);

            if (ExpandType != ExpandType.None && _thrown)
            {
                surface = collision.gameObject.GetComponent<PhysicsSurface>();
                if (surface != null)
                {
                    if (surface.TargetDistorterType == DistorterType)
                    {
                        TriggerSuccess();
                    }
                    Expanded = true;

                    if (ExpandType == ExpandType.Disc)
                    {
                        ExpandDisc();
                    }
                    else if (ExpandType == ExpandType.Sphere)
                    {
                        ExpandSphere();
                    }
                    else if (ExpandType == ExpandType.Bubble)
                    { 
                        ExpandBubble();
                    }
                    this.gameObject.layer = 0;
                }                
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

        private void ExpandSphere()
        {            
            _rigidbody.useGravity = false;         
            _boxCollider.enabled = false;
            _sphereCollider.enabled = true;
            _rigidbody.velocity = new Vector3(0, 0, 0);
            _rigidbody.freezeRotation = true;
            StartCoroutine(ScaleOverTime(0.2f, transform, new Vector3(0.4f, 0.4f, 0.4f)));
            //transform.localScale = new Vector3(2, 2, 2);
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.z);
            _grabInteractable.interactionLayerMask = LayerMask.GetMask("Nothing");
        }

        public void TriggerSuccess()
        {
            //you did well!
            GetComponents<AudioSource>()[1].Play();
            GetComponent<ParticleSystem>()?.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            OutputLogManager.OutputText(name + " triggered " + other.gameObject.name);

            if (DisolvedByForcefield && other.gameObject.CompareTag("Forcefield"))
            {
                gameObject.SetActive(false);
            }

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

                if (!Expanded)
                {
                    HintArea hintArea;
                    hintArea = other.gameObject.GetComponent<HintArea>();
                    if (hintArea != null)
                    {
                        if (hintArea.DistorterType == DistorterType)
                        {
                            TriggerSuccess();
                        }
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
        Show,
        Hint,
        None,
        BlackHole,
        Light
    }

    public enum ExpandType
    {
        Bubble,
        Disc,
        None,
        Sphere
    }
}
