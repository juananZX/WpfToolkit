using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace System.Windows.Controls
{
  public abstract class BaseValidator : TextBlock, INotifyPropertyChanged
  {
    #region Dependency properties
    public static readonly DependencyProperty FieldProperty = DependencyProperty.Register("Field", typeof(object), typeof(BaseValidator), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>
    /// Identifica la propiedad de dependencia HideOnValidate.
    /// </summary>
    public static readonly DependencyProperty HideOnValidateProperty = DependencyProperty.Register(
        "HideOnValidate",
        typeof(bool),
        typeof(BaseValidator),
        new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// Identifica la propiedad de dependencia IsMandatory.
    /// </summary>
    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        "IsMandatory",
        typeof(bool),
        typeof(BaseValidator),
        new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// Indentifica la propiedad de dependencia IsValid.
    /// </summary>
    public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(
        "IsValid",
        typeof(bool),
        typeof(BaseValidator),
        new PropertyMetadata(false, OnIsValidChanged));
    #endregion

    #region Constructors
    /// <summary>
    /// Contructor estático de la clase BaseValidator.
    /// </summary>
    static BaseValidator()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseValidator), new FrameworkPropertyMetadata(typeof(BaseValidator)));
    } // BaseValidator

    /// <summary>
    /// Constructor por defecto de la clase.
    /// </summary>
    protected BaseValidator()
    {
      this.Initialized += (o, e) => { this.Validate(); };
      this.IsEnabledChanged += (o, e) => { this.Validate(); this.OnIsValidChanged(); };
    } // BaseValidator
    #endregion

    #region Public Events
    /// <summary>
    /// Evento que notifica los cambios de valor de las propiedades.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion

    #region Public properties
    /// <summary>
    /// Obtiene o establece el campo a validar.
    /// </summary>
    public object Field
    {
      get { return this.GetValue(ThisClass.FieldProperty); }

      set { this.SetValue(ThisClass.FieldProperty, value); }
    } // Field

    /// <summary>
    /// Obtiene o establece si se oculta cuando es válido.
    /// </summary>
    public bool HideOnValidate
    {
      get { return (bool)this.GetValue(ThisClass.HideOnValidateProperty); }

      set { this.SetValue(ThisClass.HideOnValidateProperty, value); }
    } // HideOnValidate

    /// <summary>
    /// Obtiene o establece si campo a validar es obligatorio.
    /// </summary>
    public bool IsMandatory
    {
      get { return (bool)this.GetValue(ThisClass.IsMandatoryProperty); }

      set { this.SetValue(ThisClass.IsMandatoryProperty, value); }
    } // IsMandatory

    /// <summary>
    /// Obtiene o establece si la validación es correcta.
    /// </summary>
    public bool IsValid
    {
      get { return (bool)this.GetValue(ThisClass.IsValidProperty); }

      set { this.SetValue(ThisClass.IsValidProperty, value); }
    } // IsValid
    #endregion

    #region Métodos públicos
    /// <summary>
    /// Valida la condición.
    /// </summary>
    public abstract void Validate();
    #endregion

    #region Protected methods
    /// <summary>
    /// Callback para las propiedades de dependencia.
    /// </summary>
    /// <param name="sender">Objeto que desencadena el evento.</param>
    /// <param name="e">Datos del evento.</param>
    protected static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      BaseValidator validator = sender as BaseValidator;

      if (validator != null)
      {
        if (e.Property.Name == "HideOnValidate")
        {
          if ((bool)e.NewValue == false)
          {
            if (validator.Visibility != Visibility.Visible && validator.IsEnabled && validator.IsMandatory)
            {
              validator.SetCurrentValue(ThisClass.VisibilityProperty, Visibility.Visible);
            }
          }
          else
          {
            if (validator.IsValid && validator.Visibility != Visibility.Collapsed)
            {
              validator.SetCurrentValue(ThisClass.VisibilityProperty, Visibility.Collapsed);
            }
          }
        }
        else
        {
          validator.Validate();
        }
      }
    } // OnIsMadatoryChanged

    /// <summary>
    /// Validación de comparación de valores.
    /// </summary>
    /// <param name="field1">Valor 1.</param>
    /// <param name="field2">Valor 2.</param>
    /// <param name="comparisonOperator">Operador de comparación.</param>
    /// <param name="type">Tipo de valor.</param>
    /// <param name="isMandatory">Especifica si el campo es obligatorio.</param>
    /// <returns>Si la validación es correcta true, en caso contrario false.</returns>
    protected bool ValidateComparison(object field1, object field2, ComparisonOperatorEnum comparisonOperator, string type, bool isMandatory)
    {
      bool isEmpty = string.IsNullOrEmpty(string.Format("{0}", field1)) || string.IsNullOrEmpty(string.Format("{0}", field2));

      if (!this.IsEnabled || (!isMandatory && isEmpty))
      {
        return true;
      }

      object value1 = null, value2 = null, valueValid1 = null, valueValid2 = null;
      MethodInfo method = null;

      Type t = Type.GetType(type);

      bool isValidField1 = this.ValidateType(field1, type, true, out value2);
      bool isValidField2 = this.ValidateType(field2, type, true, out value1);
      bool isValidType = t != null;
      bool isValidValue1 = this.ValidateType(field1, type, true, out valueValid1);
      bool isValidValue2 = this.ValidateType(field2, type, true, out valueValid2);
      bool isValid = isValidField1 && isValidField2 && isValidType && isValidValue1 && isValidValue2;

      if (isValid)
      {
        method = t.GetMethods().FirstOrDefault(i => i.Name.Equals("CompareTo"));

        isValid = method != null;
      }

      if (isValid)
      {
        var comparisonResult = (int)method.Invoke(value1, new object[] { value2 });

        switch (comparisonOperator)
        {
          case ComparisonOperatorEnum.Equal:
            isValid = comparisonResult == 0;

            break;

          case ComparisonOperatorEnum.EqualOrGraterThan:
            isValid = comparisonResult >= 0;

            break;

          case ComparisonOperatorEnum.EqualOrLessThan:
            isValid = comparisonResult <= 0;

            break;

          case ComparisonOperatorEnum.GreaterThan:
            isValid = comparisonResult > 0;

            break;

          case ComparisonOperatorEnum.LessThan:
            isValid = comparisonResult < 0;

            break;

          case ComparisonOperatorEnum.NotEqual:
            isValid = comparisonResult != 0;

            break;

          default:
            isValid = false;

            break;
        }
      }

      return isValid;
    } // ValidateComparison

    /// <summary>
    /// Validación que campo obligatorio.
    /// </summary>
    /// <param name="field">Campo que se valída.</param>
    /// <param name="isMandatory">Especifica si el campo es obligatorio.</param>
    /// <returns>Si la validación es correcta true, en caso contrario false.</returns>
    protected bool ValidateField(object field, bool isMandatory)
    {
      string fieldStr = string.Format("{0}", field);
      bool isEmpty = string.IsNullOrEmpty(fieldStr);

      if (!this.IsEnabled || (!isMandatory && isEmpty))
      {
        return true;
      }
      else
      {
        return !string.IsNullOrWhiteSpace(fieldStr);
      }
    } // ValidateMandatoryField

    /// <summary>
    /// Validación de rangos.
    /// </summary>
    /// <param name="minValue">Valor mínimo.</param>
    /// <param name="maxValue">Valor máximo.</param>
    /// <param name="field">Valor que se valida.</param>
    /// <param name="type">Tipo de valor.</param>
    /// <param name="isMandatory">Especifica si el campo es obligatorio.</param>
    /// <returns>Si la validación es correcta true, en caso contrario false.</returns>
    protected bool ValidateRange(object minValue, object maxValue, object field, string type, bool isMandatory)
    {
      bool isEmpty = string.IsNullOrEmpty(string.Format("{0}", field));

      if (!this.IsEnabled || (!isMandatory && isEmpty))
      {
        return true;
      }

      object min = null, max = null, val = null;
      MethodInfo method = null;

      Type t = Type.GetType(type);

      bool isValidMax = this.ValidateType(maxValue, type, true, out max);
      bool isValidMin = this.ValidateType(minValue, type, true, out min);
      bool isValidType = t != null;
      bool isValidValue = this.ValidateType(field, type, true, out val);
      bool isValid = isValidMax && isValidMin && isValidType && isValidValue;

      if (isValid)
      {
        method = t.GetMethods().FirstOrDefault(i => i.Name.Equals("CompareTo"));

        isValid = method != null;
      }

      if (isValid)
      {
        isValid = ((int)method.Invoke(min, new object[] { val })) <= 0
            && ((int)method.Invoke(max, new object[] { val })) >= 0;
      }

      return isValid;
    } // ValidateRange

    /// <summary>
    /// Validación de expresiones regulares.
    /// </summary>
    /// <param name="field">Campo que se valida.</param>
    /// <param name="pattern">Patrón de la expresión regular.</param>
    /// <param name="isMandatory">Especifica si el campo es obligatorio.</param>
    /// <returns>Si la validación es correcta true, en caso contrario false.</returns>
    protected bool ValidateRegularExpression(object field, string pattern, bool isMandatory)
    {
      string fieldStr = string.Format("{0}", field);
      bool isEmpty = string.IsNullOrEmpty(fieldStr);

      if (!this.IsEnabled || (!isMandatory && isEmpty))
      {
        return true;
      }

      Regex regex = new Regex(pattern);
      bool isValid = this.ValidateField(field, true);

      if (isValid)
      {
        isValid = regex.IsMatch(fieldStr);
      }

      return isValid;
    } // ValidateRegularExpression

    /// <summary>
    /// Validación de tipo.
    /// </summary>
    /// <param name="field">Campo que se valida.</param>
    /// <param name="type">Tipo que se  valida.</param>
    /// <param name="isMandatory">Especifica si el campo es obligatorio.</param>
    /// <returns>Si la validación es correcta true, en caso contrario false.</returns>
    protected bool ValidateType(object field, string type, bool isMandatory)
    {
      object value = null;

      return this.ValidateType(field, type, isMandatory, out value);
    } // ValidateType

    /// <summary>
    /// Validación de tipo.
    /// </summary>
    /// <param name="field">Campo que se valida.</param>
    /// <param name="type">Tipo que se  valida.</param>
    /// <param name="isMandatory">Indica si es o no obligatorio.</param>
    /// <param name="value">Valor convertido al tipo especificado.</param>
    /// <returns>Si la validación es correcta true, en caso contrario false.</returns>
    protected bool ValidateType(object field, string type, bool isMandatory, out object value)
    {
      value = null;

      string fieldStr = string.Format("{0}", field);
      bool isEmpty = string.IsNullOrEmpty(fieldStr);

      if (!this.IsEnabled || (!isMandatory && isEmpty))
      {
        return true;
      }

      Type t = Type.GetType(type);

      bool isValid = t != null && this.ValidateField(field, true);

      if (isValid)
      {
        isValid = field.GetType().Equals(t);

        if (!isValid)
        {
          MethodInfo method = t.GetMethods().Where(i => i.Name.Equals("TryParse") && i.GetParameters().Count() == 2).FirstOrDefault();

          if (method != null)
          {
            value = Activator.CreateInstance(t);

            object[] parameters = new object[] { fieldStr, value };

            isValid = (bool)method.Invoke(t, parameters);

            if (isValid)
            {
              value = parameters[1];
            }
          }
        }
        else
        {
          value = field;
        }
      }

      return isValid;
    } // ValidateType
    #endregion

    #region Private static methods
    /// <summary>
    /// Callback para la propiedad IsValid.
    /// </summary>
    /// <param name="sender">Objeto que desencadena el evento.</param>
    /// <param name="e">Datos del evento.</param>
    private static void OnIsValidChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      if (sender is ThisClass)
      {
        ((ThisClass)sender).OnIsValidChanged();
      }
    } // OnIsValidChanged
    #endregion

    #region Private methods
    /// <summary>
    /// Notifica el cambio de valor en una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad de la que cambia el valor.</param>
    private void NotifyPropertyChanges(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    } // NotifyPropertyChanges

    /// <summary>
    /// Callback para la propiedad IsValid.
    /// </summary>
    private void OnIsValidChanged()
    {
      this.SetCurrentValue(BaseValidator.VisibilityProperty, this.IsEnabled && (!this.IsValid || !this.HideOnValidate) ? Visibility.Visible : Visibility.Collapsed);
    } // OnIsValidChanged
    #endregion
  }
}
