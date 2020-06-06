using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace BubbleDistortionPhysics
{
    public class VendingButton : XRSimpleInteractable
    {
        private VendingMachine _vendingMachine;
        public DistorterType Type;
        public bool ButtonEnabled;        

        protected override void Awake()
        {
            base.Awake();

            _vendingMachine = GetComponentInParent<VendingMachine>();

            if (!ButtonEnabled)
            {
                Color32 col = gameObject.GetComponent<MeshRenderer>().material.GetColor("_Color");
                col.a = 60;
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", col);
            }
        }

        protected override void OnHoverEnter(XRBaseInteractor interactor)
        {
            if (ButtonEnabled && (DateTime.Now - _vendingMachine.LastButtonPressed).TotalMilliseconds > 500)
            {
                _vendingMachine.LastButtonPressed = DateTime.Now;
                GameObject bubbleClone;

                if (Type == DistorterType.Slow)
                {
                    bubbleClone = Instantiate(_vendingMachine.BubbleSlow.gameObject);
                    _vendingMachine.SetStockSlowLevel(_vendingMachine.StockSlowLevel - 1);
                }
                else if (Type == DistorterType.Grow)
                {
                    bubbleClone = Instantiate(_vendingMachine.BubbleGrow.gameObject);
                    _vendingMachine.SetStockGrowLevel(_vendingMachine.StockGrowLevel - 1);
                }
                else
                {
                    bubbleClone = Instantiate(_vendingMachine.BubbleShrink.gameObject);
                    _vendingMachine.SetStockShrinkLevel(_vendingMachine.StockShrinkLevel - 1);
                }
                
                bubbleClone.tag = "Untagged";
                bubbleClone.transform.position = new Vector3(_vendingMachine.transform.position.x - 0.46f, _vendingMachine.transform.position.y + 0.505f, _vendingMachine.transform.position.z + 0.049f);
                bubbleClone.transform.rotation = new Quaternion(120, 90, 0, 0);
                bubbleClone.GetComponent<Rigidbody>().useGravity = true;
                bubbleClone.GetComponent<AudioSource>().Play();

                _vendingMachine.MyBubbles.Add(bubbleClone);                
            }

            base.OnSelectEnter(interactor);
        }                      
    }    
}
