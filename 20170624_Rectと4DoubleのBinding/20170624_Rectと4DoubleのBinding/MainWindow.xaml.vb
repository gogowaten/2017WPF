'わからん

Imports System.Globalization

Class MainWindow

    Private Sub AddLeft10()
        MyLeft += 10
    End Sub
    Private Sub SetNewRect()
        MyRect = New Rect(10, 20, 30, 40)
    End Sub
    Private Sub AddRectX10()
        MyRect = New Rect(MyRect.X + 10, MyRect.Y, MyRect.Width, MyRect.Height)
        'MyRect.X += 10 'これはエラー、Rectは構造体なので値型
    End Sub

    Public Shared ReadOnly MyRectProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyRect), GetType(Rect), GetType(MainWindow))
    Public Property MyRect As Rect
        Get
            Return GetValue(MyRectProperty)
        End Get
        Set(value As Rect)
            SetValue(MyRectProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MyLeftProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyLeft), GetType(Double), GetType(MainWindow))
    Public Property MyLeft As Double
        Get
            Return GetValue(MyLeftProperty)
        End Get
        Set(value As Double)
            SetValue(MyLeftProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MyTopProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyTop), GetType(Double), GetType(MainWindow))
    Public Property MyTop As Double
        Get
            Return GetValue(MyTopProperty)
        End Get
        Set(value As Double)
            SetValue(MyTopProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MyWidthProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyWidth), GetType(Double), GetType(MainWindow))
    Public Property MyWidth As Double
        Get
            Return GetValue(MyWidthProperty)
        End Get
        Set(value As Double)
            SetValue(MyWidthProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MyHeightProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyHeight), GetType(Double), GetType(MainWindow))
    Public Property MyHeight As Double
        Get
            Return GetValue(MyHeightProperty)
        End Get
        Set(value As Double)
            SetValue(MyHeightProperty, value)
        End Set
    End Property

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        MyRect = New Rect(29, 30, 10, 80)

        AddHandler btnCheck.Click, AddressOf AddLeft10
        AddHandler btnSetNewRect.Click, AddressOf SetNewRect
        AddHandler btnAddRectX.Click, AddressOf AddRectX10

        '表示用Binding
        Dim b As New Binding With {.Source = Me, .Path = New PropertyPath(MyRectProperty)}
        tbRect.SetBinding(TextBlock.TextProperty, b)
        b = New Binding With {.Source = Me, .Path = New PropertyPath(MyLeftProperty)}
        tbLeft.SetBinding(TextBlock.TextProperty, b)

        'Rectと4values
        Dim l As New Binding With {.Source = Me, .Path = New PropertyPath(MyLeftProperty), .Mode = BindingMode.TwoWay}
        Dim t As New Binding With {.Source = Me, .Path = New PropertyPath(MyTopProperty), .Mode = BindingMode.TwoWay}
        Dim w As New Binding With {.Source = Me, .Path = New PropertyPath(MyWidthProperty), .Mode = BindingMode.TwoWay}
        Dim h As New Binding With {.Source = Me, .Path = New PropertyPath(MyHeightProperty), .Mode = BindingMode.TwoWay}
        Dim mb As New MultiBinding
        With mb
            .Converter = New MyConverter
            .Mode = BindingMode.TwoWay

            With .Bindings
                .Add(l) : .Add(t) : .Add(w) : .Add(h)
            End With
        End With
        BindingOperations.SetBinding(Me, MyRectProperty, mb)

    End Sub
End Class

Public Class MyConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        '  Throw New NotImplementedException()
        Dim r As New Rect(values(0), values(1), values(2), values(3))
        Return r
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object,
                                culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        'Throw New NotImplementedException()
        Dim r As Rect = value
        Dim obj() As Object = New Object() {r.X, r.Y, r.Width, r.Height}
        Return obj

    End Function
End Class