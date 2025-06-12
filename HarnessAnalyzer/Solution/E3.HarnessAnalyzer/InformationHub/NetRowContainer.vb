Public Structure NetRowInfoContainer

    Property NetName As String
    Property NetType As String

    Public Overrides Function ToString() As String
        Return String.Format("{0}|{1}", NetName, NetType) ' HINT: this is to simulate compatibility which was done be calling row.Tag?.ToString on the netrow when this object is set to the net-row-tag, if you want to access the extra information in this container it must be casted to the type
    End Function

    Public Function GetConnections(kbl As KblMapper) As IEnumerable(Of Connection)
        Dim myNetName As String = Me.NetName
        Return kbl.GetConnections.Where(Function(con) Not String.IsNullOrEmpty(con.Signal_name) AndAlso con.Signal_name = myNetName)
    End Function

End Structure
