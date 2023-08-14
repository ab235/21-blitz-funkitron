using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousedown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnMouseDown()
    {
        print("Hello from hint button");
    }
    private void OnMouseExit()
    {
        print("Hello from hint button entering");
    }
    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            print("w pressed");
        }
    }
}
