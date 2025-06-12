Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

<Serializable>
Public Class CarTransformationSetting_2023
    Implements ISerializable

    Public Sub New()
    End Sub

    Private Const TRANSFORMATION_PROPERTY_2023_NAME As String = "Transformation"
    Private Const MATRIX_PROPERTY_2023_NAME As String = "Matrix"
    Private Const CAR_FILE_NAME_PROPERTY_2023_NAME As String = "CarFileName"

    Friend Sub New(info As SerializationInfo, context As StreamingContext)
        For Each entry As SerializationEntry In info
            If entry.Name.TrimStart("_"c) = TRANSFORMATION_PROPERTY_2023_NAME Then ' HINT: standard transfromation-case
                Dim myValue As HelperTransformation = CType(entry.Value, HelperTransformation)
                Me.Transformation = CType(myValue.Matrix, Double(,))
            ElseIf entry.Name.TrimStart("_"c) = MATRIX_PROPERTY_2023_NAME Then ' HINT: fallback to alternative version/type
                Me.Transformation = CType(entry.Value, Double(,))
            ElseIf entry.Name.TrimStart("_"c) = CAR_FILE_NAME_PROPERTY_2023_NAME Then
                Me.CarFileName = CStr(entry.Value)
            End If
        Next
    End Sub

    Property CarFileName As String = String.Empty
    Property Transformation As Double(,)

    Public Shared Function Load(s As Stream) As CarTransformationSetting_2023
        If s.CanSeek Then
            s.Position = 0
        End If
        Dim bf As New BinaryFormatter
        bf.Binder = New TypeBinder
        Return CType(bf.Deserialize(s), CarTransformationSetting_2023)
    End Function

    Private Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
        Throw New NotSupportedException("Writing 2023 car transformation is Not longer supported please use the New format (/class) instead!")
    End Sub

    Private Class TypeBinder
        Inherits SerializationBinder

        Private Const TRANSFORMATION_SETTING_2023_TYPE_NAME As String = "Zuken.E3.HarnessAnalyzer.D3D.CarTransformationSetting"
        Private Const TRANSFORMATION_SETTING_2023_TYPE_NAME_2 As String = "Zuken.E3.HarnessAnalyzer.D3D.Consolidated.Controls.CarTransformationSetting" ' HINT: the type-name changed in the version history, we support both
        Private Const TRANSFORMANTION_EYESHOT_TYPE_NAME As String = "devDept.Geometry.Transformation"

        Public Overrides Function BindToType(assemblyName As String, typeName As String) As Type
            If typeName = TRANSFORMATION_SETTING_2023_TYPE_NAME Or typeName = TRANSFORMATION_SETTING_2023_TYPE_NAME_2 Then
                Return GetType(CarTransformationSetting_2023)
            End If
            If typeName = TRANSFORMANTION_EYESHOT_TYPE_NAME Then
                Return GetType(HelperTransformation) 'HINT: acts as deserializing class for the matrix, to avoid a reference to devdept eyeshot
            End If
            Return Nothing
        End Function

    End Class

    <Serializable>
    Public Class HelperTransformation
        Implements ISerializable

        Friend Sub New()

        End Sub

        ReadOnly Property Matrix As Object

        Public Sub New(info As SerializationInfo, context As StreamingContext)
            For Each entry As SerializationEntry In info
                If entry.Name = MATRIX_PROPERTY_2023_NAME Then
                    Me.Matrix = entry.Value
                    Exit For
                End If
            Next
        End Sub

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            Throw New NotSupportedException("Serialization write is not supported for this helper class")
        End Sub
    End Class

End Class
