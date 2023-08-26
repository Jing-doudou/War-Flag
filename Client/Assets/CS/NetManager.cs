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
        //判断是否为链接状态
        if (!clientSocket.Connected)
        {
            return "";
        }
        return clientSocket.LocalEndPoint.ToString();
    }
    /// <summary>
    /// 链接sock
    /// </summary> 
    public static void Connect(string ip, int port)
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //链接
        clientSocket.Connect(ip, port);
        //异步接收
        clientSocket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, clientSocket);
    }
    /// <summary>
    /// 接受的回调函数
    /// </summary>
    private static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);//消息长度
            string recvStr = Encoding.UTF8.GetString(readBuff, 0, count);//将缓冲池中的信息进行解码
            msglist.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);//异步接收内开启异步接收
        }
        catch (SocketException ex)
        {
            Debug.Log("连接失败");
        }
    }
    /// <summary>
    /// 发送消息
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
        //当前sockect不为空 且 处理连接状态
        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);//字符串转换二进制
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
