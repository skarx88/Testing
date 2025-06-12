Imports System.IO

Friend Class Utilities

    Friend Shared Function FileStreamOpenRead(filePath As String) As System.IO.FileStream
        Return New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read) ' HINT: this is needed because we are in a sandbox with restricted rights, so we always must declare that we are reading or writing
    End Function

    Friend Shared Function HasKBLZData(kblData As System.IO.Stream) As Boolean
        Dim cartesianPoints As New List(Of Double())

        Dim kblDoc As XDocument = XDocument.Load(kblData)
        For Each cPt As XElement In kblDoc.Root.Elements.Where(Function(el) el.Name.LocalName = "Cartesian_point")
            Dim valueList As New List(Of Double)
            If cPt.HasElements Then
                For Each coord As XElement In cPt.Elements
                    Dim value As Double
                    If Double.TryParse(coord.Value, Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, value) Then
                        valueList.Add(value)
                    End If
                Next
            End If
            cartesianPoints.Add(valueList.ToArray)
        Next

        Return cartesianPoints.Where(Function(pt) pt.Length > 2).Any(Function(pts) pts(2) <> 0)
    End Function

    Friend Shared Function GetTempFilePath() As String
        Dim ppszPath As IntPtr
        Dim hr As String = System.Windows.Native.SHGetKnownFolderPath(System.Windows.Native.FOLDERID_LocalAppDataLow, 0, ppszPath)
        Dim dir As New DirectoryInfo(hr)
        Return IO.Path.Combine(dir.FullName, IO.Path.ChangeExtension(IO.Path.GetRandomFileName, ".svg"))
    End Function

End Class
