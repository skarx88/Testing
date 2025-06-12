Imports System.Runtime.CompilerServices
Imports Zuken.E3.Lib.Eyeshot.Model

Namespace D3D.Document.Controls

    <HideModuleName>
    Friend Module Extensions

        <Extension>
        Public Function IsHidden(en As IBaseModelEntityEx, hiddens As List(Of HiddenEntity)) As Boolean
            Dim res As Boolean
            If hiddens.Count > 0 Then
                Dim myHiddens As List(Of String) = hiddens.Select(Function(h) CType(h.Entity, BaseModelEntity).Id).ToList()
                If myHiddens.Contains(en.Id) Then
                    res = True
                End If
            End If
            Return res
        End Function

        <Extension>
        Public Sub ResetColor(en As IBaseModelEntityEx, colorMapper As Dictionary(Of String, Color))
            en.Color = colorMapper(en.Id)
        End Sub

        <Extension>
        Public Sub IsNotActive(en As IBaseModelEntityEx, colorMapper As Dictionary(Of String, Color))
            en.Color = Color.FromArgb(CInt(colorMapper(en.Id).A * 0.04), 125, 125, 125)
        End Sub

    End Module

End Namespace