using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace LostArkAuctionHelper
{
  public class ApplicationEntryPoint
  {
    private const int HWND_BROADCAST = 0xffff;

    public static readonly int WM_SHOWME = RegisterWindowMessage("LOST_ARK_AUCTION_HELPER_WINDOWSHOW");
    private static Mutex _mutex = new Mutex(true, "LOST_ARK_AUCTION_HELPER_MUTEX");

    [STAThread]
    public static void Main(string[] args)
    {
      if (_mutex.WaitOne(TimeSpan.Zero, true))
      {
        var app = new App();
        app.InitializeComponent();
        app.Run();
        _mutex.ReleaseMutex();
      }
      else
      {
        PostMessage((IntPtr)HWND_BROADCAST, WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
      }
    }

    [DllImport("user32")]
    private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    [DllImport("user32")]
    private static extern int RegisterWindowMessage(string message);
  }
}
