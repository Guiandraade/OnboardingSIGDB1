namespace OnboardingSIGDB1.Domain.Utils;

public class StringUtils
{
    public static string OnlyNumbers(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        
        return new string(value.Where(c => char.IsDigit(c)).ToArray());
    }
}