using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Espamatica.WpfToolkit
{
  public class RegexTextBox : TextBox
  {
    #region Fields
    private readonly Brush currentBackground;
    #endregion

    #region Dependency properties
    public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(RegexTextBox), new PropertyMetadata(false, OnIsValidChanged));

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
      currentBackground = this.Background;
    }
    #endregion

    #region Propiedades públicas
    public bool IsValid 
    {
      get => (bool)GetValue(IsValidProperty);
      set { SetValue(IsValidProperty, value); }
    }

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
    private static void OnIsValidChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is RegexTextBox tb)
        tb.OnIsValidChanged(e);
    }

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

    private void OnIsValidChanged(DependencyPropertyChangedEventArgs e) => SetCurrentValue(BackgroundProperty, (bool)e.NewValue ? currentBackground : Brushes.LightCoral);

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
            SetValue(IsValidProperty, Regex.IsMatch(this.Text, this.Pattern));
        }
        catch
        {
          // Do nothing if the regular expression is invalid.
          // No hacer nada si la expresión regular no es válida.
        }
      }
      else
        SetValue(IsValidProperty, true);
    }
    #endregion
  }
}
