using LostArkAuctionHelper.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace LostArkAuctionHelper
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool _isEqualShare;

    public bool IsEqualShare
    {
      get { return _isEqualShare; }
      set
      {
        _isEqualShare = value;
        TryCalculate();
      }
    }

    private string _bias;

    public string Bias
    {
      get { return _bias; }
      set
      {
        _bias = value;
        TryCalculate();
      }
    }

    private string _lowestPrice;

    public string LowestPrice
    {
      get { return _lowestPrice; }
      set
      {
        _lowestPrice = value;
        TryCalculate();
      }
    }

    private string _partySize;

    public string PartySize
    {
      get { return _partySize; }
      set
      {
        _partySize = value;
        if (int.TryParse(_partySize, out var parsedSize))
        {
          _activePartySize = parsedSize;
          TryCalculate();
        }
      }
    }

    private string _endPrice;

    public string EndPrice
    {
      get { return _endPrice; }
      set
      {
        _endPrice = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndPrice)));
      }
    }

    private int _activePartySize;
    private int _activePartyOption;

    public int ActivePartyOption
    {
      get { return _activePartyOption; }
      set
      {
        _activePartyOption = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActivePartyOption)));
      }
    }

    public MainWindow()
    {
      _bias = "25";
      _isEqualShare = false;
      _lowestPrice = "50";
      _partySize = "10";
      _endPrice = "0";
      _activePartySize = 4;
      _activePartyOption = 0;

      DataContext = this;
      InitializeComponent();

      TryCalculate();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
      hwndSource.AddHook(WndProc);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lparam, ref bool handled)
    {
      if (msg == ApplicationEntryPoint.WM_SHOWME)
      {
        if (WindowState == WindowState.Minimized)
        {
          WindowState = WindowState.Normal;
        }
        else
        {
          Topmost = true;
          Keyboard.Focus(PriceTextBox);
          Topmost = false;
        }
      }

      return IntPtr.Zero;
    }

    private void TryCalculate()
    {
      if (string.IsNullOrEmpty(_lowestPrice) || string.IsNullOrEmpty(PartySize))
      {
        return;
      }

      if (!_lowestPrice.TryParseInt(out var price)
        || !_bias.TryParseInt(out var bias))
      {
        return;
      }

      var actualItemValue = price * .95;

      var perPlayer = actualItemValue / _activePartySize;

      var equalCutPrice = perPlayer * (_activePartySize - 1);

      if (_isEqualShare)
      {
        EndPrice = Math.Round(equalCutPrice).ToString();
      }
      else
      {
        var equalCutBidBefore = equalCutPrice / 1.1;

        var bidAmount = equalCutPrice - equalCutBidBefore;

        EndPrice = Math.Round(equalCutBidBefore + (bidAmount * (bias / 100d))).ToString();
      }
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      var keyCode = (int)e.Key;

      if (!(keyCode >= 34 && keyCode <= 43) && !(keyCode >= 74 && keyCode <= 83) && keyCode != 3)
      {
        e.Handled = true;
      }
    }

    private void PriceTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      if (sender is TextBox textBox)
      {
        textBox.SelectAll();
      }
    }

    private void Grid_MouseDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => ClipboardUtil.WriteToClipboard(EndPrice ?? string.Empty);

    private void Button_Click(object sender, RoutedEventArgs e) => Close();

    private void Button_Click_0(object sender, RoutedEventArgs e)
    {
      ActivePartyOption = 0;
      _activePartySize = 4;
      TryCalculate();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      ActivePartyOption = 1;
      _activePartySize = 8;
      TryCalculate();
    }

    private void PartySizeTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      if (sender is TextBox textBox)
      {
        textBox.SelectAll();
        if (int.TryParse(_partySize, out var parsedSize))
        {
          _activePartySize = parsedSize;
          ActivePartyOption = 2;
          TryCalculate();
        }
      }
    }

    private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
    {
      var keyCode = (int)e.Key;

      if (!(keyCode >= 34 && keyCode <= 43) && !(keyCode >= 74 && keyCode <= 83) && keyCode != 3)
      {
        e.Handled = true;
      }
      else
      {
        ActivePartyOption = 2;
      }
    }
  }
}