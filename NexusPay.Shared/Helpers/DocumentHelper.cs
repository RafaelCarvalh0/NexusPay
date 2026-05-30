namespace NexusPay.Shared.Helpers
{
    public static class DocumentHelper
    {
        public static bool IsValidCpf(string cpf)
        {
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            int[] multipliers1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multipliers2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            int sum = cpf.Take(9).Select((d, i) => (d - '0') * multipliers1[i]).Sum();
            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            sum = cpf.Take(10).Select((d, i) => (d - '0') * multipliers2[i]).Sum();
            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf[9] - '0' == digit1 && cpf[10] - '0' == digit2;
        }

        public static bool IsValidCnpj(string cnpj)
        {
            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (cnpj.Length != 14 || cnpj.Distinct().Count() == 1)
                return false;

            int[] multipliers1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multipliers2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            int sum = cnpj.Take(12).Select((d, i) => (d - '0') * multipliers1[i]).Sum();
            int remainder = sum % 11;
            int digit1 = remainder < 2 ? 0 : 11 - remainder;

            sum = cnpj.Take(13).Select((d, i) => (d - '0') * multipliers2[i]).Sum();
            remainder = sum % 11;
            int digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cnpj[12] - '0' == digit1 && cnpj[13] - '0' == digit2;
        }

        public static bool IsValidDocument(string document)
        {
            string digits = new(document.Where(char.IsDigit).ToArray());
            return digits.Length == 11 ? IsValidCpf(document) : IsValidCnpj(document);
        }
    }
}
