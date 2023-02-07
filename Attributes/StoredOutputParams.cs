using System.Data;

namespace Dapper.BaseRepository.Attributes
{
    /// <summary>
    /// Attribute for DbType <see cref="DbType.String"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputString : Attribute
    {
        public int Length { get; set; }

        public SpOutputString(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.StringFixedLength"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputStringFixed : Attribute
    {
        public int Length { get; set; }

        public SpOutputStringFixed(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.AnsiString"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputAnsiString : Attribute
    {
        public int Length { get; set; }

        public SpOutputAnsiString(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.AnsiStringFixedLength"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputAnsiStringFixed : Attribute
    {
        public int Length { get; set; }

        public SpOutputAnsiStringFixed(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Int32"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputInt : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Int64"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputBigInt : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.DateTime"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputDateTime : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Date"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputDate : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Guid"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpOutputGuid : Attribute { }
}
