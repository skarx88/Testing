Imports System.ComponentModel
Imports System.Printing
Imports System.Runtime.Serialization

Namespace Printing

    <DataContract>
    Public Class PageEx
        Implements INotifyPropertyChanged

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        'HINT: backing fields are needed for serialization of readonly properties
        <DataMember()> Private WithEvents _margins As MarginsEx 'HINT: for serialization
        <DataMember()> Private _Name As String
        <DataMember()> Private _PageType As PageMediaSizeName
        <DataMember()> Private _Height As MilliInch96
        <DataMember()> Private _Width As MilliInch96
        <DataMember()> Private _IsCustom As Boolean
        <DataMember()> Private _PageKind As Drawing.Printing.PaperKind

        <IgnoreDataMember> Private _eventsEnabled As Boolean

        Friend Sub New(page2023 As Compatibility.Printing.PageEx_2023)
            Me.New(page2023.Name, page2023.PageType, page2023.Width, page2023.Height)
            SetMarginsCore(New MarginsEx(page2023.Margins))
        End Sub

        Public Sub New(pageMediaSize As PageMediaSize)
            Me.New(MediaSizeNameConverter.GetCleanName(pageMediaSize.PageMediaSizeName.GetValueOrDefault), pageMediaSize.PageMediaSizeName.GetValueOrDefault, pageMediaSize.Width.GetValueOrDefault, pageMediaSize.Height.GetValueOrDefault)
        End Sub

        Public Sub New(page As Drawing.Printing.PaperSize)
            Me.New(page.PaperName, PaperTypeConverter.ToPageMediaSize(page.Kind), New MilliInch100(page.Width).ToInch96, New MilliInch100(page.Height).ToInch96)
        End Sub

        Public Sub New(name As String, pageType As PageMediaSizeName, widthMilliInch96 As Double, heightMilliInch96 As Double)
            Me._Name = name
            Me._PageType = pageType
            Me._Height = New MilliInch96(heightMilliInch96)
            Me._Width = New MilliInch96(widthMilliInch96)
            Me._PageKind = PaperTypeConverter.ToKind(_PageType)
            SetMarginsCore(New MarginsEx)
            _eventsEnabled = True
        End Sub

        Private Sub SetMarginsCore(margins As MarginsEx)
            _margins = margins
        End Sub

        ReadOnly Property Name As String
            Get
                Return _Name
            End Get
        End Property

        ReadOnly Property PageType As PageMediaSizeName
            Get
                Return _PageType
            End Get
        End Property

        ReadOnly Property Height As MilliInch96
            Get
                Return _Height
            End Get
        End Property

        ReadOnly Property Width As MilliInch96
            Get
                Return _Width
            End Get
        End Property

        ReadOnly Property IsCustom As Boolean
            Get
                Return _IsCustom
            End Get
        End Property

        ReadOnly Property PageKind As Drawing.Printing.PaperKind
            Get
                Return _PageKind
            End Get
        End Property

        Property Margins As MarginsEx
            Get
                Return _margins
            End Get
            Set(value As MarginsEx)
                If Not value Is _margins Then
                    SetMarginsCore(value)
                    OnPropertyChanged(New PropertyChangedEventArgs(NameOf(Margins)))
                End If
            End Set
        End Property

        Protected Overridable Sub OnPropertyChanged(e As PropertyChangedEventArgs)
            If _eventsEnabled Then
                RaiseEvent PropertyChanged(Me, e)
            End If
        End Sub

        Private Sub _margins_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _margins.PropertyChanged
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Margins)))
        End Sub

        Public Function ToPaperSize() As System.Drawing.Printing.PaperSize
            Dim ps As New Drawing.Printing.PaperSize(Me.Name, CInt(Me.Width.ToInch100), CInt(Me.Height.ToInch100))
            ps.RawKind = Me.PageKind
            Return ps
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0} ({1}x{2})", Me.Name, Me.Width, Me.Height)
        End Function

        Public Shared Function NewCustom(widthMilliInch96 As Double, heightMilliInch96 As Double) As PageEx
            Return NewCustom(New MilliInch96(widthMilliInch96), New MilliInch96(heightMilliInch96))
        End Function

        Public Shared Function NewCustom(widthInch As MilliInch96, heightInch As MilliInch96) As PageEx
            Return New PageEx("Custom", PageMediaSizeName.Unknown, Nothing, Nothing) With {._IsCustom = True, ._Height = heightInch, ._Width = widthInch}
        End Function

        Public Shared Function NewCustom(widthInch As MilliInch96, heightInch As MilliInch96, margins As MarginsEx) As PageEx
            Return New PageEx("Custom", PageMediaSizeName.Unknown, Nothing, Nothing) With {._IsCustom = True, ._Height = heightInch, ._Width = widthInch, .Margins = margins}
        End Function

        Public Shared Function NewCustom(widthInch As MilliInch100, heightInch As MilliInch100) As PageEx
            Return NewCustom(widthInch.ToInch96, heightInch.ToInch96)
        End Function

    End Class

End Namespace