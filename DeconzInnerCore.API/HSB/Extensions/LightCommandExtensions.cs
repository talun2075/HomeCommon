using System;
using InnerCore.Api.DeConz.Models.Lights;

namespace InnerCore.Api.DeConz.ColorConverters.HSB.Extensions
{
    public static class LightCommandExtensions
    {
        public static LightCommand SetColor(this LightCommand lightCommand, RGBColor color, bool forColorCoordinates = false)
        {
            if (lightCommand == null)
                throw new ArgumentNullException(nameof(lightCommand));

            var hsb = color.GetHSB();
            lightCommand.Brightness = (byte)hsb.Brightness;
            if (forColorCoordinates)
            {
                SetCoordinates(lightCommand, color);
            }
            else
            {
                lightCommand.Hue = hsb.Hue;
                lightCommand.Saturation = hsb.Saturation;
            }


            return lightCommand;
        }
        
        private static void SetCoordinates(LightCommand lightCommand, RGBColor color)
        {
            var red = color.R; 
            var green = color.G;
            var blue = color.B;

            if (red > 0.04045)
            {
                red = Math.Pow((red + 0.055) / (1.0 + 0.055), 2.4);
            }
            else red = (red / 12.92);

            if (green > 0.04045)
            {
                green = Math.Pow((green + 0.055) / (1.0 + 0.055), 2.4);
            }
            else green = (green / 12.92);

            if (blue > 0.04045)
            {
                blue = Math.Pow((blue + 0.055) / (1.0 + 0.055), 2.4);
            }
            else blue = (blue / 12.92);

            var X = red * 0.664511 + green * 0.154324 + blue * 0.162028;
            var Y = red * 0.283881 + green * 0.668433 + blue * 0.047685;
            var Z = red * 0.000088 + green * 0.072310 + blue * 0.986039;
            var x = X / (X + Y + Z);
            var y = Y / (X + Y + Z);
            double[] coords = new double[2];
            coords[0] = x;
            coords[1] = y;
            lightCommand.ColorCoordinates = coords;
        }

    }
}
