Imports System.IO
Imports System.Reflection

Public Class Tools
    ''' <summary>
    ''' Return Type object by sstring in forms: "Full.Class.Name" or "AssemblyName:Full.Class.Name"
    ''' </summary>
    ''' <param name="fullClassName" type="String">"Full.Class.Name" or "AssemblyName:Full.Class.Name"</param>
    ''' <returns type="Type">Desired type</returns>
    Public Shared Function GetTypeGlobaly(fullClassName As String) As Type

        Dim type As Type = Type.GetType(fullClassName)
        If type IsNot Nothing Then
            Return type
        End If

        'chybka:
        If fullClassName.IndexOf(":") > 1 Then
            Dim fullNameAndAssembly As String() = fullClassName.Split(":")
            type = Tools.GetTypeGlobaly(fullNameAndAssembly(0), fullNameAndAssembly(1))
        End If

        If type Is Nothing Then
            Dim assemblies As Reflection.Assembly() = AppDomain.CurrentDomain.GetAssemblies()
            For Each assembly As Reflection.Assembly In assemblies
                type = assembly.GetType(fullClassName)
                If type IsNot Nothing Then
                    Exit For
                End If
            Next
        End If

        Return type

    End Function

    ''' <summary>
    ''' Return Type object by two strings in form: "AssemblyName", "Full.Class.Name"
    ''' </summary>
    ''' <param name="assemblyName" type="String">"AssemblyName" for AssemblyName.dll</param>
    ''' <param name="fullClassName" type="String">Full class name including namespace</param>
    ''' <returns type="Type">Desired type</returns>
    Public Shared Function GetTypeGlobaly(assemblyName As String, fullClassName As String) As Type

        Dim type As Type = Nothing

        ' ziska pouze assemblies, které jsou načteny do paměti protože se použily
        'Dim assemblies As Reflection.Assembly() = AppDomain.CurrentDomain.GetAssemblies()

        ' načte všechny assemblies které jsou v nastavení referencí
        'AppDomain.CurrentDomain.GetAssemblies()

        ' načte všechny assemblies které jsou ve složce

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


    Public Shared Sub DispatchEvent(source As Object, eventName As String, eventArgs As EventArgs)

        Dim eventObject As EventInfo = source.GetType().GetEvent(eventName)

        If eventObject IsNot Nothing Then

            Dim fis As FieldInfo() = source.GetType().GetFields(
                BindingFlags.Instance Or BindingFlags.Static Or BindingFlags.Public Or BindingFlags.NonPublic
            )
            For Each fi As FieldInfo In fis
                If fi.Name = eventName + "Event" Then

                    Dim del As System.Delegate = TryCast(fi.GetValue(source), System.Delegate)

                    Dim invocationList As List(Of System.Delegate) = del.GetInvocationList().ToList()

                    For Each invItem As System.Delegate In invocationList

                        invItem.DynamicInvoke(source, eventArgs)

                    Next

                End If
            Next

        End If
    End Sub

End Class
