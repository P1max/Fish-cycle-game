namespace _Project.Core.Interfaces
{
    public interface IInit
    {
        void Init();
    }

    public interface ICoreInit : IInit
    {
    }

    public interface IGameplayInit : IInit
    {
    }

    public interface IUIInit : IInit
    {
    }
}