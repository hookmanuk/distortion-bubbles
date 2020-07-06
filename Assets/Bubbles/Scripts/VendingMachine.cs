using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class VendingMachine : MonoBehaviour
    {        
        public int StockSlowLevel;
        public int StockGrowLevel;
        public int StockShrinkLevel;
        public int StockGravityLevel;
        public int StockLaunchLevel;
        public int StockCutOffLevel;
        public int StockShowLevel;
        
        public VendingButton SlowButton;
        public VendingButton GrowButton;
        public VendingButton ShrinkButton;
        public VendingButton GravityButton;
        public VendingButton LaunchButton;
        public VendingButton CutOffButton;
        public VendingButton ShowButton;

        public Light PointLight;
        public int Order;        

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }
        public PhysicsDistorter BubbleGravity { get; set; }
        public PhysicsDistorter BubbleLaunch { get; set; }
        public PhysicsDistorter BubbleCutOff { get; set; }
        public PhysicsDistorter BubbleShow { get; set; }

        public DateTime LastButtonPressed { get; set; }

        public List<GameObject> MyBubbles { get; set; }
        

        public List<PhysicsObject> MyPhysicsObjects;
        private int _startStockSlowLevel;
        private int _startStockGrowLevel;
        private int _startStockShrinkLevel;
        private int _startStockGravityLevel;
        private int _startStockLaunchLevel;
        private int _startStockCutOffLevel;
        private int _startStockShowLevel;

        private void Start()
        {
            MyBubbles = new List<GameObject>();
        }

        private void Awake()
        {
            BubbleSlow = GameObject.FindGameObjectWithTag("BubbleSlow").GetComponent<PhysicsDistorter>();
            BubbleGrow = GameObject.FindGameObjectWithTag("BubbleGrow").GetComponent<PhysicsDistorter>();
            BubbleShrink = GameObject.FindGameObjectWithTag("BubbleShrink").GetComponent<PhysicsDistorter>();
            BubbleGravity = GameObject.FindGameObjectWithTag("BubbleGravity").GetComponent<PhysicsDistorter>();
            BubbleLaunch = GameObject.FindGameObjectWithTag("BubbleLaunch").GetComponent<PhysicsDistorter>();
            BubbleCutOff = GameObject.FindGameObjectWithTag("BubbleCutOff").GetComponent<PhysicsDistorter>();
            BubbleShow = GameObject.FindGameObjectWithTag("BubbleShow").GetComponent<PhysicsDistorter>();

            SetStockSlowLevel(StockSlowLevel);
            SetStockGrowLevel(StockGrowLevel);
            SetStockShrinkLevel(StockShrinkLevel);
            SetStockGravityLevel(StockGravityLevel);
            SetStockLaunchLevel(StockLaunchLevel);
            SetStockCutOffLevel(StockCutOffLevel);
            SetStockShowLevel(StockShowLevel);

            LastButtonPressed = DateTime.Now;
            
            PhysicsManager.Instance.VendingMachines.Add(this);
            _startStockSlowLevel = StockSlowLevel;
            _startStockGrowLevel = StockGrowLevel;
            _startStockShrinkLevel = StockShrinkLevel;
            _startStockGravityLevel = StockGravityLevel;
            _startStockLaunchLevel = StockLaunchLevel;
            _startStockCutOffLevel = StockCutOffLevel;
            _startStockShowLevel = StockShowLevel;
        }

        public void OnDestroy()
        {
            PhysicsManager.Instance.VendingMachines.Remove(this);
        }

        public void Reset()
        {
            foreach (var item in MyBubbles)
            {
                item.gameObject.SetActive(false);
                //Destroy(item);
            }

            foreach (PhysicsObject physicsObject in MyPhysicsObjects)
            {
                physicsObject.Reset();
            }

            SetStockSlowLevel(_startStockSlowLevel);
            SetStockGrowLevel(_startStockGrowLevel);
            SetStockShrinkLevel(_startStockShrinkLevel);
            SetStockGravityLevel(_startStockGravityLevel);
            SetStockLaunchLevel(_startStockLaunchLevel);
            SetStockCutOffLevel(_startStockCutOffLevel);
            SetStockShowLevel(_startStockShowLevel);
        }

        public void SetStockSlowLevel(int stockLevel)
        {
            StockSlowLevel = stockLevel;
            SlowButton.SetStockLevel(stockLevel);            
        }

        public void SetStockGrowLevel(int stockLevel)
        {
            StockGrowLevel = stockLevel;
            GrowButton.SetStockLevel(stockLevel);
        }

        public void SetStockShrinkLevel(int stockLevel)
        {
            StockShrinkLevel = stockLevel;
            ShrinkButton.SetStockLevel(stockLevel);
        }

        public void SetStockGravityLevel(int stockLevel)
        {
            StockGravityLevel = stockLevel;
            GravityButton.SetStockLevel(stockLevel);
        }

        public void SetStockLaunchLevel(int stockLevel)
        {
            StockLaunchLevel = stockLevel;
            LaunchButton.SetStockLevel(stockLevel);
        }

        public void SetStockCutOffLevel(int stockLevel)
        {
            StockCutOffLevel = stockLevel;
            CutOffButton.SetStockLevel(stockLevel);
        }

        public void SetStockShowLevel(int stockLevel)
        {
            StockLaunchLevel = stockLevel;
            ShowButton.SetStockLevel(stockLevel);
        }
    }
}
