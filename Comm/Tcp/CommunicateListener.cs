using System;
using System.Runtime.CompilerServices;

namespace Lin.Comm.Tcp
{
    public delegate void CommunicateListener(Session session,Package package,Response pesonse);
}

