
Namespace Schematics

    Public Class ActiveEntity
        Implements IDisposable

        Private _Owner As ActiveEntities
        Private _disposedValue As Boolean

        Public Sub New()

        End Sub

        Public Sub New(id As String, name As String, type As ActiveObjectType)
            Me.Id = id
            Me.Name = name
            Me.Type = type
        End Sub

        ReadOnly Property Id As String
        ReadOnly Property Name As String
        ReadOnly Property Type As ActiveObjectType = ActiveObjectType.None

        ReadOnly Property IsEmpty As Boolean
            Get
                Return String.IsNullOrEmpty(_Id)
            End Get
        End Property

        Property Owner As ActiveEntities
            Get
                Return _Owner
            End Get
            Friend Set
                _Owner = Value
            End Set
        End Property

        ReadOnly Property TypRessourceString As String
            Get
                Select Case Me.Type
                    Case ActiveObjectType.None
                        Return EdbObjectTypeStrings.None
                    Case ActiveObjectType.Cable
                        Return EdbObjectTypeStrings.Cable
                    Case ActiveObjectType.Cavity
                        Return EdbObjectTypeStrings.Cavity
                    Case ActiveObjectType.Component
                        Return EdbObjectTypeStrings.Component
                    Case ActiveObjectType.Connector
                        Return EdbObjectTypeStrings.Connector
                    Case ActiveObjectType.Function
                        Return EdbObjectTypeStrings._Function
                    Case ActiveObjectType.Module
                        Return EdbObjectTypeStrings._Module
                    Case ActiveObjectType.Wire
                        Return EdbObjectTypeStrings.Wire
                End Select
                Throw New NotImplementedException(String.Format("ObjectType ""{0}"" not implemented!", Me.Type.ToString))
            End Get
        End Property

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                End If

                _Type = Nothing
                _Name = Nothing
                _Id = Nothing
                Owner = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

    Public Enum ActiveObjectType
        None = 0
        Wire = 1
        Component = 2
        Connector = 3
        Cavity = 4
        Cable = 5
        [Module] = 6
        [Function] = 7
    End Enum

End Namespace