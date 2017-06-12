<Flags>
Public Enum CommandBehavior
    CloseConnection = &H20
    [Default] = 0
    KeyInfo = 4
    SequentialAccess = &H10
    SchemaOnly = 2
    SingleResult = 1
    SingleRow = 8
End Enum

Public Enum IsolationLevel
    Chaos = &H10
    ReadCommitted = &H1000
    ReadUncommitted = &H100
    RepeatableRead = &H10000
    Serializable = &H100000
    Snapshot = &H1000000
    Unspecified = -1
End Enum

Public Enum KeyType
    None
    Primary
    Unique
End Enum

Public Enum DuplicateKeyBehaviour
    ThrownException
    KeepFirstValue
    OverwriteByNewValue
End Enum

Friend Enum MemberInfoType
	Prop
	Field
End Enum
