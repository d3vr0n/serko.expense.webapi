using System;

namespace serko.expense.webapi.util
{
    public interface ISerkoLogger
    {
        void LogInfo(string message, params object[] data);
        void LogException(string message, Exception evrException, object bizData);
    }
}
