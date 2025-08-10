
namespace task2
{
    public sealed class Polynomial 
    {
        private int degree;
        private List<double> coefficients;

        public int Degree => degree;
        
        public double this[int index]
        {
            get 
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс не может быть отрицательным");
                
                if (index > degree)
                    return 0;
                    
                return coefficients[index];
            }
            set 
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Индекс не может быть отрицательным");
                
                if (index > degree)
                {
                    for (var i = coefficients.Count; i <= index; i++)
                    {
                        coefficients.Add(0);
                    }
                    degree = index;
                }
                
                coefficients[index] = value;
                DeleteLastZeros();
            }
        }

        public Polynomial(params double[] coefficients)
        {
            if (coefficients == null || coefficients.Length == 0)
            {
                this.coefficients = new List<double> { 0 };
                degree = 0;
            }
            else
            {
                this.coefficients = new List<double>(coefficients);
                degree = coefficients.Length - 1;
                DeleteLastZeros();
            }
        }

        public double Evaluate(double value)
        {
            if (coefficients.Count == 0) return 0;
            
            var result = coefficients[degree];

            for (var i = degree - 1; i >= 0; i--)
            {
                result = result * value + coefficients[i];
            }

            return result;
        }

        private void DeleteLastZeros()
        {
            while (coefficients.Count > 1 && coefficients[coefficients.Count - 1] == 0)
            {
                coefficients.RemoveAt(coefficients.Count - 1);
                degree--;
            }
        }

        public Polynomial Clone()
        {
            return new Polynomial(coefficients.ToArray());
        }

        // Унарные операторы
        public static Polynomial operator +(Polynomial polynomial) => polynomial;

        public static Polynomial operator -(Polynomial polynomial)
        {
            var result = polynomial.Clone();
            for (var i = 0; i <= result.degree; i++)
            {
                result.coefficients[i] *= -1;
            }
            return result;
        }

        // Операторы с double
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
            return MinusSignOperation(polynomial, value, false);
        }

        public static Polynomial operator -(double value, Polynomial polynomial)
        {
            return MinusSignOperation(polynomial, value, true);
        }

        // Операторы между полиномами
        public static Polynomial operator +(Polynomial firstPolynomial, Polynomial secondPolynomial)
        {
            return AddTwoPolynomialsWithMultiplier(firstPolynomial, secondPolynomial, 1);
        }

        public static Polynomial operator -(Polynomial firstPolynomial, Polynomial secondPolynomial)
        {
            return AddTwoPolynomialsWithMultiplier(firstPolynomial, secondPolynomial, -1);
        }

        // Операторы умножения
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
            var firstPolynomialCoefficients = firstPolynomial.coefficients.ToArray();
            var secondPolynomialCoefficients = secondPolynomial.coefficients.ToArray();

            var len = firstPolynomialCoefficients.Length + secondPolynomialCoefficients.Length - 1;
            var newCoef = new double[len];

            for (var i = 0; i < firstPolynomialCoefficients.Length; i++)
            {
                for (var j = 0; j < secondPolynomialCoefficients.Length; j++)
                    newCoef[i + j] += firstPolynomialCoefficients[i] * secondPolynomialCoefficients[j];
            }
            return new Polynomial(newCoef);
        }

        // Операторы деления
        public static Polynomial operator /(Polynomial polynomial, double value)
        {
            if (Math.Abs(value) < double.Epsilon)
                throw new DivideByZeroException("Деление на ноль");
            return MultiplyPolynomialByValue(polynomial, 1.0 / value);
        }

        public static (Polynomial, Polynomial) operator /(Polynomial numeratorPolynomial, Polynomial denominatorPolynomial)
        {
            if (denominatorPolynomial.degree == 0 && Math.Abs(denominatorPolynomial.coefficients[0]) < double.Epsilon)
                throw new DivideByZeroException("Деление на нулевой полином");

            var numeratorCoefficients = new List<double>(numeratorPolynomial.coefficients);
            var denominatorCoefficients = new List<double>(denominatorPolynomial.coefficients);

            numeratorCoefficients.Reverse();
            denominatorCoefficients.Reverse();

            var (quotientDoubleListCoefficients, remainderDoubleListCoefficients) = 
                ExtendedSyntheticDivision(numeratorCoefficients, denominatorCoefficients);

            quotientDoubleListCoefficients.Reverse();
            remainderDoubleListCoefficients.Reverse();

            var quotientCoefficients = quotientDoubleListCoefficients.ToArray();
            var remainderCoefficients = remainderDoubleListCoefficients.ToArray();

            var quotientPolynomial = new Polynomial(quotientCoefficients);
            var remainderPolynomial = new Polynomial(remainderCoefficients);

            return (quotientPolynomial, remainderPolynomial);
        }

        // ToString методы
        public override string ToString()
        {
            if (coefficients.Count == 0)
            {
                return "0";
            }

            var terms = new List<string>();

            for (var index = degree; index >= 0; index--)
            {
                var coefficient = coefficients[index];
                if (coefficient != 0)
                {
                    var term = FormatTerm(coefficient, index, terms.Count == 0);
                    terms.Add(term);
                }
            }

            return terms.Count == 0 ? "0" : string.Join("", terms);
        }

        private string FormatTerm(double coefficient, int power, bool isFirstTerm)
        {
            var sign = GetSign(coefficient, isFirstTerm);
            var absCoefficient = Math.Abs(coefficient);
            
            switch (power)
            {
                case 0:
                    return $"{sign}{absCoefficient}";
                    
                case 1:
                    if (absCoefficient == 1)
                        return $"{sign}x";
                    else
                        return $"{sign}{absCoefficient}x";
                        
                default:
                    if (absCoefficient == 1)
                        return $"{sign}x^{power}";
                    else
                        return $"{sign}{absCoefficient}x^{power}";
            }
        }

        private string GetSign(double coefficient, bool isFirstTerm)
        {
            if (isFirstTerm)
            {
                return coefficient < 0 ? "-" : "";
            }
            else
            {
                return coefficient < 0 ? " - " : " + ";
            }
        }

        // Приватные методы для операторов
        private static Polynomial PlusSignOperation(Polynomial polynomial, double value)
        {
            var result = polynomial.Clone();
            result.coefficients[0] += value;
            return result;
        }

        private static Polynomial MinusSignOperation(Polynomial polynomial, double value, bool isValueFirst)
        {
            var result = polynomial.Clone();
            if (isValueFirst)
            {
                for (var i = 0; i <= result.degree; i++)
                {
                    result.coefficients[i] *= -1;
                }
                result.coefficients[0] += value;
            }
            else
            {
                result.coefficients[0] -= value;
            }
            return result;
        }

        private static Polynomial MultiplyPolynomialByValue(Polynomial polynomial, double value)
        {
            var resultCoefficients = polynomial.coefficients.ToArray();
            for (var i = 0; i < resultCoefficients.Length; ++i)
            {
                resultCoefficients[i] *= value;
            }
            return new Polynomial(resultCoefficients);
        }

        private static Polynomial AddTwoPolynomialsWithMultiplier(Polynomial firstPolynomial, Polynomial secondPolynomial, int multiplier)
        {
            var firstPolynomialCoefficients = firstPolynomial.coefficients.ToArray();
            var secondPolynomialCoefficients = secondPolynomial.coefficients.ToArray();

            double[] resultCoefficients;

            if (firstPolynomial.degree > secondPolynomial.degree)
            {
                resultCoefficients = AddDifferentSizePolynomials(firstPolynomial, secondPolynomial, multiplier);
            }
            else if (firstPolynomial.degree < secondPolynomial.degree)
            {
                resultCoefficients = AddDifferentSizePolynomials(secondPolynomial, firstPolynomial, multiplier);
            }
            else
            {
                resultCoefficients = new double[firstPolynomial.degree + 1];
                for (var i = 0; i <= firstPolynomial.degree; i++)
                {
                    resultCoefficients[i] = firstPolynomialCoefficients[i] + (multiplier * secondPolynomialCoefficients[i]);
                }
            }
            return new Polynomial(resultCoefficients);
        }

        private static double[] AddDifferentSizePolynomials(Polynomial biggestPolynomial, Polynomial smallestPolynomial, int multiplier)
        {
            var resultCoefficients = new double[biggestPolynomial.degree + 1];
            var biggerPolynomialCoefficients = biggestPolynomial.coefficients.ToArray();
            var smallestPolynomialCoefficients = smallestPolynomial.coefficients.ToArray();

            for (var i = 0; i <= biggestPolynomial.degree; i++)
            {
                if (i <= smallestPolynomial.degree)
                {
                    resultCoefficients[i] = biggerPolynomialCoefficients[i] + (multiplier * smallestPolynomialCoefficients[i]);
                }
                else
                {
                    resultCoefficients[i] = biggerPolynomialCoefficients[i];
                }
            }

            return resultCoefficients;
        }

        private static (List<double>, List<double>) ExtendedSyntheticDivision(List<double> dividendCoefficients, List<double> divisorCoefficients)
        {
            var quotientCoefficients = new List<double>(dividendCoefficients);

            var normalizer = divisorCoefficients[0];

            for (var i = 0; i < dividendCoefficients.Count - (divisorCoefficients.Count - 1); i++)
            {
                quotientCoefficients[i] /= normalizer;

                var coefficient = quotientCoefficients[i];
                if (coefficient != 0)
                {
                    for (var j = 1; j < divisorCoefficients.Count; j++)
                    {
                        quotientCoefficients[i + j] += -divisorCoefficients[j] * coefficient;
                    }
                }
            }

            var separator = quotientCoefficients.Count - (divisorCoefficients.Count - 1);

            var remainderCoefficients = quotientCoefficients.GetRange(separator, quotientCoefficients.Count - separator);
            for (var i = 0; i < separator; i++)
            {
                remainderCoefficients.Insert(0, 0);
            }

            return (quotientCoefficients.GetRange(0, separator), remainderCoefficients);
        }
    }
}