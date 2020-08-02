using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionArrow : MonoBehaviour
{
    public float BlinkInterval = 1;

    private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            yield return new WaitForSeconds(BlinkInterval);

            _meshRenderer.enabled = false;

            yield return new WaitForSeconds(0.5f);

            _meshRenderer.enabled = true;
        }                   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
