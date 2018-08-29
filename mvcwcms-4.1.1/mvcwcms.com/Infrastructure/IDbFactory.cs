using MVCwCMS.Data;
using System;

namespace MVCwCMS.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        StoreEntities Init();
    }
}