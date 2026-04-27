namespace OnboardingSIGDB1.Domain.Utils;

public static class CpfValidator
{
    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = StringUtils.OnlyNumbers(cpf);

        if (cpf.Length != 11)
            return false;


        if (cpf.All(c => c == cpf[0]))
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (cpf[i] - '0') * (10 - i);

        int firstDigit = (sum * 10) % 11;
        if (firstDigit == 10)
            firstDigit = 0;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (cpf[i] - '0') * (11 - i);

        int secondDigit = (sum * 10) % 11;
        if (secondDigit == 10)
            secondDigit = 0;

        return cpf[9] - '0' == firstDigit &&
               cpf[10] - '0' == secondDigit;
    }
}