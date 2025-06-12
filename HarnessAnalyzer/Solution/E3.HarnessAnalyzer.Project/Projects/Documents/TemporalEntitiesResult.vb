Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities

Namespace Documents

    Public Class TemporalEntitiesResult
        Implements IDisposable

        Private _disposedValue As Boolean
        Private _Selected As Boolean
        Private _ws As IWorkspace
        Private _tempAdded As Boolean = False

        Friend Sub New(document As HcvDocument, tempEntities As IEnumerable(Of MeshEx))
            Me.Document = document
            Me.Entities = tempEntities.ToArray
            Dim envs As IWorkspace() = document.Entities.Select(Function(ent) ent.Workspace).Distinct.ToArray

            If envs.Count > 1 Then
                Throw New NotSupportedException($"Multiple design environments ({NameOf(devDept.Eyeshot.Design)}) are not supported ({Me.GetType.Name}) !")
            End If

            _ws = envs.SingleOrDefault ' HINT: when no tempEntities whre provided we will have also a nothing env here -> exception when more than one env at once (shoudn't be happen because all the entities are expected to be at the same model/design)!
        End Sub

        ReadOnly Property Document As HcvDocument
        ReadOnly Property Entities As MeshEx()

        Property InvalidateAfterDispose As Boolean = True

        Property Selected As Boolean
            Get
                Return Entities.Where(Function(ent) ent.Selectable).All(Function(ent) ent.Selected)
            End Get
            Set
                If Me.Entities.Count > 0 Then
                    _ws.Entities.ClearSelection()
                End If

                For Each entity As MeshEx In Me.Entities
                    If entity.Selectable Then
                        entity.Selected = Value
                    End If
                Next

                If _ws IsNot Nothing AndAlso TypeOf _ws Is DesignEx Then
                    CType(_ws, DesignEx).UpdateVisibleSelection()
                End If
            End Set
        End Property


        Public Sub AddAndRegen()
            AddEntitiesCore()
            Me.Regen()
        End Sub

        Private Function AddEntitiesCore() As MeshEx()
            Dim list As MeshEx() = Me.Entities.ToArray
            For Each ent As MeshEx In list
                If Not _ws.Entities.Contains(ent) Then
                    _ws.Entities.Add(ent)
                End If
            Next

            Return list
        End Function

        ''' <summary>
        ''' Adds entities to TempEntities-collection. If entity is selected the selectionColor will also be used when adding to tempEntities-Collection
        ''' </summary>
        Public Sub AddAsTempEntitiesAndRegen(Optional color As Nullable(Of System.Drawing.Color) = Nothing)
            AddEntitiesCore() ' HINT: needed also here or zoomFit will not work (no entities to calculate boundingBox?)
            For Each ent As MeshEx In Me.Entities.ToArray
                If TypeOf _ws Is Workspace AndAlso Not CType(_ws, Workspace).TempEntities.Contains(ent) Then
                    CType(_ws, Workspace).TempEntities.Add(ent, If(color, If(Me.Selected, CType(_ws, Workspace).Selection.Color, ent.Color)))
                End If
            Next
            _tempAdded = True
            Me.Regen()
        End Sub

        ReadOnly Property Workspace As devDept.Eyeshot.Workspace
            Get
                Return TryCast(_ws, Workspace)
            End Get
        End Property

        Public Sub Regen()
            If _ws IsNot Nothing Then
                If TypeOf _ws Is DesignEx Then
                    CType(_ws, DesignEx).Entities.Regen()
                ElseIf TypeOf _ws Is Workspace Then
                    CType(_ws, Workspace).Entities.Regen()
                End If
            End If
        End Sub

        Public Sub DesignInvalidate()
            If _ws IsNot Nothing Then
                If TypeOf _ws Is DesignEx Then
                    CType(_ws, DesignEx).Invalidate()
                Else
                    _ws.Invalidate
                End If
            End If
        End Sub

        Public Sub ZoomFit(Optional selectedOnly As Boolean = False)
            If _ws IsNot Nothing Then
                CType(_ws, Workspace).ZoomFit(Me.Entities.OfType(Of Entity).ToList, selectedOnly, 25)
            End If
        End Sub

        Public Sub Clear()
            If TypeOf _ws Is Workspace Then
                If Entities IsNot Nothing Then
                    If _tempAdded Then
                        For Each ent As MeshEx In Me.Entities
                            CType(_ws, Workspace).TempEntities.Remove(ent)
                        Next
                        _tempAdded = False
                    End If

                    For Each ent As MeshEx In Me.Entities
                        ent.Selected = False
                        _ws.Entities.Remove(ent)
                    Next

                    For Each ent As MeshEx In Me.Entities.ToArray
                        ent.Dispose()
                    Next

                    _Entities = New MeshEx() {}
                End If
            End If
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposedValue Then
                If disposing Then
                    Me.Clear()
                    If InvalidateAfterDispose Then
                        DesignInvalidate()
                    End If
                End If

                _ws = Nothing
                _Entities = Nothing
                _disposedValue = True
            End If
        End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(disposing As Boolean)" ein.
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace