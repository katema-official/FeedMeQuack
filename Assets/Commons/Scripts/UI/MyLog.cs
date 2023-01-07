using UnityEngine;
using System.Collections;

public class MyLog : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue();

    void Start()
    {
        //Debug.Log("Log1");
        //Debug.Log("Log2");
        //Debug.Log("Log3");
        //Debug.Log("Log4");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;

        var min = myLogQueue.Count - 5;
        if (min < 0) min = 0;

        var c = myLogQueue.Count - 1;
        if (c < 0) c = 0;

        for (int i = c; i >= min;i--)
        {
            myLog += myLogQueue.ToArray()[i];
        }
    }

    void OnGUI()
    {
        GUILayout.Label(myLog);
    }
}