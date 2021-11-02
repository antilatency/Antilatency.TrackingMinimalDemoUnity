using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antilatency.SDK;
using Antilatency.DeviceNetwork;
using System.Linq;

public class AltTrackersSpawner : MonoBehaviour
{
    public DeviceNetwork Network;
    public AltEnvironment Environment;

    public GameObject TrackerObject;

    private INetwork _nativeNetwork;
    protected Antilatency.Alt.Tracking.ILibrary _trackingLibrary;
    private Dictionary<NodeHandle, AltTrackingNode> _trackers = new Dictionary<NodeHandle, AltTrackingNode>();

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        if (Network == null)
        {
            Debug.Log("Network is null");
            return;
        }

        Network.DeviceNetworkChanged.AddListener(OnDeviceNetworkChanged);

        OnDeviceNetworkChanged();
    }

    private void Init()
    {
        if (Network == null)
        {
            Debug.LogError("Network is null");
            return;
        }

        _nativeNetwork = GetNativeNetwork();

        _trackingLibrary = Antilatency.Alt.Tracking.Library.load();

        if (_trackingLibrary == null)
        {
            Debug.LogError("Failed to create tracking library");
            return;
        }
    }

    private void OnDeviceNetworkChanged()
    {
        var nodesToRemove = _trackers.Keys.Where(node => _nativeNetwork.nodeGetStatus(node) == NodeStatus.Invalid).ToArray();
        foreach (var item in nodesToRemove)
        {
            Destroy(_trackers[item].gameObject);
            _trackers.Remove(item);
        }

        var nodes = GetIdleTrackerNodes();

        foreach (var node in nodes)
        {
            if (!_trackers.ContainsKey(node))
            {
                var name = _nativeNetwork.nodeGetStringProperty(_nativeNetwork.nodeGetParent(node), Antilatency.DeviceNetwork.Interop.Constants.HardwareNameKey);

                var tracker = Instantiate(TrackerObject, transform);
                var trackingNode = tracker.GetComponent<AltTrackingNode>();
                trackingNode.Setup(Network, Environment, node);
                trackingNode.enabled = true;
                trackingNode.StartTracking();

                if (name.Contains("Socket"))
                {
                    trackingNode.Switcher.CurrentSocket = SocketSwitcher.Sockets.Socket;
                }
                else if (name.Contains("Bracer"))
                {
                    trackingNode.Switcher.CurrentSocket = SocketSwitcher.Sockets.Bracer;
                }
                else if (name.Contains("Tag"))
                {
                    trackingNode.Switcher.CurrentSocket = SocketSwitcher.Sockets.Tag;
                }

                _trackers.Add(node, trackingNode);
            }
        }
    }

    private NodeHandle[] GetIdleTrackerNodes()
    {
        var nativeNetwork = GetNativeNetwork();

        if (nativeNetwork == null)
        {
            return new NodeHandle[0];
        }

        using (var cotaskConstructor = _trackingLibrary.createTrackingCotaskConstructor())
        {
            var nodes = cotaskConstructor.findSupportedNodes(nativeNetwork).Where(v =>
                    nativeNetwork.nodeGetStatus(v) == NodeStatus.Idle
                ).ToArray();

            return nodes;
        }
    }

    protected INetwork GetNativeNetwork()
    {
        if (Network == null)
        {
            Debug.LogError("Network is null");
            return null;
        }

        if (Network.NativeNetwork == null)
        {
            Debug.LogError("Native network is null");
            return null;
        }

        return Network.NativeNetwork;
    }
}