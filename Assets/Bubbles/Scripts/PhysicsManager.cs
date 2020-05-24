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

        private void FixedUpdate()
        {

        }
    }
}
