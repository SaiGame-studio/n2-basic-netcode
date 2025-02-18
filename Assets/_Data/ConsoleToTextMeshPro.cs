using UnityEngine;
using TMPro;

public class ConsoleToTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI logText;
    public int maxText = 1000;
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

        if (logMessages.Length > this.maxText)
        {
            logMessages = logMessages.Substring(logMessages.Length - this.maxText);
        }

        if (logText != null)
        {
            logText.text = logMessages;
        }
    }
}
