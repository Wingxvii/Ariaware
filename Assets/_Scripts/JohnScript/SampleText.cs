using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
public class SampleText : MonoBehaviour
{
    [DllImport("CNET.dll")]
    static extern int Add(int a, int b);

    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Add(4,2).ToString();
    }
}
