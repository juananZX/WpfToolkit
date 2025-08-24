using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Espamatica.WpfToolkit
{
  public class RegexTextBox : TextBox
  {
    #region Dependency properties
    public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(nameof(Pattern), typeof(string), typeof(RegexTextBox), new PropertyMetadata(string.Empty, OnPatternChanged));
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes a new instance of the RegexTextBox class.
    /// Inicializa una nueva instancia de la clase RegexTextBox.
    /// </summary>
    public RegexTextBox() : base()
    {
      TextChanged += new TextChangedEventHandler(RegexTextBox_TextChanged);
    }
    #endregion

    #region Propiedades públicas
    /// <summary>
    /// Gets or sets the regular expression pattern to search for matches.
    /// Obtiene o establece el modelo de la expresión regular de la que van a buscarse coincidencias.
    /// </summary>
    public string Pattern
    {
      get => $"{GetValue(PatternProperty)}";
      set => SetValue(PatternProperty, value);
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Callback for the PatternProperty property.
    /// Callback para la propiedad PatternProperty.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnPatternChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is RegexTextBox tb)
        tb.OnPatternChanged(e);
    }

    /// <summary>
    /// TextChanged event handler.
    /// Controlador del evento TextChanged.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void RegexTextBox_TextChanged(object sender, TextChangedEventArgs e) => ValidateText();

    /// <summary>
    /// Callback for the PatternProperty property.
    /// Callback para la propiedad PatternProperty.
    /// </summary>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private void OnPatternChanged(DependencyPropertyChangedEventArgs e) => ValidateText();

    /// <summary>
    /// Validate the input text.
    /// Valida el texto introducido.
    /// </summary>
    private void ValidateText()
    {
      if (!string.IsNullOrWhiteSpace(this.Pattern) && !string.IsNullOrWhiteSpace(this.Text))
      {
        try
        {
          if (!Regex.IsMatch(this.Text, this.Pattern) && this.SelectionStart - 1 > -1)
          {
            int selectionStart = this.SelectionStart - 1;

            var normalized = this.Text.Remove(selectionStart, 1);

            this.SetValue(TextBox.TextProperty, normalized);

            this.SelectionStart = selectionStart;
          }
        }
        catch
        {
          // Do nothing if the regular expression is invalid.
          // No hacer nada si la expresión regular no es válida.
        }
      }
    }
    #endregion
  }
}
