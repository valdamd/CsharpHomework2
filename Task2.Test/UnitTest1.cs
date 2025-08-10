using Xunit;
using FluentAssertions;
using task2;
using Assert = Xunit.Assert;

namespace Task2;

public class Tests
{
    [Fact]
    public void Test_Addition_Operator()
    {
        var p1 = new Polynomial(1, 2, 3);
        var p2 = new Polynomial(3, 2, 1);
        var expected = new Polynomial(4, 4, 4);

        (p1 + p2).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Subtraction_Operator()
    {
        var p1 = new Polynomial(5, 3, 1);
        var p2 = new Polynomial(1, 2, 3);
        var expected = new Polynomial(4, 1, -2);

        (p1 - p2).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Unary_Negation()
    {
        var p = new Polynomial(1, -2, 3);
        var expected = new Polynomial(-1, 2, -3);

        (-p).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Add_Scalar()
    {
        var p = new Polynomial(1, 2, 3);
        var expected = new Polynomial(6, 2, 3);

        (p + 5).Should().BeEquivalentTo(expected);
        (5 + p).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Subtract_Scalar()
    {
        var p = new Polynomial(6, 2, 3);
        var expected = new Polynomial(1, 2, 3);

        (p - 5).Should().BeEquivalentTo(expected);
        (6 - new Polynomial(5, -2, -3)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Multiplication_By_Scalar()
    {
        var p = new Polynomial(1, 2, 3);
        var expected = new Polynomial(2, 4, 6);

        (p * 2).Should().BeEquivalentTo(expected);
        (2 * p).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Multiplication_By_Polynomial()
    {
        var p1 = new Polynomial(1, 1);   // 1 + x
        var p2 = new Polynomial(1, -1);  // 1 - x
        var expected = new Polynomial(1, 0, -1); // 1 - x^2

        (p1 * p2).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Division_By_Scalar()
    {
        var p = new Polynomial(2, 4, 6);
        var expected = new Polynomial(1, 2, 3);

        (p / 2).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Test_Division_By_Polynomial()
    {
        var dividend = new Polynomial(1, -3, 2);
        var divisor = new Polynomial(1, -1);

        var (quotient, remainder) = dividend / divisor;

        var expectedQuotient = new Polynomial(1, -2);
        var expectedRemainder = new Polynomial(0);

        quotient.Should().BeEquivalentTo(expectedQuotient);
        remainder.Should().BeEquivalentTo(expectedRemainder);
    }

    [Fact]
    public void Test_Evaluate()
    {
        var p = new Polynomial(1, 2, 3); // 1 + 2x + 3x^2
        double result = p.Evaluate(2);   // 1 + 2*2 + 3*4 = 1 + 4 + 12 = 17

        result.Should().Be(17);
    }

    [Fact]
    public void Test_Equality()
    {
        var p1 = new Polynomial(1, 2, 3);
        var p2 = new Polynomial(1, 2, 3);

        // Для этого теста нужно будет добавить метод Equals в класс Polynomial
        p1.Should().BeEquivalentTo(p2);
    }

    [Fact]
    public void Test_ToString()
    {
        var p = new Polynomial(3, -2, 1); // 3 - 2x + x^2

        var str = p.ToString();

        str.Should().Be("x^2 - 2x + 3");
    }

    [Fact]
    public void Test_EmptyPolynomial_ToString()
    {
        var p = new Polynomial(0); // Только нулевой коэффициент

        p.ToString().Should().Be("0");
    }

    [Fact]
    public void Test_Indexer()
    {
        var p = new Polynomial(1, 2, 3);
        p[1].Should().Be(2);

        p[1] = 5;
        p[1].Should().Be(5);
    }

    [Fact]
    public void Test_Clone()
    {
        var p1 = new Polynomial(1, 2, 3);
        var p2 = p1.Clone();

        p2.Should().BeEquivalentTo(p1);
        p2.Should().NotBeSameAs(p1);
    }

    [Fact]
    public void Test_RemoveLeadingZeros()
    {
        var p = new Polynomial(1, 0, 0, 0); // DeleteLastZeros вызывается автоматически в конструкторе

        p.Degree.Should().Be(0);
    }

    [Fact]
    public void Test_ZeroDegreePolynomial()
    {
        var p = new Polynomial(5);
        p.Degree.Should().Be(0);
        p.Evaluate(100).Should().Be(5);
    }

    [Fact]
    public void Test_Negative_Coefficients_ToString()
    {
        var p = new Polynomial(-1, 0, 2); // -1 + 2x^2
        var str = p.ToString();

        str.Should().Be("2x^2 - 1");
    }

    [Fact]
    public void Test_Single_Variable_ToString()
    {
        var p = new Polynomial(0, 1); // x
        p.ToString().Should().Be("x");
    }

    [Fact]
    public void Test_Constant_ToString()
    {
        var p = new Polynomial(5); // 5
        p.ToString().Should().Be("5");
    }

    [Fact]
    public void Test_Negative_Constant_ToString()
    {
        var p = new Polynomial(-3); // -3
        p.ToString().Should().Be("-3");
    }

    [Fact]
    public void Test_Complex_Polynomial_ToString()
    {
        var p = new Polynomial(1, -1, 0, 1, -1); // 1 - x + x^3 - x^4
        p.ToString().Should().Be("-x^4 + x^3 - x + 1");
    }

    [Fact]
    public void Test_Indexer_Out_Of_Range()
    {
        var p = new Polynomial(1, 2, 3);
        
        // Получение коэффициента за пределами степени должно возвращать 0
        p[5].Should().Be(0);
        
        // Установка коэффициента за пределами степени должна расширить полином
        p[5] = 7;
        p[5].Should().Be(7);
        p.Degree.Should().Be(5);
    }

    [Fact]
    public void Test_Indexer_Negative_Index()
    {
        var p = new Polynomial(1, 2, 3);
        
        // Отрицательный индекс должен выбрасывать исключение
        Assert.Throws<ArgumentOutOfRangeException>(() => p[-1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => p[-1] = 5);
    }
}