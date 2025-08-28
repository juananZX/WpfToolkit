using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Espamatica.WpfToolkit
{
  public class NumericTextBox : TextBox
  {
    #region Constructors
    public NumericTextBox()
    {
      PreviewKeyDown += NumericTextBox_PreviewKeyDown;
      PreviewTextInput += NumericTextBox_PreviewTextInput;
      TextChanged += NumericTextBox_TextChanged;
    }
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the number of decimal places.
    /// Obtiene o establece el número de decimales.
    /// </summary>
    public byte Decimals { get; set; }

    /// <summary>
    /// Gets or sets the maximum value.
    /// Obtiene o establece el valor máximo.
    /// </summary>
    public decimal MaxValue { get; set; } = decimal.MaxValue;

    /// <summary>
    /// Gets or sets the minimum value.
    /// Obtiene o establece el valor mínimo.
    /// </summary>
    public decimal MinValue { get; set; } = decimal.MinValue;
    #endregion

    #region Private methods
    /// <summary>
    /// Gets whether it is a control key.
    /// Obtiene si es un tecla de control.
    /// </summary>
    /// <param name="key">
    /// Specified key.
    /// Tecla especificada.
    /// </param>
    /// <returns>
    /// If key is a key assigned to a control key, true; otherwise, false.
    /// Si key es una tecla asignada a una tecla de control true, en caso contario false.
    /// </returns>
    private static bool IsControlKey(Key key)
    {
      return key == Key.Delete || key == Key.Back ||
        key == Key.Left || key == Key.Right ||
        key == Key.Home || key == Key.End ||
        key == Key.OemFinish || key == Key.Tab ||
        key == Key.OemBackTab || key == Key.Return ||
        key == Key.Enter;
    }

    /// <summary>
    /// Gets whether Key is a key assigned to a digit.
    /// Obtiene si Key es una tecla asignada a un dígito.
    /// </summary>
    /// <param name="key">
    /// Specified key.
    /// Tecla especificada.
    /// </param>
    /// <returns>
    /// If key is a key assigned to a digit, true; otherwise, false.
    /// Si key es una tecla asignada a un dígito true, en caso contario false.
    /// </returns>
    private static bool IsDigitKey(Key key)
    {
      return key == Key.D0 || key == Key.NumPad0 ||
        key == Key.D1 || key == Key.NumPad1 ||
        key == Key.D2 || key == Key.NumPad2 ||
        key == Key.D3 || key == Key.NumPad3 ||
        key == Key.D4 || key == Key.NumPad4 ||
        key == Key.D5 || key == Key.NumPad5 ||
        key == Key.D6 || key == Key.NumPad6 ||
        key == Key.D7 || key == Key.NumPad7 ||
        key == Key.D8 || key == Key.NumPad8 ||
        key == Key.D9 || key == Key.NumPad9;
    }

    /// <summary>
    /// Gets whether Key is a key assigned to another valid key.
    /// Obtiene si Key es una tecla asignada a otra tecla válida.
    /// </summary>
    /// <param name="key">
    /// Specified key.
    /// Tecla especificada.
    /// </param>
    /// <returns>
    /// If key is a key assigned to another valid key, true; otherwise, false.
    /// Si key es una tecla asignada a otra tecla válida true, en caso contario false.
    /// </returns>
    private static bool IsOtherKey(Key key)
    {
      return key == Key.OemComma || key == Key.Decimal ||
        key == Key.OemPeriod ||
        key == Key.Subtract || key == Key.Add ||
        key == Key.OemMinus || key == Key.OemPlus;
    }
    #endregion

    #region Listeners
    private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (IsControlKey(e.Key)) return;
      e.Handled = !IsDigitKey(e.Key) && !IsOtherKey(e.Key);
      if (e.Handled) return;
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      char comma = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator[0];
      string value = Text.Insert(SelectionStart, e.Text.Replace('.', comma).Replace(',', comma));
      
      e.Handled = (decimal.TryParse(value, CultureInfo.CurrentUICulture, out decimal valD) == false)
        || valD < MinValue || valD > MaxValue;
      if (e.Handled) return;
      
      string[] parts = value.Split(comma);
      e.Handled = parts.Length > 1 && parts[1].Length > Decimals;
    }

    private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      int selStart = SelectionStart;
      char comma = CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator[0];
      string text = decimal.TryParse(Text.Replace('.', comma).Replace(',', comma), out _) ? Text.Replace('.', comma).Replace(',', comma) : string.Empty;
      SetCurrentValue(TextProperty, text);
      SelectionStart = selStart;
      e.Handled = true;
    }
    #endregion
  }
}
