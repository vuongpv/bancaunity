
using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace GFramework {

    public static class ObjectDumper {

		public static string Dump (this object o) {

			StringBuilder sb = new StringBuilder();
			Dump(sb, o, 0, new ArrayList());
			return sb.ToString();
        }

        private static string Pad (int level, string msg, params object[] args) {
            string val = String.Format (msg, args);
            return val.PadLeft ((level * 4) + val.Length);
        }

        private static void Dump (StringBuilder sb, object o, int level, ArrayList previous) {
			// Limit level deep
			if (level >= 1)
				return;

            Type type = null;

            if (o != null) {
                type = o.GetType ();
            }
            
            Dump (sb, o, type, null, level, previous);
        }
        
        private static void Dump (StringBuilder sb, object o, Type type, string name, int level, ArrayList previous) {
            if (o == null) {
				sb.AppendLine(Pad(level, "{0} ({1}): (null)", name, type.Name));
                return;
            }

            if (previous.Contains (o)) {
                return;
            }

            previous.Add (o);

            if (type.IsPrimitive || o is string) {
                DumpPrimitive (sb, o, type, name, level, previous);
            } else {
                DumpComposite (sb, o, type, name, level, previous);
            }
        }

		private static void DumpPrimitive(StringBuilder sb, object o, Type type, string name, int level, ArrayList previous)
		{
            if (name != null) {
				sb.AppendLine(Pad(level, "{0} ({1}): {2}", name, type.Name, o));
            } else {
                sb.AppendLine(Pad(level, "({0}) {1}", type.Name, o));
            }
        }

		private static void DumpComposite(StringBuilder sb, object o, Type type, string name, int level, ArrayList previous)
		{

            if (name != null) {
				sb.AppendLine(Pad(level, "{0} ({1}):", name, type.Name));
            } else {
				sb.AppendLine(Pad(level, "({0})", type.Name));
            }

            if (o is IDictionary) {
                DumpDictionary (sb, (IDictionary) o, level, previous);
            } else if (o is ICollection) {
                DumpCollection (sb, (ICollection) o, level, previous);
            } else {
                MemberInfo[] members = o.GetType ().GetMembers (BindingFlags.Instance | BindingFlags.Public |
                                                                BindingFlags.NonPublic);
                
                foreach (MemberInfo member in members) {
                    try {
                        DumpMember (sb, o, member, level, previous);
                    } catch {}
                }
            }
        }

		private static void DumpCollection(StringBuilder sb, ICollection collection, int level, ArrayList previous)
		{
            foreach (object child in collection) {
                Dump (sb, child, level + 1, previous);
            }
        }

		private static void DumpDictionary(StringBuilder sb, IDictionary dictionary, int level, ArrayList previous)
		{
            foreach (object key in dictionary.Keys) {
				sb.AppendLine(Pad(level + 1, "[{0}] ({1}):", key, key.GetType().Name));

                Dump (sb, dictionary[key], level + 2, previous);
            }
        }

		private static void DumpMember(StringBuilder sb, object o, MemberInfo member, int level, ArrayList previous)
		{
            if (member is MethodInfo || member is ConstructorInfo ||
                member is EventInfo)
                return;

            if (member is FieldInfo) {
                FieldInfo field = (FieldInfo) member;

                string name = member.Name;
                if ((field.Attributes & FieldAttributes.Public) == 0) {
                    name = "#" + name;
                }
                
                Dump (sb, field.GetValue (o), field.FieldType, name, level + 1, previous);
            } else if (member is PropertyInfo) {
                PropertyInfo prop = (PropertyInfo) member;

                if (prop.GetIndexParameters ().Length == 0 && prop.CanRead) {
                    string name = member.Name;
                    MethodInfo getter = prop.GetGetMethod ();

                    if ((getter.Attributes & MethodAttributes.Public) == 0) {
                        name = "#" + name;
                    }
                    
                    Dump (sb, prop.GetValue (o, null), prop.PropertyType, name, level + 1, previous);
                }
            }
        }
    }
}
