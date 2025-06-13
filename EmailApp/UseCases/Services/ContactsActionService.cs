using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailApp.UseCases.Services
{
    public class ContactsActionService
    {
        public static event Action OnAddContact;

        public void TriggerAddContact()
        {
            OnAddContact?.Invoke();
        }
    }
}
