'WPF、CanvasLeftTopとSliderValueをBinding ( ソフトウェア ) - 午後わてんのブログ - Yahoo!ブログ
'https://blogs.yahoo.co.jp/gogowaten/14988621.html



Class MainWindow
    '右へ10
    Private Sub AddLeft10()
        'Canvas.SetLeft(MyBorder, Canvas.GetLeft(MyBorder) + 10)
        MyLeft += 10
    End Sub
    '左へ10
    Private Sub SubLeft10()
        'Canvas.SetLeft(MyBorder, Canvas.GetLeft(MyBorder) - 10)
        MyLeft -= 10
    End Sub

    Public Shared ReadOnly MyLeftProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyLeft), GetType(Double), GetType(Border), New PropertyMetadata(0.0))
    Public Property MyLeft As Double
        Get
            Return GetValue(MyLeftProperty)
        End Get
        Set(value As Double)
            SetValue(MyLeftProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MyTopProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MyTop), GetType(Double), GetType(Border), New PropertyMetadata(0.0))
    Public Property MyTop As Double
        Get
            Return GetValue(MyTopProperty)
        End Get
        Set(value As Double)
            SetValue(MyTopProperty, value)
        End Set
    End Property


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
        'Dim bindL As Binding = GetMyBinding(MyBorder, Canvas.LeftProperty, "CanvasLeft = {0:0.0}")
        'sldCanvasLeft.SetBinding(Slider.ValueProperty, bindL)
        'tbCanvasLeft.SetBinding(TextBlock.TextProperty, bindL)
        'Dim bindT As Binding = GetMyBinding(MyBorder, Canvas.TopProperty, "CanvasTop = {0:0.0}")
        'sldCanvasTop.SetBinding(Slider.ValueProperty, bindT)
        'tbCanvasTop.SetBinding(TextBlock.TextProperty, bindT)

        Dim bindL As Binding = GetMyBinding(Me, MyLeftProperty, "MyLeft = {0:0.0}")
        MyBorder.SetBinding(LeftProperty, bindL)
        sldCanvasLeft.SetBinding(Slider.ValueProperty, bindL)
        tbCanvasLeft.SetBinding(TextBlock.TextProperty, bindL)
        Dim bindT As Binding = GetMyBinding(Me, MyTopProperty, "MyTop = {0:0.0}")
        MyBorder.SetBinding(TopProperty, bindT)
        sldCanvasTop.SetBinding(Slider.ValueProperty, bindT)
        tbCanvasTop.SetBinding(TextBlock.TextProperty, bindT)
    End Sub

End Class

