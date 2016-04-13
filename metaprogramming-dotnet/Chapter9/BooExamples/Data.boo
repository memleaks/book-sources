# booc Data.boo
import System

class Data:
	def constructor():
		pass
		
	def constructor(value as Guid):
		_value = value
		
	[Getter(Value)]
	_value as Guid
		
[STAThread]
def Main(args as (string)):
	print(Data().Value)
	print(Data(Guid.NewGuid()).Value)