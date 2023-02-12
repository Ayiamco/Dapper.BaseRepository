namespace Dapper.BaseRepository.Attributes
{
    /// <summary>
    /// Attribute for DbType <see cref="DbType.String"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnString : Attribute
    {
        public int Length { get; set; }

        public SpReturnString(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.StringFixedLength"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnStringFixed : Attribute
    {
        public int Length { get; set; }

        public SpReturnStringFixed(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.AnsiString"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnAnsiString : Attribute
    {
        public int Length { get; set; }

        public SpReturnAnsiString(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.AnsiStringFixedLength"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnAnsiStringFixed : Attribute
    {
        public int Length { get; set; }

        public SpReturnAnsiStringFixed(int Length)
        {
            this.Length = Length;
        }
    }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Int32"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnInt : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Int64"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnBigInt : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.DateTime"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnDateTime : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Date"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnDate : Attribute { }

    /// <summary>
    /// Attribute for DbType <see cref="DbType.Guid"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnGuid : Attribute { }
}

