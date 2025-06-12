Imports System.ComponentModel
Imports System.Reflection
Imports Infragistics.Win.UltraWinGrid

Namespace Checks.Cavities.Views

    Public Class BaseView
        Implements INotifyPropertyChanged

        Private _visible As Boolean = True
        Private _readonly As Boolean = False
        Private _initialForeColor As Nullable(Of Color)

        Private Shared _propertiesAllowedDisabling As String() = Nothing

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub New()
        End Sub

        Protected Sub New(other As BaseView)
            Me._visible = other._visible
            Me._readonly = other._readonly
        End Sub

        <Browsable(False)>
        ReadOnly Property Id As Guid = Guid.NewGuid

        <Browsable(False)>
        Overridable Property Visible As Boolean
            <DebuggerStepThrough>
            Get
                Return _visible
            End Get
            <DebuggerStepThrough>
            Set(value As Boolean)
                If _visible <> value Then
                    _visible = value
                    OnPropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Visible)))
                End If
            End Set
        End Property

        <Browsable(False)>
        Overridable Property [Readonly] As Boolean
            <DebuggerStepThrough>
            Get
                Return _readonly
            End Get
            <DebuggerStepThrough>
            Set(value As Boolean)
                If _readonly <> value Then
                    _readonly = value
                    OnPropertyChanged(Me, New PropertyChangedEventArgs(NameOf([Readonly])))
                End If
            End Set
        End Property

        <DebuggerStepThrough>
        Public Function IsPropertyReadonly(propertyName As String) As Boolean
            Return Me.Readonly AndAlso Me.PropertiesAllowedDisabling.Contains(propertyName)
        End Function

        <Browsable(False)>
        Property Model As Model.ModelView

        <DebuggerStepThrough>
        Friend Sub InitializeRow(row As UltraGridRow)
            row.Hidden = Not Visible
            If Not _initialForeColor.HasValue Then
                _initialForeColor = row.Appearance.ForeColor
            End If

            InitializeRowCore(row, Color.Gray, _initialForeColor.GetValueOrDefault)
        End Sub

        Protected Overridable Sub InitializeRowCore(row As UltraGridRow, disabledColor As Color, normalColor As Color)
            If [Readonly] Then
                row.Appearance.ForeColor = disabledColor
            Else
                row.Appearance.ForeColor = normalColor
            End If
        End Sub

        Protected Overridable Sub OnPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
            RaiseEvent PropertyChanged(sender, e)
        End Sub

        Protected ReadOnly Property PropertiesAllowedDisabling As String()
            Get
                If _propertiesAllowedDisabling Is Nothing Then
                    Dim list As New List(Of String)
                    For Each p As PropertyInfo In Me.GetType.GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                        Dim attribAllowed As AllowDisableAttribute = p.GetCustomAttribute(Of AllowDisableAttribute)
                        If attribAllowed IsNot Nothing AndAlso attribAllowed.Allowed Then
                            list.Add(p.Name)
                        End If
                    Next
                    _propertiesAllowedDisabling = list.ToArray
                End If
                Return _propertiesAllowedDisabling
            End Get
        End Property

        Friend Property Parent As BaseView

    End Class

End Namespace