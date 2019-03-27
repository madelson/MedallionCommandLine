using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Medallion.CommandLine
{
    internal static class Helpers
    {
        public static object InvokeWithOriginalException(this MethodInfo method, object obj, object[] arguments)
        {
            try { return method.Invoke(obj, arguments); }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw; // will never get here
            }
        }

        public static object InvokeWithOriginalException(this ConstructorInfo constructor, object[] arguments)
        {
            try { return constructor.Invoke(arguments); }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw; // will never get here
            }
        }
    }
}
