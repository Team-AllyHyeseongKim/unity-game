using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Linq;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

public struct rt_packet

{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public float[] data;
    public uint addr;

}


class SLAMSocketThread
{
    private Socket handler;


    public SLAMSocketThread(Socket handler)
    {
        this.handler = handler;
    }

    private T ByteToStruct<T>(byte[] buffer) where T : struct
    {
        int size = Marshal.SizeOf(typeof(T));
        if (size > buffer.Length) { throw new System.Exception(); }

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(buffer, 0, ptr, size);
        T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
        Marshal.FreeHGlobal(ptr);
        return obj;
    }


    public void networkCode()
    {

        byte[] bytes = null;
        try
        {
            // An incoming connection needs to be processed.
            while (true)
            {
                bytes = new byte[68];
                int bytesRec = 0;
                for (int i = 0; i < 68; i += bytesRec)
                {
                    bytesRec = handler.Receive(bytes, i, 68 - i, 0);
                }
                rt_packet packet = default(rt_packet);
                try
                {
                    packet = ByteToStruct<rt_packet>(bytes);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.ToString());
                    continue;
                }

                lock (MovingSocket.movingQueueLock)
                {
                    MovingSocket.MovingQueue.Enqueue(packet);
                }
                System.Threading.Thread.Sleep(1);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }

    }

}







public class MovingSocket : MonoBehaviour
{
    
    public static Queue<rt_packet> MovingQueue;
    public static readonly object movingQueueLock = new object();


    Dictionary<string, PlayerMover> ip2playerTable;
    Thread mainThread;
    List<Thread> threadList;


    // Start is called before the first frame update
    void Start()
    {
       
        ip2playerTable = new Dictionary<string, PlayerMover>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // 같은 태그를 가진 Object들을 GameObject[] 형태로 반환

        foreach (GameObject player in players)
        {
            PlayerMover temp = player.GetComponent<PlayerMover>();
            //Debug.Log(temp.GUN_IP);
            ip2playerTable.Add(temp.GUN_IP, temp);
        }

        MovingQueue = new Queue<rt_packet>();
        threadList = new List<Thread>();

        mainThread = new System.Threading.Thread(startServer);
        mainThread.IsBackground = true;
        mainThread.Start();

        

    }

    // Update is called once per frame
    void Update()
    {


        lock (MovingSocket.movingQueueLock)
        {

            while (MovingQueue.Count > 0)
            {

                rt_packet packet = MovingQueue.Dequeue();
                //Debug.Log(UInt32ToIPAddress(packet.addr));

                ip2playerTable[UInt32ToIPAddress(packet.addr)].move(packet.data);
            }
        }
         
    }

    public static string UInt32ToIPAddress(uint address)
    {
        byte[] ip = new byte[] {
                (byte)((address>>24) & 0xFF) ,
                (byte)((address>>16) & 0xFF) ,
                (byte)((address>>8)  & 0xFF) ,
                (byte)( address & 0xFF)};

        
        return ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3];
    }






    void startServer()
    {

        Socket listener;
        Socket handler = null;
        try
        {
            // host running the application.
            IPAddress ip = IPAddress.Parse("192.168.0.21");
            IPEndPoint localEndPoint = new IPEndPoint(ip, 6488);

            
            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            
            // Start listening for connections.
            while (true)
            {
                Debug.Log("Moving Socket : wait on...");
                handler = listener.Accept();
                Debug.Log("Moving Socket : one of SLAM Connected");

                SLAMSocketThread t = new SLAMSocketThread(handler);
                Thread socketThread = new System.Threading.Thread(t.networkCode);
                threadList.Add(socketThread);
                socketThread.IsBackground = true;
                socketThread.Start();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }

       



    }

    void stopServer()
    {
        if (mainThread != null)
        {
            mainThread.Abort();
        }
        //stop thread
        if (threadList != null)
        {
            foreach (Thread t in threadList)
            {
                t.Abort();
            }
        }

    }

    void OnDisable()
    {
        stopServer();
    }
}
