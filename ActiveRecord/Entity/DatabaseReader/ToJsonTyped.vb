Imports System.Data.Common
Imports System.IO
Imports System.Text
Imports Databasic.ActiveRecord

Namespace ActiveRecord
	Partial Public MustInherit Class Entity

		Friend Shared Sub ToJson(Of TValue)(
			reader As DbDataReader,
			stream As Stream,
			Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		)








			'Dim result As String = ""
			'Dim serializer As JavaScriptSerializer = New JavaScriptSerializer()
			'Dim resultStream As New StringBuilder()
			'serializer.Serialize(Nothing, )
			'Return result

		End Sub

		Friend Shared Function ToJson(Of TValue)(
			reader As DbDataReader,
			Optional ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo) = Nothing
		)
		End Function

	End Class
End Namespace