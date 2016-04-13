# booc Literals.boo

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.Parser
import System

def Add(x as int, y as int):
	return x + y

print(Add(3, 4))
	
xParameter = ParameterDeclaration(
	Name: 'x', 
	Type: SimpleTypeReference(Name: 'int'))
yParameter = ParameterDeclaration(
	Name: 'y', 
	Type: SimpleTypeReference(Name: 'int'))
parameters = ParameterDeclarationCollection()
parameters.Add(xParameter)
parameters.Add(yParameter)

apiAdd = Method(
	Name: 'LiteralAdd', 
	Parameters: parameters,
	Body: Block(ReturnStatement(BinaryExpression(
		Left: Expression.Lift(xParameter), 
		Right: Expression.Lift(yParameter), 
		Operator: BinaryOperatorType.Addition))))

print(apiAdd)

literalAdd = [| 
	class QA:
		static def QuotedAdd(x as int, y as int):
			return x + y 
|]

compiledLiteralAdd as duck = compile(literalAdd)
print(compiledLiteralAdd.QuotedAdd(5, 6))

stringifiedAdd = """
class SA:
	static def stringifiedAdd(x as int, y as int):
		return x + y
"""
			 
compiledStringifiedAdd as duck = compile(BooParser.ParseString(
	'SA', stringifiedAdd)).GetType('SA')
print(compiledStringifiedAdd.stringifiedAdd(11, 22))