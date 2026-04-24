using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using LostArkAuctionHelper.Helpers;

namespace LostArkAuctionHelper
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand BiasCommand { get; private set; }

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

    private int _bias;

    public int Bias
    {
      get { return _bias; }
      set
      {
        _bias = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bias)));
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

    private string _whiteMaterialAmount;

    public string WhiteMaterialAmount
    {
      get { return _whiteMaterialAmount; }
      set
      {
        _whiteMaterialAmount = value;
        RecalculateMaterials();
      }
    }

    private string _greenMaterialAmount;

    public string GreenMaterialAmount
    {
      get { return _greenMaterialAmount; }
      set
      {
        _greenMaterialAmount = value;
        RecalculateMaterials();
      }
    }

    private string _blueMaterialAmount;

    public string BlueMaterialAmount
    {
      get { return _blueMaterialAmount; }
      set
      {
        _blueMaterialAmount = value;
        RecalculateMaterials();
      }
    }

    public int CraftableAmount { get; set; }

    public int WhiteMaterialNeed { get; set; }

    public int GreenMaterialNeed { get; set; }

    public int BlueMaterialNeed { get; set; }

    public string WhiteMaterialConvert { get; set; }

    public string GreenMaterialConvert { get; set; }

    public MainWindow()
    {
      _bias = 75;
      _isEqualShare = false;
      _lowestPrice = "50";
      _partySize = "10";
      _endPrice = "0";
      _activePartySize = 4;
      _activePartyOption = 0;

      BiasCommand = new RelayCommand(o => Bias = int.Parse(o.ToString()));

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

      if (!_lowestPrice.TryParseInt(out var price))
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

        EndPrice = Math.Round(equalCutBidBefore + (bidAmount * (1 - (_bias / 100d)))).ToString();
      }
    }

    private const int WHITE_NEEDED = 112;
    private const int GREEN_NEEDED = 59;
    private const int BLUE_NEEDED = 43;

    private const double TOTAL_CONVERTED_BLUES = 61.4;
    // 100 -> 8 blues, 50 -> 8 blues
    // so 1 white -> .08 blues, 1 green -> .16 blues
    private const double WHITE_CONVERT_RATIO = .08;
    private const double GREEN_CONVERT_RATIO = .16;

    private int _whiteMaterialConvertNum;
    private int _greenMaterialConvertNum;

    private void RecalculateMaterials()
    {
      var whiteAmount = GetNumericValueFromString(WhiteMaterialAmount);
      var greenAmount = GetNumericValueFromString(GreenMaterialAmount);
      var blueAmount = GetNumericValueFromString(BlueMaterialAmount);

      if (whiteAmount == 0 || greenAmount == 0)
      {
        return;
      }

      var whiteAsBlue = whiteAmount * WHITE_CONVERT_RATIO;
      var greenAsBlue = greenAmount * GREEN_CONVERT_RATIO;

      var totalMaterialSum = whiteAsBlue + greenAsBlue + blueAmount;
      CraftableAmount = (int)Math.Floor(totalMaterialSum / TOTAL_CONVERTED_BLUES);

      WhiteMaterialNeed = CraftableAmount * WHITE_NEEDED;
      if (WhiteMaterialNeed > whiteAmount)
      {
        CraftableAmount = (int)Math.Floor(whiteAmount / (double)WHITE_NEEDED);
        WhiteMaterialNeed = CraftableAmount * WHITE_NEEDED;
      }

      GreenMaterialNeed = CraftableAmount * GREEN_NEEDED;
      if (GreenMaterialNeed > greenAmount)
      {
        CraftableAmount = (int)Math.Floor(greenAmount / (double)GREEN_NEEDED);
        WhiteMaterialNeed = CraftableAmount * WHITE_NEEDED;
        GreenMaterialNeed = CraftableAmount * GREEN_NEEDED;
      }

      BlueMaterialNeed = CraftableAmount * BLUE_NEEDED;

      if (BlueMaterialNeed > blueAmount)
      {
        var blueDifference = FindNextIntDivisibleByTen(BlueMaterialNeed - blueAmount);
        var notNeededWhite = whiteAmount - WhiteMaterialNeed;

        var blueDifferenceInWhite = FindNextIntDivisibleByHundred(blueDifference * 12.5d);
        if (blueDifferenceInWhite < notNeededWhite)
        {
          _whiteMaterialConvertNum = blueDifferenceInWhite / 100;
          WhiteMaterialConvert = $"{blueDifferenceInWhite}({_whiteMaterialConvertNum})";
          _greenMaterialConvertNum = 0;
          GreenMaterialConvert = "-";
        }
        else
        {
          _whiteMaterialConvertNum = (int)Math.Floor(notNeededWhite / 100d);
          WhiteMaterialConvert = $"{_whiteMaterialConvertNum * 100}({_whiteMaterialConvertNum})";

          var dustFromWhite = _whiteMaterialConvertNum * WHITE_CONVERT_RATIO;
          var dustNeededFromGreen = (blueDifference * 10) - dustFromWhite;

          _greenMaterialConvertNum = (int)Math.Ceiling(dustNeededFromGreen / 80d);
          GreenMaterialConvert = $"{_greenMaterialConvertNum * 50}({_greenMaterialConvertNum})";
        }
      }
      else
      {
        _whiteMaterialConvertNum = 0;
        _greenMaterialConvertNum = 0;
        WhiteMaterialConvert = "-";
        GreenMaterialConvert = "-";
      }

      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CraftableAmount)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WhiteMaterialNeed)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GreenMaterialNeed)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlueMaterialNeed)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WhiteMaterialConvert)));
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GreenMaterialConvert)));
    }

    private int GetNumericValueFromString(string value_) => int.TryParse(value_, out int intVal) ? intVal : 0;

    private int FindNextIntDivisibleByTen(int value_) => (int)(Math.Ceiling(value_ / 10d) * 10);

    private int FindNextIntDivisibleByHundred(double value_) => (int)(Math.Ceiling(value_ / 100d) * 100);

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

    private void GreenMaterialConvert_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => ClipboardUtil.WriteToClipboard(_greenMaterialConvertNum.ToString());

    private void WhiteMaterialConvert_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => ClipboardUtil.WriteToClipboard(_whiteMaterialConvertNum.ToString());
  }
}