// Copyright 2006 Alp Toker <alp@atoker.com>
// This software is made available under the MIT License
// See COPYING for details

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace DBus
{
	using Protocol;

	//marked internal because this is really an implementation detail and needs to be replaced
	internal class DProxy : RealProxy
	{
		protected BusObject busObject;

		public DProxy (BusObject busObject, Type type) : base(type)
		{
			this.busObject = busObject;
		}

		static MethodInfo mi_GetHashCode = typeof (object).GetMethod ("GetHashCode");
		static MethodInfo mi_Equals = typeof (object).GetMethod ("Equals", BindingFlags.Instance);
		static MethodInfo mi_ToString = typeof (object).GetMethod ("ToString");
		static MethodInfo mi_GetLifetimeService = typeof (MarshalByRefObject).GetMethod ("GetLifetimeService");

		object GetDefaultReturn (MethodBase mi, object[] inArgs)
		{
			if (mi == mi_GetHashCode)
				return busObject.Path.Value.GetHashCode ();
			if (mi == mi_Equals)
				return busObject.Path.Value == ((BusObject)((MarshalByRefObject)inArgs[0]).GetLifetimeService ()).Path.Value;
			if (mi == mi_ToString)
				return busObject.Path.Value;
			if (mi == mi_GetLifetimeService)
				return busObject;

			return null;
		}

		public override IMessage Invoke (IMessage message)
		{
			IMethodCallMessage callMessage = (IMethodCallMessage) message;

			object defaultRetVal = GetDefaultReturn (callMessage.MethodBase, callMessage.InArgs);
			if (defaultRetVal != null) {
				MethodReturnMessageWrapper defaultReturnMessage = new MethodReturnMessageWrapper ((IMethodReturnMessage) message);
				defaultReturnMessage.ReturnValue = defaultRetVal;

				return defaultReturnMessage;
			}

			object[] outArgs;
			object retVal;
			Exception exception;
			busObject.Invoke (callMessage.MethodBase, callMessage.MethodName, callMessage.InArgs, out outArgs, out retVal, out exception);

			var returnMessage = ConstructReturnMessage (retVal, outArgs, exception, callMessage);

			return returnMessage;
		}

		~DProxy ()
		{
			//FIXME: remove handlers/match rules here
			if (ProtocolInformation.Verbose)
				Console.Error.WriteLine ("Warning: Finalization of " + busObject.Path + " not yet supported");
		}

		internal static IMethodReturnMessage ConstructReturnMessage(object retVal,
			object[] outArgs,
			Exception exception,
			IMethodCallMessage callMessage)
		{
			if (null != exception) {
				return new ReturnMessage (exception, callMessage);
			}

			var outIndices = callMessage.MethodBase.GetParameters ()
				.Where (x => x.IsOut || x.ParameterType.IsByRef)
				.Select (x => x.Position);

			var args = new object[callMessage.ArgCount];
			int count = 0;
			foreach (var index in outIndices) {
				args [index] = outArgs [count++];
			}

			return new ReturnMessage (
				retVal,
				args,
				count,
				callMessage.LogicalCallContext,
				callMessage
			);
		}
	}
}
