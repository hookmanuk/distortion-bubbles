using System;
using System.Collections.Generic;
using UnityEngine;


namespace BubbleDistortionPhysics
{
    class PhysicsManager : ScriptableObject
    {
        public PhysicsManager()
        {            
            PhysicsObjects = new List<PhysicsObject>();
            PhysicsDistorters = new List<PhysicsDistorter>();
        }

        private static PhysicsManager _instance;
        public static PhysicsManager Instance { 
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance<PhysicsManager>();
                }
                return _instance;
            }            
        }

        public List<PhysicsObject> PhysicsObjects { get; set; }
        public List<PhysicsDistorter> PhysicsDistorters { get; set; }

        private void FixedUpdate()
        {

        }
    }
}
