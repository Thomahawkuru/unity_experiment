using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;


namespace RosSharp.RosBridgeClient
{
    public class PoscmdPublisher : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        // topic = /servo_server/delta_twist_cmds
        Controller controller;
        public Transform Lpalm; //L_index_b
        public Transform Rpalm; //R_index_b
        public GameObject Screen;
        public float gain=0.65F;        
        public float Kp=10F;
        public float Ki=0F;
        public float Kd=0F;
        public float tracking; // 0.5 - 0.3
        public float offset; // 0.4 - 0.16
         public float correction; // 0.1 - 0.16

        private MessageTypes.Geometry.TwistStamped message;
        private bool Controlling;
        private bool ini = true;
        private float[] palmVelocity = new float[3];
        private float[] PIDVelocity = new float[6];
        private string HandName;
        private float[] lastError = new float[6];
        private float[] integral = new float[6];
        private float[] Panda = new float[6];
        private Vector3 palmVelOld;
        private Vector3 palmVelNew;
        Vector3 ScreenPosition;
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
            Frame frame = controller.Frame();
            ScreenPosition = Screen.transform.position;
            Panda = transformsubscriber.Transform();

            if (frame.Hands.Count > 0)
            {
                List<Hand> hands = frame.Hands;
                Controlling = false;

                foreach (Hand hand in hands)
                {                    
                    HandName = hand.IsLeft ? "L" : "R";
                   
                    switch (HandName) {
                        case "L":
                            CheckInput(Lpalm, ScreenPosition);
                            break;
                        case "R":
                            CheckInput(Rpalm, ScreenPosition);
                            break; }

                    if (Controlling == true) {
                        break; }
                    
                }

                if (Controlling == true)
                {
                    switch (HandName) {
                        case "L":
                            CalculateVelocity(Lpalm, Panda, ScreenPosition, HandName);
                            break;
                        case "R":
                            CalculateVelocity(Rpalm, Panda, ScreenPosition, HandName);
                            break; }
                }
                else { PIDVelocity = new float[6]; palmVelocity = new float[3]; }

            }
            else { PIDVelocity = new float[6]; palmVelocity = new float[3]; }

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

        private void CheckInput(Transform trans, Vector3 p)
        {
            var s = 1.1;
            //screen size
            var sx = 0.6 * s;
            var sz = 0.35 * s;
            var sy = tracking * s;

            if (Math.Abs(trans.position.x - p.x) < sx / 2)
            {
                if (Math.Abs(trans.position.y - (p.y)) < sy / 2)
                {
                    if (Math.Abs(trans.position.z + offset - p.z) < sz / 2)
                    {
                        Controlling = true;
                        //Debug.Log("Controlling is true!");
                    }
                }
            }
        }

        private void CalculateVelocity(Transform palm, float[] panda, Vector3 screen,string side)
        {
            if (ini == true)
            {
                palmVelNew = palm.position;
                palmVelocity = new float[6];
                ini = false;
            }
            else
            {
                palmVelOld = palmVelNew;
                palmVelNew = palm.position;

                palmVelocity[0] = (palmVelNew.x - palmVelOld.x) / Time.deltaTime;
                palmVelocity[1] = (palmVelNew.z - palmVelOld.z) / Time.deltaTime;
                palmVelocity[2] = 0;// (palmVelNew.z - palmVelOld.z) / Time.deltaTime;

            }

            float[] error = new float[6];

            error[0] = palm.position.x + gain * panda[0] - screen[0];
            error[1] = correction - palm.position.z - gain * panda[1]; 
            error[2] = (float)0.5 - panda[2];
            error[3] = panda[3];
            error[4] = panda[4];
            error[5] = panda[5]-1;

            //Debug.Log(palm.rotation.z);

            PID(Kp, Ki, Kd, error[0], Time.deltaTime, 0);
            PID(Kp, Ki, Kd, error[1], Time.deltaTime, 1);
            PID(Kp, Ki, Kd, error[2], Time.deltaTime, 2);
            PID(Kp, Ki, Kd, error[3], Time.deltaTime, 3);
            PID(Kp, Ki, Kd, error[4], Time.deltaTime, 4);
            PID(Kp, Ki, Kd, error[5], Time.deltaTime, 5);

        }   

        private void PID(float P, float I, float D, float error, float dt, int n)
        {   
            float derivative = (error - lastError[n]) / dt;
            integral[n] += error * dt;
            lastError[n] = error;
            PIDVelocity[n] = (P * error + I * integral[n] + D * derivative);
            //palmVelocity[n] = Mathf.Clamp(P * error + I * integral[n] + D * derivative,-1,1); 
        }

        private void UpdateMessage()
        {
            message.header.Update();

            message.twist.linear.x = PIDVelocity[2] + palmVelocity[2] / gain; 
            message.twist.linear.y = -PIDVelocity[0] - palmVelocity[0] / gain;
            message.twist.linear.z = PIDVelocity[1] - palmVelocity[1] / gain;
            message.twist.angular.x = 0; // -palmVelocity[5];  //panda.x perpendicualr plane
            message.twist.angular.y = PIDVelocity[4];  //panda.y vertical plane
            message.twist.angular.z = -PIDVelocity[3];  //panda.z horizontal plane

            Publish(message);
        }
    }
}

