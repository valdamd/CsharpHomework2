namespace Task2;

internal sealed class Polynomial
{
    private readonly List<double> _coefficients;
    private int _degree;

    public Polynomial(params double[] coefficients)
    {
        if (coefficients.Length == 0)
        {
            _coefficients = new List<double> { 0 };
            _degree = 0;
        }
        else
        {
            _coefficients = new List<double>(coefficients);
            _degree = coefficients.Length - 1;
            DeleteLastZeros();
        }
    }

    public int Degree => _degree;

    public double this[int index]
    {
        get
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс не может быть отрицательным");
            }

            if (index > _degree)
            {
                return 0;
            }

            return _coefficients[index];
        }

        set
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс не может быть отрицательным");
            }

            if (index > _degree)
            {
                for (var i = _coefficients.Count; i <= index; i++)
                {
                    _coefficients.Add(0);
                }

                _degree = index;
            }

            _coefficients[index] = value;
            DeleteLastZeros();
        }
    }

    public static Polynomial operator +(Polynomial polynomial) => polynomial;

    public static Polynomial operator -(Polynomial polynomial)
    {
        var result = polynomial.Clone();
        for (var i = 0; i < result._coefficients.Count; i++)
        {
            result._coefficients[i] *= -1;
        }

        return result;
    }

    public static Polynomial operator +(Polynomial polynomial, double value)
    {
        return PlusSignOperation(polynomial, value);
    }

    public static Polynomial operator +(double value, Polynomial polynomial)
    {
        return PlusSignOperation(polynomial, value);
    }

    public static Polynomial operator -(Polynomial polynomial, double value)
    {
        return MinusSignOperation(polynomial, value);
    }

    public static Polynomial operator -(double value, Polynomial polynomial)
    {
        return MinusSignOperation(-polynomial, -value);
    }

    public static Polynomial operator +(Polynomial firstPolynomial, Polynomial secondPolynomial)
    {
        return AddTwoPolynomials(firstPolynomial, secondPolynomial);
    }

    public static Polynomial operator -(Polynomial firstPolynomial, Polynomial secondPolynomial)
    {
        return SubtractTwoPolynomials(firstPolynomial, secondPolynomial);
    }

    public static Polynomial operator *(Polynomial polynomial, double value)
    {
        return MultiplyPolynomialByValue(polynomial, value);
    }

    public static Polynomial operator *(double value, Polynomial polynomial)
    {
        return MultiplyPolynomialByValue(polynomial, value);
    }

    public static Polynomial operator *(Polynomial firstPolynomial, Polynomial secondPolynomial)
    {
        var len = firstPolynomial._coefficients.Count + secondPolynomial._coefficients.Count - 1;
        var newCoef = new List<double>(new double[len]);

        for (var i = 0; i < firstPolynomial._coefficients.Count; i++)
        {
            for (var j = 0; j < secondPolynomial._coefficients.Count; j++)
            {
                newCoef[i + j] += firstPolynomial._coefficients[i] * secondPolynomial._coefficients[j];
            }
        }

        return new Polynomial(newCoef.ToArray());
    }

    public static Polynomial operator /(Polynomial polynomial, double value)
    {
        if (Math.Abs(value) < double.Epsilon)
        {
            throw new DivideByZeroException("Деление на ноль");
        }

        return MultiplyPolynomialByValue(polynomial, 1.0 / value);
    }

    public static (Polynomial Quotient, Polynomial Remainder) operator /(Polynomial numeratorPolynomial, Polynomial denominatorPolynomial)
    {
        if (denominatorPolynomial._degree == 0 && Math.Abs(denominatorPolynomial._coefficients[0]) < double.Epsilon)
        {
            throw new DivideByZeroException("Деление на нулевой полином");
        }

        var numeratorCoefficients = new List<double>(numeratorPolynomial._coefficients);
        numeratorCoefficients.Reverse();
        var denominatorCoefficients = new List<double>(denominatorPolynomial._coefficients);
        denominatorCoefficients.Reverse();

        var (quotientCoefficients, remainderCoefficients) =
            ExtendedSyntheticDivision(numeratorCoefficients, denominatorCoefficients);

        quotientCoefficients.Reverse();
        remainderCoefficients.Reverse();

        return (new Polynomial(quotientCoefficients.ToArray()), new Polynomial(remainderCoefficients.ToArray()));
    }

    public Polynomial Clone()
    {
        return new Polynomial(_coefficients.ToArray());
    }

    public double Evaluate(double value)
    {
        var result = 0.0;
        for (var i = _degree; i >= 0; i--)
        {
            result = (result * value) + _coefficients[i];
        }

        return result;
    }

    public override string ToString()
    {
        if (_coefficients.Count == 0)
        {
            return "0";
        }

        string FormatTerm(double coefficient, int power, bool isFirstTerm)
        {
            string GetSign(double coef, bool firstTerm)
            {
                if (firstTerm)
                {
                    return coef < 0 ? "-" : string.Empty;
                }

                return coef < 0 ? " - " : " + ";
            }

            var sign = GetSign(coefficient, isFirstTerm);
            if (power == 0)
            {
                return $"{sign}{Math.Abs(coefficient)}";
            }

            if (power == 1)
            {
                if (coefficient == 1 || coefficient == -1)
                {
                    return $"{sign}x";
                }

                return $"{sign}{Math.Abs(coefficient)}x";
            }

            if (coefficient == 1 || coefficient == -1)
            {
                return $"{sign}x^{power}";
            }

            return $"{sign}{Math.Abs(coefficient)}x^{power}";
        }

        var terms = new List<string>();
        for (var index = _degree; index >= 0; index--)
        {
            var coefficient = _coefficients[index];
            if (Math.Abs(coefficient) > double.Epsilon)
            {
                terms.Add(FormatTerm(coefficient, index, terms.Count == 0));
            }
        }

        return terms.Count == 0 ? "0" : string.Join(string.Empty, terms);
    }

    private static Polynomial MinusSignOperation(Polynomial polynomial, double value)
    {
        var result = polynomial.Clone();
        result._coefficients[0] -= value;
        return result;
    }

    private static Polynomial MultiplyPolynomialByValue(Polynomial polynomial, double value)
    {
        var resultCoefficients = new List<double>(polynomial._coefficients.Count);
        for (var i = 0; i < polynomial._coefficients.Count; i++)
        {
            resultCoefficients.Add(polynomial._coefficients[i] * value);
        }

        return new Polynomial(resultCoefficients.ToArray());
    }

    private static Polynomial AddTwoPolynomials(Polynomial firstPolynomial, Polynomial secondPolynomial)
    {
        var resultCoefficients = new List<double>();
        int maxDegree = Math.Max(firstPolynomial._degree, secondPolynomial._degree);

        for (var i = 0; i <= maxDegree; i++)
        {
            double firstCoef = i <= firstPolynomial._degree ? firstPolynomial._coefficients[i] : 0;
            double secondCoef = i <= secondPolynomial._degree ? secondPolynomial._coefficients[i] : 0;
            resultCoefficients.Add(firstCoef + secondCoef);
        }

        return new Polynomial(resultCoefficients.ToArray());
    }

    private static Polynomial SubtractTwoPolynomials(Polynomial firstPolynomial, Polynomial secondPolynomial)
    {
        var resultCoefficients = new List<double>();
        int maxDegree = Math.Max(firstPolynomial._degree, secondPolynomial._degree);

        for (var i = 0; i <= maxDegree; i++)
        {
            double firstCoef = i <= firstPolynomial._degree ? firstPolynomial._coefficients[i] : 0;
            double secondCoef = i <= secondPolynomial._degree ? secondPolynomial._coefficients[i] : 0;
            resultCoefficients.Add(firstCoef - secondCoef);
        }

        return new Polynomial(resultCoefficients.ToArray());
    }

    private static (List<double> Quotient, List<double> Remainder) ExtendedSyntheticDivision(List<double> dividendCoefficients, List<double> divisorCoefficients)
    {
        var quotientCoefficients = new List<double>(dividendCoefficients);
        var normalizer = divisorCoefficients[0];

        for (var i = 0; i <= dividendCoefficients.Count - divisorCoefficients.Count; i++)
        {
            quotientCoefficients[i] /= normalizer;
            var coefficient = quotientCoefficients[i];

            for (var j = 1; j < divisorCoefficients.Count; j++)
            {
                quotientCoefficients[i + j] -= divisorCoefficients[j] * coefficient;
            }
        }

        var separator = quotientCoefficients.Count - (divisorCoefficients.Count - 1);
        var remainderCoefficients = quotientCoefficients.GetRange(separator, quotientCoefficients.Count - separator);
        quotientCoefficients.RemoveRange(separator, quotientCoefficients.Count - separator);

        while (remainderCoefficients.Count > 1 && Math.Abs(remainderCoefficients[^1]) < double.Epsilon)
        {
            remainderCoefficients.RemoveAt(remainderCoefficients.Count - 1);
        }

        return (quotientCoefficients, remainderCoefficients);
    }

    private static Polynomial PlusSignOperation(Polynomial polynomial, double value)
    {
        var result = polynomial.Clone();
        result._coefficients[0] += value;
        return result;
    }

    private void DeleteLastZeros()
    {
        while (_coefficients.Count > 1 && Math.Abs(_coefficients[_coefficients.Count - 1]) < double.Epsilon)
        {
            _coefficients.RemoveAt(_coefficients.Count - 1);
            _degree--;
        }
    }
}