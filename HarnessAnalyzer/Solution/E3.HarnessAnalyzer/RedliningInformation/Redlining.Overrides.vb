Imports System.ComponentModel
Imports System.Xml
Imports System.Xml.Serialization

Partial Public Class Redlining

    <EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>
    <XmlElement(NameOf(ObjectType))>
    Public Property ObjectType_Serialize As String
        Get
            Return Me.ObjectType.ToStringOrXmlName
        End Get
        Set(value As String)
            Dim res As Object = Nothing
            If Xml.UtilsEx.TryParseXmlEnumValue(Me.ObjectType.GetType, value, res) Then
                Me.ObjectType = CType(res, KblObjectType)
            Else
                'HINT: this is the fallback to the old value which was stored localized (!) -> this backward.resolver searches for any localized string who is mapped to KblObjectType and returns it
                Me.ObjectType = E3.Lib.Schema.Kbl.Utils.FindKblObjectTypeFromLocalizedName(value)
            End If
        End Set
    End Property

End Class
