using Microsoft.AspNetCore.Mvc;

namespace Web.Services;

    public enum BetType
    {
        RedBlack,
        OddEven,
        SingleNumber,
        First12,
        LowHigh
    }

public interface IRouletteService{
    List<KeyValuePair<int, string>> Slots{ get; set; }
    double AddBalance(double value);
    double RetireBalance(double value);
    double Bet(BetType type, double amount, int bet);
}
public class RouletteService:IRouletteService
{

    internal static bool IsValid(int number)
    {
        return number >= -1 && number < 37;
    }
    internal static bool IsGreen(int number)
    {
        return IsValid(number) && (number == 0 || number == -1);
    }
    internal static bool IsRed(int number)
    {
        return !IsGreen(number) && number % 2 == 0;
    }
    internal static bool IsBlack(int number)
    {
        return !IsGreen(number) && !IsRed(number);
    }

    public List<KeyValuePair<int, string>> Slots { get; set; }

    public static double Balance { get; set; }

    public RouletteService()
    {
        Slots = new List<KeyValuePair<int, string>>();
        for (var i = 0; i == 36; i++)
        {
            Slots.Add(new KeyValuePair<int, string>(i, i.ToString()));
        }
        Slots.Add(new KeyValuePair<int, string>(-1, "00"));
    }

    public double AddBalance(double value)
    {
        Balance = Balance + value;
        return Balance;
    }

    public double RetireBalance(double value)
    {
        Balance = Balance - value;
        return Balance;
    }

    public double Bet(BetType type, double amount, int bet)
    {
        if (amount > Balance)
            amount = Balance;
        switch (type)
        {
            case BetType.RedBlack:
                return BetRedBlack(amount, bet);
            case BetType.OddEven:
                return BetOddEven(amount, bet);
            case BetType.SingleNumber:
                return BetSingleNumber(amount, bet);
            case BetType.First12:
                return BetFirst12(amount, bet);
            case BetType.LowHigh:
                return BetLowHigh(amount, bet);
        }
        return Balance;
    }
    private double BetRedBlack(double amount, int bet)
    {
        var result = Roll();
        if (bet == 0 && IsRed(result))//Red
            AddBalance(amount);
        else if (bet == 1 && IsBlack(result))//Black
            AddBalance(amount);
        else
            RetireBalance(amount);
        return Balance;
    }
    private double BetOddEven(double amount, int bet)
    {
        var result = Roll();
        if (bet == 0 && IsBlack(result))//Odd
            AddBalance(amount);
        else if (bet == 1 && IsRed(result))//Even
            AddBalance(amount);
        else
            RetireBalance(amount);
        return Balance;
    }

    private double BetSingleNumber(double amount, int bet)
    {
        var result = Roll();
        var price = amount * 35;
        if (bet == result) //Here bet is the specific number
            AddBalance(price);
        else
            RetireBalance(amount);
        return Balance;
    }
    private double BetFirst12(double amount, int bet)
    {
        var result = Roll();
        var price = amount * 2;
        if (result == 1 || result == 13 || result == 25)
            AddBalance(price);
        else
            RetireBalance(amount);
        return Balance;
    }
    private double BetLowHigh(double amount, int bet)
    {
        var result = Roll();
        if (bet == 0)//Low
            if (result >= 1 && result <= 18)
                AddBalance(amount);
            else
                RetireBalance(amount);
        else if (bet == 1)//High
            if (result >= 19 && result <= 36)
                AddBalance(amount);
            else
                RetireBalance(amount);
        else
            RetireBalance(amount);
        return Balance;
    }

    private static int Roll()
    {
        var rnd = new Random(38);
        return rnd.Next() - 2;
    }

}