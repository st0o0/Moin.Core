namespace FicktEuchAllee.Core;

/// <summary>
/// </summary>
/// <param name="host"></param>
/// <param name="port"></param>
/// <param name="username"></param>
/// <param name="password"></param>
public class MqttConfiguration(string host, int port, string username, string password)
{
    /// <summary>
    /// </summary>
    public string Host => host;
 
    /// <summary>
    /// </summary>
    public int Port => port;
 
    /// <summary>
    /// </summary>
    public string Username => username;

    /// <summary>
    /// </summary>
    public string Password => password;
}
