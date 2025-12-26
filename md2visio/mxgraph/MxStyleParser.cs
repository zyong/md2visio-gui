using System.Text.RegularExpressions;

namespace md2visio.mxgraph
{
    /// <summary>
    /// Parser for mxGraph style strings
    /// Example: "ellipse;whiteSpace=wrap;html=1;fillColor=#dae8fc;strokeColor=#6c8ebf;"
    /// </summary>
    internal class MxStyleParser
    {
        public Dictionary<string, string> Parse(string? styleString)
        {
            var styles = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(styleString))
                return styles;

            // Split by semicolon
            var parts = styleString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                // Check if it contains '='
                if (trimmed.Contains('='))
                {
                    var kvp = trimmed.Split(new[] { '=' }, 2);
                    if (kvp.Length == 2)
                    {
                        styles[kvp[0].Trim()] = kvp[1].Trim();
                    }
                }
                else
                {
                    // Shape name without value (e.g., "ellipse", "rhombus")
                    styles["shape"] = trimmed;
                }
            }

            return styles;
        }

        public string? GetFillColor(Dictionary<string, string> styles)
        {
            return styles.ContainsKey("fillColor") ? styles["fillColor"] : null;
        }

        public string? GetStrokeColor(Dictionary<string, string> styles)
        {
            return styles.ContainsKey("strokeColor") ? styles["strokeColor"] : null;
        }

        public string? GetShape(Dictionary<string, string> styles)
        {
            return styles.ContainsKey("shape") ? styles["shape"] : null;
        }

        public bool IsRounded(Dictionary<string, string> styles)
        {
            return styles.ContainsKey("rounded") && styles["rounded"] == "1";
        }

        /// <summary>
        /// Convert hex color to RGB format for Visio
        /// </summary>
        public static string? ConvertColorToRGB(string? hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return null;

            // Remove '#' if present
            string hex = hexColor.StartsWith("#") ? hexColor.Substring(1) : hexColor;

            if (hex.Length == 6)
            {
                try
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    return $"RGB({r}, {g}, {b})";
                }
                catch
                {
                    return null;
                }
            }
            else if (hex.Length == 3)
            {
                try
                {
                    // Handle shorthand like #FFF
                    int r = Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
                    int g = Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
                    int b = Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
                    return $"RGB({r}, {g}, {b})";
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}
