public class DeleteProductDto
{
    public string Id { get; set; }

    public int GetId()
    {
        return int.Parse(Id);
    }

    public List<string> InputValidation()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Id))
            {
                errors.Add("Id is required.");
            }
            else
            {
                if (!int.TryParse(Id, out _))
                {
                    errors.Add("Id must be a valid number.");
                }
            }
            return errors;
    }
}