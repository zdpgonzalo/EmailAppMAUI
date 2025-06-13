using MailAppMAUI.Core;

namespace MailAppMAUI.UseCases
{
    public interface IAction
    {
        public Task<object> Action(string oper, object[] values);

        public object GetData(string name, object item);

        public bool SetData(string name, object item, object value);

        public object OnEvent(string @event, params object[] info);
        public object OnParentEvent(string @event, params object[] info);

        public DataResul GetDataResul();
    }
}
