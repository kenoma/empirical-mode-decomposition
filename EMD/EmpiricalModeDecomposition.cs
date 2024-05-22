using System.Runtime.CompilerServices;
using MathNet.Numerics.Interpolation;

[assembly: InternalsVisibleTo("EMD.UnitTests")]

namespace EMD;

public static class EmpiricalModeDecomposition
{
    public static IEnumerable<double[]> Decompose(IReadOnlyList<double> input,
        Func<double[], double[], IInterpolation> interpolation, double stdThreshold)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(stdThreshold);
        ArgumentNullException.ThrowIfNull(interpolation);
        ArgumentNullException.ThrowIfNull(input);

        var r = input.ToArray();
        while (!IsOneOrLessExtrema(r))
        {
            var std = stdThreshold;
            var hCurrent = r.ToArray();
            while (std >= stdThreshold)
            {
                (std, var hNext) = Sift(hCurrent, interpolation);
                hCurrent = hNext;
            }

            yield return hCurrent;
            r = Substract(r, hCurrent);
        }

        yield return r;
    }

    internal static double[] Substract(IReadOnlyList<double> r, IReadOnlyList<double> h)
    {
        return r.Zip(h, (x, y) => (x - y)).ToArray();
    }

    internal static bool IsOneOrLessExtrema(IReadOnlyList<double> x)
    {
        var extremaCount = 0;
        for (var i = 1; i < x.Count - 1; i++)
        {
            if ((x[i - 1] < x[i] && x[i + 1] <= x[i]) || (x[i - 1] <= x[i] && x[i + 1] < x[i]))
            {
                extremaCount++;
            }
            else if ((x[i - 1] >= x[i] && x[i + 1] > x[i]) || (x[i - 1] > x[i] && x[i + 1] >= x[i]))
            {
                extremaCount++;
            }

            if (extremaCount > 1)
                return false;
        }

        return true;
    }

    internal static (double Std, double[] Decomposition) Sift(IReadOnlyList<double> hk,
        Func<double[], double[], IInterpolation> interpolation)
    {
        var maxX = new List<double>();
        var maxY = new List<double>();
        var minX = new List<double>();
        var minY = new List<double>();


        for (var i = 1; i < hk.Count - 1; i++)
        {
            if ((hk[i - 1] < hk[i] && hk[i + 1] <= hk[i]) || (hk[i - 1] <= hk[i] && hk[i + 1] < hk[i]))
            {
                maxX.Add(i);
                maxY.Add(hk[i]);
            }
            else if ((hk[i - 1] >= hk[i] && hk[i + 1] > hk[i]) || (hk[i - 1] > hk[i] && hk[i + 1] >= hk[i]))
            {
                minX.Add(i);
                minY.Add(hk[i]);
            }
        }

        if (maxX.Count < 3 || maxY.Count < 3)
            return (0, hk.ToArray());

        if (hk[0] > hk[1])
        {
            maxX.Insert(0, 0);
            maxY.Insert(0, hk[0]);
            minX.Insert(0, 0);
            minY.Insert(0, minY[0]);
        }
        else
        {
            maxX.Insert(0, 0);
            maxY.Insert(0, maxY[0]);
            minX.Insert(0, 0);
            minY.Insert(0, hk[0]);
        }

        if (hk[^1] > hk[^2])
        {
            maxX.Add(hk.Count);
            maxY.Add(hk[^1]);
            minX.Add(2 * minX[^1] - minX[^2]);
            minY.Add(minY[^1]);
        }
        else
        {
            maxX.Add(2 * maxX[^1] - maxX[^2]);
            maxY.Add(maxY[^1]);
            minX.Add(hk.Count);
            minY.Add(hk[^1]);
        }

        var U = interpolation(maxX.ToArray(), maxY.ToArray());
        var L = interpolation(minX.ToArray(), minY.ToArray());
        var retval = new double[hk.Count];
        var std = 0.0;
        for (var i = 0; i < hk.Count; i++)
        {
            var m = 0.5 * (U.Interpolate(i) + L.Interpolate(i));
            retval[i] = hk[i] - m;
            std += Math.Pow(m / retval[i], 2);
        }

        return (std, retval);
    }
}