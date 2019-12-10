using System;
using System.Collections.Generic;
using System.Text;

namespace ApiEntity
{
    public interface IMessage
    {
        Guid From { get; }
        Guid To { get; }
    }
}
