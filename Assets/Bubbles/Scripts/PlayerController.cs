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
        private MeshRenderer _outOfBoundsFace;

        Vector2 currentState;
        Vector3 direction;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _outOfBoundsFace = GameObject.FindGameObjectWithTag("OutOfBoundsFace").GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            direction = characterController.GetComponentInChildren<Camera>().transform.TransformDirection(new Vector3(currentState.x, 0, currentState.y)); //Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
        }

        void FixedUpdate()
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

                movement = new Vector3(0, -9.81f, 0) * Time.deltaTime;

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                //stop!
                //OutputLogManager.OutputText("Player entered object " + other.gameObject.name);
                _preventCharacterMovement = true;
                Color32 col = _outOfBoundsFace.material.GetColor("_Color");
                col.a = 255;
                _outOfBoundsFace.material.SetColor("_Color", col);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("PhysicsObject"))
            {
                //go!
                //OutputLogManager.OutputText("Player exited object " + other.gameObject.name);
                _preventCharacterMovement = false;
                Color32 col = _outOfBoundsFace.material.GetColor("_Color");
                col.a = 0;
                _outOfBoundsFace.material.SetColor("_Color", col);
            }
        }

        void LateUpdate()
        {
            
                //characterController.center = new Vector3(characterController.center.x, MainCamera.transform.localPosition.y, characterController.center.z);
            
        }
    }
}