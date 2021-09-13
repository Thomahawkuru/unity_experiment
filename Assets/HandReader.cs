using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class HandReader : MonoBehaviour
    {
        Controller controller;

        private double LGrab;
        private double LPinch;
        private double RGrab;
        private double RPinch;

        public void Start()
        {
            controller = new Controller();
        }

        public void Update()
        {
            Frame frame = controller.Frame(); // controller is a Controller object

            if (frame.Hands.Count > 0)
            {
                List<Hand> hands = frame.Hands;
                
                foreach (Hand hand in hands)
                {                    
                    if (hand.IsLeft)
                    {
                        LGrab = hand.GrabStrength;
                        LPinch = hand.PinchStrength;                     
                    }                  
                    else
                    {
                        RGrab = hand.GrabStrength;
                        RPinch = hand.PinchStrength;
                    }
                }
            }
        }
        public double[] Read()
        {
            double[] HandData = new double[4];
            HandData[0] = LGrab;
            HandData[1] = LPinch;
            HandData[2] = RGrab;
            HandData[3] = RPinch;

            return HandData;
        }
    }
}
