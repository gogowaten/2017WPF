Imports System.Globalization

Class MainWindow

    Public Shared ReadOnly MyRProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyR), GetType(Integer), GetType(MainWindow))
    Public Property MyR As Integer
        Get
            Return GetValue(MyRProperty)
        End Get
        Set(value As Integer)
            SetValue(MyRProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyGProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyG), GetType(Integer), GetType(MainWindow))
    Public Property MyG As Integer
        Get
            Return GetValue(MyGProperty)
        End Get
        Set(value As Integer)
            SetValue(MyGProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MyBProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyB), GetType(Integer), GetType(MainWindow))
    Public Property MyB As Integer
        Get
            Return GetValue(MyBProperty)
        End Get
        Set(value As Integer)
            SetValue(MyBProperty, value)
        End Set
    End Property

    '未使用
    Public Shared ReadOnly MyBrushProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyBRush), GetType(SolidColorBrush), GetType(MainWindow), New PropertyMetadata(New SolidColorBrush))
    Public Property MyBRush As SolidColorBrush
        Get
            Return GetValue(MyBrushProperty)
        End Get
        Set(value As SolidColorBrush)
            SetValue(MyBrushProperty, value)
        End Set
    End Property

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btSetColor1.Click, AddressOf SetSolidColorBrush
        AddHandler btSetColor2.Click, AddressOf SetBrushes
        AddHandler btSetColor3.Click, AddressOf SetColorRed200

        Dim bindR As New Binding With {.Source = Me, .Path = New PropertyPath(MyRProperty), .Mode = BindingMode.TwoWay}
        Dim bindG As New Binding With {.Source = Me, .Path = New PropertyPath(MyGProperty), .Mode = BindingMode.TwoWay}
        Dim bindB As New Binding With {.Source = Me, .Path = New PropertyPath(MyBProperty), .Mode = BindingMode.TwoWay}
        sldR.SetBinding(Slider.ValueProperty, bindR)
        sldG.SetBinding(Slider.ValueProperty, bindG)
        sldB.SetBinding(Slider.ValueProperty, bindB)

        Dim mb As New MultiBinding
        With mb
            .Converter = New BrushColorConverter
            .Mode = BindingMode.TwoWay
            .ConverterParameter = Me
            With .Bindings
                .Add(bindR) : .Add(bindG) : .Add(bindB)
            End With
        End With
        'BindingOperations.SetBinding(Me, MyBrushProperty, mb)
        'Dim bind As New Binding With {.Source = Me, .Path = New PropertyPath(MyBrushProperty), .Mode = BindingMode.TwoWay}
        'MyBorder.SetBinding(Border.BackgroundProperty, bind)

        MyBorder.SetBinding(Border.BackgroundProperty, mb)



        'うまくいかないバージョン
        'Dim b As New Binding With {.Source = Me, .Path = New PropertyPath(MyBrushProperty), .Mode = BindingMode.TwoWay}
        'BindingOperations.SetBinding(MyBorder, BackgroundProperty, b)

        'Dim bindR As New Binding With {.Source = Me, .Path = New PropertyPath(MyRProperty), .Mode = BindingMode.TwoWay}
        'Dim bindG As New Binding With {.Source = Me, .Path = New PropertyPath(MyGProperty), .Mode = BindingMode.TwoWay}
        'Dim bindB As New Binding With {.Source = Me, .Path = New PropertyPath(MyBProperty), .Mode = BindingMode.TwoWay}
        'sldR.SetBinding(Slider.ValueProperty, bindR)
        'sldG.SetBinding(Slider.ValueProperty, bindG)
        'sldB.SetBinding(Slider.ValueProperty, bindB)

        'Dim mb As New MultiBinding
        'With mb
        '    .Converter = New BrushColorConverter2
        '    .Mode = BindingMode.TwoWay
        '    With .Bindings
        '        .Add(bindR) : .Add(bindG) : .Add(bindB)
        '    End With
        'End With
        ''BindingOperations.SetBinding(Me, MyBrushProperty, mb)
        'MyBorder.SetBinding(Border.BackgroundProperty, mb)


        MyBorder.Background = Brushes.RoyalBlue
    End Sub

    Private Sub SetSolidColorBrush()
        MyBorder.Background = New SolidColorBrush(Color.FromRgb(255, 180, 50))
        'これはうまくいかない
        'MyBorder.Background.SetValue(SolidColorBrush.ColorProperty, Color.FromRgb(0, 220, 0)) 
    End Sub
    Private Sub SetBrushes()
        MyBorder.Background = Brushes.AliceBlue
    End Sub
    Private Sub SetColorRed200()
        Me.MyR = 200
    End Sub

End Class

Public Class BrushColorConverter
    Implements IMultiValueConverter
    'RGB各値を使ってブラシを作って返す
    Public Function Convert(values() As Object,
                            targetType As Type,
                            parameter As Object,
                            culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        '    Throw New NotImplementedException()
        Dim b As New SolidColorBrush(Color.FromRgb(values(0), values(1), values(2)))
        Return b
    End Function

    'ブラシの色をRGB各値に変換
    'parameterにRGBの各値を設定したいプロパティを持つオブジェクトを入れる
    Public Function ConvertBack(value As Object,
                                targetTypes() As Type,
                                parameter As Object,
                                culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        'Throw New NotImplementedException()
        Dim c As MainWindow = parameter
        Dim b As SolidColorBrush = value
        'RGB各値を設定
        With b.Color
            c.MyR = .R : c.MyG = .G : c.MyB = .B
        End With
        'RGBの各値を戻り値にしてもうまくいかないので空を返している
        'Dim obj() As Object = New Object() {b.Color.R, b.Color.G, b.Color.B}
        Return New Object() {} ' obj
    End Function
End Class


''これはConvertBackがうまくいかないバージョン
''未使用
'Public Class BrushColorConverter2
'    Implements IMultiValueConverter

'    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
'        '    Throw New NotImplementedException()
'        Dim b As New SolidColorBrush(Color.FromRgb(values(0), values(1), values(2)))
'        Return b
'    End Function

'    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
'        Dim b As SolidColorBrush = value
'        Dim obj() As Object = New Object() {b.Color.R, b.Color.G, b.Color.B}
'        Return obj
'    End Function
'End Class
