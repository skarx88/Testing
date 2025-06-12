Imports System.ComponentModel

<Serializable()>
Public Class UltrasonicSpliceTerminalDistanceMap

    Public Property MinDistance As Single = 0
    Public Property TerminalPartNumber As String = String.Empty


    Public Sub New()
        MinDistance = 0
        TerminalPartNumber = String.Empty
    End Sub

    Public Sub New(minDist As Single, terminalPartNo As String)
        MinDistance = minDist
        TerminalPartNumber = terminalPartNo
    End Sub

End Class


Public Class UltrasonicSpliceTerminalDistanceMapList
    Inherits BindingList(Of UltrasonicSpliceTerminalDistanceMap)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub AddUltrasonicSpliceTerminalDistanceMap(ultrasonicSpliceTerminalDistanceMap As UltrasonicSpliceTerminalDistanceMap)
        If (Not ContainsUltrasonicSpliceTerminalDistanceMap(ultrasonicSpliceTerminalDistanceMap)) Then
            Me.Add(ultrasonicSpliceTerminalDistanceMap)
        End If
    End Sub

    Public Function ContainsUltrasonicSpliceTerminalDistanceMap(ultrasonicSpliceTerminalDistanceMap As UltrasonicSpliceTerminalDistanceMap) As Boolean
        For Each usSpliceTerminalMap As UltrasonicSpliceTerminalDistanceMap In Me
            If (usSpliceTerminalMap.TerminalPartNumber = ultrasonicSpliceTerminalDistanceMap.TerminalPartNumber) Then Return True
        Next

        Return False
    End Function

    Public Sub DeleteUltrasonicSpliceTerminalDistanceMap(ultrasonicSpliceTerminalDistanceMap As UltrasonicSpliceTerminalDistanceMap)
        Me.Remove(ultrasonicSpliceTerminalDistanceMap)
    End Sub

    Public Function FindUltrasonicSpliceTerminalDistanceMapFromPartNumber(partNumber As String) As UltrasonicSpliceTerminalDistanceMap
        If (partNumber <> String.Empty) Then
            For Each ultrasonicSpliceTerminalDistanceMap As UltrasonicSpliceTerminalDistanceMap In Me
                If (ultrasonicSpliceTerminalDistanceMap.TerminalPartNumber = partNumber) Then Return ultrasonicSpliceTerminalDistanceMap
            Next
        End If

        Return Nothing
    End Function

End Class