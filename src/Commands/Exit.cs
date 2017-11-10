namespace ipfs_pswmgr
{
    class ExitCommand : AbstractCommand
    {
        protected override void Execute()
        {
            App.Current.Shutdown();
        }
    }
}
