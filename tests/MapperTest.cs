using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

namespace DBus.Tests
{
	[TestFixture]
	public class MapperTest
	{
		MethodInfo withOutParameters = typeof(ITestOne).GetMethod ("WithOutParameters");

		[Test]
		public void MapperGetTypes ()
		{
			Assert.IsNotNull (withOutParameters);

			var parms = withOutParameters.GetParameters ();

			Type[] inTypes = Mapper.GetTypes (ArgDirection.In, parms);
			Type[] outTypes = Mapper.GetTypes (ArgDirection.Out, parms);

			Assert.AreEqual (new [] { typeof(string) }, inTypes);
			Assert.AreEqual (new [] { typeof(uint), typeof(string) }, outTypes);
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

		[Test]
		public void MapperGetHierarchy ()
		{
			var got = Mapper.GetHierarchy (typeof(Test));

			Assert.AreEqual (0, got.Count ());

			got = Mapper.GetHierarchy (typeof(TestByRef));

			Assert.AreEqual (1, got.Count ());

			got = Mapper.GetHierarchy (typeof(ITestOne));

			Assert.AreEqual (1, got.Count ());
		}
	}
}

