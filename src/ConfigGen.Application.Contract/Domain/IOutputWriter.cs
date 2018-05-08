using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGen.Application.Contract.Domain
{
    public interface IOutputWriter
    {
        Task Write(Configuration configuration, string contents);
    }
}
