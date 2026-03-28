using System.Collections.Generic;

namespace Quack.sh.Models;

public class Config
{
    public List<ClientPreferences> ClientPreferences { get; set; } = new();
    public List<Connections> Connections { get; set; } = new();
}