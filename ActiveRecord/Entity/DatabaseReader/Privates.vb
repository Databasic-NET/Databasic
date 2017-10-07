Imports System.Data.Common
Imports Databasic.ActiveRecord

Namespace ActiveRecord
    Partial Public MustInherit Class Entity

        ''' <summary>
        ''' Get column names from reader as list of strings.
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <returns></returns>
        Private Shared Function _getReaderRowColumns(reader As DbDataReader) As List(Of String)
            Return Enumerable.ToList(Of String)(
                Enumerable.Select(Of Integer, String)(
                    Enumerable.Range(0, reader.FieldCount),
                    New Func(Of Integer, String)(AddressOf reader.GetName)
                )
            )
        End Function
        ''' <summary>
        ''' Return True if type is descriptable type by custom attributes, not primitive, not an object.
        ''' </summary>
        ''' <param name="instanceType">Instance type to check out.</param>
        ''' <returns>True if descriptable.</returns>
        Private Shared Function _isDescriptableType(ByRef instanceType As Type) As Boolean
            Return Not instanceType.IsPrimitive AndAlso instanceType.FullName <> "System.String" AndAlso instanceType.FullName <> "System.Object"
        End Function
        ''' <summary>
        ''' Set up current reader row columns into instance properties and fields.
        ''' </summary>
        ''' <param name="reader">DbDataReader with current row moved, where current row will be used to fill instance properties and fields.</param>
        ''' <param name="readerColumnNames">Columns in reader in proper order.</param>
        ''' <param name="instance">Instance to fill.</param>
        Private Shared Sub _readerRowToInstance(ByRef reader As DbDataReader, ByRef readerColumnNames As List(Of String), ByRef columnsByDbNames As Dictionary(Of String, Databasic.MemberInfo), ByRef instance As Object, isEntity As Boolean)
            Dim mi As Databasic.MemberInfo
            Dim rawValueTypeCode As TypeCode
            Dim assigned As Boolean
            Dim propInfo As Reflection.PropertyInfo
            Dim targetName As String
            Dim formatProvider As IFormatProvider
            Dim rawValue As Object
            Dim targetValue As Object
            Dim entity As Databasic.ActiveRecord.Entity
            For Each readerColumnName As String In readerColumnNames
                assigned = False
                rawValue = reader(readerColumnName)
                targetName = readerColumnName
                targetValue = Nothing
                If columnsByDbNames.ContainsKey(readerColumnName) Then
                    mi = columnsByDbNames(readerColumnName)
                    rawValueTypeCode = Type.GetTypeCode(rawValue.GetType())
                    If (rawValueTypeCode = Constants.StringTypeCode AndAlso mi.TrimChars.Length > 0) Then
                        rawValue = rawValue.ToString().Trim(mi.TrimChars)
                    ElseIf rawValueTypeCode = Type.GetTypeCode(mi.Type) Then
                        targetValue = rawValue
                    Else
                        formatProvider = If(mi.FormatProvider, System.Globalization.CultureInfo.CurrentCulture)
                        targetValue = If(TypeOf rawValue Is DBNull, Nothing, Convert.ChangeType(rawValue, mi.Type, formatProvider))
                    End If
                    If (mi.MemberInfoType = MemberInfoType.Prop) Then
                        propInfo = DirectCast(mi.MemberInfo, Reflection.PropertyInfo)
                        If propInfo.CanWrite Then
                            propInfo.SetValue(instance, targetValue, Nothing)
                            assigned = True
                        Else
                            targetName = mi.Name
                        End If
                    Else
                        DirectCast(mi.MemberInfo, Reflection.FieldInfo).SetValue(instance, targetValue)
                        assigned = True
                    End If
                End If
                If isEntity Then
                    targetValue = If(targetValue, rawValue)
                    entity = DirectCast(instance, Databasic.ActiveRecord.Entity)
                    If Not assigned Then
                        entity._reserveStore.Item(targetName) = targetValue
                    End If
                    If entity._initialData.ContainsKey(targetName) Then
                        entity._initialData(targetName) = targetValue
                    Else
                        entity._initialData.Add(targetName, targetValue)
                    End If
                End If
            Next
        End Sub

    End Class
End Namespace