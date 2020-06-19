using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace BubbleDistortionPhysics
{
    class PhysicsManager : ScriptableObject
    {
        public PhysicsManager()
        {            
            PhysicsObjects = new List<PhysicsObject>();
            PhysicsDistorters = new List<PhysicsDistorter>();
            VendingMachines = new List<VendingMachine>();
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
        public List<VendingMachine> VendingMachines { get; set; }

        private void FixedUpdate()
        {

        }

        public Vector3 Reset()
        {
            Vector3 newPlayerPosition;

            newPlayerPosition = Vector3.zero;
            VendingMachine vendingMachine = VendingMachines.OrderByDescending(vm => vm.LastButtonPressed).FirstOrDefault();

            if (vendingMachine != null)
            {                
                vendingMachine.Reset();

                newPlayerPosition = vendingMachine.transform.position + new Vector3(-1, 0, 0);
            }

            return newPlayerPosition;
        }

        public void GravityInverse()
        {
            foreach (var item in PhysicsDistorters)
            {
                item.GetComponent<Rigidbody>().useGravity = false;
            }
        }

        public void GravityNormal()
        {
            foreach (var item in PhysicsDistorters)
            {
                item.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }
}
