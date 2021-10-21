using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AssemblyBrowserLib.Data;
using AssemblyBrowserLib.Data.MembersInfo;
using static System.Reflection.BindingFlags;

namespace AssemblyBrowserLib
{
    public class AssemblyBrowser
    {
        public List<DataContainer> GetAssemblyInfo(string filePath)
        {
            var assembly = Assembly.LoadFrom(filePath);

            var assemblyInfo = new Dictionary<string, DataContainer>();

            foreach (var type in assembly.GetTypes())
            {
                try
                {
                    if (!assemblyInfo.ContainsKey(type.Namespace))
                        assemblyInfo.Add(type.Namespace, new DataContainer(type.Namespace, ClassInformation.GetInfo(type)));

                    assemblyInfo.TryGetValue(type.Namespace, out var container);

                    container.Members.Add(GetMembers(type));

                }
                catch (NullReferenceException e) { Console.WriteLine(e.StackTrace); }
            }

            return assemblyInfo.Values.ToList();
        }

        private static DataContainer GetMembers(Type type)
        {
            var member = new DataContainer(ClassInformation.GetInfo(type), ClassInformation.GetInfo(type));

            var members = GetFields(type);
            members.AddRange(GetProperties(type));
            members.AddRange(GetMethods(type));

            member.Members = members;

            return member;
        }

        private static IEnumerable<MemberInformation> GetMethods(Type type)
        {
            var methodInfos = new List<MemberInformation>();

            methodInfos.AddRange(GetConstructors(type));
            
            foreach (var method in type.GetMethods(Instance | Static | Public | NonPublic | DeclaredOnly))
            {

                if (type.IsDefined(typeof(ExtensionAttribute), false) && method.IsDefined(typeof(ExtensionAttribute), false))
                    continue;

                var signature = MethodInformation.GetInfo(method);
                methodInfos.Add(new MemberInformation(signature, ClassInformation.GetInfo(type)));
            }

            return methodInfos;
        }

        private static List<MemberInformation> GetFields(Type type)
        {
            return type.GetFields().Select(field => new MemberInformation(FieldInformation.GetInfo(field), ClassInformation.GetInfo(type))).ToList(); 
        }

        private static IEnumerable<MemberInformation> GetProperties(Type type)
        {
            return type.GetProperties().Select(property => new MemberInformation(PropertyInformation.GetInfo(property), ClassInformation.GetInfo(type))).ToList();
        }

        private static IEnumerable<MemberInformation> GetConstructors(Type type)
        {
            return type.GetConstructors().Select(constructor => new MemberInformation(ConstructorInformation.GetInfo(constructor), ClassInformation.GetInfo(type))).ToArray();
        }
    }
}
