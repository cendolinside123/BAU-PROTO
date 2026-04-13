public class UpdateProducDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }

    public new List<string> InputValidation()
    {
        List<string> errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Id))
        {
            errors.Add("Id is required.");
        } else
        {
            if (!int.TryParse(Id, out _))
            {
                errors.Add("Id must be a valid number.");
            }
        }
        if (string.IsNullOrWhiteSpace(Name))
        {
            errors.Add("Name is required.");
        }
        if (string.IsNullOrWhiteSpace(Description))
        {
            errors.Add("Description is required.");
        }
        if (string.IsNullOrWhiteSpace(Price))
        {
            errors.Add("Price is required.");
        }
        else
        {
            if (!double.TryParse(Price, out _))
            {
                errors.Add("Price must be a valid number.");
            }
        }
        return errors;
    }

    public int GetId()
    {
        return int.Parse(Id);
    }

    public double GetPrice()
    {
        return double.Parse(Price);
    }

}