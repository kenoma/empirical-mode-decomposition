using FluentAssertions;
using MathNet.Numerics.Interpolation;

namespace EMD.UnitTests;

public class EmpiricalModeDecompositionUnitTests
{
    private double[] _lineAndSine;

    [SetUp]
    public void Setup()
    {
        _lineAndSine = new double[100];
        for (var i = 0; i < _lineAndSine.Length; i++)
        {
            _lineAndSine[i] = i / 100.0 + Math.Sin(0.7 * i);
        }
    }


    [Test]
    public void Sift_SineAndLinear()
    {
        var sift = EmpiricalModeDecomposition.Sift(_lineAndSine, LinearSpline.Interpolate);

        sift.Decomposition
            .Should()
            .HaveCount(_lineAndSine.Length);
    }

    [Test]
    public void Decompose_SineAndLinear()
    {
        EmpiricalModeDecomposition.Decompose(_lineAndSine, CubicSpline.InterpolatePchipSorted, 1)
            .Should()
            .HaveCountGreaterThan(2);
    }
}