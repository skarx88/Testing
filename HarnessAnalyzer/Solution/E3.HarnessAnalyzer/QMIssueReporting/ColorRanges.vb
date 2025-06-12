Namespace IssueReporting

    Public Class ColorRangeCollection
        Inherits System.Collections.Generic.SortedList(Of UInteger, CRange)

        Private _buffer As New Dictionary(Of UInteger, CRange)
        Private _max As UInteger = 0

        Public Sub New()
        End Sub

        Public Function AddNew(toMax As UInteger, color As System.Drawing.Color) As CRange
            Dim cRange As New CRange() With {.Color = color, .toMax = toMax}
            Me.Add(cRange.toMax, cRange)
            Return cRange
        End Function

        Shadows Sub Add(key As UInteger, value As CRange)
            MyBase.Add(key, value)
            If value.toMax > _max Then _max = value.toMax
            _buffer.Clear()
        End Sub

        Shadows Function Remove(key As UInteger) As Boolean
            If MyBase.Remove(key) Then
                _buffer.Clear()
                Return True
            End If
            Return False
        End Function

        Shadows Sub RemoveAt(index As Integer)
            MyBase.RemoveAt(index)
            _buffer.Clear()
        End Sub

        Shadows Sub Clear()
            MyBase.Clear()
            _buffer.Clear()
        End Sub

        Public Function GetColor(number As UInteger) As System.Drawing.Color
            If Not _buffer.ContainsKey(number) Then
                If Not Me.ContainsKey(number) Then
                    If Me.Count > 0 Then
                        Dim countIdx As Integer = 0
                        For i As Integer = Me.Keys.Count - 1 To 0 Step -1
                            If number > Keys(i) Then
                                countIdx = i
                                Exit For
                            End If
                        Next

                        Dim key As UInteger
                        If number > Keys(countIdx) Then
                            If (Keys.Count - 1) > countIdx Then
                                key = Me.Keys(countIdx + 1)
                            Else
                                Throw New ArgumentOutOfRangeException(NameOf(number), String.Format("Given value ({0}) is over the maximum defined color range {1}!", number, Me(Me.Keys(Me.Keys.Count - 1)).toMax))
                            End If
                        ElseIf number <= Keys(countIdx) Then
                            key = Me.Keys(countIdx)
                        End If
                        _buffer.Add(number, Me(key))
                        Return Me(key).Color
                    Else
                        Return Color.Empty
                    End If
                Else
                    Return Me(number).Color
                End If
            Else
                Return _buffer(number).Color
            End If
        End Function

        ReadOnly Property Max As UInteger
            Get
                Return _max
            End Get
        End Property

    End Class

    Public Class CRange
        Property toMax As UInteger
        Property Color As System.Drawing.Color
    End Class


End Namespace
