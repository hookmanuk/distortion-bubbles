using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class MaskedObject : MonoBehaviour
    {        
        public void Start()
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.material.renderQueue = 3002;
            }
            
        }            
    }
}
