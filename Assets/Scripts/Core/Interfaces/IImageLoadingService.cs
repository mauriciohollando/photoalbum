using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IImageLoadingService : IService
{
    Task<Texture2D> LoadImageAsync(string url);
}
