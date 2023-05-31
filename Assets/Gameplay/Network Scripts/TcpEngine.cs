using System;
using System.Net.Sockets;
using System.Threading;

using UnityEngine;

public class TcpEngine : MonoBehaviour
{
    private Thread thread;
    private TcpClient client;

    private string tcpHost;
    private int tcpPort;

    internal bool ConnectBlocking(string host, int port)
    {
        tcpHost = host;
        tcpPort = port;

        try
        {
            client = new TcpClient(host, port);

            return true;
        }
        catch //(Exception ex)
        {
            return false;
        }
    }



    internal void ConnectNotBlocking(string host, int port)
    {
        tcpHost = host;
        tcpPort = port;

        Thread t = new Thread(new ThreadStart(connect));
        t.IsBackground = true;
        t.Start();
    }
    private void connect()
    {

        try
        {
            client = new TcpClient(tcpHost, tcpPort);

            ConnectionResolve(true);
        }
        catch //(Exception ex)
        {
            ConnectionResolve(false);
        }
    }



    internal void Refresh()
    {
        while (client != null)
        {
            Byte[] bytes = new Byte[8192];

            using (NetworkStream stream = client.GetStream())
            {
                int length;

                while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    byte[] data = new byte[length];

                    Array.Copy(bytes, 0, data, 0, length);

                    Packet(data);
                }
            }
        }
    }

    internal void Disconnect()
    {
        if (client != null)
        {
            client.GetStream().Close();
            client.Close();
            print("Disconnected");
        }
    }


    // Override this method
    public virtual void Packet(byte[] data)
    {
        Debug.LogError("Packet method not overriden");
    }

    // Override this method
    public virtual void ConnectionResolve(bool success)
    {
        Debug.LogError("ConnectionResolve method not overriden");
    }



    internal void Send(byte[] data)
    {

        if (client == null)
        {
            return;
        }
        else
        {
            try
            {
                NetworkStream ns = client.GetStream();

                if (ns.CanWrite)
                {
                    ns.Write(data, 0, data.Length);
                }

            }
            catch (SocketException se)
            {
                Debug.LogError("SE:" + se);
            }
        }
    }

}