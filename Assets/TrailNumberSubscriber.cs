using UnityEngine;
using RosSharp.RosBridgeClient;
 
[RequireComponent(typeof(RosConnector))]
public class TrailNumberSubscriber : UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.Int64>
{
    private bool isMessageReceived;
    private static long trail_number;

    protected override void Start()
    {
		base.Start();
    }
    private void Update()
    {
        if (isMessageReceived)
            isMessageReceived = false;
    }

    protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Std.Int64 msg)
    {
        trail_number = msg.data;
        isMessageReceived = true;
    }

    public long Trail()
    {        
        return trail_number;
    }

}