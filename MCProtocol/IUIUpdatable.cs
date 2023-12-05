namespace MCProtocol
{
    public interface IUIUpdatable
    {
        void UpdateConnect(int count);
        void AddCommLog(string adr, string message);
    }
}