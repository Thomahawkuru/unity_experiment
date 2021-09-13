using UnityEngine;
using System.Collections.Generic;
using Leap;

public class PosReader : MonoBehaviour
{
    Controller controller;
    public string Side;
    Vector PalmPosition;
    // Start is called before the first frame update
    public Vector Read()

    {
        controller = new Controller();

        // Update is called once per frame
        Frame frame = controller.Frame(); // controller is a Controller object
        if (frame.Hands.Count > 0)
        {
            List<Hand> hands = frame.Hands;
            foreach (Hand hand in hands)
            {
                string handName = hand.IsLeft ? "Left" : "Right";
                if (handName == Side)
                {
                    PalmPosition = hand.PalmPosition;
                    Debug.Log(Side + ": " + hand.PalmPosition.ToString());
                }
            }
        }
        return PalmPosition;
    }
}
