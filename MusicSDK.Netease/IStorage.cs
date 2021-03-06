namespace MusicSDK.Netease
{
    public interface IStorage
    {
        Session? LoadSession();
        
        void SaveSession(Session session);

        void ClearSession();
    }
}