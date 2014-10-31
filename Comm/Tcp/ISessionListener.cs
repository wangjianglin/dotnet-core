using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lin.Comm.Tcp
{
    public interface ISessionListener
    {
        void Create(Session session);

        void Destory(Session session);
    }
}
