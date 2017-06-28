Imports System.ComponentModel
Imports System.Globalization

Class MainWindow
    Dim TestRect As New MyDependencyRect
    Private Sub Add10()
        TestRect.MyLeft += 10
    End Sub
    Private Sub SetRect()
        'TestRect = New MyDependencyRect(1, 2, 300, 100) 'Bindingが無郊になってしまう、Newだから

    End Sub
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btn1.Click, AddressOf Add10
        AddHandler btn2.Click, AddressOf SetRect


        tb1.SetBinding(TextBlock.TextProperty, New Binding With {.Source = TestRect, .Path = New PropertyPath(MyDependencyRect.MyLeftProperty), .Mode = BindingMode.TwoWay})
        tb2.SetBinding(TextBlock.TextProperty, New Binding With {.Source = TestRect, .Path = New PropertyPath(MyDependencyRect.MyRectProperty), .Mode = BindingMode.TwoWay})


    End Sub
End Class

Public Class MyRect
    Implements System.ComponentModel.INotifyPropertyChanged

    Private Property _MyRect As Rect
    Public Property MyRect As Rect
        Get
            Return _MyRect
        End Get
        Set(value As Rect)
            _MyRect = value
        End Set
    End Property

    Private Property _MyLeft As Double
    Public Property MyLeft As Double
        Get
            Return _MyLeft
        End Get
        Set(value As Double)
            _MyLeft = value
        End Set
    End Property
    Private Property _MyTop As Double
    Public Property MyTop As Double
        Get
            Return _MyTop
        End Get
        Set(value As Double)
            _MyTop = value
        End Set
    End Property
    Private Property _MyWidth As Double
    Public Property MyWidth As Double
        Get
            Return _MyWidth
        End Get
        Set(value As Double)
            _MyWidth = value
        End Set
    End Property
    Private Property _MyHeight As Double
    Public Property MyHeight As Double
        Get
            Return _MyHeight
        End Get
        Set(value As Double)
            _MyHeight = value
            Call NotifyPropertyChanged()
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(<System.Runtime.CompilerServices.CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

Public Class MyDependencyRect
    Inherits DependencyObject

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
    Private Sub test()
        Dim c As New MyMultiConverter
    End Sub
    Private Class MyMultiConverter
        Implements IMultiValueConverter

        Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
            '    Throw New NotImplementedException()
            Dim r As New Rect(values(0), values(1), values(2), values(3))
            Return r
        End Function

        Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
            'Throw New NotImplementedException()
            Dim r As Rect = value
            Dim v(3) As Double
            v(0) = r.X
            Return New Object() {r.X, r.Y, r.Width, r.Height}
            'Return New String() {r.X, r.Y, r.Width, r.Height}
            'Return New String() {"", ""}

            'Return New Double() {} 'error
            'Return {r.X, r.Y, r.Width, r.Height}
            'Return Binding.DoNothing
        End Function
    End Class
    Private Class MyConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
            '    Throw New NotImplementedException()
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
            Throw New NotImplementedException()
        End Function
    End Class
    Private Sub SetMyBinding()
        Dim bl As New Binding With {.Source = Me, .Path = New PropertyPath(MyLeftProperty), .Mode = BindingMode.TwoWay}
        Dim bt As New Binding With {.Source = Me, .Path = New PropertyPath(MyTopProperty), .Mode = BindingMode.TwoWay}
        Dim bw As New Binding With {.Source = Me, .Path = New PropertyPath(MyWidthProperty), .Mode = BindingMode.TwoWay}
        Dim bh As New Binding With {.Source = Me, .Path = New PropertyPath(MyHeightProperty), .Mode = BindingMode.TwoWay}
        Dim mb As New MultiBinding
        With mb
            .Converter = New MyMultiConverter
            .Mode = BindingMode.TwoWay
            With .Bindings
                .Add(bl) : .Add(bt) : .Add(bw) : .Add(bh)
            End With
        End With
        BindingOperations.SetBinding(Me, MyRectProperty, mb)
    End Sub
    Public Sub New()
        Call Me.New(1, 1, 1, 1)
    End Sub
    Public Sub New(x As Double, y As Double, w As Double, h As Double)
        Call SetMyBinding()
        MyRect = New Rect(x, y, w, h)
    End Sub
End Class