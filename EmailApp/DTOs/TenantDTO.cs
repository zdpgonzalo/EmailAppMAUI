using MailAppMAUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailAppMAUI.DTOs
{
    public class TenantDTO : BaseDTO, IComparable<TenantDTO>
    {
        public int CompareTo(TenantDTO? other)
        {
            throw new NotImplementedException();
        }
    }
}
