using MailAppMAUI.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.ContextProvider
{
    public interface IDbContextProvider
    {
        Context GetContext();

    }
}
