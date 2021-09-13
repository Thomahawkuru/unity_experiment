using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;


namespace RosSharp.RosBridgeClient
{
    public class VelcmdPublisher : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        Controller controller;
        public double Pinch;
        public double gain;
        public float interval;
        
        public Transform Lpalm;
        public Transform Rpalm;
        private MessageTypes.Geometry.TwistStamped message;
        private bool ini;
        private bool Controlling;
        private Vector3 palmVelocity;
        private Vector3 palmVelOld;
        private Vector3 palmVelNew;
        private float[] Panda;
        private string handName;
        private Vector3 lastError;
        private Vector3 integral;
        private TransformSubscriber transformsubscriber;


        private void Start()
        {
            controller = new Controller();
            transformsubscriber = gameObject.AddComponent<TransformSubscriber>();
            base.Start();
            InitializeMessage();
        }

        private void Update()
        {
            Frame frame = controller.Frame(); // controller is a Controller object
            Panda = transformsubscriber.Transform();

            if (frame.Hands.Count > 0)
            {
                List<Hand> hands = frame.Hands;
                Controlling = false;

                foreach (Hand hand in hands)
                {
                    handName = hand.IsLeft ? "L" : "R";

                    switch (handName)
                    {
                        case "L":
                            if (hand.PinchStrength > Pinch) {
                                Controlling = true; }
                            break;
                        case "R":
                            if (hand.PinchStrength > Pinch) {
                                Controlling = true; }
                            break;
                    }
                    if (Controlling == true) {
                        break; }
                }

                if (Controlling == true)
                {
                    switch (handName) {
                        case "L":
                            CalculateVelocity(Lpalm);
                            break;
                        case "R":
                            CalculateVelocity(Rpalm);
                            break; }                        
                }
                else {palmVelocity = Vector3.zero;}
                
            }
            else {palmVelocity = Vector3.zero;}

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

        private void CalculateVelocity(Transform input)
        {
            if (ini == true)
            {
                palmVelNew = input.position;
                palmVelocity = Vector3.zero;
                ini = false;
            }
            else
            {
                palmVelOld = palmVelNew;
                palmVelNew = input.position;

                palmVelocity[0] = (palmVelNew.x - palmVelOld.x) / Time.deltaTime;
                palmVelocity[1] = (palmVelNew.y - palmVelOld.y) / Time.deltaTime;
            }

            var error_z = (float)0.5 - Panda[2];
            PID(error_z, Time.deltaTime, 2);
        }       
    
        private void PID(float error, float dt, int n)
        {
            var p = (float)40;
            var i = (float)0.5;
            var d = (float)0.5;

            float derivative = (error - lastError[n]) / dt;
            integral[n] += error * dt;
            lastError[n] = error;
            palmVelocity[n] = p * error + i * integral[n] + d * derivative;
        }

    private void UpdateMessage()
        {
            message.header.Update();

            message.twist.linear.x = palmVelocity[2];
            message.twist.linear.y = palmVelocity[0] * gain;
            message.twist.linear.z = palmVelocity[1] * gain;
            message.twist.angular.x = 0;
            message.twist.angular.y = 0;
            message.twist.angular.z = 0;

            Publish(message);
        }
    }
}
