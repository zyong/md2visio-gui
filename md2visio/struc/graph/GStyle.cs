using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace md2visio.struc.graph
{
    /// <summary>
    /// Represents a style class definition from classDef
    /// </summary>
    internal class GStyleClass
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public GStyleClass(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Parse style properties from string like "fill:#F5EBFF,stroke:#BE8FED,stroke-width:2px"
        /// </summary>
        public void ParseProperties(string styleString)
        {
            if (string.IsNullOrWhiteSpace(styleString)) return;

            // Split by comma and parse each property
            var props = styleString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var prop in props)
            {
                var parts = prop.Split(new[] { ':' }, 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    Properties[key] = value;
                }
            }
        }

        public string? GetProperty(string name)
        {
            return Properties.ContainsKey(name) ? Properties[name] : null;
        }
    }

    /// <summary>
    /// Stores node styling information
    /// </summary>
    internal class GNodeStyle
    {
        public string? Fill { get; set; }
        public string? Stroke { get; set; }
        public string? StrokeWidth { get; set; }
        public string? Color { get; set; } // text color
        public string? StrokeDasharray { get; set; }

        /// <summary>
        /// Apply style from a style class
        /// </summary>
        public void ApplyStyleClass(GStyleClass styleClass)
        {
            var fill = styleClass.GetProperty("fill");
            if (fill != null) Fill = fill;

            var stroke = styleClass.GetProperty("stroke");
            if (stroke != null) Stroke = stroke;

            var strokeWidth = styleClass.GetProperty("stroke-width");
            if (strokeWidth != null) StrokeWidth = strokeWidth;

            var color = styleClass.GetProperty("color");
            if (color != null) Color = color;

            var strokeDasharray = styleClass.GetProperty("stroke-dasharray");
            if (strokeDasharray != null) StrokeDasharray = strokeDasharray;
        }

        /// <summary>
        /// Parse CSS color to RGB values (hex format)
        /// Returns RGB(r, g, b) format for Visio
        /// </summary>
        public static string? ParseColorToRGB(string? color)
        {
            if (string.IsNullOrWhiteSpace(color)) return null;

            // Handle hex colors like #F5EBFF
            if (color.StartsWith("#"))
            {
                string hex = color.Substring(1);
                if (hex.Length == 6)
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    return $"RGB({r}, {g}, {b})";
                }
                else if (hex.Length == 3)
                {
                    // Handle shorthand like #FFF
                    int r = Convert.ToInt32(hex.Substring(0, 1) + hex.Substring(0, 1), 16);
                    int g = Convert.ToInt32(hex.Substring(1, 1) + hex.Substring(1, 1), 16);
                    int b = Convert.ToInt32(hex.Substring(2, 1) + hex.Substring(2, 1), 16);
                    return $"RGB({r}, {g}, {b})";
                }
            }
            // Handle rgb(r,g,b) format
            else if (color.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase))
            {
                return color.ToUpper();
            }

            return null;
        }

        /// <summary>
        /// Parse stroke-width to points (Visio format)
        /// </summary>
        public static string? ParseStrokeWidth(string? width)
        {
            if (string.IsNullOrWhiteSpace(width)) return null;

            // Handle "2px" format - convert to points
            var match = Regex.Match(width, @"(\d+(?:\.\d+)?)px");
            if (match.Success)
            {
                double pixels = double.Parse(match.Groups[1].Value);
                // Convert pixels to points (rough approximation: 1px ≈ 0.75pt)
                double points = pixels * 0.75;
                return $"{points} pt";
            }

            // Handle direct point format
            match = Regex.Match(width, @"(\d+(?:\.\d+)?)pt");
            if (match.Success)
            {
                return $"{match.Groups[1].Value} pt";
            }

            return width;
        }
    }
}
