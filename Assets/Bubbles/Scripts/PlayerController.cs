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
        private MeshRenderer _outOfBoundsFace;
        public bool ReverseGravity;
        private float GravityMultiplier = 1f;

        Vector2 currentState;
        Vector3 direction;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            //_outOfBoundsFace = GameObject.FindGameObjectWithTag("OutOfBoundsFace").GetComponent<MeshRenderer>();
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

                var originalGravity = GravityMultiplier;
                var t = 0f;
                var intTimeToOpen = 0.3f;
                while (t < 1)
                {
                    t += Time.deltaTime / intTimeToOpen;

                    GravityMultiplier = originalGravity + (t * 2 * -originalGravity);
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
        }

        public void FlipGravity()
        {
            _flipping = true;
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

                    movement = new Vector3(0, -9.81f * GravityMultiplier, 0) * Time.deltaTime;

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
                    PhysicsManager.Instance.Reset();
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
    }
}