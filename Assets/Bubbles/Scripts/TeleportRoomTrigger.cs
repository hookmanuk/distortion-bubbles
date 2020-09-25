using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class TeleportRoomTrigger : MonoBehaviour
    {
        public TeleportRoom Room;
        public int TriggerNumber;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (TriggerNumber == 1)
                {
                    Room.Trigger1();
                }
                else if (TriggerNumber == 2)
                {
                    Room.Trigger2();
                }
            }
        }
    }
}
