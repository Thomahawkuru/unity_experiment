using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class GripPublisher : UnityPublisher<MessageTypes.Std.Float64MultiArray>
    {
        Controller controller;
        public string Side;

        private MessageTypes.Std.Float64MultiArray message;
        private double gripper_position;
        private HandReader handreader;

        public void Start()
        {
            controller = new Controller();
            handreader = gameObject.AddComponent<HandReader>();
            base.Start();
            InitializeMessage();
        }

        public void Update()
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
                        double[]  HandData = handreader.Read();
                        gripper_position = HandData[2];
                    }                   
                }
            }
            
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Std.Float64MultiArray
            {
                data = new double[2]
            };
        }
        private void UpdateMessage()
        {
            message.data[0] = 0; // 0.04-gripper_position * 0.04;
            message.data[1] = 0; // 0.04-gripper_position * 0.04;

            Publish(message);
        }
    }
}
