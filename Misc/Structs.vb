Friend Structure MetaDescription
	Friend ClassType As Type
	Friend ConnectionIndex As Int32
	Friend Tables As String()
	Friend ColumnsByDbNames As Dictionary(Of String, Databasic.MemberInfo)
	Friend ColumnsByCodeNames As Dictionary(Of String, Databasic.MemberInfo)
	Friend PrimaryColumnsByDbNames As Dictionary(Of String, List(Of String))
	Friend PrimaryColumnsByCodeNames As Dictionary(Of String, List(Of String))
	Friend UniqueColumnsByDbNames As Dictionary(Of String, List(Of String))
	Friend UniqueColumnsByCodeNames As Dictionary(Of String, List(Of String))
End Structure

Friend Structure MemberInfo
	Friend Value As Object
	Friend Type As Type
	Friend Name As String
	Friend MemberInfo As System.Reflection.MemberInfo
	Friend MemberInfoType As MemberInfoType
	Friend FormatProvider As IFormatProvider
	Friend TrimChars As Char()
End Structure