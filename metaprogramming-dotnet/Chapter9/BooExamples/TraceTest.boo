#booc -r:Trace.dll TraceTest.boo

import System

[Trace]
class TracedClass:
	def TraceMe():
		Console.Out.WriteLine("I should have been traced.")

TracedClass().TraceMe()