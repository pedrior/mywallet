using System.Diagnostics.CodeAnalysis;

namespace MyWallet.Domain;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Currency codes")]
public sealed class Currency : Enum<Currency>
{
    public static readonly Currency USD = new("USD", 1);
    public static readonly Currency EUR = new("EUR", 2);
    public static readonly Currency JPY = new("JPY", 3);
    public static readonly Currency GBP = new("GBP", 4);
    public static readonly Currency AUD = new("AUD", 5);
    public static readonly Currency CAD = new("CAD", 6);
    public static readonly Currency CHF = new("CHF", 7);
    public static readonly Currency CNY = new("CNY", 8);
    public static readonly Currency SEK = new("SEK", 9);
    public static readonly Currency NZD = new("NZD", 10);
    public static readonly Currency MXN = new("MXN", 11);
    public static readonly Currency SGD = new("SGD", 12);
    public static readonly Currency HKD = new("HKD", 13);
    public static readonly Currency NOK = new("NOK", 14);
    public static readonly Currency KRW = new("KRW", 15);
    public static readonly Currency TRY = new("TRY", 16);
    public static readonly Currency INR = new("INR", 17);
    public static readonly Currency BRL = new("BRL", 18);
    public static readonly Currency ZAR = new("ZAR", 19);
    public static readonly Currency RUB = new("RUB", 20);
    public static readonly Currency IDR = new("IDR", 21);
    public static readonly Currency MYR = new("MYR", 22);
    public static readonly Currency PHP = new("PHP", 23);
    public static readonly Currency THB = new("THB", 24);
    public static readonly Currency TWD = new("TWD", 25);
    public static readonly Currency AED = new("AED", 26);
    public static readonly Currency SAR = new("SAR", 27);
    public static readonly Currency ARS = new("ARS", 28);
    public static readonly Currency VND = new("VND", 29);
    public static readonly Currency NGN = new("NGN", 30);

    private Currency(string name, int value) : base(name, value)
    {
    }
}