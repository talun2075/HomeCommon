using System;

namespace InnerCore.Api.DeConz.ColorConverters.HSB
{
    /// <summary>
    /// Based on code contributed by https://github.com/CharlyTheKid
    /// </summary>
    public static class RGBExtensions
    {
        public static HSB GetHSB(this RGBColor rgb)
        {
            var hsb = new HSB
            (
                (int)rgb.GetHue(),
                (int)rgb.GetSaturation(),
                (int)rgb.GetBrightness()
            );
            return hsb;
        }

        public static double[] GetCoordinates(this RGBColor rgb)
        {
            double[] coords = new double[2];
            var red = rgb.R; 
            var green = rgb.G;
            var blue = rgb.B;
            if (red > 0.04045)
            {
                red = Math.Pow((red + 0.055) / (1.0 + 0.055), 2.4);
            }
            else red /= 12.92;

            if (green > 0.04045)
            {
                green = Math.Pow((green + 0.055) / (1.0 + 0.055), 2.4);
            }
            else green /= 12.92;

            if (blue > 0.04045)
            {
                blue = Math.Pow((blue + 0.055) / (1.0 + 0.055), 2.4);
            }
            else blue /= 12.92;

            var X = red * 0.664511 + green * 0.154324 + blue * 0.162028;
            var Y = red * 0.283881 + green * 0.668433 + blue * 0.047685;
            var Z = red * 0.000088 + green * 0.072310 + blue * 0.986039;
            var x = X / (X + Y + Z);
            var y = Y / (X + Y + Z);
            coords[0] = x;
            coords[1] = y;
            return coords;
        }

        public static double GetHue(this RGBColor rgb)
        {
            var R = rgb.R;
            var G = rgb.G;
            var B = rgb.B;

            if (R == G && G == B)
                return 0;

            double hue;

            var min = Numbers.Min(R, G, B);
            var max = Numbers.Max(R, G, B);

            var delta = max - min;

            if (R.AlmostEquals(max))
                hue = (G - B) / delta; // between yellow & magenta
            else if (G.AlmostEquals(max))
                hue = 2 + (B - R) / delta; // between cyan & yellow
            else
                hue = 4 + (R - G) / delta; // between magenta & cyan

            hue *= 60; // degrees

            if (hue < 0)
                hue += 360;

            return hue * 182.04f;
        }

        public static double GetSaturation(this RGBColor rgb)
        {
            var R = rgb.R;
            var G = rgb.G;
            var B = rgb.B;

            var min = Numbers.Min(R, G, B);
            var max = Numbers.Max(R, G, B);

            if (max.AlmostEquals(min))
                return 0;
            return ((max.AlmostEquals(0f)) ? 0f : 1f - (1f * min / max)) * 255;
        }

        public static double GetBrightness(this RGBColor rgb)
        {
            var R = rgb.R;
            var G = rgb.G;
            var B = rgb.B;

            return Numbers.Max(R, G, B) * 255;
        }
    }
}
