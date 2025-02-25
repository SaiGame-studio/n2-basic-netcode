[System.Serializable]
public class ServerInfo
{
    public string ServerName;
    public string IPAddress;
    public int Port;

    public ServerInfo(string name, string ip, int port)
    {
        ServerName = name;
        IPAddress = ip;
        Port = port;
    }
}