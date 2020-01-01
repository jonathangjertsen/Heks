using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    // https://stackoverflow.com/a/8509802
    public static Color ShiftHue(this Color input, float shiftAngleRad)
    {
        float u = Mathf.Cos(shiftAngleRad * Mathf.PI);
        float w = Mathf.Sin(shiftAngleRad * Mathf.PI);

        var result = new Color(
            r: (.299f + .701f * u + .168f * w) * input.r
              + (.587f - .587f * u + .330f * w) * input.g
              + (.114f - .114f * u - .497f * w) * input.b,
            g: (.299f - .299f * u - .328f * w) * input.r
               + (.587f + .413f * u + .035f * w) * input.g
               + (.114f - .114f * u + .292f * w) * input.b,
            b: (.299f - .3f * u + 1.25f * w) * input.r
              + (.587f - .588f * u - 1.05f * w) * input.g
              + (.114f + .886f * u - .203f * w) * input.b
        );
        result.a = input.a;
        return result;
    }
}
