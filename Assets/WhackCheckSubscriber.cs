using UnityEngine;
using RosSharp.RosBridgeClient;
 
[RequireComponent(typeof(RosConnector))]
public class WhackCheckSubscriber : UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>
{
    private bool isMessageReceived;
    private static bool whacked;

    protected override void Start()
    {
		base.Start();
    }
    private void Update()
    {
        if (isMessageReceived)
            isMessageReceived = false;
    }

    protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Std.Bool msg)
    {
        whacked = msg.data;
        isMessageReceived = true;
    }

    public bool Whack()
    {        
        return whacked;
    }

}