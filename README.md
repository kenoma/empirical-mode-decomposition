# Empirical Mode Decomposition

About algorithm you can read [here](https://en.wikipedia.org/wiki/Hilbert%E2%80%93Huang_transform#Empirical_mode_decomposition_2)

## How to use

The input signal is a linear trend with a sine wave

```csharp

var t = new double[100];
for (var i = 0; i < t.Length; i++)
{
    t[i] = i / 100.0 + Math.Sin(0.7 * i);
}

var decompositions = EmpiricalModeDecomposition.Decompose(t, CubicSpline.InterpolatePchipSorted, 1);

```
This code results in these decompositions
![image](https://github.com/kenoma/empirical-mode-decomposition/assets/6205671/c6c12be3-2296-48fc-861b-e384f60c34a9)


Any interpolation alorithm from [Math.Net](https://numerics.mathdotnet.com/api/MathNet.Numerics/Interpolate.htm) can be used.


