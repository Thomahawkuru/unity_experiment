using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform screen;
    public float f;
    private TransformSubscriber transformsubscriber;
    private Vector3 position;

    private void Start()
    {
        transformsubscriber = gameObject.AddComponent<TransformSubscriber>();
    }
    // Update is called once per frame
    void Update()
    {
        
        float[] panda = transformsubscriber.Transform();

        position[0] = f * panda[0] + screen.position.x;
        position[1] = f * panda[1] + screen.position.y - (float)0.25;
        position[2] = screen.position.z;

        transform.position = position;
    }
    
}
