using InnerCore.Api.DeConz.Models;
using Newtonsoft.Json.Linq;
using System;

namespace InnerCore.Api.DeConz.ColorConverters.HSB.Extensions
{
    public static class StateExtensions
    {
        public static RGBColor ToRgb(this State state)
        {
            HSB hsb = new(state.Hue ?? 0, state.Saturation ?? 0, state.Brightness);
            return hsb.GetRGB();
        }

        public static string ToHex(this State state)
        {
            if (state.Hue != null)
                return state.ToRgb().ToHex();
            if (state.ColorCoordinates != null)
                return state.CoordinatesToHex();
            return "";
        }

        public static string CoordinatesToHex(this State state)
        {
            var x = state.ColorCoordinates[0];
            var y = state.ColorCoordinates[1];
            var z = 1.0 - x - y;

            var Y = state.Brightness / 255.0; // Brightness of lamp
            var X = (Y / y) * x;
            var Z = (Y / y) * z;
            var r = X * 1.612 - Y * 0.203 - Z * 0.302;
            var g = -X * 0.509 + Y * 1.412 + Z * 0.066;
            var b = X * 0.026 - Y * 0.072 + Z * 0.962;
            r = r <= 0.0031308 ? 12.92 * r : (1.0 + 0.055) * Math.Pow(r, (1.0 / 2.4)) - 0.055;
            g = g <= 0.0031308 ? 12.92 * g : (1.0 + 0.055) * Math.Pow(g, (1.0 / 2.4)) - 0.055;
            b = b <= 0.0031308 ? 12.92 * b : (1.0 + 0.055) * Math.Pow(b, (1.0 / 2.4)) - 0.055;

            var maxValue = Math.Max(r, g);
            maxValue = Math.Max(maxValue, b);
            r /= maxValue;
            g /= maxValue;
            b /= maxValue;
            r *= 255; if (r < 0) { r = 255; };
            g *= 255; if (g < 0) { g = 255; };
            b *= 255; if (b < 0) { b = 255; };

            var rstring = ((int)Math.Round(r)).ToString("X");
            var gstring = ((int)Math.Round(g)).ToString("X");
            var bstring = ((int)Math.Round(b)).ToString("X");

            if (rstring.Length < 2)
                rstring = "0" + r;
            if (gstring.Length < 2)
                gstring = "0" + g;
            if (bstring.Length < 2)
                bstring = "0" + r;
            var rgb = "#" + rstring + gstring + bstring;
            return rgb;



        }


    }
}
