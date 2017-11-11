namespace Salus
{
    public interface ISaveableObject
    {
        bool Dirty
        {
            get;
            set;
        }
    }
}
