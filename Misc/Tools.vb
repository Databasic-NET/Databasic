Imports System.Diagnostics
Imports System.IO
Imports System.Reflection
Imports System.Threading

Public Class Tools
	Public Shared Function GetEntryClassType() As Type
		Return (New StackTrace(True)).GetFrames(2).GetMethod().DeclaringType
		'Dim stackTrace As System.Diagnostics.StackTrace = New System.Diagnostics.StackTrace(True)
		'Dim caller = stackTrace.GetFrames(2)
		'Return caller.GetMethod().DeclaringType
	End Function

	Public Shared Function GetProcessAndThreadKey() As String
		Return String.Format("{0}_{1}", Process.GetCurrentProcess.Id, Thread.CurrentThread.ManagedThreadId)
	End Function

	''' <summary>
	''' Return Type object by sstring in forms: "Full.Class.Name" or "AssemblyName:Full.Class.Name"
	''' </summary>
	''' <param name="fullClassName" type="String">"Full.Class.Name" or "AssemblyName:Full.Class.Name"</param>
	''' <returns type="Type">Desired type</returns>
	Public Shared Function GetTypeGlobaly(ByVal fullClassName As String) As Type
		Dim type As Type = Type.GetType(fullClassName)
		If type IsNot Nothing Then Return type

		If fullClassName.IndexOf(":") > -1 Then
			Dim fullNameAndAssembly As String() = fullClassName.Split(":")
			type = Tools.GetTypeGlobaly(fullNameAndAssembly(0), fullNameAndAssembly(1))
			If type IsNot Nothing Then Return type
		End If

		Dim assemblies As Reflection.Assembly() = AppDomain.CurrentDomain.GetAssemblies()
		For Each assembly As Reflection.Assembly In assemblies
			type = assembly.GetType(fullClassName)
			If type IsNot Nothing Then Return type
		Next
	End Function

	''' <summary>
	''' Return Type object by two strings in form: "AssemblyName", "Full.Class.Name"
	''' </summary>
	''' <param name="assemblyName" type="String">"AssemblyName" for AssemblyName.dll</param>
	''' <param name="fullClassName" type="String">Full class name including namespace</param>
	''' <returns type="Type">Desired type</returns>
	Public Shared Function GetTypeGlobaly(assemblyName As String, fullClassName As String) As Type
		Dim type As Type = Nothing
		Try
			Dim assemblies As IEnumerable(Of Assembly) =
				From file In Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory)
				Where Path.GetExtension(file) = ".dll"
				Select Assembly.LoadFrom(file)
			For Each assembly As Reflection.Assembly In assemblies
				If assembly.GetName().Name = assemblyName Then
					type = assembly.GetType(fullClassName)
					Exit For
				End If
			Next
		Catch ex As Exception
		End Try

		Return type

	End Function

	Public Shared Function GetConnectionIndexByClassAttr(type As Type, Optional throwException As Boolean = True) As Int32
		Dim connAttr As ConnectionAttribute = DirectCast(Attribute.GetCustomAttribute(type, Constants.ConnectionAttrType), ConnectionAttribute)
		If Not TypeOf connAttr Is ConnectionAttribute Then
			If throwException Then
				Throw New Exception(
					$"Class '{type.FullName}' has no 'Connection' attribute. " +
					"Add 'Connection' class attribute or specify connection instance, " +
					"connection config index or connection config name."
				)
			Else
				Return Databasic.Defaults.CONNECTION_INDEX
			End If
		End If
		Return connAttr.ConnectionIndex
	End Function
End Class
