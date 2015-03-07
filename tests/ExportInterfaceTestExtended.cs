using System;
using System.Collections.Generic;

using org.freedesktop.DBus;

using NUnit.Framework;

namespace DBus.Tests
{
	[TestFixture]
	public class ExportInterfaceTestExtended
	{
		internal const string InterfaceName = "org.dbussharp.test";
		internal static readonly ObjectPath Path = new ObjectPath ("/org/dbussharp/test");

		Foo server;
		ITest client;

		[SetUp]
		public void Setup ()
		{
			server = new Foo ();
			Bus.Session.Register (Path, server);
			Assert.AreEqual (RequestNameReply.PrimaryOwner, Bus.Session.RequestName (InterfaceName));

			client = Bus.Session.GetObject<ITest> (InterfaceName, Path);
		}

		[TearDown]
		public void TearDown ()
		{
			Bus.Session.ReleaseName (InterfaceName);
			Bus.Session.Unregister (Path);
		}

		[Test]
		public void Can_Call_Complex_Method ()
		{
			IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>> objs;
			client.GetManagedObjects (out objs);

			Assert.IsNotNull (objs);
		}

		[Test]
		public void Can_Connect_Complex_Signals ()
		{
			var addedSignal = false;
			var removedSignal = false;

			var dict = new Dictionary<string, IDictionary<string, object>> ();
			var array = new [] { "foo", "bar" };

			client.InterfacesAdded += (path, interfaces_and_properties) => {
				addedSignal = true;
				Assert.AreEqual (Path, path);
				Assert.IsNotNull (interfaces_and_properties);
				Assert.AreEqual (dict, interfaces_and_properties);
			};

			client.InterfacesRemoved += (path, interfaces) => {
				removedSignal = true;
				Assert.AreEqual (Path, path);
				Assert.IsNotNull (interfaces);
				Assert.AreEqual (array, interfaces);
			};

			server.OnInterfacesAdded (dict);
			server.OnInterfacesRemoved (array);

			Bus.Session.Iterate ();
			Bus.Session.Iterate ();

			Assert.IsTrue (addedSignal, "InterfacesAdded wasn't fired");
			Assert.IsTrue (removedSignal, "InterfacesRemoved wasn't fired");
		}

		public delegate void InterfacesAddedHandler (ObjectPath path, IDictionary<string, IDictionary<string, object>> interfaces_and_properties);
		public delegate void InterfacesRemovedHandler (ObjectPath path, string[] interfaces);

		[Interface(InterfaceName)]
		public interface ITest {

			void GetManagedObjects (out IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>> objpath_interfaces_and_properties);

			event InterfacesAddedHandler InterfacesAdded;
			event InterfacesRemovedHandler InterfacesRemoved;

		}

		class Foo : ITest
		{
			public IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>> seed;

			public void GetManagedObjects (out IDictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>> objpath_interfaces_and_properties)
			{
				objpath_interfaces_and_properties = seed ?? new Dictionary<ObjectPath, IDictionary<string, IDictionary<string, object>>> ();
			}

			public void OnInterfacesAdded (IDictionary<string, IDictionary<string, object>> interfaces_and_properties)
			{
				var h = InterfacesAdded;
				if (null != h) {
					h (Path, interfaces_and_properties);
				}
			}

			public void OnInterfacesRemoved (string[] interfaces)
			{
				var h = InterfacesRemoved;
				if (null != h) {
					h (Path, interfaces);
				}
			}

			public event InterfacesAddedHandler InterfacesAdded;
			public event InterfacesRemovedHandler InterfacesRemoved;
		}
	}
}

