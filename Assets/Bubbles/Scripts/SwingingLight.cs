using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingLight : MonoBehaviour
{
    public Rigidbody LightBulb;

    // Start is called before the first frame update
    void Start()
    {
        LightBulb.AddForce(Vector3.left * 5f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
