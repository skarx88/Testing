Public Class CompareSignatures

    Private _moveDistanceTolerance As Integer
    Private _considerText As Boolean
    Private _boxSignature As Boolean
    Private _entity As VdSVGGroup
    Private _refEntity As VdSVGGroup
    Private _key As String

    Public Sub New(key As String, entity As VdSVGGroup, refEntity As VdSVGGroup, boxSignature As Boolean, considerText As Boolean, moveDistanceTolerance As Integer)
        _key = key
        _moveDistanceTolerance = moveDistanceTolerance
        _considerText = considerText
        _boxSignature = boxSignature
        _entity = entity
        _refEntity = refEntity
    End Sub

    Public Sub Update(changeType As CompareChangeType)
        Select Case changeType
            Case CompareChangeType.Deleted
                Me._ReferenceSignature = GetCompSignature(_key, _refEntity, _boxSignature)

                If Inverse Then
                    Me._EntitySignature = NameOf(GraphicalCompareFormStrings.Added)
                    Me._DiffType = NameOf(GraphicalCompareFormStrings.Added)
                Else
                    Me._EntitySignature = NameOf(GraphicalCompareFormStrings.Deleted)
                    Me._DiffType = NameOf(GraphicalCompareFormStrings.Deleted)
                End If
            Case CompareChangeType.Modified
                Me._DiffType = NameOf(GraphicalCompareFormStrings.Modified)
                Me._EntitySignature = GetCompSignature(_key, _entity, _boxSignature)
                Me._ReferenceSignature = GetCompSignature(_key, _refEntity, _boxSignature)
            Case CompareChangeType.New
                Me._EntitySignature = GetCompSignature(_key, _entity, _boxSignature)

                If Inverse Then
                    Me._ReferenceSignature = NameOf(GraphicalCompareFormStrings.Deleted)
                    Me._DiffType = NameOf(GraphicalCompareFormStrings.Deleted)
                Else
                    Me._ReferenceSignature = NameOf(GraphicalCompareFormStrings.Added)
                    Me._DiffType = NameOf(GraphicalCompareFormStrings.Added)
                End If
            Case Else
                Throw New NotImplementedException(String.Format("{0}: {1}", GetType(CompareChangeType).Name, changeType))
        End Select
    End Sub

    ReadOnly Property EntitySignature As String
    ReadOnly Property DiffType As String
    ReadOnly Property ReferenceSignature As String

    ReadOnly Property Inverse As Boolean
        Get
            Return Application.OpenForms.OfType(Of MainForm).Single.GeneralSettings.InverseCompare
        End Get
    End Property

    Private Function GetCompSignature(key As String, group As VdSVGGroup, Optional includeBoundingBox As Boolean = True) As String
        If includeBoundingBox Then
            Return String.Format("{0}|{1}|{2}|{3}", key, group.GetHashString(True, _considerText), CInt(group.BoundingBox.Min.x / _moveDistanceTolerance), CInt(group.BoundingBox.Min.y / _moveDistanceTolerance))
        Else
            Return String.Format("{0}|{1}", key, group.GetHashString(True, _considerText))
        End If
    End Function

End Class
