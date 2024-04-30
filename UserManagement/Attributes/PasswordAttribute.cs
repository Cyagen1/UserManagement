using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UserManagement.Attributes
{
    public class PasswordAttribute : ValidationAttribute
    {
        private readonly string _regex;

        public PasswordAttribute(string regex)
        {
            _regex = regex;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string password = value.ToString();
                if (!Regex.IsMatch(password, _regex))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
