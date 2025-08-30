namespace TodoApi.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class EnumValueAttribute : ValidationAttribute
    {
        private readonly Type _enumType;

        public EnumValueAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var validValues = Enum.GetNames(_enumType);

            if (!validValues.Contains(value.ToString()))
            {
                var allowed = string.Join(", ", validValues);
                return new ValidationResult(
                    $"'{value}' is not valid. Allowed values are: {allowed}."
                );
            }

            return ValidationResult.Success;
        }
    }
}
