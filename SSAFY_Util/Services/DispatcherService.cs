using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SSAFY_Util.Services
{
    public static class DispatcherService
    {
        public static void Invoke(Action action)
        {
            Dispatcher? dispatchObject = System.Windows.Application.Current?.Dispatcher ?? null;
            if (dispatchObject == null || dispatchObject.CheckAccess())
                action();
            else
                dispatchObject.Invoke(action);
        }
    }
}
