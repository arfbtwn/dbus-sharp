using System;
using System.Collections.Generic;
using System.Linq;

namespace DBus
{
	public class InterfaceTree {

		bool built;
		internal InterfaceTree[] parents;

		public readonly Type Type;

		public IEnumerable<InterfaceTree> Parents {
			get { 
				if (!built) {
					BuildOne ();
					built = true;
				}

				return parents;
			}
		}

		public bool IsInTree (Type type)
		{
			return type.IsAssignableFrom (Type);
		}

		public IEnumerable<InterfaceTree> Where (Func<Type, bool> selector)
		{
			if (selector (Type)) {
				yield return this;
			}

			foreach (var result in Parents.SelectMany (x => x.Where (selector)))
			{
				yield return result;
			}
		}

		public InterfaceTree SubTreeRootedAt (Type type)
		{
			if (!IsInTree (type)) {
				throw new Exception ();
			}

			if (Type == type) {
				return this;
			}

			var parent = Parents.First (x => x.IsInTree (type));

			return parent.SubTreeRootedAt (type);
		}

		public InterfaceTree (Type type)
		{
			Type = type;
		}

		void BuildOne ()
		{
			var children = Type.GetInterfaces ();
			var firstGeneration = children
				.WhereAll ((x, y) => !x.IsAssignableFrom(y))
				.ToArray ();

			parents = new InterfaceTree[firstGeneration.Length];

			for (int i = 0; i < parents.Length; ++i) {
				parents [i] = new InterfaceTree (firstGeneration [i]);
			}
		}

		public sealed override bool Equals (object obj)
		{
			return obj is InterfaceTree && Type == ((InterfaceTree)obj).Type;
		}

		public sealed override int GetHashCode ()
		{
			return Type.GetHashCode ();
		}

		public int Depth {
			get {
				return 1 + (Parents.Any () ? Parents.Max (x => x.Depth) : 0);
			}
		}
	}
}

