namespace ipfs_pswmgr
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
