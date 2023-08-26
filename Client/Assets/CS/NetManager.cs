using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Text;

public class NetManager
{
    static Socket clientSocket;
    static byte[] readBuff = new byte[1024];
    public delegate void MsgListener(string str);
    static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    static List<string> msglist = new List<string>();
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }
    public static string GetDesc()
    {
        if (clientSocket == null)
        {
            return "";
        }
        //�ж��Ƿ�Ϊ����״̬
        if (!clientSocket.Connected)
        {
            return "";
        }
        return clientSocket.LocalEndPoint.ToString();
    }
    /// <summary>
    /// ����sock
    /// </summary> 
    public static void Connect(string ip, int port)
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //����
        clientSocket.Connect(ip, port);
        //�첽����
        clientSocket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, clientSocket);
    }
    /// <summary>
    /// ���ܵĻص�����
    /// </summary>
    private static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);//��Ϣ����
            string recvStr = Encoding.UTF8.GetString(readBuff, 0, count);//��������е���Ϣ���н���
            msglist.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);//�첽�����ڿ����첽����
        }
        catch (SocketException ex)
        {
            Debug.Log("����ʧ��");
        }
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public static void Send(string sendStr)
    {
        if (clientSocket == null)
        {
            return;
        }
        if (!clientSocket.Connected)
        {
            return;
        }
        //��ǰsockect��Ϊ�� �� ��������״̬
        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);//�ַ���ת��������
        clientSocket.Send(sendBytes);
    }
    public static void Updata()
    {
        if (msglist.Count == 0)
        {
            return;
        }
        string msgStr = msglist[0];
        msglist.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgAtgs = split[1];
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgAtgs);
        }
    }
}
