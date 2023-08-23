using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class color_add : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnMouseDown()
    {
        if (GetComponent<Renderer>().material.color == Color.red)
        {
            GetComponent<Renderer>().material.color = Color.black;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
