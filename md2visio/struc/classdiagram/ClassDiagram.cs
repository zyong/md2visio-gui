using md2visio.struc.figure;

namespace md2visio.struc.classdiagram
{
    /// <summary>
    /// 类图数据结构
    /// </summary>
    internal class ClassDiagram : Figure
    {
        public List<ClassEntity> Classes { get; set; } = new List<ClassEntity>();
        public List<ClassRelationship> Relationships { get; set; } = new List<ClassRelationship>();

        public override void ToVisio(string path)
        {
            new md2visio.vsdx.VBuilderClass(this).Build(path);
        }
    }

    /// <summary>
    /// 类实体
    /// </summary>
    internal class ClassEntity
    {
        public string Name { get; set; }
        public ClassStereotype Stereotype { get; set; } = ClassStereotype.None;
        public List<ClassMember> Attributes { get; set; } = new List<ClassMember>();
        public List<ClassMember> Methods { get; set; } = new List<ClassMember>();

        // Position for Visio drawing
        public double X { get; set; }
        public double Y { get; set; }

        public ClassEntity(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 类成员（属性或方法）
    /// </summary>
    internal class ClassMember
    {
        public string Name { get; set; }
        public string Type { get; set; }  // 返回类型或属性类型
        public AccessModifier Access { get; set; } = AccessModifier.Public;
        public bool IsStatic { get; set; } = false;
        public bool IsAbstract { get; set; } = false;

        public ClassMember(string name, string type = "", AccessModifier access = AccessModifier.Public)
        {
            Name = name;
            Type = type;
            Access = access;
        }
    }

    /// <summary>
    /// 访问修饰符
    /// </summary>
    internal enum AccessModifier
    {
        Public,     // +
        Private,    // -
        Protected,  // #
        Package     // ~
    }

    /// <summary>
    /// 类的刻板印象（stereotype）
    /// </summary>
    internal enum ClassStereotype
    {
        None,
        Interface,      // <<interface>>
        Abstract,       // <<abstract>>
        Enumeration     // <<enumeration>>
    }

    /// <summary>
    /// 类关系
    /// </summary>
    internal class ClassRelationship
    {
        public string From { get; set; }
        public string To { get; set; }
        public RelationType Type { get; set; }
        public string Label { get; set; }
        public string FromMultiplicity { get; set; }  // "1", "0..1", "1..*", "*"
        public string ToMultiplicity { get; set; }

        public ClassRelationship(string from, string to, RelationType type, string label = "")
        {
            From = from;
            To = to;
            Type = type;
            Label = label;
            FromMultiplicity = "";
            ToMultiplicity = "";
        }
    }

    /// <summary>
    /// UML 关系类型
    /// </summary>
    internal enum RelationType
    {
        Inheritance,     // <|--  继承（泛化）
        Implementation,  // <|..  实现接口
        Composition,     // *--   组合
        Aggregation,     // o--   聚合
        Association,     // -->   关联
        Dependency       // ..>   依赖
    }
}
