using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;


namespace RosSharp.RosBridgeClient
{
    public class VelcmdPublisher : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {        
        Controller controller;      // topic = /servo_server/delta_twist_cmds
        public Transform Lhand;     //L_index_b
        public Transform Rhand;     //R_index_b
        public GameObject Screen;
        public float gain = 0.65F; 	//size factor between panda- and input-position      
        public float Kp = 10F; 	    // proportional gain factor for PID controller
        public float Ki = 0F;  	    // integral gain factor for PID controller
        public float Kd = 0F;   	// derivative gain factor for PID controller
        public float offset; 	    // input y-offset: separated: 0.4, situated: 0
        public float correction; 	// input y-correction: separated: 0.1, situated: 0.16


        private MessageTypes.Geometry.TwistStamped message;
        private bool Controlling;
        private bool ini = true;
        private float[] handVelocity = new float[3];
        private float[] PIDVelocity = new float[6];
        private string HandName;
        private float[] lastError = new float[6];
        private float[] integral = new float[6];
        private float[] Panda = new float[6];
        private Vector3 handVelOld;
        private Vector3 handVelNew;
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
                            CheckInput(Lhand, ScreenPosition);
                            break;
                        case "R":
                            CheckInput(Rhand, ScreenPosition);
                            break; }

                    if (Controlling == true) {
                        break; }
                    
                }

                if (Controlling == true)
                {
                    switch (HandName) {
                        case "L":
                            CalculateVelocity(Lhand, Panda, ScreenPosition, HandName);
                            break;
                        case "R":
                            CalculateVelocity(Rhand, Panda, ScreenPosition, HandName);
                            break; }
                }
                else { PIDVelocity = new float[6]; handVelocity = new float[3]; }

            }
            else { PIDVelocity = new float[6]; handVelocity = new float[3]; }

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
            var sy = 0.3 * s;

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

        private void CalculateVelocity(Transform hand, float[] panda, Vector3 screen,string side)
        {
            if (ini == true)
            {
                handVelNew = hand.position;
                handVelocity = new float[6];
                ini = false;
            }
            else
            {
                handVelOld = handVelNew;
                handVelNew = hand.position;

                handVelocity[0] = (handVelNew.x - handVelOld.x) / Time.deltaTime;
                handVelocity[1] = (handVelNew.z - handVelOld.z) / Time.deltaTime;
                handVelocity[2] = 0;// (handVelNew.z - handVelOld.z) / Time.deltaTime;

            }

            float[] error = new float[6];

            error[0] = hand.position.x + gain * panda[0] - screen[0];
            error[1] = correction - hand.position.z - gain * panda[1]; 
            error[2] = (float)0.5 - panda[2];
            error[3] = panda[3];
            error[4] = panda[4];
            error[5] = panda[5]-1;

            //Debug.Log(hand.rotation.z);

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
            //handVelocity[n] = Mathf.Clamp(P * error + I * integral[n] + D * derivative,-1,1); 
        }

        private void UpdateMessage()
        {
            message.header.Update();

            message.twist.linear.x = PIDVelocity[2] + handVelocity[2] / gain; 
            message.twist.linear.y = -PIDVelocity[0] - handVelocity[0] / gain;
            message.twist.linear.z = PIDVelocity[1] - handVelocity[1] / gain;
            message.twist.angular.x = 0; // -handVelocity[5];  //panda.x perpendicualr plane
            message.twist.angular.y = PIDVelocity[4];  //panda.y vertical plane
            message.twist.angular.z = -PIDVelocity[3];  //panda.z horizontal plane

            Publish(message);
        }
    }
}

