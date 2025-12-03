using System.Windows.Media;

namespace Pipboy.Wallpaper.Utils
{
    public static class ColorUtils
    {
        public static string ColorToHex(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        public static string NormalizeColorHex(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Color hex cannot be null or empty.");

            var hex = input.Trim();
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6 && hex.Length != 8)
                throw new ArgumentException("Color hex must be 6 (RRGGBB) or 8 (AARRGGBB) hex digits.");

            if (hex.Length == 6)
                hex = "FF" + hex;

            _ = Convert.ToUInt32(hex, 16);

            return "#" + hex.ToUpperInvariant();
        }

        public static Color HexToColor(string hexWithHash)
        {
            if (string.IsNullOrWhiteSpace(hexWithHash))
                throw new ArgumentException("Color hex string cannot be null or empty.");

            var hex = hexWithHash.TrimStart('#');

            if (hex.Length == 6)
            {
                var r = Convert.ToByte(hex.Substring(0, 2), 16);
                var g = Convert.ToByte(hex.Substring(2, 2), 16);
                var b = Convert.ToByte(hex.Substring(4, 2), 16);
                return Color.FromArgb(255, r, g, b);
            }
            else if (hex.Length == 8)
            {
                var a = Convert.ToByte(hex.Substring(0, 2), 16);
                var r = Convert.ToByte(hex.Substring(2, 2), 16);
                var g = Convert.ToByte(hex.Substring(4, 2), 16);
                var b = Convert.ToByte(hex.Substring(6, 2), 16);
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                throw new ArgumentException($"Invalid color hex format: {hexWithHash}. Expected #RRGGBB or #AARRGGBB.");
            }
        }
    }
}
