using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

namespace UnityEngine.XR.Interaction.Toolkit
{

    public class PlayerController : MonoBehaviour
    {
        public float speed = 1;
        public CharacterController characterController;
        public XRController controller;
        public GameObject MainCamera;

        Vector2 currentState;
        Vector3 direction;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Update()
        {
            direction = characterController.GetComponentInChildren<Camera>().transform.TransformDirection(new Vector3(currentState.x, 0, currentState.y)); //Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
        }

        void FixedUpdate()
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

        void LateUpdate()
        {
            characterController.center = new Vector3(characterController.center.x, MainCamera.transform.localPosition.y, characterController.center.z);
        }
    }
}