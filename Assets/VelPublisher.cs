using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class VelPublisher : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        Controller controller;
        public string Side;
        public double Grab;
        public double x;
        public double y;
        public double z;

        private MessageTypes.Geometry.TwistStamped message;
        private Vector palmVelocity;

        private void Start()
        {
            controller = new Controller();
            base.Start();
            InitializeMessage();
        }

        private void Update()
        {
            Frame frame = controller.Frame(); // controller is a Controller object

            if (frame.Hands.Count > 0)
            {
                List<Hand> hands = frame.Hands;

                foreach (Hand hand in hands)
                {
                    string handName = hand.IsLeft ? "Left" : "Right";

                    if (handName == Side && hand.GrabStrength > Grab)
                    {
                        palmVelocity = hand.PalmVelocity / 100; //UnityVectorExtension.ToVector3(hand.PalmVelocity);
                        //Debug.Log(Side + " velocity: " + palmVelocity.ToString());
                        Debug.Log(Side + " hand detected");
                    }
                    else
                    {
                        palmVelocity = Vector.Zero;
                        Debug.Log("No grab detected");
                    }
                }
            }
            else
            {
                palmVelocity = Vector.Zero;
                Debug.Log("No hands detected");
            }
            
            UpdateMessage();
        }
  
        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.TwistStamped();
            message.header.frame_id = Side;
            message.twist.linear.x = new double();
            message.twist.linear.y = new double();
            message.twist.linear.z = new double();
            message.twist.linear.x = new double();
            message.twist.linear.y = new double();
            message.twist.linear.z = new double();
        }

        private void UpdateMessage()
        {
            //Debug.Log(Handpalm.position.x.ToString());
            message.header.Update();

            message.twist.linear.x = palmVelocity[0] * -x;
            message.twist.linear.y = palmVelocity[1] * -y;
            message.twist.linear.z = palmVelocity[2] * -z;

            message.twist.angular.x = 0;
            message.twist.angular.y = 0;
            message.twist.angular.z = 0;

            Publish(message);
        }
    }
}
