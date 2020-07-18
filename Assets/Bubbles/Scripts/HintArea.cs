using BubbleDistortionPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintArea : MonoBehaviour
{
    public DistorterType DistorterType;
    private PhysicsDistorter Distorter;

    private void OnTriggerEnter(Collider other)
    {
        PhysicsDistorter distorter;
        if (other.TryGetComponent(out distorter))
        {
            if (distorter.DistorterType == DistorterType && !distorter.Expanded)
            {
                Distorter = distorter;
                Distorter.TriggerSuccess();                
                //_material.SetFloat("GLOW_ALPHA", 0f);
                gameObject.SetActive(false);
            }
        }
    }

    public void Reset()
    {
        gameObject.SetActive(true);        
    }
}
