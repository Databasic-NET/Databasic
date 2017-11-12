Imports System.ComponentModel
Imports System.Dynamic
Imports System.Reflection

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject

		'<CompilerGenerated>
		'Public Property Resource As Resource

		''' <summary>
		''' Automaticly initializes 'Resource' class member (static or instance) with singleton Resource instance, 
		''' if there is no instance initialized manualy by programmer.
		''' </summary>
		Public Sub New()
			Me.initResource()
		End Sub

        ''' <summary>
        ''' Initializes 'Resource' class member (static or instance) with singleton Resource instance,
        ''' if there is no instance yet. This method is always called by Entity constructor.
        ''' </summary>
        Protected Overridable Sub initResource()
            Dim meType As Type = Me.GetType()
            Dim resourceMember As Reflection.MemberInfo = (
                From
                    member As Reflection.MemberInfo In meType.GetMembers(
                        BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic
                    )
                Where
                    member.Name = "Resource" AndAlso (
                        TypeOf member Is Reflection.PropertyInfo OrElse
                        TypeOf member Is Reflection.FieldInfo
                    )
                Select
                    member
            ).FirstOrDefault()
            If resourceMember = Nothing Then Return
            If TypeOf resourceMember Is Reflection.PropertyInfo Then
                Dim pi As Reflection.PropertyInfo = DirectCast(resourceMember, Reflection.PropertyInfo)
                If pi.GetValue(Me, Nothing) = Nothing AndAlso pi.CanWrite Then pi.SetValue(Me, Resource.GetInstance(pi.PropertyType), Nothing)
            ElseIf TypeOf resourceMember Is Reflection.FieldInfo Then
                Dim fi As Reflection.FieldInfo = DirectCast(resourceMember, Reflection.FieldInfo)
                If fi.GetValue(Me) = Nothing Then fi.SetValue(Me, Resource.GetInstance(fi.FieldType))
            End If
        End Sub

    End Class
End Namespace