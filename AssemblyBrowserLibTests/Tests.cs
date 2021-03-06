using System;
using System.Collections.Generic;
using System.Reflection;
using AssemblyBrowserLib;
using AssemblyBrowserLib.Data;
using NUnit.Framework;

namespace AssemblyBrowserLibTests
{
    public class Tests
    {
        private AssemblyBrowser _assemblyBrowser;

        private List<DataContainer> _namespaces;
        private Assembly _lib;

        [SetUp]
        public void Init()
        {
            _assemblyBrowser = new AssemblyBrowser();
            _lib = Assembly.Load("TestableLib");
            _namespaces = _assemblyBrowser.GetAssemblyInfo(_lib.Location);
        }


        [Test]
        public void MembersCountTest()
        {
            var expectedCount = _lib.GetTypes().Length;
            Assert.AreEqual(_namespaces[0].Members.Count, expectedCount);
        }

        [Test]
        public void NamespacesNameTest()
        {
            foreach (var _namespace in _namespaces)
            {
                if (_namespace.Signature != "TestableLib" &&
                    _namespace.Signature != "System")
                {
                    Assert.Fail($"Error in namespace name {_namespace.Signature}");
                }
            }
        }

        [Test]
        public void ExtensionMethodName()
        {
            var types = _namespaces[0].Members;
            foreach (var type in types)
            {
                if (type.Signature == "ExtClass")
                {
                    bool flag = false;
                    foreach (var member in ((DataContainer)type).Members)
                    {
                        if (member.Signature == "CharCount")
                        {
                            flag = true;
                        }
                    }

                    Assert.IsTrue(flag);
                }
            }
        }

        [Test]
        public void ClassesNamesTest()
        {
            var types = _namespaces[0].Members;
            foreach (var type in types)
            {
                if (type.Signature != "public static class  ExtClass" && type.Signature != "public  class  TestClass1" && type.Signature != "private   class  TestClass2" && type.Signature != "public abstract class  TestClass3")
                {
                    Assert.Fail($"Error in type name {type.Signature}");
                }

            }
        }

        [Test]
        public void FieldNameTest()
        {
            var type = _namespaces[0].Members[1];
            if (type.Signature == "TestClass1")
            {
                Assert.AreEqual(((DataContainer)type).Members[0], "private int foo");
            }
        }
    }
}