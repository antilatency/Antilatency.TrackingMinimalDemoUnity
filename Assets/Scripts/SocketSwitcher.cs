using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SocketSwitcher : MonoBehaviour
{
    public enum Sockets
    {
        Socket,
        Tag,
        Bracer
    }

    public Sockets CurrentSocket
    {
        get { return _currentSocket; }
        set
        {
            _currentSocket = value;
            SwitchSocket();
        }
    }

    private Sockets _currentSocket = Sockets.Socket;

    private void SwitchSocket()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(child.name == _currentSocket.ToString());
    }
}