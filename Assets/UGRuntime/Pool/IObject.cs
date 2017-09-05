namespace UGFramework.Pool
{
    public interface IObject 
    {
        void Alloc(params object[] args);    
        void Dealloc();    
    }
}