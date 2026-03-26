namespace InvoiceGenerator.Services
{
    // Services/GstHelper.cs
    public static class GstHelper
    {
        public static (decimal cgst, decimal sgst, decimal igst, decimal total)
            Calculate(decimal taxable, int stateCode)
        {
            decimal cgst = 0, sgst = 0, igst = 0;
            if (stateCode == 27) { cgst = taxable * 0.09m; sgst = taxable * 0.09m; }
            else { igst = taxable * 0.18m; }
            return (cgst, sgst, igst, taxable + cgst + sgst + igst);
        }

        public static string ConvertToWords(decimal amount)
        {
            long rupees = (long)Math.Floor(amount);
            int paise = (int)Math.Round((amount - rupees) * 100);

            string[] ones = { "", "ONE","TWO","THREE","FOUR","FIVE","SIX","SEVEN",
                          "EIGHT","NINE","TEN","ELEVEN","TWELVE","THIRTEEN",
                          "FOURTEEN","FIFTEEN","SIXTEEN","SEVENTEEN","EIGHTEEN","NINETEEN" };
            string[] tens = { "","","TWENTY","THIRTY","FORTY","FIFTY",
                          "SIXTY","SEVENTY","EIGHTY","NINETY" };

            string N2W(long n)
            {
                if (n == 0) return "";
                if (n < 20) return ones[n] + " ";
                if (n < 100) return tens[n / 10] + " " + ones[n % 10] + " ";
                if (n < 1000) return ones[n / 100] + " HUNDRED " + N2W(n % 100);
                if (n < 100000) return N2W(n / 1000) + "THOUSAND " + N2W(n % 1000);
                if (n < 10000000) return N2W(n / 100000) + "LAKH " + N2W(n % 100000);
                return N2W(n / 10000000) + "CRORE " + N2W(n % 10000000);
            }

            string result = "RUPEES " + N2W(rupees).Trim();
            if (paise > 0) result += " AND " + N2W(paise).Trim() + " PAISE";
            return result + " ONLY";
        }
    }
}
