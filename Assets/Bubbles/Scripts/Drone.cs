using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class Drone : MonoBehaviour
    {        
        private void OnCollisionEnter(Collision collision)
        {
            //OutputLogManager.OutputText(this.name + " collided with " + collision.gameObject.name);
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.gameObject.GetComponent<PlayerController>().Reset();
            }
        }
    }
}
