using System.Text.RegularExpressions;

public class RegisterRequestDto
{

    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public new List<string> InputValidation()
    {
        List<string> errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Email))
        {
            errors.Add("Email is required.");
        }

        else
        {
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(Email, pattern, RegexOptions.IgnoreCase))
            {
                errors.Add("Email is not valid.");
            }
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            errors.Add("Password is required.");
        }

        if (string.IsNullOrWhiteSpace(Role))
        {
            errors.Add("Role is required.");
        }
        
        return errors;
    }

}
