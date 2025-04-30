namespace SharedModels;

public class UserMessage
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string RG { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
}