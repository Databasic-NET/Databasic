Imports System.Dynamic

Namespace ActiveRecord
	Partial Public MustInherit Class Entity
		Inherits DynamicObject





        Public Shared Function Columns(tableIndex As Integer, Optional separator As String = ",") As String
            Return String.Join(separator, ProviderResource.ColumnsArray(
                Tools.GetEntryClassType(), tableIndex
            ))
        End Function

        Public Shared Function Columns(Of TActiveRecord)(tableIndex As Integer, Optional separator As String = ",") As String
            Return String.Join(
                separator, ProviderResource.ColumnsArray(GetType(TActiveRecord), tableIndex
            ))
        End Function

        Public Shared Function Columns(Optional separator As String = ",", Optional tableIndex As Integer = 0) As String
            Return String.Join(separator, ProviderResource.ColumnsArray(
                Tools.GetEntryClassType(), tableIndex
            ))
        End Function

        Public Shared Function Columns(Of TActiveRecord)(Optional separator As String = ",", Optional tableIndex As Integer = 0) As String
            Return String.Join(
                separator, ProviderResource.ColumnsArray(GetType(TActiveRecord), tableIndex
            ))
        End Function

        Public Shared Function ColumnsExcept(exceptColumns As String(), Optional tableIndex As Short = 0, Optional separator As String = ",") As String
            Dim source As List(Of String) = ProviderResource.ColumnsList(
                Tools.GetEntryClassType(), tableIndex
            )
            For Each exceptColumn As String In exceptColumns
                If source.Contains(exceptColumn) Then source.Remove(exceptColumn)
            Next
            Return String.Join(separator, Enumerable.ToArray(Of String)(source))
        End Function

        Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), Optional tableIndex As Short = 0, Optional separator As String = ",") As String
            Dim source As List(Of String) = ProviderResource.ColumnsList(
                GetType(TResource), tableIndex
            )
            For Each exceptColumn As String In exceptColumns
                If source.Contains(exceptColumn) Then source.Remove(exceptColumn)
            Next
            Return String.Join(separator, Enumerable.ToArray(Of String)(source))
        End Function

        Public Shared Function ColumnsExcept(exceptColumns As String(), separator As String, Optional tableIndex As Short = 0) As String
            Dim source As List(Of String) = ProviderResource.ColumnsList(
                Tools.GetEntryClassType(), tableIndex
            )
            For Each exceptColumn As String In exceptColumns
                If source.Contains(exceptColumn) Then source.Remove(exceptColumn)
            Next
            Return String.Join(separator, Enumerable.ToArray(Of String)(source))
        End Function

        Public Shared Function ColumnsExcept(Of TResource)(exceptColumns As String(), separator As String, Optional tableIndex As Short = 0) As String
            Dim source As List(Of String) = ProviderResource.ColumnsList(
                GetType(TResource), tableIndex
            )
            For Each exceptColumn As String In exceptColumns
                If source.Contains(exceptColumn) Then source.Remove(exceptColumn)
            Next
            Return String.Join(separator, Enumerable.ToArray(Of String)(source))
        End Function





    End Class
End Namespace
