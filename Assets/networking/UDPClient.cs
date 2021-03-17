using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading;

public class UDPClient : MonoBehaviour
{
    public string ipToListen;
    public int port;

    public IPEndPoint ipep;

    public UdpClient Client;

    public Thread networkThread;

    private byte[] receivedData;
    private string dataString;
    public string nickName;
    public string pass;
    public string secondPlayer;
    public int room;

    void Start()
    {
        receivedData = new byte[0];
        string[] args = Environment.GetCommandLineArgs();

        Client = new UdpClient(0);

        try
        {
            Client.Connect(IPAddress.Parse(ipToListen), port);
        }
        catch (SocketException e)
        {
            Debug.Log(e);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("Declined");
            Application.Quit();
#endif
        }
        catch (Exception err)
        {
            Debug.Log(err);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Log("Declined");
            Application.Quit();
#endif
        }

        // pass should be already received hashed by launcher

#if UNITY_EDITOR
    nickName = "islan";
    pass = "puissant";
#else
        nickName = args[1];
        pass = args[2];
#endif
    }

    public string ReceiveData()
    {
        if (Client.Available > 0)
        {
            try
            {
                receivedData = Client.Receive(ref ipep);
                dataString = Encoding.ASCII.GetString(receivedData).Trim();
                dataString = parseIt(dataString);
                return dataString;
            }
            catch (Exception err)
            {
                print(err.ToString());
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public bool SendData(string opcode)
    {
        try
        {
            opcode += "0x12" + '\n';
            byte[] data = Encoding.UTF8.GetBytes(opcode);
            Client.Send(data, data.Length);
            return true;
        }
        catch (Exception err)
        {
            print(err.ToString());
            return false;
        }
    }

    private string parseIt(string dataString)
    {
        string[] isValidCommand;

        if (dataString.Contains("0x12"))
        {
            isValidCommand = dataString.Split(new[] { "0x12" }, StringSplitOptions.None);
            return isValidCommand[0];
        }
        return null;
    }

    void OnApplicationQuit()
    {
        SendData("S_LOGOUT:" + nickName);
        Client.Close();
    }
}