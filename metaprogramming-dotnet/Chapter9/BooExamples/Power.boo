#booc -t:library Power.boo

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import System

macro power:
	yield [| Console.Out.WriteLine(
		Math.Pow($(power.Arguments[0]), $(power.Arguments[1]))) |]