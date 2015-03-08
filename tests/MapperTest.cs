using System;
using System.Linq;
using System.Reflection;

using DBus;

using NUnit.Framework;

namespace DBus.Tests
{
	[TestFixture]
	public class MapperTest
	{
		[TestFixture(
			typeof(ITestOne),
			"WithOutParameters",
			new [] { typeof(string) },
			new []{ typeof(uint), typeof(string) }
		)]
		class MapperGetTypes {
			readonly Type type;
			readonly string methodName;
			readonly Type[] inParams;
			readonly Type[] outParams;
			public MapperGetTypes (Type type, string methodName, Type[] inParams, Type[] outParams)
			{
				this.type = type;
				this.methodName = methodName;
				this.inParams = inParams;
				this.outParams = outParams;
			}

			[Test]
			public void Test () {

				var method = type.GetMethod (methodName);

				Assert.IsNotNull (method);

				var parms = method.GetParameters ();

				Type[] inTypes = Mapper.GetTypes (ArgDirection.In, parms);
				Type[] outTypes = Mapper.GetTypes (ArgDirection.Out, parms);

				Assert.AreEqual (inParams, inTypes);
				Assert.AreEqual (outParams, outTypes);
			}
		}

		[Test]
		public void MapperGetInterfaceName ()
		{
			var typeOfInt = typeof(ITestOne);
			var interfaceName = "org.dbussharp.test";

			var typeByRef = typeof(TestByRef);
			var typeByInt = typeof(Test);

			var nameByRef = Mapper.GetInterfaceName (typeByRef);
			var nameByInt = Mapper.GetInterfaceName (typeOfInt);

			Assert.AreEqual (interfaceName, nameByRef);
			Assert.AreEqual (interfaceName, nameByInt);

			var typeFromRefByName = Mapper.GetInterfaceType (typeByRef, interfaceName);
			var typeFromIntByName = Mapper.GetInterfaceType (typeByInt, interfaceName);

			Assert.AreEqual (typeof(TestByRef), typeFromRefByName);
			Assert.AreEqual (typeof(ITestOne), typeFromIntByName);
		}

		[TestFixture(typeof(IComposite), 4)]
		class InterfaceTreeDepthTests {
			readonly Type type;
			readonly int expected;
			public InterfaceTreeDepthTests(Type type, int expected)
			{
				this.type = type;
				this.expected = expected;
			}

			[Test]
			public void Test ()
			{
				var tree = new InterfaceTree (type);
				Assert.AreEqual (expected, tree.Depth);
			}
		}

		[TestFixture(
			typeof(IComposite),
			new [] {
				typeof(IDerivedMore),
				typeof(IDerived),
				typeof(IBase),
				typeof(IOther)
			}
		)]
		class InterfaceTreeSubTreeTest {
			readonly Type type;
			readonly Type[] contents;
			public InterfaceTreeSubTreeTest(Type type, params Type[] contents)
			{
				this.type = type;
				this.contents = contents;
			}

			[Test]
			public void Test ()
			{
				var tree = new InterfaceTree (type);

				foreach (var super in contents) {
					Assert.IsTrue (tree.IsInTree (super));
				}
			}
		}

		[Test]
		public void InterfaceTreeWhereTest ()
		{
			var tree = new InterfaceTree (typeof(IComposite));
			var subtree = tree
				.Where (type => DerivedMoreInterfaceName == Mapper.GetInterfaceName (type))
				.First ();

			Assert.AreEqual (typeof(IDerivedMore), subtree.Type);
		}

		[TestFixture(typeof(Test), 1)]
		[TestFixture(typeof(TestByRef), 1)]
		[TestFixture(typeof(ITestOne), 1)]
		[TestFixture(typeof(BadClass), true, 4)]
		[TestFixture(typeof(IDerivedMore), 1)]
		[TestFixture(typeof(IDerivedMore), true, 3)]
		class MapperGetHierarchyTests {
			readonly Type type;
			readonly bool includeInterfaces;
			readonly int expected;
			public MapperGetHierarchyTests(Type type, bool includeInterfaces, int expected)
			{
				this.type = type;
				this.includeInterfaces = includeInterfaces;
				this.expected = expected;
			}

			public MapperGetHierarchyTests (Type type, int expected)
				: this (type, false, expected) { }

			[Test]
			public void Test ()
			{
				var got = Mapper.GetHierarchy (type, includeInterfaces);

				Assert.AreEqual (expected, got.Count ());
			}
		}

		interface IFoo { }

		class BadClass : Test, IFoo { }

		interface IBase { }

		interface IDerived : IBase { }

		[Interface(DerivedMoreInterfaceName)]
		interface IDerivedMore : IDerived { }

		[Interface(DerivedRenamedInterfaceName)]
		interface IDerivedRenamed : IDerived { }

		[Interface(OtherInterfaceName)]
		interface IOther { }

		interface IComposite : IDerivedMore, IDerivedRenamed, IOther { }

		const string DerivedInterfaceName = "org.dbussharp.test";
		const string DerivedMoreInterfaceName = "org.dbussharp.test.more";
		const string DerivedRenamedInterfaceName = "org.dbussharp.test.renamed";
		const string OtherInterfaceName = "org.dbussharp.test.other";
	}
}

