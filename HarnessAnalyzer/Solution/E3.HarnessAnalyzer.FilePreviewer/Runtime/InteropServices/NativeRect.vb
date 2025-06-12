Imports System.Runtime.InteropServices

''' <summary>
''' A wrapper for a RECT struct
''' </summary>
<StructLayout(LayoutKind.Sequential)>
Public Structure NativeRect
    ''' <summary>
    ''' Position of left edge
    ''' </summary>            
    Public Property Left As Integer

    ''' <summary>
    ''' Position of top edge
    ''' </summary>            
    Public Property Top As Integer

    ''' <summary>
    ''' Position of right edge
    ''' </summary>            
    Public Property Right As Integer

    ''' <summary>
    ''' Position of bottom edge
    ''' </summary>            
    Public Property Bottom As Integer

    ''' <summary>
    ''' Creates a new NativeRect initialized with supplied values.
    ''' </summary>
    ''' <param name="left">Position of left edge</param>
    ''' <param name="top">Position of top edge</param>
    ''' <param name="right">Position of right edge</param>
    ''' <param name="bottom">Position of bottom edge</param>
    Friend Sub New(left As Integer, top As Integer, right As Integer, bottom As Integer)
        Me.New()
        Me.Left = left
        Me.Top = top
        Me.Right = right
        Me.Bottom = bottom
    End Sub

    ''' <summary>
    ''' Determines if two NativeRects are equal.
    ''' </summary>
    ''' <param name="first">First NativeRect</param>
    ''' <param name="second">Second NativeRect</param>
    ''' <returns>True if first NativeRect is equal to second; false otherwise.</returns>
    Public Shared Operator =(first As NativeRect, second As NativeRect) As Boolean
        Return first.Left = second.Left AndAlso first.Top = second.Top AndAlso first.Right = second.Right AndAlso first.Bottom = second.Bottom
    End Operator

    ''' <summary>
    ''' Determines if two NativeRects are not equal
    ''' </summary>
    ''' <param name="first">First NativeRect</param>
    ''' <param name="second">Second NativeRect</param>
    ''' <returns>True if first is not equal to second; false otherwise.</returns>
    Public Shared Operator <>(first As NativeRect, second As NativeRect) As Boolean
        Return Not first = second
    End Operator

    ''' <summary>
    ''' Determines if the NativeRect is equal to another Rect.
    ''' </summary>
    ''' <param name="obj">Another NativeRect to compare</param>
    ''' <returns>True if this NativeRect is equal to the one provided; false otherwise.</returns>
    Public Overrides Function Equals(obj As Object) As Boolean
        Return If(obj IsNot Nothing AndAlso TypeOf obj Is NativeRect, Me = CType(obj, NativeRect), False)
    End Function

    ''' <summary>
    ''' Creates a hash code for the NativeRect
    ''' </summary>
    ''' <returns>Returns hash code for this NativeRect</returns>
    Public Overrides Function GetHashCode() As Integer
        Dim hash As Integer = Left.GetHashCode()
        hash = hash * 31 + Top.GetHashCode()
        hash = hash * 31 + Right.GetHashCode()
        hash = hash * 31 + Bottom.GetHashCode()
        Return hash
    End Function
End Structure
