using System.Text.RegularExpressions;
using md2visio.struc.classdiagram;

namespace md2visio.mermaid.classdiagram
{
    /// <summary>
    /// Mermaid 类图解析器
    /// </summary>
    internal class SimpleClassParser
    {
        // Class definition: class ClassName {
        private static readonly Regex ClassStartRegex = new Regex(
            @"^\s*class\s+([\w~<>,]+)\s*\{\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Stereotype: <<interface>>, <<enumeration>>, <<abstract>>
        private static readonly Regex StereotypeRegex = new Regex(
            @"^\s*<<(\w+)>>\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Member (attribute or method): +String name or +method()
        private static readonly Regex MemberRegex = new Regex(
            @"^\s*([+\-#~])?([\w<>,~]+\s+)?(\w+)(\([^)]*\))?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Enum value: ACTIVE, DISABLED, etc.
        private static readonly Regex EnumValueRegex = new Regex(
            @"^\s*([A-Z_][A-Z0-9_]*)\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Relationships
        private static readonly Regex RelationshipRegex = new Regex(
            @"^\s*([\w~<>,]+)\s+(""[^""]*"")?\s*(<\|\-\-|<\|\.\.|\*\-\-|o\-\-|\-\->|\.\.>)\s+(""[^""]*"")?\s*([\w~<>,]+)\s*(?::\s*(.+))?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Relationship with multiplicity: Order "1" *-- "1..*" OrderItem : 包含
        private static readonly Regex RelationshipWithMultiplicityRegex = new Regex(
            @"^\s*([\w~<>,]+)\s+""([^""]+)""\s+(<\|\-\-|<\|\.\.|\*\-\-|o\-\-|\-\->|\.\.>)\s+""([^""]+)""\s+([\w~<>,]+)\s*(?::\s*(.+))?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        public ClassDiagram Parse(string content)
        {
            var diagram = new ClassDiagram();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"[SimpleClassParser] Parsing {lines.Length} lines");

            ClassEntity? currentClass = null;
            bool inEnumeration = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines, comments, and classDiagram keyword
                if (string.IsNullOrWhiteSpace(trimmedLine) ||
                    trimmedLine.StartsWith("%%") ||
                    trimmedLine == "classDiagram" ||
                    trimmedLine.StartsWith("direction") ||
                    trimmedLine.StartsWith("```"))
                {
                    continue;
                }

                // Check for class end
                if (trimmedLine == "}")
                {
                    if (currentClass != null)
                    {
                        Console.WriteLine($"  Finished class: {currentClass.Name} with {currentClass.Attributes.Count} attributes and {currentClass.Methods.Count} methods");
                        currentClass = null;
                        inEnumeration = false;
                    }
                    continue;
                }

                // Try to parse as class start
                var classMatch = ClassStartRegex.Match(trimmedLine);
                if (classMatch.Success)
                {
                    string className = classMatch.Groups[1].Value;
                    currentClass = new ClassEntity(className);
                    diagram.Classes.Add(currentClass);
                    Console.WriteLine($"  Found class: {className}");
                    continue;
                }

                // If we're inside a class definition
                if (currentClass != null)
                {
                    // Check for stereotype
                    var stereoMatch = StereotypeRegex.Match(trimmedLine);
                    if (stereoMatch.Success)
                    {
                        string stereoType = stereoMatch.Groups[1].Value.ToLower();
                        currentClass.Stereotype = stereoType switch
                        {
                            "interface" => ClassStereotype.Interface,
                            "abstract" => ClassStereotype.Abstract,
                            "enumeration" => ClassStereotype.Enumeration,
                            _ => ClassStereotype.None
                        };
                        inEnumeration = currentClass.Stereotype == ClassStereotype.Enumeration;
                        Console.WriteLine($"    Stereotype: {stereoType}");
                        continue;
                    }

                    // If it's an enumeration, parse enum values
                    if (inEnumeration)
                    {
                        var enumMatch = EnumValueRegex.Match(trimmedLine);
                        if (enumMatch.Success)
                        {
                            string enumValue = enumMatch.Groups[1].Value;
                            currentClass.Attributes.Add(new ClassMember(enumValue, "", AccessModifier.Public));
                            Console.WriteLine($"    Enum value: {enumValue}");
                            continue;
                        }
                    }

                    // Parse member (attribute or method)
                    var memberMatch = MemberRegex.Match(trimmedLine);
                    if (memberMatch.Success)
                    {
                        string accessSymbol = memberMatch.Groups[1].Success ? memberMatch.Groups[1].Value : "+";
                        string memberType = memberMatch.Groups[2].Success ? memberMatch.Groups[2].Value.Trim() : "";
                        string memberName = memberMatch.Groups[3].Value;
                        bool isMethod = memberMatch.Groups[4].Success; // has ()

                        AccessModifier access = accessSymbol switch
                        {
                            "+" => AccessModifier.Public,
                            "-" => AccessModifier.Private,
                            "#" => AccessModifier.Protected,
                            "~" => AccessModifier.Package,
                            _ => AccessModifier.Public
                        };

                        var member = new ClassMember(memberName, memberType, access);

                        if (isMethod)
                        {
                            currentClass.Methods.Add(member);
                            Console.WriteLine($"    Method: {accessSymbol}{memberType} {memberName}()");
                        }
                        else
                        {
                            currentClass.Attributes.Add(member);
                            Console.WriteLine($"    Attribute: {accessSymbol}{memberType} {memberName}");
                        }
                        continue;
                    }
                }

                // Try to parse as relationship (with multiplicity)
                var multiRelMatch = RelationshipWithMultiplicityRegex.Match(trimmedLine);
                if (multiRelMatch.Success)
                {
                    string from = multiRelMatch.Groups[1].Value;
                    string fromMult = multiRelMatch.Groups[2].Value;
                    string relSymbol = multiRelMatch.Groups[3].Value;
                    string toMult = multiRelMatch.Groups[4].Value;
                    string to = multiRelMatch.Groups[5].Value;
                    string label = multiRelMatch.Groups[6].Success ? multiRelMatch.Groups[6].Value : "";

                    var relType = ParseRelationType(relSymbol);
                    var rel = new ClassRelationship(from, to, relType, label)
                    {
                        FromMultiplicity = fromMult,
                        ToMultiplicity = toMult
                    };
                    diagram.Relationships.Add(rel);
                    Console.WriteLine($"  Relationship: {from} {fromMult} {relSymbol} {toMult} {to} : {label}");
                    continue;
                }

                // Try to parse as simple relationship
                var relMatch = RelationshipRegex.Match(trimmedLine);
                if (relMatch.Success)
                {
                    string from = relMatch.Groups[1].Value;
                    string relSymbol = relMatch.Groups[3].Value;
                    string to = relMatch.Groups[5].Value;
                    string label = relMatch.Groups[6].Success ? relMatch.Groups[6].Value : "";

                    var relType = ParseRelationType(relSymbol);
                    var rel = new ClassRelationship(from, to, relType, label);
                    diagram.Relationships.Add(rel);
                    Console.WriteLine($"  Relationship: {from} {relSymbol} {to} : {label}");
                }
            }

            Console.WriteLine($"[SimpleClassParser] Parsed {diagram.Classes.Count} classes and {diagram.Relationships.Count} relationships");
            return diagram;
        }

        private RelationType ParseRelationType(string symbol)
        {
            return symbol switch
            {
                "<|--" => RelationType.Inheritance,
                "<|.." => RelationType.Implementation,
                "*--" => RelationType.Composition,
                "o--" => RelationType.Aggregation,
                "-->" => RelationType.Association,
                "..>" => RelationType.Dependency,
                _ => RelationType.Association
            };
        }
    }
}
