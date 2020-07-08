using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class PuzzleArea : MonoBehaviour
    {        
        public List<LightStrip> Lights;     

        public void Start()
        {            
            
        }        

        private void OnTriggerEnter(Collider other)
        {
            OutputLogManager.OutputText(other.name + " entered " + this.name + " with tag " + other.tag);
            if (other.CompareTag("Player"))
            {
                PlayerController.Instance.AddPuzzleArea(this);                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController.Instance.RemovePuzzleArea(this);                
            }
        }
    }
}
