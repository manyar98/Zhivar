using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace OMF.Common.Extensions
{
    public static class DynamicTypeBuilder
    {
        private static AssemblyName assemblyName = new AssemblyName()
        {
            Name = "DynamicLinqTypes"
        };
        private static ModuleBuilder moduleBuilder = (ModuleBuilder)null;
        private static Dictionary<string, Tuple<string, Type>> builtTypes = new Dictionary<string, Tuple<string, Type>>();

        static DynamicTypeBuilder()
        {
            DynamicTypeBuilder.moduleBuilder = Thread.GetDomain().DefineDynamicAssembly(DynamicTypeBuilder.assemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(DynamicTypeBuilder.assemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> fields)
        {
            string str = string.Empty;
            foreach (KeyValuePair<string, Type> keyValuePair in (IEnumerable<KeyValuePair<string, Type>>)fields.OrderBy<KeyValuePair<string, Type>, string>((Func<KeyValuePair<string, Type>, string>)(v => v.Key)).ThenBy<KeyValuePair<string, Type>, string>((Func<KeyValuePair<string, Type>, string>)(v => v.Value.Name)))
                str = str + keyValuePair.Key + ";" + keyValuePair.Value.Name + ";";
            return str;
        }

        public static Type GetDynamicType(
          Dictionary<string, Type> fields,
          Type basetype,
          Type[] interfaces)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            if (fields.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(fields), "fields must have at least 1 field definition");
            try
            {
                Monitor.Enter((object)DynamicTypeBuilder.builtTypes);
                string typeKey = DynamicTypeBuilder.GetTypeKey(fields);
                if (DynamicTypeBuilder.builtTypes.ContainsKey(typeKey))
                    return DynamicTypeBuilder.builtTypes[typeKey].Item2;
                string name = "DynamicLinqType" + DynamicTypeBuilder.builtTypes.Count.ToString();
                TypeBuilder typeBuilder = DynamicTypeBuilder.moduleBuilder.DefineType(name, TypeAttributes.Public | TypeAttributes.Serializable, (Type)null, Type.EmptyTypes);
                foreach (KeyValuePair<string, Type> field in fields)
                    typeBuilder.DefineField(field.Key, field.Value, FieldAttributes.Public);
                DynamicTypeBuilder.builtTypes[typeKey] = new Tuple<string, Type>(name, typeBuilder.CreateType());
                return DynamicTypeBuilder.builtTypes[typeKey].Item2;
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit((object)DynamicTypeBuilder.builtTypes);
            }
        }
    }
}
