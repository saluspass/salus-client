namespace ipfs_pswmgr
{
    public interface IDirtableObject
    {
        bool Dirty
        {
            get;
            set;
        }
    }
}
