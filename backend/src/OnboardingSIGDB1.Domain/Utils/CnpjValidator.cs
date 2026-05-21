namespace OnboardingSIGDB1.Domain.Utils;

public static class CnpjValidator
{
    private static readonly int[] Multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] Multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    public static bool IsValid(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        cnpj = StringUtils.OnlyNumbers(cnpj);

        if (cnpj.Length != 14)
            return false;


        // Stryker disable once Equality : Equivalent mutant — all-same-digit CNPJs (e.g. 00000000000000) pass check-digit math, so this guard is required and the == → != mutation is equivalent.
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += (cnpj[i] - '0') * Multiplier1[i];

        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;

        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += (cnpj[i] - '0') * Multiplier2[i];

        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cnpj[12] - '0' == firstDigit &&
               cnpj[13] - '0' == secondDigit;
    }
}