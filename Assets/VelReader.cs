using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class VelReader : MonoBehaviour
{
    Controller controller;
    public string Side;
    private Vector palmVelocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = new Controller();
    }

    // Update is called once per frame
    void Update()
    {
        Frame frame = controller.Frame(); // controller is a Controller object
        if (frame.Hands.Count > 0)
        {
            List<Hand> hands = frame.Hands;
            foreach (Hand hand in hands)
            {
                string handName = hand.IsLeft ? "Left" : "Right";
                if (handName == Side)
                {
                    palmVelocity = hand.PalmVelocity;
                    Debug.Log(Side + ": " + palmVelocity.ToString());
                }
            }
        }
    }
}
