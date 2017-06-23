


Class MainWindow
    '右へ10
    Private Sub AddLeft10()
        Canvas.SetLeft(MyBorder, Canvas.GetLeft(MyBorder) + 10)
    End Sub
    '左へ10
    Private Sub SubLeft10()
        Canvas.SetLeft(MyBorder, Canvas.GetLeft(MyBorder) - 10)
    End Sub

    'バインディング作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty, strF As String) As Binding
        Dim b As New Binding With {
           .Source = sObj,
           .Path = New PropertyPath(sDp),
           .Mode = BindingMode.TwoWay,
           .StringFormat = strF}
        Return b
    End Function

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'ボタンのクリックイベントに関連付け
        AddHandler btAdd10.Click, AddressOf AddLeft10
        AddHandler btSub10.Click, AddressOf SubLeft10

        'バインディング
        Dim bindL As Binding = GetMyBinding(MyBorder, Canvas.LeftProperty, "CanvasLeft = {0:0.0}")
        sldCanvasLeft.SetBinding(Slider.ValueProperty, bindL)
        tbCanvasLeft.SetBinding(TextBlock.TextProperty, bindL)
        Dim bindT As Binding = GetMyBinding(MyBorder, Canvas.TopProperty, "CanvasTop = {0:0.0}")
        sldCanvasTop.SetBinding(Slider.ValueProperty, bindT)
        tbCanvasTop.SetBinding(TextBlock.TextProperty, bindT)
    End Sub

End Class

