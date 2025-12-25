using System.Collections.Generic;
using md2visio.struc.figure;

namespace md2visio.struc.er
{
    /// <summary>
    /// ER图数据结构
    /// </summary>
    internal class ErDiagram : Figure
    {
        public List<Entity> Entities { get; set; } = new List<Entity>();
        public List<Relationship> Relationships { get; set; } = new List<Relationship>();

        public override void ToVisio(string path)
        {
            new md2visio.vsdx.VBuilderEr(this).Build(path);
        }
    }

    /// <summary>
    /// 实体
    /// </summary>
    internal class Entity
    {
        public string Name { get; set; }
        public List<EntityAttribute> Attributes { get; set; } = new List<EntityAttribute>();

        // Position for Visio drawing
        public double X { get; set; }
        public double Y { get; set; }

        public Entity(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 实体属性（字段）
    /// </summary>
    internal class EntityAttribute
    {
        public string Type { get; set; }      // uuid, string, int, bool, datetime, json
        public string Name { get; set; }
        public string Constraint { get; set; } // pk, fk, etc.
        public string Comment { get; set; }

        public EntityAttribute(string type, string name, string constraint = "", string comment = "")
        {
            Type = type;
            Name = name;
            Constraint = constraint;
            Comment = comment;
        }

        public bool IsPrimaryKey => Constraint?.Contains("pk") == true;
        public bool IsForeignKey => Constraint?.Contains("fk") == true;
    }

    /// <summary>
    /// 关系
    /// </summary>
    internal class Relationship
    {
        public string From { get; set; }
        public string To { get; set; }
        public Cardinality FromCardinality { get; set; }
        public Cardinality ToCardinality { get; set; }
        public RelationshipType Type { get; set; }
        public string Label { get; set; }

        public Relationship(string from, string to, Cardinality fromCard, Cardinality toCard, RelationshipType type, string label = "")
        {
            From = from;
            To = to;
            FromCardinality = fromCard;
            ToCardinality = toCard;
            Type = type;
            Label = label;
        }
    }

    /// <summary>
    /// 基数（一对一、一对多等）
    /// </summary>
    internal enum Cardinality
    {
        ZeroOrOne,   // o|
        ExactlyOne,  // ||
        ZeroOrMore,  // o{
        OneOrMore    // |{
    }

    /// <summary>
    /// 关系类型（标识关系或非标识关系）
    /// </summary>
    internal enum RelationshipType
    {
        Identifying,    // --
        NonIdentifying  // ..
    }
}
