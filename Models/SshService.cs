using System;
using Renci.SshNet;
using Renci.SshNet.Common;
namespace Quack.sh.Models;

public class SshService
{
    public string RunCommand(string host, int port, string user, string password, string command)
    {
        using (var client = new SshClient(host, port, user, password))
        {
            client.Connect();
            using SshCommand cmd = client.RunCommand(command);
            return cmd.Result;
        }
    }
}