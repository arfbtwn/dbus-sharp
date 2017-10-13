// Copyright 2009 Alp Toker <alp@atoker.com>
// Copyright 2010 Alan McGovern <alan.mcgovern@gmail.com>
// This software is made available under the MIT License
// See COPYING for details

using System;
using NUnit.Framework;
using DBus;

namespace DBus.Tests
{
	[TestFixture]
	public class ObjectPathTest
	{
		[Test]
		public void InvalidStartingCharacter ()
		{
			// Paths must start with "/"
			Assert.Throws<ArgumentException>(() => new ObjectPath ("no_starting_slash"));
		}

		[Test]
		public void InvalidEndingCharacter ()
		{
			// Paths must not end with "/"
			Assert.Throws<ArgumentException>(() => new ObjectPath ("/ends_with_slash/"));
		}

		[Test]
		public void MultipleConsecutiveSlash ()
		{
			// Paths must not contains consecutive "/"
			Assert.Throws<ArgumentException>(() => new ObjectPath ("/foo//bar"));
		}

		[Test]
		public void InvalidCharacters ()
		{
			// Paths must be in the range "[A-Z][a-z][0-9]_"
			Assert.Throws<ArgumentException>(() => new ObjectPath ("/?valid/path/invalid?/character.^"));
		}

		[Test]
		public void ConstructorTest ()
		{
			var x = new ObjectPath ("/");
			Assert.AreEqual (x.ToString (), "/", "#1");
			Assert.AreEqual (x, ObjectPath.Root, "#2");

			x = new ObjectPath ("/this/01234567890/__Test__");
			Assert.AreEqual ("/this/01234567890/__Test__", x.ToString (), "#3");
		}

		[Test]
		public void Equality ()
		{
			string pathText = "/org/freedesktop/DBus";

			ObjectPath a = new ObjectPath (pathText);
			ObjectPath b = new ObjectPath (pathText);

			Assert.IsTrue (a.Equals (b));
			Assert.AreEqual (String.Empty.CompareTo (null), a.CompareTo (null));
			Assert.IsTrue (a == b);
			Assert.IsFalse (a != b);

			ObjectPath c = new ObjectPath (pathText + "/foo");
			Assert.IsFalse (a == c);
		}

		[Test]
		public void NullConstructor ()
		{
			Assert.Throws<ArgumentNullException>(() => new ObjectPath (null));
		}

		[Test]
		public void EmptyStringConstructor ()
		{
			Assert.Throws<ArgumentException>(() => new ObjectPath (""));
		}
	}
}
