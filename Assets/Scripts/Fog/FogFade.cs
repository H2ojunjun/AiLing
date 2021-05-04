using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFade : MonoBehaviour
{
    float tempTime;
    // Start is called before the first frame update
    void Start()
    {
        tempTime = 0;

        this.GetComponent<Renderer>().material.color = new Color(
            this.GetComponent<Renderer>().material.color.r,
            this.GetComponent<Renderer>().material.color.g,
            this.GetComponent<Renderer>().material.color.b,

            this.GetComponent<Renderer>().material.color.a);
    }

    // Update is called once per frame
    void Update()
    {
        if (tempTime < 1)
        {
            tempTime = tempTime + Time.deltaTime;
        }
        if (this.GetComponent<Renderer>().material.color.a <= 1)
        {
            this.GetComponent<Renderer>().material.color = new Color(
            this.GetComponent<Renderer>().material.color.r,
            this.GetComponent<Renderer>().material.color.g,
            this.GetComponent<Renderer>().material.color.b,

            this.GetComponent<Renderer>().material.color.a-tempTime/5000);
        }
    }
}
