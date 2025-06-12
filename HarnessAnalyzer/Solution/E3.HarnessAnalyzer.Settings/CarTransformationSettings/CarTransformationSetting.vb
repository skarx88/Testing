Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.Serialization


<Serializable>
Public Class CarTransformationSetting
    Implements ISerializable

    Public Sub New()
    End Sub

    Private _matrix As Double(,)

    Public Sub New(matrix As Double(,), carFileName As String)
        _matrix = matrix
    End Sub

    Friend Sub New(info As SerializationInfo, context As StreamingContext)
        Dim dataStr As String = info.GetString("TransformationData")
        If Not String.IsNullOrEmpty(dataStr) Then
            Dim data As Byte() = Convert.FromBase64String(dataStr)
            _matrix = CreateMatrixFromData(data)
        End If
        Me.CarFileName = info.GetString(NameOf(CarFileName))
    End Sub

    Property CarFileName As String = String.Empty

    <IgnoreDataMember>
    Property Transformation As Double(,)
        Get
            Return _matrix
        End Get
        Set(value As Double(,))
            _matrix = value
        End Set
    End Property

    Public Shared Function Load(s As Stream) As CarTransformationSetting
        Return DataContractXmlFile.Load(Of CarTransformationSetting)(s)
    End Function

    Private Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
        Dim data As Byte()
        If _matrix IsNot Nothing Then
            data = CreateTransformationData(_matrix)
        Else
            data = Array.Empty(Of Byte)
        End If

        info.AddValue("TransformationData", Convert.ToBase64String(data))
        info.AddValue(NameOf(CarFileName), Me.CarFileName)
    End Sub

    Public Shared Function CreateTransformationData(matrix As Double(,)) As Byte()
        Using ms As New MemoryStream
            Using compress As New System.IO.Compression.GZipStream(ms, Compression.CompressionLevel.Optimal, True)
                Dim multiDimList As List(Of List(Of Double)) = matrix.ToMultiDimList ' HINT: the list is serializable, the multi-dim-array not!
                DataContractXmlFile.WriteObject(multiDimList, compress)
            End Using

            Return ms.ToArray
        End Using
    End Function

    Public Shared Function CreateMatrixFromData(data As Byte()) As Double(,)
        If data Is Nothing Then
            data = Array.Empty(Of Byte)
        End If

        Using ms As New MemoryStream(data)
            If ms.Length > 0 Then
                If System.IO.Compression.UtilsEx.IsZip(ms) Then
                    Using zipStream As New DeflateStream(ms, CompressionMode.Decompress, False)
                        Dim multiDimList As List(Of List(Of Double)) = DataContractXmlFile.ReadObject(Of List(Of List(Of Double)))(zipStream)
                        Return multiDimList.ToMultiDimArray
                    End Using
                ElseIf System.IO.Compression.UtilsEx.IsGZip(ms) Then
                    Using gzip As New GZipStream(ms, CompressionMode.Decompress, False)
                        Dim multiDimList As List(Of List(Of Double)) = DataContractXmlFile.ReadObject(Of List(Of List(Of Double)))(gzip)
                        Return multiDimList.ToMultiDimArray
                    End Using
                ElseIf XmlFile.IsXml(ms) Then
                    Dim multiDimList As List(Of List(Of Double)) = DataContractXmlFile.ReadObject(Of List(Of List(Of Double)))(ms)
                    Return multiDimList.ToMultiDimArray
                Else
                    Throw New NotSupportedException("Data format of Transformation not supported for deserializing!")
                End If
            Else
                Return New Double(,) {}
            End If
        End Using
    End Function

    Public Sub SaveTo(path As String)
        Using fs As New FileStream(path, FileMode.Create)
            Save(fs)
        End Using
    End Sub

    Public Sub Save(s As System.IO.Stream)
        DataContractXmlFile.WriteObject(Me, s)
    End Sub

End Class

