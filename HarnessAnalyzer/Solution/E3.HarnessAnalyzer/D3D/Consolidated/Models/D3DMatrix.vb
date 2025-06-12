Imports devDept.Eyeshot
Imports devDept.Geometry

Namespace D3D.Consolidated.Designs

    Public Class D3DMatrix
        Public M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 As Double
        Private m() As Double
        Private mat(,) As Double

        Sub New()
            M11 = 1
            M12 = 0
            M13 = 0
            M14 = 0

            M21 = 0
            M22 = 1
            M23 = 0
            M24 = 0

            M31 = 0
            M32 = 0
            M33 = 1
            M34 = 0

            M41 = 0
            M42 = 0
            M43 = 0
            M44 = 1
            Me.mat = GetMat()
        End Sub

        Public ReadOnly Property RotX As Double
            Get
                Return GetRotX()
            End Get
        End Property

        Public ReadOnly Property RotY As Double
            Get
                Return GetRotY()
            End Get
        End Property

        Public ReadOnly Property RotZ As Double
            Get
                Return GetRotZ()
            End Get
        End Property

        Public ReadOnly Property RotXdegrees As Double
            Get
                Return GetRotX() * 180 / Math.PI
            End Get
        End Property

        Public ReadOnly Property RotYdegrees As Double
            Get
                Return GetRotY() * 180 / Math.PI
            End Get
        End Property

        Public ReadOnly Property RotZdegrees As Double
            Get
                Return GetRotZ() * 180 / Math.PI
            End Get
        End Property

        Public ReadOnly Property TransX As Double
            Get
                Return GetTransX()
            End Get
        End Property

        Public ReadOnly Property TransY As Double
            Get
                Return GetTransY()
            End Get
        End Property

        Public ReadOnly Property TransZ As Double
            Get
                Return GetTransZ()
            End Get
        End Property

        Public ReadOnly Property ScaleX As Double
            Get
                Return GetScaleX()
            End Get
        End Property

        Public ReadOnly Property ScaleY As Double
            Get
                Return GetScaleY()
            End Get
        End Property

        Public ReadOnly Property ScaleZ As Double
            Get
                Return GetScaleZ()
            End Get
        End Property

        Public InitialView As viewType

        Sub New(mArray() As Double, Optional initialView As viewType = viewType.Other)
            Me.m = CType(mArray.Clone, Double())
            M11 = CDbl(m(0))
            M12 = CDbl(m(1))
            M13 = CDbl(m(2))
            M14 = CDbl(m(3))
            M21 = CDbl(m(4))
            M22 = CDbl(m(5))
            M23 = CDbl(m(6))
            M24 = CDbl(m(7))
            M31 = CDbl(m(8))
            M32 = CDbl(m(9))
            M33 = CDbl(m(10))
            M34 = CDbl(m(11))
            M41 = CDbl(m(12))
            M42 = CDbl(m(13))
            M43 = CDbl(m(14))
            M44 = CDbl(m(15))
            mat = GetMat()
            initialView = initialView
        End Sub

        Sub New(m(,) As Double)
            Me.mat = CType(m.Clone, Double(,))
            M11 = mat(0, 0)
            M12 = mat(0, 1)
            M13 = mat(0, 2)
            M14 = mat(0, 3)

            M21 = mat(1, 0)
            M22 = mat(1, 1)
            M23 = mat(1, 2)
            M24 = mat(1, 3)

            M31 = mat(2, 0)
            M32 = mat(2, 1)
            M33 = mat(2, 2)
            M34 = mat(2, 3)

            M41 = mat(3, 0)
            M42 = mat(3, 1)
            M43 = mat(3, 2)
            M44 = mat(3, 3)
        End Sub

        Public Function Inverse4x4() As Double(,)
            Dim trans As New devDept.Geometry.Transformation
            trans.Matrix = mat
            trans.Invert()
            Return trans.Matrix
        End Function

        Public Function Transpose4x4() As Double(,)
            Dim trans As New devDept.Geometry.Transformation
            trans.Matrix = mat
            trans.Transpose()
            Return trans.Matrix
        End Function

        Public Sub GetArray()
            m(0) = M11
            m(1) = M12
            m(2) = M13
            m(3) = M14

            m(4) = M21
            m(5) = M22
            m(6) = M23
            m(7) = M24

            m(8) = M31
            m(9) = M32
            m(10) = M33
            m(11) = M34

            m(12) = M41
            m(13) = M42
            m(14) = M43
            m(15) = M44
        End Sub

        Public Function GetMat() As Double(,)
            Dim myMat(3, 3) As Double
            myMat(0, 0) = M11
            myMat(0, 1) = M12
            myMat(0, 2) = M13
            myMat(0, 3) = M14

            myMat(1, 0) = M21
            myMat(1, 1) = M22
            myMat(1, 2) = M23
            myMat(1, 3) = M24

            myMat(2, 0) = M31
            myMat(2, 1) = M32
            myMat(2, 2) = M33
            myMat(2, 3) = M34

            myMat(3, 0) = M41
            myMat(3, 1) = M42
            myMat(3, 2) = M43
            myMat(3, 3) = M44

            Return myMat
        End Function

        Public Function RotateXMatrix(angle As Double) As D3DMatrix
            Dim m As New D3DMatrix
            m.M22 = Math.Cos(angle)
            m.M23 = Math.Sin(angle)
            m.M32 = -m.M23
            m.M33 = m.M22
            Return m
        End Function

        Public Function rotateYMatrix(angle As Double) As D3DMatrix
            Dim m As New D3DMatrix
            m.M11 = Math.Cos(angle)
            m.M13 = -Math.Sin(angle)
            m.M31 = -m.M13
            m.M33 = m.M11
            Return m
        End Function

        Public Function RotateZMatrix(angle As Double) As D3DMatrix
            Dim m As New D3DMatrix
            m.M11 = Math.Cos(angle)
            m.M12 = Math.Sin(angle)
            m.M12 = -m.M12
            m.M22 = m.M11
            Return m
        End Function

        Public Function GetRotX() As Double
            Dim ang As Double
            ang = Math.Atan2(M23, M33)
            Return ang
        End Function

        Public Function GetRotY() As Double
            Dim ang As Double
            ang = Math.Atan2(-M13, Math.Sqrt(M23 * M23 + M33 * M33))
            Return ang
        End Function

        Public Function GetRotZ() As Double
            Dim ang As Double
            ang = Math.Atan2(M12, M11)
            Return ang
        End Function

        Public Function Trans() As Vector3D
            Dim v As New Vector3D(GetTransX, GetTransY, GetTransZ)
            Return v
        End Function

        Public Function Scale() As Vector3D
            Dim v As New Vector3D(GetScaleX, GetScaleY, GetScaleZ)
            Return v
        End Function

        Public Function GetTransX() As Double
            Return M41 / M44
        End Function

        Public Function GetTransY() As Double
            Return M42 / M44
        End Function

        Public Function GetTransZ() As Double
            Return M43 / M44
        End Function

        Public Function GetScaleX() As Double
            Return M11
        End Function

        Public Function GetScaleY() As Double
            Return M22
        End Function

        Public Function GetScaleZ() As Double
            Return M33
        End Function

    End Class

End Namespace