using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using RosSharp.RosBridgeClient;

public class TransReader : MonoBehaviour
{
    public string Side;
    public Transform palm;

    public Transform Read()
    {
        return palm;
    }

        // Update is called once per frame
        void Update()
    {
        Debug.Log(Side + ": " + palm.position.x.ToString() + ", " + palm.position.y.ToString() + ", " + palm.position.z.ToString());
    } 
}
