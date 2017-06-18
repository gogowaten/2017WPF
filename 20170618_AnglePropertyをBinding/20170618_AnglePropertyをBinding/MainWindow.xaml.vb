Class MainWindow
    Private ActBorder As Class1

    '今の回転角度に+10する
    Private Sub AddAngle10()
        ActBorder.MyAngle += 10
    End Sub

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnAngle.Click, AddressOf AddAngle10
        'Class1作成、回転角度の初期値に20を設定
        Dim MyBorder = New Class1(20) With {.Width = 100, .Height = 50, .Background = Brushes.Red}
        MyCanvas.Children.Add(MyBorder)
        Canvas.SetLeft(MyBorder, 20)
        Canvas.SetTop(MyBorder, 30)
        ActBorder = MyBorder

        'バインディングソース作成
        Dim b As New Binding With {
    .Source = MyBorder,
    .Path = New PropertyPath(Class1.MyAngleProperty),
    .Mode = BindingMode.TwoWay,
    .StringFormat = "Angle = {0:0}"}
        'スライダーとテキストブロックそれぞれにバインディング
        sldAngle.SetBinding(Slider.ValueProperty, b)
        tbAngle.SetBinding(TextBlock.TextProperty, b)
    End Sub
End Class
