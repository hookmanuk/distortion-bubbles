using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int StockShowLevel;
        
        public VendingButton SlowButton;
        public VendingButton GrowButton;
        public VendingButton ShrinkButton;
        public VendingButton GravityButton;
        public VendingButton LaunchButton;        
        public VendingButton ShowButton;

        public Light PointLight;
        public int Order;        

        public PhysicsDistorter BubbleSlow { get; set; }
        public PhysicsDistorter BubbleGrow { get; set; }
        public PhysicsDistorter BubbleShrink { get; set; }
        public PhysicsDistorter BubbleGravity { get; set; }
        public PhysicsDistorter BubbleLaunch { get; set; }        
        public PhysicsDistorter BubbleShow { get; set; }

        public DateTime LastButtonPressed { get; set; }        

        public List<GameObject> MyBubbles { get; set; }
        

        public List<PhysicsObject> MyPhysicsObjects;

        public List<HintArea> MyHintAreas;
        private int _startStockSlowLevel;
        private int _startStockGrowLevel;
        private int _startStockShrinkLevel;
        private int _startStockGravityLevel;
        private int _startStockLaunchLevel;
        private int _startStockCutOffLevel;
        private int _startStockShowLevel;

        private bool _firstButtonPress = true;

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
            BubbleShow = GameObject.FindGameObjectWithTag("BubbleShow").GetComponent<PhysicsDistorter>();

            _startStockSlowLevel = StockSlowLevel;
            _startStockGrowLevel = StockGrowLevel;
            _startStockShrinkLevel = StockShrinkLevel;
            _startStockGravityLevel = StockGravityLevel;
            _startStockLaunchLevel = StockLaunchLevel;
            _startStockShowLevel = StockShowLevel;

            InitStockLevels();

            LastButtonPressed = DateTime.Now;
            
            PhysicsManager.Instance.VendingMachines.Add(this);            
        }

        private void InitStockLevels()
        {
            _firstButtonPress = false;

            SetStockSlowLevel(_startStockSlowLevel);
            SetStockGrowLevel(_startStockGrowLevel);
            SetStockShrinkLevel(_startStockShrinkLevel);
            SetStockGravityLevel(_startStockGravityLevel);
            SetStockLaunchLevel(_startStockLaunchLevel);
            SetStockShowLevel(_startStockShowLevel);

            _firstButtonPress = true;
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

            foreach (HintArea area in MyHintAreas)
            {
                area.Reset();
            }

            InitStockLevels();

            _firstButtonPress = true;
        }

        public void SetStockSlowLevel(int stockLevel)
        {
            StockSlowLevel = stockLevel;
            SlowButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockGrowLevel(int stockLevel)
        {
            StockGrowLevel = stockLevel;
            GrowButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockShrinkLevel(int stockLevel)
        {
            StockShrinkLevel = stockLevel;
            ShrinkButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockGravityLevel(int stockLevel)
        {
            StockGravityLevel = stockLevel;
            GravityButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        public void SetStockLaunchLevel(int stockLevel)
        {
            StockLaunchLevel = stockLevel;
            LaunchButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }       

        public void SetStockShowLevel(int stockLevel)
        {
            StockShowLevel = stockLevel;
            ShowButton.SetStockLevel(stockLevel);
            CheckForKeyReset();
        }

        private void CheckForKeyReset()
        {
            if (_firstButtonPress)
            {
                foreach (PhysicsObject physicsObject in MyPhysicsObjects)
                {
                    KeyObject keyObject;
                    if (physicsObject.TryGetComponent<KeyObject>(out keyObject))
                    {
                        OutputLogManager.OutputText("Resetting key " + keyObject.name);
                        physicsObject.Reset();
                    }                    
                }

                _firstButtonPress = false;
            }
        }
    }
}
