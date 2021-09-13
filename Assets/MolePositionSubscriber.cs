using UnityEngine;
using RosSharp.RosBridgeClient;
 
[RequireComponent(typeof(RosConnector))]
public class MolePositionSubscriber : UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.Float64>
{
    private bool isMessageReceived;
    private static double mole_position;

    protected override void Start()
    {
		base.Start();
    }
    private void Update()
    {
        if (isMessageReceived)
            isMessageReceived = false;
    }

    protected override void ReceiveMessage(RosSharp.RosBridgeClient.MessageTypes.Std.Float64 msg)
    {
        mole_position = msg.data;
        isMessageReceived = true;
    }

    public double Mole()
    {        
        return mole_position;
    }

}