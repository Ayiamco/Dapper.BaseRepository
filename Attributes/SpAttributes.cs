namespace Dapper.BaseRepository.Attributes
{


    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnStringAttribute : Attribute
    {
        public int Size { get; set; }

        public SpReturnStringAttribute(int size)
        {
            Size = size;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnIntAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SpReturnBigIntAttribute : Attribute
    {

    }
}
