namespace System.Windows.Controls
{
  public class ComparisonValidator : BaseValidator
  {
    #region Dependency property
    public static readonly DependencyProperty ComparisonOperatorProperty = DependencyProperty.Register(nameof(ComparisonOperator), typeof(ComparisonOperatorEnum), typeof(ComparisonValidator), new PropertyMetadata(ComparisonOperatorEnum.Equal, OnPropertyChanged));

    public static readonly DependencyProperty Field2Property = DependencyProperty.Register(nameof(Field2), typeof(object), typeof(ComparisonValidator), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty TypeFieldProperty = DependencyProperty.Register(nameof(TypeField), typeof(string), typeof(ComparisonValidator), new PropertyMetadata(string.Empty, OnPropertyChanged));
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the comparison operator.
    /// Obtiene o establece el operador de la comparación.
    /// </summary>
    public ComparisonOperatorEnum ComparisonOperator
    {
      get => (ComparisonOperatorEnum)GetValue(ComparisonOperatorProperty);
      set => SetValue(ComparisonOperatorProperty, value);
    }

    /// <summary>
    /// Gets or sets the second field to validate.
    /// Obtiene o establece el campo 2 a validar.
    /// </summary>
    public object Field2
    {
      get => GetValue(Field2Property);
      set => SetValue(Field2Property, value);
    }

    /// <summary>
    /// Gets or sets the type to validate.
    /// Obtiene o establece el tipo a validar.
    /// </summary>
    public string TypeField
    {
      get => string.Format("{0}", GetValue(TypeFieldProperty));
      set => SetValue(TypeFieldProperty, value);
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Validate the range.
    /// Valida el rango.
    /// </summary>
    public override void Validate() => SetCurrentValue(IsValidProperty, ValidateComparison(Field, Field2, ComparisonOperator, TypeField, IsMandatory));
    #endregion
  }
}
