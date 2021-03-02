using Antilatency.DeviceNetwork;
using Antilatency.Integration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltTrackingNode : AltTracking
{
    public NodeHandle NodeHandle;

    public SocketSwitcher Switcher;
    public Text TrackingState;
    public Text Position;

    protected override void Awake()
    {
        if(Network != null){
            base.Awake();
        }
    }

    public void Setup(DeviceNetwork network, AltEnvironmentComponent environment, NodeHandle node){
        Network = network;
        Environment = environment;
        NodeHandle = node;

        Awake();
    }

    protected override NodeHandle GetAvailableTrackingNode()
    {
        if (NodeHandle != NodeHandle.Null && GetNativeNetwork().nodeGetStatus(NodeHandle) != NodeStatus.Invalid)
        {
            return NodeHandle;
        }
        return NodeHandle.Null;
    }

    protected override Pose GetPlacement()
    {
        return new Pose(Vector3.zero, Quaternion.identity);
    }

    public void StartTracking()
    {
        if (NodeHandle != _trackingNode) {
            StartTracking(NodeHandle);
        }
    }

    protected override void Update()
    {
        base.Update();

        Antilatency.Alt.Tracking.State trackingState;

        if (!GetTrackingState(out trackingState))
        {
            return;
        }

        var color = Color.red;

        if (trackingState.stability.stage == Antilatency.Alt.Tracking.Stage.Tracking3Dof)
        {
            color = Color.yellow;
        }
        else if (trackingState.stability.stage == Antilatency.Alt.Tracking.Stage.Tracking6Dof)
        {
            color = Color.green;
        }
        else if (trackingState.stability.stage == Antilatency.Alt.Tracking.Stage.TrackingBlind6Dof)
        {
            color = new Color(0, 0.75f, 1f);
        }

        TrackingState.text = string.Format("Tracking state: <color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">{0}</color>", trackingState.stability.stage);
        Position.text = $"<color=red>x:</color> {trackingState.pose.position.x.ToString("F2")} <color=green>y:</color> {trackingState.pose.position.y.ToString("F2")} <color=blue>z:</color> {trackingState.pose.position.z.ToString("F2")}";

        transform.localPosition = trackingState.pose.position;
        transform.localRotation = trackingState.pose.rotation;
    }
}