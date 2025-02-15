using UnityEngine;
using TMPro;

public class ConsoleToTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI logText; 
    private string logMessages = "";

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
        logMessages += logString + "\n";

        if (logMessages.Length > 5000)
        {
            logMessages = logMessages.Substring(logMessages.Length - 5000);
        }

        if (logText != null)
        {
            logText.text = logMessages;
        }
    }
}
