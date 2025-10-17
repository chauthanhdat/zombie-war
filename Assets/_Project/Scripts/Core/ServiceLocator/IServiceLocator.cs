using System;

namespace ZombieWar.Core.ServiceLocator
{
    public interface IServiceLocator
    {
        void Register<T>(T service);
        void Register(Type type, object service);
        bool Unregister<T>();
        bool Unregister<T>(Type type);
        T Get<T>();
        object Get(Type type);
        bool IsRegistered<T>();
        bool IsRegistered(Type type);

    }
}
