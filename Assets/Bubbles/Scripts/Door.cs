using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class Door : MonoBehaviour
    {
        public GameObject DoorPanel;
        public GameObject KeyLock;
        private int _intTimeToOpen = 1;
        private bool _doorOpened;

        public void Start()
        {            
        }
        
        public IEnumerator OpenDoor()
        {
            if (!_doorOpened)
            {
                _doorOpened = true;
                KeyLock.GetComponent<AudioSource>().Play();

                var t = 0f;
                while (t < 1)
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                DoorPanel.GetComponent<AudioSource>().Play();
                var currentPos = DoorPanel.transform.localPosition;
                var currentScale = DoorPanel.transform.localScale;
                var rigidBody = DoorPanel.GetComponent<Rigidbody>();
                t = 0;
                while (t < 2)
                {
                    t += Time.deltaTime / _intTimeToOpen;
                    DoorPanel.transform.localPosition = Vector3.Lerp(currentPos, new Vector3(currentPos.x, currentPos.y + 1, currentPos.z), t);
                    DoorPanel.transform.localScale = Vector3.Lerp(currentScale, new Vector3(currentScale.x, currentScale.y - 1.99f, currentScale.z), t);
                    yield return null;
                }
            }            
        }        
    }
}
