using BubbleDistortionPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintArea : MonoBehaviour
{
    public DistorterType DistorterType;
    private PhysicsDistorter Distorter;
    private Material _material;

    private void Awake()
    {
        _material = gameObject.GetComponent<MeshRenderer>().material;
    }

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
                //gameObject.SetActive(false);
                DisableGlow();
            }
        }
    }

    public void EnableGlow()
    {
        _material.SetFloat("GLOW_ALPHA", 0.5f);
    }

    public void DisableGlow()
    {
        _material.SetFloat("GLOW_ALPHA", 0f);
    }

    public void Reset()
    {
        DisableGlow();
    }
}
