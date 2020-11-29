using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Linq;
using UnityEngine.UI;

/*
 * 테스트를 위한 파이썬 코드, 5초마다 아무거나 쏜다. 
 
import socket 
import time

HOST = '192.168.0.21'
PORT = 1756

client_socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM) 

client_socket.connect((HOST, PORT)) 



# 키보드로 입력한 문자열을 서버로 전송하고 

# 서버에서 에코되어 돌아오는 메시지를 받으면 화면에 출력합니다. 

# quit를 입력할 때 까지 반복합니다. 
while True: 
    print("good")
    time.sleep(5)
    message="1"
    client_socket.send(message.encode()) 


client_socket.close() 

*/

class ClientSocketThread
{
    private Socket handler;
    private string clientIP;

    public ClientSocketThread(Socket handler)
    {
        this.handler = handler;
        string address = handler.RemoteEndPoint.ToString(); // 123.123.123.123:12345 의 형태입니다.
        // array[0] 는 IP 이고, array[1] 은 Port 입니다.
        string[] array = address.Split(new char[] { ':' });
        clientIP = array[0];

    }

    public void networkCode()
    {

        byte[] bytes = null;
        try
        {
            // An incoming connection needs to be processed.
            while (true)
            {
                bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                // use bytes, bytesRec

                if (bytesRec <= 0)
                {
                    handler.Disconnect(true);
                    break;
                }

                if (bytesRec < bytes.Length)
                {
                    //Debug.Log("fired");
                    TriggerSocket.fireQueue.Enqueue(clientIP);

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


public class TriggerSocket : MonoBehaviour
{
    //thread safe 하지 않는데, 일단 되니까 넘어가자.
    public static Queue<string> fireQueue;
    Dictionary<string, PlayerMover> ip2playerTable;
    

    List<Thread> threadList;
    Thread mainThread;

    Socket listener;
    Socket handler;

    void Start()
    {
        Application.runInBackground = true;


        ip2playerTable = new Dictionary<string, PlayerMover>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // 같은 태그를 가진 Object들을 GameObject[] 형태로 반환

        foreach (GameObject player in players)
        {
            PlayerMover temp = player.GetComponent<PlayerMover>();
           //Debug.Log(temp.GUN_IP);
            ip2playerTable.Add(temp.GUN_IP, temp);
        }


        fireQueue = new Queue<string>();
        threadList = new List<Thread>();
        mainThread = new System.Threading.Thread(startServer);
        mainThread.IsBackground = true;
        mainThread.Start();


    }
    void Update()
    {
        while (fireQueue.Count > 0)
        {
            //Debug.Log(fireQueue.Dequeue());
            string ip = fireQueue.Dequeue();
            if (ip != null) {
                PlayerMover p = ip2playerTable[ip];
                    if (p != null)
                {
                    p.fire();
                }
                    
            }
        }
    }

    void startServer()
    {
        try
        {
            // host running the application.
            IPAddress ip = IPAddress.Parse("192.168.0.21");
            IPEndPoint localEndPoint = new IPEndPoint(ip, 7888);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.
            while (true)
            {
                // Program is suspended while waiting for an incoming connection.
                handler = listener.Accept();
                Debug.Log("Trigger : one Client is Connected");

                ClientSocketThread CSthread = new ClientSocketThread(handler);
                Thread socketThread = new System.Threading.Thread(CSthread.networkCode);
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
