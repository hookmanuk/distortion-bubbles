﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleDistortionPhysics
{
    public class TeleportDRoom : MonoBehaviour
    {
        public Vector3 StartPosition { get; set; }

        private void Start()
        {
            StartPosition = transform.position;    
        }
    }
}
