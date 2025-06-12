'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.7.3081.0")>
<System.SerializableAttribute()>
<System.Diagnostics.DebuggerStepThroughAttribute()>
<System.ComponentModel.DesignerCategoryAttribute("code")>
<System.Xml.Serialization.XmlTypeAttribute(AnonymousType:=True)>
<System.Xml.Serialization.XmlRootAttribute([Namespace]:="", IsNullable:=False)>
Partial Public Class ValidityCheckEntry

    Private codeField As String

    Private descriptionField As String

    Private textField As String

    Private resolutionHintField As String

    Private typeField As String

    Private idField As String

    Private objectTypeField As KblObjectType

    Private objectTypeFieldSpecified As Boolean

    '''<remarks/>
    Public Property Code() As String
        Get
            Return Me.codeField
        End Get
        Set
            Me.codeField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Description() As String
        Get
            Return Me.descriptionField
        End Get
        Set
            Me.descriptionField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Text() As String
        Get
            Return Me.textField
        End Get
        Set
            Me.textField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property ResolutionHint() As String
        Get
            Return Me.resolutionHintField
        End Get
        Set
            Me.resolutionHintField = Value
        End Set
    End Property

    '''<remarks/>
    Public Property Type() As String
        Get
            Return Me.typeField
        End Get
        Set
            Me.typeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property ID() As String
        Get
            Return Me.idField
        End Get
        Set
            Me.idField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlAttributeAttribute()>
    Public Property ObjectType() As KblObjectType
        Get
            Return Me.objectTypeField
        End Get
        Set
            Me.objectTypeField = Value
        End Set
    End Property

    '''<remarks/>
    <System.Xml.Serialization.XmlIgnoreAttribute()>
    Public Property ObjectTypeSpecified() As Boolean
        Get
            Return Me.objectTypeFieldSpecified
        End Get
        Set
            Me.objectTypeFieldSpecified = Value
        End Set
    End Property
End Class
