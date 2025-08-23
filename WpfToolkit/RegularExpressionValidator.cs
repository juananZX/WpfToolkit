namespace System.Windows.Controls
{
  public class RegularExpressionValidator : BaseValidator
  {
    #region Dependency properties
    public static readonly DependencyProperty PatternProperty = DependencyProperty.Register(nameof(Pattern), typeof(string), typeof(RegularExpressionValidator), new PropertyMetadata(string.Empty, OnPropertyChanged));
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the regular expression pattern to validate.
    /// Obtiene o establece el patrón de la expresión regular a validar.
    /// </summary>
    public string Pattern
    {
      get => $"{GetValue(PatternProperty)}";
      set => SetValue(PatternProperty, value);
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Validates the regular expression.
    /// Valida la expresión regular.
    /// </summary>
    public override void Validate() => SetCurrentValue(IsValidProperty, ValidateRegularExpression(Field, Pattern, IsMandatory));
    #endregion
  }
}
