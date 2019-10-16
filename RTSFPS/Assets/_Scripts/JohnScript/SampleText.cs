using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;

public class SampleText : MonoBehaviour
{
    [DllImport("CNET.dll")]
    static extern int Add(int a, int b);
    [DllImport("CNET.dll")]
    static extern IntPtr CreateClient();      //Creates a client
    [DllImport("CNET.dll")]
    static extern void DeleteClient(IntPtr client);      //Destroys a client

    [DllImport("CNET.dll")]
    static extern void Connect(string str, IntPtr client);      //Connects to c++


    [DllImport("CNET.dll")]
    static extern void SendString(StringBuilder str, int len);      //this recieves from c++
    [DllImport("CNET.dll")]
    static extern void RecieveString(string str);   //this sends to c++

    Text text;
    IntPtr ClientModule;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<Text>();
        ClientModule = CreateClient();
        Connect("127.0.0.1", ClientModule);
    }

    // Update is called once per frame
    void Update()
    {


     /*   
        string x = "awfwv";
        RecieveString(x);

        StringBuilder str = new StringBuilder(256);
        SendString(str, str.Capacity);

        text.text = str.ToString();
        //text.text = SendString(str);
   
    */

    }

    private void OnDestroy()
    {
        DeleteClient(ClientModule);
    }
}
