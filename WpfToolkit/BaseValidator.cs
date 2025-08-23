using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace System.Windows.Controls
{
  public abstract class BaseValidator : TextBlock, INotifyPropertyChanged
  {
    #region Consts
    private const string CompareToMethod = "CompareTo";
    private const string TryParseMethod = "TryParse";
    #endregion

    #region Dependency properties
    public static readonly DependencyProperty FieldProperty = DependencyProperty.Register(nameof(Field), typeof(object), typeof(BaseValidator), new PropertyMetadata(null, OnPropertyChanged));

    public static readonly DependencyProperty HideOnValidateProperty = DependencyProperty.Register(nameof(HideOnValidate), typeof(bool), typeof(BaseValidator), new PropertyMetadata(true, OnPropertyChanged));

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(nameof(IsMandatory), typeof(bool), typeof(BaseValidator), new PropertyMetadata(true, OnPropertyChanged));

    public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(BaseValidator), new PropertyMetadata(false, OnIsValidChanged));
    #endregion

    #region Constructors
    static BaseValidator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseValidator), new FrameworkPropertyMetadata(typeof(BaseValidator)));
    }

    protected BaseValidator()
    {
      Initialized += (o, e) => { Validate(); };
      IsEnabledChanged += (o, e) => { Validate(); OnIsValidChanged(); };
    }
    #endregion

    #region Public Events
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Public properties
    /// <summary>
    /// Gets or sets the field to validate.
    /// Obtiene o establece el campo a validar.
    /// </summary>
    public object Field
    {
      get => GetValue(FieldProperty);
      set => SetValue(FieldProperty, value);
    }

    /// <summary>
    /// Gets or sets whether it is hidden when valid.
    /// Obtiene o establece si se oculta cuando es válido.
    /// </summary>
    public bool HideOnValidate
    {
      get => (bool)GetValue(HideOnValidateProperty);
      set => SetValue(HideOnValidateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the field to be validated is mandatory.
    /// Obtiene o establece si campo a validar es obligatorio.
    /// </summary>
    public bool IsMandatory
    {
      get => (bool)GetValue(IsMandatoryProperty);
      set => SetValue(IsMandatoryProperty, value);
    }

    /// <summary>
    /// Gets or sets whether validation is correct.
    /// Obtiene o establece si la validación es correcta.
    /// </summary>
    public bool IsValid
    {
      get => (bool)GetValue(IsValidProperty);
      set { SetValue(IsValidProperty, value); }
    }
    #endregion

    #region Public methods
    /// <summary>
    /// Validates the condition.
    /// Valida la condición.
    /// </summary>
    public abstract void Validate();
    #endregion

    #region Protected methods
    /// <summary>
    /// Callback for dependency properties.
    /// Callback para las propiedades de dependencia.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    protected static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      BaseValidator? validator = sender as BaseValidator;

      if (validator != default)
      {
        if (e.Property.Name == nameof(HideOnValidate))
        {
          if ((bool)e.NewValue == false)
            if (validator.Visibility != Visibility.Visible && validator.IsEnabled && validator.IsMandatory)
              validator.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            else
            if (validator.IsValid && validator.Visibility != Visibility.Collapsed)
              validator.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
        else
          validator.Validate();
      }
    }

    /// <summary>
    /// Validation of value comparison.
    /// Validación de comparación de valores.
    /// </summary>
    /// <param name="field1">
    /// Field 1
    /// Campo 1.
    /// </param>
    /// <param name="field2">
    /// Field 2.
    /// Campo 2.
    /// </param>
    /// <param name="comparisonOperator">
    /// Comparison operator.
    /// Operador de comparación.
    /// </param>
    /// <param name="type">
    /// Value type.
    /// Tipo de valor.
    /// </param>
    /// <param name="isMandatory">
    /// Specifies whether the field is mandatory.
    /// Especifica si el campo es obligatorio.
    /// </param>
    /// <returns>
    /// If the validation is correct, true; otherwise, false.
    /// Si la validación es correcta true, en caso contrario false.
    /// </returns>
    protected bool ValidateComparison(object field1, object field2, ComparisonOperator comparisonOperator, string type, bool isMandatory)
    {
      bool isEmpty = string.IsNullOrEmpty($"{field1}") || string.IsNullOrEmpty($"{field2}");

      if (!IsEnabled || (!isMandatory && isEmpty))
        return true;

      MethodInfo? method = null;
      Type? t = Type.GetType(type);

      bool isValidField1 = ValidateType(field1, type, true, out object? value2);
      bool isValidField2 = ValidateType(field2, type, true, out object? value1);
      bool isValidType = t != default;
      bool isValidValue1 = ValidateType(field1, type, true, out object? valueValid1);
      bool isValidValue2 = ValidateType(field2, type, true, out object? valueValid2);
      bool isValid = isValidField1 && isValidField2 && isValidType && isValidValue1 && isValidValue2;

      if (isValid)
      {
        method = t?.GetMethods().FirstOrDefault(i => i.Name.Equals(CompareToMethod));
        isValid = method != null;
      }

      if (isValid)
      {
        int? comparisonResult = (int?)method?.Invoke(value1, [value2]);

        switch (comparisonOperator)
        {
          case ComparisonOperator.Equal:
            isValid = comparisonResult == 0;
            break;
          case ComparisonOperator.EqualOrGraterThan:
            isValid = comparisonResult >= 0;
            break;
          case ComparisonOperator.EqualOrLessThan:
            isValid = comparisonResult <= 0;
            break;
          case ComparisonOperator.GreaterThan:
            isValid = comparisonResult > 0;
            break;
          case ComparisonOperator.LessThan:
            isValid = comparisonResult < 0;
            break;
          case ComparisonOperator.NotEqual:
            isValid = comparisonResult != 0;
            break;
          default:
            isValid = false;
            break;
        }
      }

      return isValid;
    }

    /// <summary>
    /// Field validation with content.
    /// Validación de campo con contenido.
    /// </summary>
    /// <param name="field">
    /// Field to be validated.
    /// Campo que se valida.
    /// </param>
    /// <param name="isMandatory">
    /// Specifies whether the field is mandatory.
    /// Especifica si el campo es obligatorio.
    /// </param>
    /// <returns>
    /// If the validation is correct, true; otherwise, false.
    /// Si la validación es correcta true, en caso contrario false.
    /// </returns>
    protected bool ValidateField(object field, bool isMandatory)
    {
      string fieldStr = $"{field}";
      bool isEmpty = string.IsNullOrEmpty(fieldStr);

      if (!IsEnabled || (!isMandatory && isEmpty))
        return true;
      else
        return !string.IsNullOrWhiteSpace(fieldStr);
    }

    /// <summary>
    /// Range validation.
    /// Validación de rangos.
    /// </summary>
    /// <param name="minValue">
    /// Minimum value.
    /// Valor mínimo.
    /// </param>
    /// <param name="maxValue">
    /// Miximum value.
    /// Valor máximo.
    /// </param>
    /// <param name="field">
    /// Field to be validated.
    /// Campo que se valida.
    /// </param>
    /// <param name="type">
    /// Value type.
    /// Tipo de valor.
    /// </param>
    /// <param name="isMandatory">
    /// Specifies whether the field is mandatory.
    /// Especifica si el campo es obligatorio.
    /// </param>
    /// <returns>
    /// If the validation is correct, true; otherwise, false.
    /// Si la validación es correcta true, en caso contrario false.
    /// </returns>
    protected bool ValidateRange(object minValue, object maxValue, object field, string type, bool isMandatory)
    {
      bool isEmpty = string.IsNullOrEmpty($"{field}");

      if (!IsEnabled || (!isMandatory && isEmpty))
        return true;

      MethodInfo? method = default;
      Type? t = Type.GetType(type);

      bool isValidMax = ValidateType(maxValue, type, true, out object? max);
      bool isValidMin = ValidateType(minValue, type, true, out object? min);
      bool isValidType = t != default;
      bool isValidValue = ValidateType(field, type, true, out object? val);
      bool isValid = isValidMax && isValidMin && isValidType && isValidValue;

      if (isValid)
      {
        method = t?.GetMethods().FirstOrDefault(i => i.Name.Equals(CompareToMethod));
        isValid = method != null;
      }

      if (isValid)
        isValid = ((int?)method?.Invoke(min, [val])) <= 0 && ((int?)method?.Invoke(max, [val])) >= 0;

      return isValid;
    }

    /// <summary>
    /// Validation of regular expressions.
    /// Validación de expresiones regulares.
    /// </summary>
    /// <param name="field">
    /// Field to be validated.
    /// Campo que se valida.
    /// </param>
    /// <param name="pattern">
    /// Regular expression pattern.
    /// Patrón de la expresión regular.
    /// </param>
    /// <param name="isMandatory">
    /// Specifies whether the field is mandatory.
    /// Especifica si el campo es obligatorio.
    /// </param>
    /// <returns>
    /// If the validation is correct, true; otherwise, false.
    /// Si la validación es correcta true, en caso contrario false.
    /// </returns>
    protected bool ValidateRegularExpression(object field, string pattern, bool isMandatory)
    {
      string fieldStr = $"{field}";
      bool isEmpty = string.IsNullOrEmpty(fieldStr);

      if (!IsEnabled || (!isMandatory && isEmpty))
        return true;

      Regex regex = new(pattern);
      bool isValid = ValidateField(field, true);

      if (isValid)
        isValid = regex.IsMatch(fieldStr);

      return isValid;
    }

    /// <summary>
    /// Type validation.
    /// Validación de tipo.
    /// </summary>
    /// <param name="field">
    /// Field to be validated.
    /// Campo que se valida.
    /// </param>
    /// <param name="type">
    /// Type to be validated.
    /// Tipo que se valida.
    /// </param>
    /// <param name="isMandatory">
    /// Specifies whether the field is mandatory.
    /// Especifica si el campo es obligatorio.
    /// </param>
    /// <returns>
    /// If the validation is correct, true; otherwise, false.
    /// Si la validación es correcta true, en caso contrario false.
    /// </returns>
    protected bool ValidateType(object field, string type, bool isMandatory) => ValidateType(field, type, isMandatory, out _);

    /// <summary>
    /// Type validation.
    /// Validación de tipo.
    /// </summary>
    /// <param name="field">
    /// Field to be validated.
    /// Campo que se valida.
    /// </param>
    /// <param name="type">
    /// Type to be validated.
    /// Tipo que se  valida.
    /// </param>
    /// <param name="isMandatory">
    /// Specifies whether the field is mandatory.
    /// Indica si es o no obligatorio.
    /// </param>
    /// <param name="value">
    /// Value converted to the specified type.
    /// Valor convertido al tipo especificado.
    /// </param>
    /// <returns>
    /// If the validation is correct, true; otherwise, false.
    /// Si la validación es correcta true, en caso contrario false.
    /// </returns>
    protected bool ValidateType(object field, string type, bool isMandatory, out object? value)
    {
      value = null;

      string fieldStr = $"{field}";
      bool isEmpty = string.IsNullOrEmpty(fieldStr);

      if (!IsEnabled || (!isMandatory && isEmpty))
        return true;

      Type? t = Type.GetType(type);
      bool isValid = t != default && ValidateField(field, true);

      if (isValid)
      {
        isValid = field.GetType().Equals(t);

        if (!isValid)
        {
          MethodInfo? method = t?.GetMethods().Where(i => i.Name.Equals(TryParseMethod) && i.GetParameters().LongLength == 2).FirstOrDefault();

          if (method != default)
          {
            value = t != default ? Activator.CreateInstance(t) : default;

            object[]? parameters = value != default ? [fieldStr, value] : default;

            isValid = parameters != default ? ((bool?)method?.Invoke(t, parameters)).GetValueOrDefault() : false;

            if (isValid)
              value = parameters?[1];
          }
        }
        else
          value = field;
      }

      return isValid;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Callback for the IsValid property.
    /// Callback para la propiedad IsValid.
    /// </summary>
    /// <param name="sender">
    /// Object that fires the event.
    /// Objeto que desencadena el evento.
    /// </param>
    /// <param name="e">
    /// Event details.
    /// Datos del evento.
    /// </param>
    private static void OnIsValidChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      BaseValidator? validator = sender as BaseValidator;
      if (validator != default)
        validator.OnIsValidChanged();
    }

    /// <summary>
    /// Notifica el cambio de valor en una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad de la que cambia el valor.</param>
    private void NotifyPropertyChanges(string propertyName)
    {
      if (PropertyChanged != default)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Callback for the IsValid property.
    /// Callback para la propiedad IsValid.
    /// </summary>
    private void OnIsValidChanged() => SetCurrentValue(VisibilityProperty, IsEnabled && (!IsValid || !HideOnValidate) ? Visibility.Visible : Visibility.Collapsed);
    #endregion
  }
}
