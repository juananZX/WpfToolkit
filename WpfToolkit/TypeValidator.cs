namespace System.Windows.Controls
{
  public class TypeValidator : BaseValidator
  {
    #region Dependency properties
    public static readonly DependencyProperty TypeFieldProperty = DependencyProperty.Register(nameof(TypeField), typeof(string), typeof(TypeValidator), new PropertyMetadata(string.Empty, OnPropertyChanged));
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the type to validate.
    /// Obtiene o establece el tipo a validar.
    /// </summary>
    public string TypeField
    {
      get => $"{GetValue(TypeFieldProperty)}";
      set => SetValue(TypeFieldProperty, value);
    } // TypeField
    #endregion

    #region Public methods
    /// <summary>
    /// Validates the type.
    /// Valida el tipo.
    /// </summary>
    public override void Validate() => SetCurrentValue(IsValidProperty, ValidateType(Field, TypeField, IsMandatory));
    #endregion
  }
}
