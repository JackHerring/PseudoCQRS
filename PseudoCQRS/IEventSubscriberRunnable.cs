﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PseudoCQRS
{
	public interface IEventSubscriberRunnable<TEvent>
	{
		bool CanRun( TEvent @event );
	}
}
