Public Structure MetaDescription
    Public ClassType As Type
    Public ConnectionIndex As Int32
    Public Tables As String()
    Public ColumnsByDbNames As Dictionary(Of String, Databasic.MemberInfo)
    Public ColumnsByCodeNames As Dictionary(Of String, Databasic.MemberInfo)
    Public PrimaryColumnsByDbNames As Dictionary(Of String, List(Of String))
    Public PrimaryColumnsByCodeNames As Dictionary(Of String, List(Of String))
    Public UniqueColumnsByDbNames As Dictionary(Of String, List(Of String))
    Public UniqueColumnsByCodeNames As Dictionary(Of String, List(Of String))
End Structure

Public Structure MemberInfo
    Public Value As Object
    Public Type As Type
    Public Name As String
    Public MemberInfo As System.Reflection.MemberInfo
    Public MemberInfoType As MemberInfoType
    Public FormatProvider As IFormatProvider
    Public TrimChars As Char()
End Structure