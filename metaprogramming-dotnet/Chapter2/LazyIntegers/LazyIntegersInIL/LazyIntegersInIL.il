.assembly extern mscorlib
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
	.ver 4:0:0:0
}

.assembly LazyIntegersInIL
{
	.ver 1:0:0:0
}

.class private abstract auto ansi sealed beforefieldinit LazyIntegersInIL.Program
	extends [mscorlib]System.Object
{
	.field private static class [mscorlib]System.Func`1<int32> LazyIntegerValueFactoryDelegate
	
	.method private hidebysig static void Main(string[] args) cil managed
	{
		.entrypoint
		.maxstack 3
		.locals init ([0] class [mscorlib]System.Lazy`1<int32> lazyInteger)
		
		ldnull
		ldftn int32 LazyIntegersInIL.Program::LazyIntegerValueFactory()
		newobj instance void class [mscorlib]System.Func`1<int32>::.ctor(object, native int)
		stsfld class [mscorlib]System.Func`1<int32> LazyIntegersInIL.Program::LazyIntegerValueFactoryDelegate
		
		ldsfld class [mscorlib]System.Func`1<int32> LazyIntegersInIL.Program::LazyIntegerValueFactoryDelegate
		newobj instance void class [mscorlib]System.Lazy`1<int32>::.ctor(class [mscorlib]System.Func`1<!0>)
		stloc.0
		
		call class [mscorlib]System.IO.TextWriter [mscorlib]System.Console::get_Out()
		ldloc.0
		callvirt instance !0 class [mscorlib]System.Lazy`1<int32>::get_Value()
		callvirt instance void [mscorlib]System.IO.TextWriter::WriteLine(int32)
		ret
	}

	.method private hidebysig static int32 LazyIntegerValueFactory() cil managed
	{
		.maxstack 1
		newobj instance void [mscorlib]System.Random::.ctor()
		callvirt instance int32 [mscorlib]System.Random::Next()
		ret
	}
}