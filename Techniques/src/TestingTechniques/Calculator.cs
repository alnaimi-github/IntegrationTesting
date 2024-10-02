namespace TestingTechniques;

public class Calculator
{
    public int Add(int a, int b)
    {
        return a + b;
    }

    public int Subtract(int a, int b)
    {
        return a - b;
    }

    public int Multiply(int a, int b)
    {
        return a * b;
    }

    public float Divide(float a, float b)
    {
        EnsureThatDivisorIsNotZero(b);

        return a / b;
    }

    private static void EnsureThatDivisorIsNotZero(float b)
    {
        if (b == 0)
        {
            throw new DivideByZeroException();
        }
    }
}

