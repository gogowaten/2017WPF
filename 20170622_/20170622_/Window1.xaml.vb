Imports System.Globalization

Public Class Window1

    Public Shared ReadOnly MyRProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyR), GetType(Double), GetType(Window1))
    Public Property MyR As Double
        Get
            Return GetValue(MyRProperty)
        End Get
        Set(value As Double)
            SetValue(MyRProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyGProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyG), GetType(Double), GetType(Window1))
    Public Property MyG As Double
        Get
            Return GetValue(MyGProperty)
        End Get
        Set(value As Double)
            SetValue(MyGProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyBProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyB), GetType(Double), GetType(Window1))
    Public Property MyB As Double
        Get
            Return GetValue(MyBProperty)
        End Get
        Set(value As Double)
            SetValue(MyBProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyBrushProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyBRush), GetType(SolidColorBrush), GetType(Window1))
    Public Property MyBRush As SolidColorBrush
        Get
            Return GetValue(MyBrushProperty)
        End Get
        Set(value As SolidColorBrush)
            SetValue(MyBrushProperty, value)
        End Set
    End Property

    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'Dim br As New Binding With {.Source = sldR, .Path = New PropertyPath(Slider.ValueProperty), .Mode = BindingMode.TwoWay}
        'Dim bg As New Binding With {.Source = sldG, .Path = New PropertyPath(Slider.ValueProperty), .Mode = BindingMode.TwoWay}
        'Dim bb As New Binding With {.Source = sldB, .Path = New PropertyPath(Slider.ValueProperty), .Mode = BindingMode.TwoWay}
        ''Dim mb As New MultiBinding
        ''With mb.Bindings
        ''    .Add(br) : .Add(bg) : .Add(bb)
        ''End With
        ''mb.Converter = New BrushColorConverter
        ''mb.Mode = BindingMode.TwoWay
        ''MyBorder.SetBinding(Border.BackgroundProperty, mb)

        'MyBorder.Background = New SolidColorBrush(Color.FromRgb(0, 0, 0)) ' Brushes.Red
        'Dim mb2 As New MultiBinding
        'With mb2
        '    .Mode = BindingMode.TwoWay
        '    .Converter = New MyColorConverter
        '    With .Bindings
        '        .Add(br) : .Add(bg) : .Add(bb)
        '    End With
        'End With
        'BindingOperations.SetBinding(MyBorder.Background, SolidColorBrush.ColorProperty, mb2)

        AddHandler btCheck.Click, AddressOf MyCheck

        Dim bcr As New Binding With {.Source = Me, .Path = New PropertyPath(MyRProperty), .Mode = BindingMode.TwoWay}
        Dim bcg As New Binding With {.Source = Me, .Path = New PropertyPath(MyGProperty), .Mode = BindingMode.TwoWay}
        Dim bcb As New Binding With {.Source = Me, .Path = New PropertyPath(MyBProperty), .Mode = BindingMode.TwoWay}
        Dim mb As New MultiBinding
        With mb
            .Converter = New BrushColorConverter
            .Mode = BindingMode.TwoWay
            .ConverterParameter = Me
            With .Bindings
                .Add(bcr) : .Add(bcg) : .Add(bcb)
            End With
        End With
        BindingOperations.SetBinding(Me, MyBrushProperty, mb)
        Dim bind As New Binding With {.Source = Me, .Path = New PropertyPath(MyBrushProperty), .Mode = BindingMode.TwoWay}
        MyBorder.SetBinding(Border.BackgroundProperty, bind)

        sldR.SetBinding(Slider.ValueProperty, bcr)
        sldG.SetBinding(Slider.ValueProperty, bcg)
        sldB.SetBinding(Slider.ValueProperty, bcb)

    End Sub

    Private Sub MyCheck()
        'MyBorder.Background = New SolidColorBrush(Color.FromRgb(255, 180, 50)) 
        'MyBorder.Background = Brushes.AliceBlue
        'Me.MyR = 200
        'MyBorder.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromRgb(244, 0, 0)) 'これはできなかった
    End Sub
End Class

Public Class BrushColorConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        '    Throw New NotImplementedException()
        Dim b As New SolidColorBrush(Color.FromRgb(values(0), values(1), values(2)))
        Return b
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        'Throw New NotImplementedException()
        Dim c As Window1 = parameter

        Dim b As SolidColorBrush = value
        c.MyR = b.Color.R : c.MyG = b.Color.G : c.MyB = b.Color.B
        Dim obj() As Object = New Object() {b.Color.R, b.Color.G, b.Color.B}
        Return obj
    End Function
End Class

Public Class MyColorConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        '    Throw New NotImplementedException()
        Dim c As Color = Color.FromRgb(values(0), values(1), values(2))
        Return c
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        '   Throw New NotImplementedException()
        Dim b As Color = value
        Dim obj() As Object = New Object() {b.R, b.G, b.B}
        Return obj
    End Function
End Class

Public Class MyColorConverter2
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert

    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class