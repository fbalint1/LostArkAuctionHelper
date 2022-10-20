using LostArkAuctionHelper.Helpers;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LostArkAuctionHelper
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler? PropertyChanged;

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
        TryCalculate();
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

    public MainWindow()
    {
      DataContext = this;
      _lowestPrice = "50";
      _partySize = "4";
      _endPrice = "0";

      InitializeComponent();
    }

    private void TryCalculate()
    {
      if (string.IsNullOrEmpty(_lowestPrice) || string.IsNullOrEmpty(PartySize))
      {
        return;
      }

      if (!_lowestPrice.TryParseInt(out var price) || !_partySize.TryParseInt(out var partySize))
      {
        return;
      }

      var actualItemValue = price * .95;

      var perPlayer = actualItemValue / partySize;

      var equalCutPrice = perPlayer * (partySize - 1);

      var equalCutBidBefore = equalCutPrice / 1.1;

      var bidAmount = equalCutPrice - equalCutBidBefore;

      EndPrice = Math.Round(equalCutBidBefore + (bidAmount * 0.25)).ToString();
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
    
  }
}