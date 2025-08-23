using System.Windows;

namespace Espamatica.WpfToolkit
{
  public class RangeValidator : BaseValidator
  {
    #region Dependency property
    public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(object), typeof(RangeValidator), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(object), typeof(RangeValidator), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty TypeFieldProperty = DependencyProperty.Register(nameof(TypeField), typeof(string), typeof(RangeValidator), new PropertyMetadata(string.Empty, OnPropertyChanged));
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the maximum value to be validated.
    /// Obtiene o establece el valor máximo a validar.
    /// </summary>
    public object MaxValue
    {
      get => GetValue(MaxValueProperty);
      set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value to be validated.
    /// Obtiene o establece el valor mínimo a validar.
    /// </summary>
    public object MinValue
    {
      get => GetValue(MinValueProperty);
      set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the type to validate.
    /// Obtiene o establece el tipo a validar.
    /// </summary>
    public string TypeField
    {
      get => $"{GetValue(TypeFieldProperty)}";
      set => SetValue(TypeFieldProperty, value);
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Validate the range.
    /// Valida el rango.
    /// </summary>
    public override void Validate() => SetCurrentValue(IsValidProperty, ValidateRange(MinValue, MaxValue, Field, TypeField, IsMandatory));
    #endregion
  }
}
