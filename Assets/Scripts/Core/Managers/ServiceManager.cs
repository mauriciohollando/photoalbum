using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceManager
{
    private Dictionary<Type, IService> serviceRegistry = new Dictionary<Type, IService>();

    public void RegisterService<T>(T service) where T : IService
    {
        var type = typeof(T);
        if (!serviceRegistry.ContainsKey(type))
        {
            serviceRegistry.Add(type, service);
            Debug.Log($"Service of type {type.Name} registered.");
        }
        else
        {
            Debug.LogWarning($"Service of type {type.Name} is already registered.");
        }
    }

    public T GetService<T>() where T : IService
    {
        var type = typeof(T);
        if (serviceRegistry.ContainsKey(type))
        {
            return (T)serviceRegistry[type];
        }
        else
        {
            Debug.LogError($"Service of type {type.Name} is not registered.");
            return default(T);
        }
    }
}
