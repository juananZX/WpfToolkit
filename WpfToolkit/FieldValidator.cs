namespace Espamatica.WpfToolkit
{
  public class FieldValidator : BaseValidator
  {
    /// <summary>
    /// Mandatory field validator.
    /// Validador de campos obligatorios.
    /// </summary>
    public override void Validate() => SetCurrentValue(IsValidProperty, ValidateField(Field, IsMandatory));
  }
}
