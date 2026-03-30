using Avalonia.Controls;
using WebViewControl;

namespace Quack.sh.Models;

public class SshTab
{
    public WebView WebView { get; set; }
    public SshService SshService { get; set; }
    public TerminalBridge Bridge { get; set; }
    public TabItem TabItem { get; set; }
}