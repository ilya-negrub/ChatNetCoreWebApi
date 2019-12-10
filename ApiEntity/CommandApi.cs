using System;
using System.Collections.Generic;
using System.Text;

namespace ApiEntity
{
    public class CommandApi
    {
        public virtual string TypeName { get; set; }
    }

    public class CommandApi<T> : CommandApi
    {
        public T Data { get; set; }
    }
}
