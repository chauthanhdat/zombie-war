using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieWar.Core.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> _services = new();

        public static void Register<T>(T service) where T : IService
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogError($"Service already registered: {type}");
            }
            else
            {
                _services.Add(type, service);
                service.Initialize();
            }
        }

        public static T Get<T>() where T : IService
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                Debug.LogError($"Service not registered: {type}");
                return default;
            }
            return (T)_services[typeof(T)];
        }

    }
}
