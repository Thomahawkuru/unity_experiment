using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;


namespace RosSharp.RosBridgeClient
{
    public class PoscmdPublisher2 : UnityPublisher<MessageTypes.Geometry.Transform>
    {
        Controller controller;
        public Transform Lpalm;
        public Transform Rpalm;
        public GameObject Screen;
        public float gain;
        public float Kp;
        public float Ki;
        public float Kd;
        public float offset;
        public float correction;

        private Transform empty;
        private MessageTypes.Geometry.Transform message;
        private bool Controlling;
        private float[] palmVelocity = new float[6];
        private string HandName;
        Vector3 ScreenPosition;

        private void Start()
        {
            controller = new Controller();
            base.Start();
            InitializeMessage();
        }

        private void Update()
        {
            Frame frame = controller.Frame();
            ScreenPosition = Screen.transform.position;

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
                            Publish(Lpalm, ScreenPosition);
                            break;
                        case "R":
                            Publish(Rpalm, ScreenPosition);
                            break; }
                }
                else { Publish(null); }

            }
            else { Publish(null); }
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.Transform();
            message.translation.x = new double();
            message.translation.y = new double();
            message.translation.z = new double();
            message.rotation.x = new double();
            message.rotation.y = new double();
            message.rotation.z = new double();
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
                        Debug.Log("Controlling is true!");
                    }
                }
            }
        }

        private void Publish(Transform T, Vector3 P)
        {
            message.translation.x = T.position.x - P.x;
            message.translation.x = T.position.y - P.y;
            message.translation.x = T.position.z + offset - P.z;
            message.rotation.x = T.rotation.x;
            message.rotation.y = T.rotation.y;
            message.rotation.z = T.rotation.z;

            Publish(message);
        }
    }
}

