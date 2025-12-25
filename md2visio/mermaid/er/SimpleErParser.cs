using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using md2visio.struc.er;

namespace md2visio.mermaid.er
{
    /// <summary>
    /// 简化的ER图解析器 - 使用正则表达式直接解析基本语法
    /// </summary>
    internal class SimpleErParser
    {
        // Entity definition: entity_name {
        private static readonly Regex EntityStartRegex = new Regex(
            @"^\s*(\w+)\s*\{\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Attribute: type name [constraint] "comment"
        private static readonly Regex AttributeRegex = new Regex(
            @"^\s*(\w+)\s+(\w+)(?:\s+(\w+))?\s*(?:""([^""]*)"")?\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        // Relationship: entity1 ||..o{ entity2 : label
        private static readonly Regex RelationshipRegex = new Regex(
            @"^\s*(\w+)\s+(\|\||o\||\|\{|o\{)(\.\.|--)(\|\||o\||\|\{|o\{)\s+(\w+)\s*:\s*(\w+)\s*$",
            RegexOptions.Compiled | RegexOptions.Multiline);

        public ErDiagram Parse(string content)
        {
            var diagram = new ErDiagram();
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"[SimpleErParser] Parsing {lines.Length} lines");

            Entity? currentEntity = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines and erDiagram keyword
                if (string.IsNullOrWhiteSpace(trimmedLine) ||
                    trimmedLine == "erDiagram" ||
                    trimmedLine.StartsWith("```"))
                {
                    continue;
                }

                // Check for entity end
                if (trimmedLine == "}")
                {
                    if (currentEntity != null)
                    {
                        Console.WriteLine($"  Finished entity: {currentEntity.Name} with {currentEntity.Attributes.Count} attributes");
                        currentEntity = null;
                    }
                    continue;
                }

                // Try to parse as entity start
                var entityMatch = EntityStartRegex.Match(trimmedLine);
                if (entityMatch.Success)
                {
                    string entityName = entityMatch.Groups[1].Value;
                    currentEntity = new Entity(entityName);
                    diagram.Entities.Add(currentEntity);
                    Console.WriteLine($"  Found entity: {entityName}");
                    continue;
                }

                // If we're inside an entity, parse attributes
                if (currentEntity != null)
                {
                    var attrMatch = AttributeRegex.Match(trimmedLine);
                    if (attrMatch.Success)
                    {
                        string type = attrMatch.Groups[1].Value;
                        string name = attrMatch.Groups[2].Value;
                        string constraint = attrMatch.Groups[3].Success ? attrMatch.Groups[3].Value : "";
                        string comment = attrMatch.Groups[4].Success ? attrMatch.Groups[4].Value : "";

                        var attribute = new EntityAttribute(type, name, constraint, comment);
                        currentEntity.Attributes.Add(attribute);
                        Console.WriteLine($"    Attribute: {type} {name} {constraint} \"{comment}\"");
                        continue;
                    }
                }

                // Try to parse as relationship
                var relMatch = RelationshipRegex.Match(trimmedLine);
                if (relMatch.Success)
                {
                    string entity1 = relMatch.Groups[1].Value;
                    string leftCard = relMatch.Groups[2].Value;
                    string relType = relMatch.Groups[3].Value;
                    string rightCard = relMatch.Groups[4].Value;
                    string entity2 = relMatch.Groups[5].Value;
                    string label = relMatch.Groups[6].Value;

                    var fromCard = ParseCardinality(leftCard);
                    var toCard = ParseCardinality(rightCard);
                    var type = relType == ".." ? RelationshipType.NonIdentifying : RelationshipType.Identifying;

                    var relationship = new Relationship(entity1, entity2, fromCard, toCard, type, label);
                    diagram.Relationships.Add(relationship);
                    Console.WriteLine($"  Relationship: {entity1} {leftCard}{relType}{rightCard} {entity2} : {label}");
                    continue;
                }

                // Log unrecognized lines
                if (!string.IsNullOrWhiteSpace(trimmedLine))
                {
                    Console.WriteLine($"  ! Unrecognized line: {trimmedLine}");
                }
            }

            Console.WriteLine($"[SimpleErParser] Result: {diagram.Entities.Count} entities, {diagram.Relationships.Count} relationships");
            return diagram;
        }

        private Cardinality ParseCardinality(string symbol)
        {
            return symbol switch
            {
                "||" => Cardinality.ExactlyOne,
                "o|" => Cardinality.ZeroOrOne,
                "|{" => Cardinality.OneOrMore,
                "o{" => Cardinality.ZeroOrMore,
                _ => Cardinality.ExactlyOne
            };
        }
    }
}
