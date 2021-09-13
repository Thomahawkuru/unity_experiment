using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Collider Check Starting");
    }

    //moves the Primitive 2 units a second in the forward direction
    void Update()
    {
    }

    //When the Primitive collides with the walls, it will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collission Enter");
    }

    //When the Primitive exits the collision, it will change Color
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Collission Exit");
    }
}
