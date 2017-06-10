Public Class ResourceAttribute
    Inherits Attribute
    Friend ResourceType As Type
    Friend Singleton As Boolean
    Public Sub New(resourceType As Type, Optional singleton As Boolean = False)
        Me.ResourceType = resourceType
        Me.Singleton = singleton
    End Sub
End Class