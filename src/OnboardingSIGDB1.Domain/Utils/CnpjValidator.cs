namespace OnboardingSIGDB1.Domain.Utils;

public static class CnpjValidator
{
    public static bool IsValid(string cnpj)
    {
        if(string.IsNullOrWhiteSpace(cnpj))
            return false;
        
        cnpj = StringUtils.OnlyNumbers(cnpj);
        
        if(cnpj.Length != 14)
            return false;

        if (cnpj.All(c => c == cnpj[0]))
            return false;

        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        int sum = 0;
        for(int i = 0; i < 12; i ++)
        sum += (cnpj[i] - '0') * multiplier1[i];
        
        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;
        
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += (cnpj[i] - '0') * multiplier2[i];

        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;
        
        return cnpj[12] - '0' == firstDigit &&
               cnpj[13] - '0' == secondDigit;
    }
}