﻿using System;
using TMS.Common.Logging;

public class GoogleLoginProvider : LoginProvider, IPlatformLoginProvider
{
	public GoogleLoginProvider(IAppContext context) : base(context)
	{
		Loggers.Default.ConsoleLogger.Write("GoogleLoginProvider -> CTOR -> AppContext: {0}", context);
	}

	public override void Login()
	{
		Loggers.Default.ConsoleLogger.Write("{0} -> {1}", GetType(), (Action)Login);
	}

	public override void Logout()
	{
		Loggers.Default.ConsoleLogger.Write("{0} -> {1}", GetType(), (Action)Logout);
	}
}