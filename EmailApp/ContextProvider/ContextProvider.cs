using MailAppMAUI.Contexto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.ContextProvider
{
    public class ContextProvider : IDbContextProvider
    {
        private readonly Context _context;
        public ContextProvider(Context context)
        {
            _context = context;
        }

        public Context GetContext() => _context;
    }

}
