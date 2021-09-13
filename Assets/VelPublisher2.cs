using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class VelPublisher2 : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        Controller controller;
        public string Side;
        public Rigidbody handRB;
        private MessageTypes.Geometry.TwistStamped message;
        private Vector3 palmVelocity;

        private void Start()
        {
            controller = new Controller();
            base.Start();
            InitializeMessage();
        }

        private void Update()
        {
            Debug.Log("1" + handRB.transform.position.ToString());
            Debug.Log("2" + handRB.velocity.ToString());
            Frame frame = controller.Frame(); // controller is a Controller object
            if (frame.Hands.Count > 0)
            {
                List<Hand> hands = frame.Hands;
                foreach (Hand hand in hands)
                {
                    string handName = hand.IsLeft ? "Left" : "Right";
                    if (handName == Side)
                    {
                        palmVelocity = handRB.velocity;
                        Debug.Log("3"+ Side + " velocity: " + palmVelocity.ToString());
                    }
                }
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

            message.twist.linear.x = palmVelocity[0];
            message.twist.linear.y = palmVelocity[1];
            message.twist.linear.z = palmVelocity[2];

            message.twist.angular.x = 0;
            message.twist.angular.y = 0;
            message.twist.angular.z = 0;

            Publish(message);
        }
    }
}
