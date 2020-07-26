using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        foreach (var item in GetComponentsInChildren<Renderer>())
        {
            item.materials[0].renderQueue = 3003;
            //item.materials[1].renderQueue = 3000;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
