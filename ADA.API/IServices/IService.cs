using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADA.API.IServices
{
    public interface IService<T> where T:class
    {
        List<T> GetAll();
    }
}
