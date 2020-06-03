using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class VendingMachine : MonoBehaviour
    {        
        public int StockLevel;        

        public SimpleHelvetica CountText;

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }

        public DateTime LastButtonPressed { get; set; }

        public List<GameObject> MyBubbles { get; set; }

        public List<PhysicsObject> MyPhysicsObjects;
        private int _startStockLevel;

        private void Start()
        {
            MyBubbles = new List<GameObject>();
        }

        private void Awake()
        {
            BubbleSlow = GameObject.FindGameObjectWithTag("BubbleSlow").GetComponent<PhysicsDistorter>();
            BubbleGrow = GameObject.FindGameObjectWithTag("BubbleGrow").GetComponent<PhysicsDistorter>();
            BubbleShrink = GameObject.FindGameObjectWithTag("BubbleShrink").GetComponent<PhysicsDistorter>();

            CountText.Text = StockLevel.ToString();
            CountText.GenerateText();

            LastButtonPressed = DateTime.Now;
            
            PhysicsManager.Instance.VendingMachines.Add(this);
            _startStockLevel = StockLevel;
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.VendingMachines.Remove(this);
        }

        public void Reset()
        {
            foreach (var item in MyBubbles)
            {
                Destroy(item);
            }

            foreach (PhysicsObject physicsObject in MyPhysicsObjects)
            {
                physicsObject.Reset();
            }

            SetStockLevel(_startStockLevel);
        }

        public void SetStockLevel(int stockLevel)
        {
            StockLevel = stockLevel;

            CountText.Text = StockLevel.ToString();
            CountText.GenerateText();
        }
    }
}
