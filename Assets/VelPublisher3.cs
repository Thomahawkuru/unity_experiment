using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class VelPublisher3 : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        Controller controller;
        public string Side;
        public double Pinch;
        public double gain;
        public Transform palm;
        public float interval;
        private MessageTypes.Geometry.TwistStamped message;
        private bool ini;
        private Vector3 palmVelocity;
        private Vector3 palmVelOld;
        private Vector3 palmVelNew;


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

                    if (handName == Side && hand.PinchStrength > Pinch)
                    {
                        if (ini == true)
                        {
                            palmVelNew = palm.position;
                            palmVelocity = Vector3.zero;
                            ini = false;
                        }
                        else
                        {
                            palmVelOld = palmVelNew;
                            palmVelNew = palm.position;

                            palmVelocity[0] = (palmVelNew.z - palmVelOld.z) / Time.deltaTime;
                            palmVelocity[1] = (palmVelNew.x - palmVelOld.x) / Time.deltaTime;
                            palmVelocity[2] = (palmVelNew.y - palmVelOld.y) / Time.deltaTime;
                        }
                    }
                    else if (handName == Side)
                    {
                        palmVelocity = Vector3.zero;
                    }
                }
            }
            else
            {
                palmVelocity = Vector3.zero;
            }

            UpdateMessage();
        }
  
        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.TwistStamped();
            message.twist.linear.x = new double();
            message.twist.linear.y = new double();
            message.twist.linear.z = new double();
            message.twist.linear.x = new double();
            message.twist.linear.y = new double();
            message.twist.linear.z = new double();
        }

        private void UpdateMessage()
        {
            message.header.Update();

            message.twist.linear.x = -palmVelocity[0] * gain;
            message.twist.linear.y = palmVelocity[1] * gain;
            message.twist.linear.z = palmVelocity[2] * gain;
            message.twist.angular.x = 0;
            message.twist.angular.y = 0;
            message.twist.angular.z = 0;

            Publish(message);
        }
    }
}
