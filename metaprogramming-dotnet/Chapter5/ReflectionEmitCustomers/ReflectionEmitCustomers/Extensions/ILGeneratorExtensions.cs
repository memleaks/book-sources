using ReflectionEmitCustomers.Extensions.Descriptors;
using System;
using System.Reflection.Emit;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.IO;

namespace ReflectionEmitCustomers.Extensions
{
	internal static class ILGeneratorExtensions
	{
		internal static void Emit(this ILGenerator @this, OpCode opcode, 
			ISymbolDocumentWriter document, StreamWriter file, int lineNumber)
		{
			var line = opcode.Name;
			file.WriteLine(line);
			@this.MarkSequencePoint(document, lineNumber, 1, lineNumber, line.Length + 1);
			@this.Emit(opcode);
		}

		internal static void Emit(this ILGenerator @this, OpCode opcode, ConstructorInfo method, 
			ISymbolDocumentWriter document, StreamWriter file, int lineNumber)
		{
			var line = opcode.Name + " " + new MethodDescriptor(
				method, method.DeclaringType.Assembly).Value;
			file.WriteLine(line);
			@this.MarkSequencePoint(document, lineNumber, 1, lineNumber, line.Length + 1);
			@this.Emit(opcode, method);
		}

		internal static void Emit(this ILGenerator @this, OpCode opcode, MethodInfo method, 
			ISymbolDocumentWriter document, StreamWriter file, int lineNumber)
		{
			var line = opcode.Name + " " + new MethodDescriptor(
				method, method.DeclaringType.Assembly).Value;
			file.WriteLine(line);
			@this.MarkSequencePoint(document, lineNumber, 1, lineNumber, line.Length + 1);
			@this.Emit(opcode, method);
		}

		internal static void Emit(this ILGenerator @this, OpCode opcode, string value, 
			ISymbolDocumentWriter document, StreamWriter file, int lineNumber)
		{
			var line = opcode.Name + " \"" + value + "\"";
			file.WriteLine(line);
			@this.MarkSequencePoint(document, lineNumber, 1, lineNumber, line.Length + 1);
			@this.Emit(opcode, value);
		}

		internal static void Emit(this ILGenerator @this, OpCode opcode, Type value, 
			ISymbolDocumentWriter document, StreamWriter file, int lineNumber)
		{
			var line = opcode.Name + " " + new TypeDescriptor(value).Value;
			file.WriteLine(line);
			@this.MarkSequencePoint(document, lineNumber, 1, lineNumber, line.Length + 1);
			@this.Emit(opcode, value);
		}
	}
}
