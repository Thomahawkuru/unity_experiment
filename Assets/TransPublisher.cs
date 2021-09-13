using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class TransPublisher : UnityPublisher<MessageTypes.Geometry.TransformStamped> { 
        public Transform Handpalm;
        private string FrameID = "Unity";
        public string Side;
        private MessageTypes.Geometry.TransformStamped message;

        private void Start() {
            base.Start();
            InitializeMessage();
        }

        private void Update() {
            //Debug.Log(Handpalm.position.ToString());
            UpdateMessage();
        }
  
        private void InitializeMessage() {
            message = new MessageTypes.Geometry.TransformStamped();
            message.header.frame_id = FrameID;
            message.child_frame_id = Side;
            message.transform.translation.x = new double();
            message.transform.translation.y = new double();
            message.transform.translation.z = new double();
            message.transform.rotation.x = new double();
            message.transform.rotation.y = new double();
            message.transform.rotation.z = new double();
            message.transform.rotation.w = new double();
        }

        private void UpdateMessage() {
            //Debug.Log(Handpalm.position.x.ToString());
            message.header.Update();

            message.transform.translation.x = Handpalm.position.x;
            message.transform.translation.y = Handpalm.position.y;
            message.transform.translation.z = Handpalm.position.z;

            message.transform.rotation.x = Handpalm.rotation.eulerAngles.x;
            message.transform.rotation.y = Handpalm.rotation.eulerAngles.y;
            message.transform.rotation.z = Handpalm.rotation.eulerAngles.z;
            message.transform.rotation.w = Handpalm.localScale.magnitude;

            Publish(message);
        }
    }
}
