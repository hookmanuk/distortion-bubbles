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
            if (ButtonEnabled && _vendingMachine.StockLevel > 0 && (DateTime.Now - _vendingMachine.LastButtonPressed).TotalMilliseconds > 500)
            {
                _vendingMachine.LastButtonPressed = DateTime.Now;
                GameObject bubbleClone;

                if (Type == DistorterType.Slow)
                {
                    bubbleClone = Instantiate(_vendingMachine.BubbleSlow.gameObject);
                }
                else if (Type == DistorterType.Grow)
                {
                    bubbleClone = Instantiate(_vendingMachine.BubbleGrow.gameObject);
                }
                else
                {
                    bubbleClone = Instantiate(_vendingMachine.BubbleShrink.gameObject);
                }
                
                bubbleClone.tag = "";
                bubbleClone.transform.position = new Vector3(_vendingMachine.transform.position.x - 0.46f, _vendingMachine.transform.position.y + 0.505f, _vendingMachine.transform.position.z + 0.049f);
                bubbleClone.transform.rotation = new Quaternion(120, 90, 0, 0);
                bubbleClone.GetComponent<Rigidbody>().useGravity = true;
                _vendingMachine.SetStockLevel(_vendingMachine.StockLevel - 1);

            }

            base.OnSelectEnter(interactor);
        }                      
    }    
}
