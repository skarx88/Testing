Imports System.Drawing
Imports System.Runtime.InteropServices

Public MustInherit Class WinFormsPreviewHandlerEx
    Inherits PreviewHandlerEx

    Public Property Control As UserControl

    Protected Sub ThrowIfNoControl()
        If Control Is Nothing Then
            Throw New InvalidOperationException("PreviewHandlerControlNotInitialized")
        End If
    End Sub

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification:="The object remains reachable through the Controls collection which can be disposed at a later time.")>
    Protected Overrides Sub HandleInitializeException(ByVal caughtException As Exception)
        If caughtException Is Nothing Then
            Throw New ArgumentNullException("caughtException")
        End If

        Control = New UserControl()
        Control.Controls.Add(New TextBox With {
        .[ReadOnly] = True,
        .Multiline = True,
        .Dock = DockStyle.Fill,
        .Text = caughtException.ToString(),
        .BackColor = Color.OrangeRed
    })
    End Sub

    Protected Overrides Sub UpdateBounds(ByVal bounds As NativeRect)
        Control.Bounds = Rectangle.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom)
        Control.Visible = True
    End Sub

    Protected Overrides Sub SetFocus()
        Control.Focus()
    End Sub

    Protected Overrides Sub SetBackground(ByVal argb As Integer)
        Control.BackColor = Color.FromArgb(argb)
    End Sub

    Protected Overrides Sub SetForeground(ByVal argb As Integer)
        Control.ForeColor = Color.FromArgb(argb)
    End Sub

    Protected Overrides Sub SetFont(ByVal font As LogFont)
        Control.Font = System.Drawing.Font.FromLogFont(font)
    End Sub

    Protected Overrides ReadOnly Property Handle As IntPtr
        Get

            If True Then
                Return Control.Handle
            End If
        End Get
    End Property

    Protected Overrides Sub SetParentHandle(ByVal handle As IntPtr)
        HandlerNativeMethods.SetParent(Control.Handle, handle)
    End Sub

    Protected Overrides Sub Finalize()
        Dispose(False)
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso Control IsNot Nothing Then
            Control.Dispose()
        End If
        Me.Control = Nothing
    End Sub
End Class

