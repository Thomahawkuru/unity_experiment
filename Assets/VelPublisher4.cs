using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class VelPublisher4 : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        Controller controller;
        public string Side;
        public double Grab;
        public double gain;
        public Transform palm;
        public float interval;
        private MessageTypes.Geometry.TwistStamped message;
        private bool ini;
        private Vector3 palmVelocity;
        private Vector3 palmVelOld;
        private Vector3 palmVelNew;
        private Vector3 palmRotation;
        private Quaternion palmRotOld;
        private Quaternion palmRotNew;


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
                        if (ini == true)
                        {
                            palmVelNew = palm.position;
                            palmVelocity = Vector3.zero;
                            palmRotation = Vector3.zero;
                            ini = false;
                        }
                        else
                        {
                            palmVelOld = palmVelNew;
                            palmRotOld = palmRotNew;
                            palmVelNew = palm.position;
                            palmRotNew = palm.rotation;

                            palmVelocity[0] = (palmVelNew.x - palmVelOld.x) / Time.deltaTime;
                            palmVelocity[1] = (palmVelNew.y - palmVelOld.y) / Time.deltaTime; 
                            palmVelocity[2] = (palmVelNew.z - palmVelOld.z) / Time.deltaTime;
                            palmRotation[0] = (palmRotNew.eulerAngles.x - palmRotOld.eulerAngles.x) / Time.deltaTime;
                            palmRotation[1] = -(palmRotNew.eulerAngles.y - palmRotOld.eulerAngles.y) / Time.deltaTime;
                            palmRotation[2] = (palmRotNew.eulerAngles.z - palmRotOld.eulerAngles.z) / Time.deltaTime;
                        }
                    }
                    else
                    {
                        palmVelocity = Vector3.zero;
                        palmRotation = Vector3.zero;
                        Debug.Log("No grab detected");
                    }
                }
            }
            else
            {
                palmVelocity = Vector3.zero;
                palmRotation = Vector3.zero;
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

            message.twist.linear.x = palmVelocity[0] * gain;
            message.twist.linear.y = (palmVelocity[1] + palmVelocity[2]) * gain;
            message.twist.linear.z = 0; // palmVelocity[2] * gain;

            message.twist.angular.x = palmRotation[2] / 2;
            message.twist.angular.y = palmRotation[1] / 2;
            message.twist.angular.z = palmRotation[0] / 2; 

            Publish(message);
        }
    }
}
