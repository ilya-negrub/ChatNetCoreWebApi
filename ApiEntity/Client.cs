using System;
using System.Collections.Generic;
using System.Text;

namespace ApiEntity
{
    public interface IClient
    {
        Guid Id { get; set; }
        string Name { get; set; }

    }
}
