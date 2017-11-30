Imports System.Data.Common
Imports System.IO
Imports Databasic.ActiveRecord

Public MustInherit Class Statement

	''' <summary>
	''' Currently prepared and executed SQL command.
	''' </summary>
	Public Overridable Property Command As DbCommand
	''' <summary>
	''' Currently executed data reader from SQL command.
	''' </summary>
	Public Overridable Property Reader As DbDataReader

	''' <summary>
	''' Flag to recognize if Me.Reader has more than one row or not
	''' </summary>
	''' <returns></returns>
	Protected Overridable Property commandBehavior As CommandBehavior

	''' <summary>
	''' Empty SQL statement constructor.
	''' </summary>
	''' <param name="sql">SQL statement code.</param>
	''' <param name="connection">Connection instance.</param>
	Public Sub New(sql As String, connection As DbConnection)
	End Sub
	''' <summary>
	''' Empty SQL statement constructor.
	''' </summary>
	''' <param name="sql">SQL statement code.</param>
	''' <param name="transaction">SQL transaction instance with connection instance inside.</param>
	Public Sub New(sql As String, transaction As DbTransaction)
	End Sub

	''' <summary>
	''' Get underlying enum value if any compilation attribute defined on target enum,
	''' else get enum as string
	''' </summary>
	''' <param name="sqlParamValue">Any sql param value which should be an enum type.</param>
	''' <returns>The right enum value for database</returns>
	Protected Overridable Function getPossibleUnderlyingEnumValue(sqlParamValue As Object) As Object
		Dim enumUnderlyingType As Type
		If sqlParamValue <> Nothing Then
			sqlParamValueType = sqlParamValue.GetType()
			If (sqlParamValueType <> Nothing AndAlso sqlParamValueType.IsEnum) Then
				enumUnderlyingValuesAttr = Attribute.GetCustomAttribute(
					sqlParamValueType,
					Databasic.Constants.UseEnumUnderlyingValuesAttrType
				)
				If TypeOf enumUnderlyingValuesAttr Is Attribute Then
					enumUnderlyingType = [Enum].GetUnderlyingType(sqlParamValueType)
					sqlParamValue = Convert.ChangeType(sqlParamValue, enumUnderlyingType)
				Else
					sqlParamValue = sqlParamValue.ToString()
				End If
			End If
		End If
		Return sqlParamValue
	End Function
	''' <summary>
	''' Set up all sql params into internal Command instance.
	''' </summary>
	''' <param name="sqlParams">Anonymous object with named keys as SQL statement params without any '@' chars in object keys.</param>
	Protected MustOverride Sub addParamsWithValue(sqlParams As Object)
	''' <summary>
	''' Set up all sql params into internal Command instance.
	''' </summary>
	''' <param name="sqlParams">Dictionary with named keys as SQL statement params without any '@' chars in dictionary keys.</param>
	Protected MustOverride Sub addParamsWithValue(sqlParams As Dictionary(Of String, Object))


	Protected Overridable Function getJsonStreamWriter() As StreamWriter
		Return New StreamWriter(New MemoryStream())
	End Function

	Protected Overridable Function getJsonStreamWriter(ByRef stream As Stream) As StreamWriter
		Return New StreamWriter(stream)
	End Function


End Class
