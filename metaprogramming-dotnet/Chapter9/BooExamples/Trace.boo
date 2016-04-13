# booc -t:library Trace.boo

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import System

class TraceAttribute(AbstractAstAttribute):
	def Apply(type as Node):
		target = type as ClassDefinition
		
		if target is null:
			raise ArgumentException(
				"TraceAttribute can only be applied to classes.", 
				"type")
		
		for member in target.Members:
			method = member as Method
			continue if method is null
			method.Body = [|
				Console.Out.WriteLine(string.Format(
					"Method {0} started.", $(method.FullName)))
				$(method.Body)
				Console.Out.WriteLine(string.Format(
					"Method {0} finished.", $(method.FullName)))				
			|]