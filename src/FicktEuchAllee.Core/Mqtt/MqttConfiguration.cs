namespace FicktEuchAllee.Core;

public class MqttConfiguration(string host, int port, string username, string password)
{
    public string Host => host;
    public int Port => port;
    public string Username => username;
    public string Password => password;
}
