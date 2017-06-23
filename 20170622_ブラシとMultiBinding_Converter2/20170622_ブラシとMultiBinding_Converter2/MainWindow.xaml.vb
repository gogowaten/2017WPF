Imports System.Globalization

'RGB各色の値をSolidColorBrushに変換する
Public Class MyConverterRGB2Brush
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

    'parameterに値を設定したい要素を渡す、今回は各スライダーのあるMainWindow
    'ブラシの色をRGB各値に変換
    Public Function ConvertBack(value As Object,
                                targetTypes() As Type,
                                parameter As Object,
                                culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        'Throw New NotImplementedException()
        Dim c As MainWindow = parameter
        Dim b As SolidColorBrush = value
        'スライダーに値設定
        c.sldR.Value = b.Color.R
        c.sldG.Value = b.Color.G
        c.sldB.Value = b.Color.B
        Return New Object() {} '空を返している
    End Function
End Class



Class MainWindow

    Private Sub SetSolidColorBrush()
        MyBorder.Background = New SolidColorBrush(Color.FromRgb(255, 180, 50))
    End Sub
    Private Sub SetBrushes()
        MyBorder.Background = Brushes.AliceBlue
    End Sub

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btSetColor1.Click, AddressOf SetSolidColorBrush
        AddHandler btSetColor2.Click, AddressOf SetBrushes

        'RGB各色のスライダーをソースにしてバインディングを作成
        Dim bindR As New Binding With {.Source = sldR, .Path = New PropertyPath(Slider.ValueProperty), .Mode = BindingMode.TwoWay}
        Dim bindG As New Binding With {.Source = sldG, .Path = New PropertyPath(Slider.ValueProperty), .Mode = BindingMode.TwoWay}
        Dim bindB As New Binding With {.Source = sldB, .Path = New PropertyPath(Slider.ValueProperty), .Mode = BindingMode.TwoWay}
        'マルチバインディング作成
        Dim mb As New MultiBinding
        With mb
            .Converter = New MyConverterRGB2Brush
            .Mode = BindingMode.TwoWay
            .ConverterParameter = Me 'これ大事
            '3つのバインディングを詰め込む
            With .Bindings
                .Add(bindR) : .Add(bindG) : .Add(bindB)
            End With
        End With
        'BorderのBackground(Brush)にマルチバインディング
        MyBorder.SetBinding(Border.BackgroundProperty, mb)

        MyBorder.Background = Brushes.Blue
    End Sub
End Class



