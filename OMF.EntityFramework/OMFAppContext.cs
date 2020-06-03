using OMF.EntityFramework.DataContext;
using System;

namespace OMF.EntityFramework
{
    public class OMFAppContext
    {
        public static string ParameterPrefix = ":";
        public static Func<IDataContextAsync> DataContextCreator;
    }
}
