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
            LightStrips = new List<LightStrip>();
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

        private bool _lightsOff = false;
        public void TurnOffLights()
        {
            _lightsOff = true;

            foreach (LightStrip light in LightStrips)
            {
                light.gameObject.SetActive(false);
            }
        }

        public List<PhysicsObject> PhysicsObjects { get; set; }
        public List<PhysicsDistorter> PhysicsDistorters { get; set; }
        public List<VendingMachine> VendingMachines { get; set; }

        public List<LightStrip> LightStrips { get; set; }       

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
                if (item.ExpandType == ExpandType.Disc && !item.Expanded)
                {
                    item.GetComponent<Rigidbody>().useGravity = false;
                }
            }
        }

        public void GravityNormal()
        {
            foreach (var item in PhysicsDistorters)
            {
                if (item.ExpandType == ExpandType.Disc && !item.Expanded)
                {
                    item.GetComponent<Rigidbody>().useGravity = true;
                }
            }
        }
    }
}
