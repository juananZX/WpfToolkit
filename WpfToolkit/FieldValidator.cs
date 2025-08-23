namespace System.Windows.Controls
{
  public class FieldValidator : BaseValidator
  {
    /// <summary>
    /// Mandatory field validator.
    /// Validador de campos obligatorios.
    /// </summary>
    public override void Validate() => this.SetCurrentValue(IsValidProperty, ValidateField(Field, IsMandatory));
  }
}
