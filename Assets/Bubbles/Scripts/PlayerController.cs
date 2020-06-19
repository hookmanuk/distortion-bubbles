using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{

    public class PlayerController : MonoBehaviour
    {
        public float speed = 1;
        public CharacterController characterController { get; set; }
        public CapsuleCollider capsuleCollider { get; set; }
        public XRController controller;
        public GameObject MainCamera { get; set; }

        private bool _preventCharacterMovement;
        private bool _resetting;
        private bool _flipping;
        private static PlayerController _instance;
        public static PlayerController Instance { get { return _instance; } }
        public bool Flipping { get { return _flipping; } }
        private MeshRenderer _outOfBoundsFace;
        public bool ReverseGravity;
        private float GravityMultiplier = 1f;

        private List<GameObject> HeldObjects { get; set; } = new List<GameObject>();
        private List<int> HeldObjectLayers { get; set; } = new List<int>();

        Vector2 currentState;
        Vector3 direction;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            //_outOfBoundsFace = GameObject.FindGameObjectWithTag("OutOfBoundsFace").GetComponent<MeshRenderer>();
            _instance = this;
        }

        private void Update()
        {
            direction = characterController.GetComponentInChildren<Camera>().transform.TransformDirection(new Vector3(currentState.x, 0, currentState.y)); //Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
        }

        public void Reset()
        {
            _resetting = true;               
        }

        private bool _blnReversingGravity = false;
        private IEnumerator ReversePlayerGravity()
        {
            if (!_blnReversingGravity)
            {
                ReverseGravity = !ReverseGravity;
                
                _blnReversingGravity = true;

                var originalGravity = Physics.gravity.y;
                var t = 0f;
                var intTimeToOpen = 0.3f;
                while (t < 1)
                {
                    t += Time.deltaTime / intTimeToOpen;

                    Physics.gravity = new Vector3(0, originalGravity + (t * 2f * -originalGravity), 0);

                    if (Math.Abs(Physics.gravity.y) > Math.Abs(originalGravity))
                    {
                        Physics.gravity = new Vector3(0, -originalGravity, 0);
                        break;
                    }
                    //GravityMultiplier = 0;

                    //transform.rotation = Quaternion.Euler(0, 0,(ReverseGravity ? t : (1-t)) * 180);                    

                    //OutputLogManager.OutputText(transform.rotation.x.ToString());

                    //OutputLogManager.OutputText(t.ToString());

                    yield return null;
                }
                
                _blnReversingGravity = false;
            }
        }

        private IEnumerator RotatePlayer()
        {            
            var t = 0f;
            var intTimeToOpen = 0.7f;
            while (t < 1)
            {
                t += Time.deltaTime / intTimeToOpen;
                
                transform.rotation = Quaternion.Euler(0, 0, (ReverseGravity ? t : (1 - t)) * 180);
                yield return null;
            }

            _flipping = false;

            SetHeldObjectsInteractable();
        }

        public void FlipGravity()
        {
            _flipping = true;

            SetHeldObjectsUninteractable();

            KeyObject keyObject = controller.gameObject.GetComponentInChildren<KeyObject>();
            if (keyObject != null)
            {
                keyObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            if (ReverseGravity)
            {
                PhysicsManager.Instance.GravityNormal();
            }
            else
            {                
                PhysicsManager.Instance.GravityInverse();
            }
            
            StartCoroutine(ReversePlayerGravity());

            StartCoroutine(RotatePlayer());
        }

        void FixedUpdate()
        {
            bool blnResetClicked = false;
            bool blnTriggerClicked = false;
            bool blnClicked = false;
            Vector2 state;
            state = new Vector2();

            controller?.inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out blnTriggerClicked);
            
            if (blnTriggerClicked)
            {
                FlipGravity();
            }            

            controller?.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out blnResetClicked);

            if (blnResetClicked)
            {
                Reset();
            }

            if (_resetting)
            {                
                if (ReverseGravity)
                {
                    OutputLogManager.OutputText("flip from reset");
                    FlipGravity();
                }
                if (!_flipping)
                {
                    _resetting = false;
                    OutputLogManager.OutputText("no longer flipping, now reset");
                    Vector3 resetPosition = PhysicsManager.Instance.Reset();
                    characterController.transform.position = resetPosition - new Vector3(1 + MainCamera.transform.localPosition.x, 0, MainCamera.transform.localPosition.z);
                    capsuleCollider.height = MainCamera.transform.localPosition.y;
                    capsuleCollider.center = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y / 2, MainCamera.transform.localPosition.z);
                    
                }                
            }
            else
            {
                capsuleCollider.height = MainCamera.transform.localPosition.y;
                capsuleCollider.center = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y / 2, MainCamera.transform.localPosition.z);

                if (!_preventCharacterMovement)
                {
                    characterController.height = MainCamera.transform.localPosition.y;
                    characterController.center = new Vector3(MainCamera.transform.localPosition.x, MainCamera.transform.localPosition.y / 2, MainCamera.transform.localPosition.z);


                    InputDevice device = controller.inputDevice;
                    InputFeatureUsage<Vector2> feature = CommonUsages.secondary2DAxis;
                    Vector3 movement;

                    //movement = new Vector3(0, -9.81f * GravityMultiplier, 0) * Time.deltaTime;
                    movement = Physics.gravity * Time.deltaTime;

                    if (device.TryGetFeatureValue(feature, out currentState))
                    {
                        if (currentState.magnitude > 0.1)
                        {
                            movement = movement + speed * Time.deltaTime * Vector3.ProjectOnPlane(direction, Vector3.up);
                        }
                    }

                    characterController.Move(movement);
                }
            }           
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                if (other.gameObject.GetComponent<PhysicsObject>().Speed > 0)
                {
                    OutputLogManager.OutputText("player hit " + other.gameObject.name);
                    if (other.gameObject.GetComponent<Drone>() != null)
                    {
                        //hit a drone, so reset
                        PhysicsManager.Instance.Reset();
                    }                    
                }
                
                //OutputLogManager.OutputText("character y pos " + characterController.gameObject.transform.position.y.ToString());

                //allow walking on platforms
                if (characterController.gameObject.transform.position.y - other.gameObject.transform.position.y > 0.1f)
                {
                    //stop!
                    //OutputLogManager.OutputText("Player entered object " + other.gameObject.name);
                    _preventCharacterMovement = true;
                    //Color32 col = _outOfBoundsFace.material.GetColor("_Color");
                    //col.a = 255;
                    //_outOfBoundsFace.material.SetColor("_Color", col);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                //go!
                //OutputLogManager.OutputText("Player exited object " + other.gameObject.name);
                _preventCharacterMovement = false;
                //Color32 col = _outOfBoundsFace.material.GetColor("_Color");
                //col.a = 0;
                //_outOfBoundsFace.material.SetColor("_Color", col);
            }
        }

        void LateUpdate()
        {
            
               // characterController.center = new Vector3(characterController.center.x, MainCamera.transform.localPosition.y, characterController.center.z);
            
        }       

        public static void AddGrabbedObject(GameObject gameObject)
        {
            OutputLogManager.OutputText("added grabbed object " + gameObject.name);
            Instance.HeldObjects.Add(gameObject);
            Instance.HeldObjectLayers.Add(gameObject.layer);
        }

        public static void RemoveGrabbedObject(GameObject gameObject)
        {
            Instance.HeldObjects.Remove(gameObject);
            Instance.HeldObjectLayers.Remove(gameObject.layer);
        }

        private void SetHeldObjectsUninteractable()
        {            
            foreach (var item in HeldObjects)
            {
                item.layer = 30;
            }
        }

        private void SetHeldObjectsInteractable()
        {         
            for (int i = 0; i < HeldObjects.Count; i++)
            {
                HeldObjects[i].layer = HeldObjectLayers[i];
            }            
        }
    }
}