Imports System
Imports System.Drawing
Imports System.Windows.Forms
#If USEWIN32PINVOKELIB
using Cyotek.Win32;
#End If

' Cyotek ImageBox
' Copyright (c) 2010-2015 Cyotek Ltd.
' http://cyotek.com/blog/tag/imagebox
' Licensed under the MIT License.
' This code is derived from http://stackoverflow.com/a/13292894/148962 and http://stackoverflow.com/a/11034674/148962

Namespace Global.System.Windows.Forms
    ''' <summary>
    ''' A message filter for WM_MOUSEWHEEL and WM_MOUSEHWHEEL. This class cannot be inherited.
    ''' </summary>
    ''' <seealso cref="T:System.Windows.Forms.IMessageFilter"/>
    Friend NotInheritable Class ImageBoxMouseWheelMessageFilter
        Implements IMessageFilter
#Region "Member Declarations"

        Private Shared _instance As ImageBoxMouseWheelMessageFilter

        Private Shared _active As Boolean

#End Region

#Region "Constructors"

        ''' <summary>
        ''' Constructor that prevents a default instance of this class from being created.
        ''' </summary>
        Private Sub New()
        End Sub

#End Region

#Region "Static Properties"

        ''' <summary>
        ''' Gets or sets a value indicating whether the filter is active
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if the message filter is active, <c>false</c> if not.
        ''' </value>
        Friend Shared Property Active As Boolean
            Get
                Return _active
            End Get
            Set(value As Boolean)
                If _active <> value Then
                    _active = value

                    If _active Then
                        If _instance Is Nothing Then
                            _instance = New ImageBoxMouseWheelMessageFilter()
                        End If
                        Application.AddMessageFilter(_instance)
                    Else
                        If _instance IsNot Nothing Then
                            Application.RemoveMessageFilter(_instance)
                        End If
                    End If
                End If
            End Set
        End Property

#End Region

#Region "IMessageFilter Interface"

        ''' <summary>
        ''' Filters out a message before it is dispatched.
        ''' </summary>
        ''' <param name="m">  [in,out] The message to be dispatched. You cannot modify this message. </param>
        ''' <returns>
        ''' <c>true</c> to filter the message and stop it from being dispatched; <c>false</c> to allow the message to
        ''' continue to the next filter or control.
        ''' </returns>
        ''' <seealso cref="M:System.Windows.Forms.IMessageFilter.PreFilterMessage(Message@)"/>
        Private Function PreFilterMessage(ByRef m As Message) As Boolean Implements IMessageFilter.PreFilterMessage
            Dim result As Boolean

            Select Case m.Msg
                Case ImageBoxNativeMethods.WM_MOUSEWHEEL, ImageBoxNativeMethods.WM_MOUSEHWHEEL ' 0x020A
                    ' 0x020E
                    Dim hControlUnderMouse As IntPtr

                    hControlUnderMouse = ImageBoxNativeMethods.WindowFromPoint(New System.Drawing.Point(CInt(m.LParam)))
                    If hControlUnderMouse = m.HWnd Then
                        ' already headed for the right control
                        result = False
                    Else
                        Dim control As ImageBox

                        control = TryCast(System.Windows.Forms.Control.FromHandle(hControlUnderMouse), ImageBox)

                        If control Is Nothing OrElse Not control.AllowUnfocusedMouseWheel Then
                            ' window under the mouse either isn't managed, isn't an imagebox,
                            ' or it is an imagebox but the unfocused whell option is disabled.
                            ' whatever the case, do not try and handle the message
                            result = False
                        Else
                            ' redirect the message to the control under the mouse
                            ImageBoxNativeMethods.SendMessage(hControlUnderMouse, m.Msg, m.WParam, m.LParam)

                            ' eat the message (otherwise it's possible two controls will scroll
                            ' at the same time, which looks awful... and is probably confusing!)
                            result = True
                        End If
                    End If

                Case Else
                    ' not a message we can process, don't try and block it
                    result = False
            End Select

            Return result
        End Function

#End Region
    End Class
End Namespace
