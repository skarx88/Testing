' Cyotek ImageBox
' http://cyotek.com/blog/tag/imagebox
' Copyright (c) 2010-2021 Cyotek Ltd.
' This work is licensed under the MIT License.

Imports System
Imports System.Drawing
Imports System.Runtime.InteropServices

Namespace Global.System.Windows.Forms

    Partial Friend Class ImageBoxNativeMethods ' partial for when linking this file into other assemblies

        <Flags>
        Friend Enum SIF
            SIF_RANGE = &H1

            SIF_PAGE = &H2

            SIF_POS = &H4

            SIF_DISABLENOSCROLL = &H8

            SIF_TRACKPOS = &H10

            SIF_ALL = SIF_PAGE Or SIF_POS Or SIF_RANGE Or SIF_TRACKPOS
        End Enum


        Friend Const GWL_STYLE As Integer = -16

        Friend Const SB_BOTH As Integer = 3

        Friend Const SB_BOTTOM As Integer = 7

        Friend Const SB_CTL As Integer = 2

        Friend Const SB_ENDSCROLL As Integer = 8

        Friend Const SB_HORZ As Integer = 0

        Friend Const SB_LEFT As Integer = 6

        Friend Const SB_LINEDOWN As Integer = 1

        Friend Const SB_LINELEFT As Integer = 0

        Friend Const SB_LINERIGHT As Integer = 1

        Friend Const SB_LINEUP As Integer = 0

        Friend Const SB_PAGEDOWN As Integer = 3

        Friend Const SB_PAGELEFT As Integer = 2

        Friend Const SB_PAGERIGHT As Integer = 3

        Friend Const SB_PAGEUP As Integer = 2

        Friend Const SB_RIGHT As Integer = 7

        Friend Const SB_THUMBPOSITION As Integer = 4

        Friend Const SB_THUMBTRACK As Integer = 5

        Friend Const SB_TOP As Integer = 6

        Friend Const SB_VERT As Integer = 1

        Friend Const WM_HSCROLL As Integer = &H114

        Friend Const WM_VSCROLL As Integer = &H115

        Friend Const WS_BORDER As Integer = &H800000

        Friend Const WS_EX_CLIENTEDGE As Integer = &H200

        Friend Const WS_HSCROLL As Integer = &H100000

        Friend Const WS_VSCROLL As Integer = &H200000

        Friend Const WM_MOUSEWHEEL As Integer = &H20A

        Friend Const WM_MOUSEHWHEEL As Integer = &H20E

        Private Sub New()
        End Sub

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function GetScrollInfo(hwnd As IntPtr, bar As Integer,
            <MarshalAs(UnmanagedType.LPStruct)> scrollInfo As SCROLLINFO) As Integer
        End Function

        <DllImport("kernel32.dll")>
        Friend Shared Function GetTickCount() As UInteger
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Shared Function GetWindowLong(hwnd As IntPtr, index As Integer) As UInteger
        End Function

        <DllImport("user32.dll")>
        Friend Shared Function SetScrollInfo(hwnd As IntPtr, bar As Integer,
            <MarshalAs(UnmanagedType.LPStruct)> scrollInfo As SCROLLINFO, redraw As Boolean) As Integer
        End Function

        <DllImport("user32.dll")>
        Friend Shared Function SetWindowLong(hwnd As IntPtr, index As Integer, newLong As UInteger) As Integer
        End Function

        <DllImport("user32.dll")>
        Friend Shared Function WindowFromPoint(point As System.Drawing.Point) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=False)>
        Friend Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
        End Function

        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Friend Class SCROLLINFO
            Friend cbSize As Integer

            Friend fMask As SIF

            Friend nMin As Integer

            Friend nMax As Integer

            Friend nPage As Integer

            Friend nPos As Integer

            Friend nTrackPos As Integer

            Friend Sub New()
                cbSize = Marshal.SizeOf(Me)
                nPage = 0
                nMin = 0
                nMax = 0
                nPos = 0
                nTrackPos = 0
                fMask = 0
            End Sub
        End Class

    End Class

End Namespace
