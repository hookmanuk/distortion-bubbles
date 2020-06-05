using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class KeyLockBottomSide : MonoBehaviour
    {
        private Door _door;
        public void Start()
        {
            _door = GetComponentInParent<Door>();         
        }

        private void OnCollisionEnter(Collision collision)
        {
            OutputLogManager.OutputText(this.name + " lock bottom hit by " + collision.gameObject.name);
            if (collision.gameObject.GetComponent<KeyObject>() != null)
            {
                OutputLogManager.OutputText("Opening door");
                StartCoroutine(_door.OpenDoor());
            }
        }
    }    
}
