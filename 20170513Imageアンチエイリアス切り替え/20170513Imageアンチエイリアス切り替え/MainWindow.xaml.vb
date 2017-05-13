
Class MainWindow
    Private MyImage As Image

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnEdge.Click, AddressOf btnEdge_Click
        AddHandler btnUseLayoutRounding.Click, AddressOf btnUseLayoutRounding_Click
        AddHandler btnSnapsToDevicePixels.Click, AddressOf btnSnapsToDevicePixels_Click
        AddHandler btnScalingMode.Click, AddressOf btnNearestNeighbor_Click
        AddHandler btnReset.Click, AddressOf btnReset_Click

        Dim bi As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\border_round.bmp"))
        'Dim bi As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\NEC_8042_2017_05_09_午後わてん_.jpg"))

        MyImage = New Image With {
            .Source = bi,
            .RenderTransform = GetRenderTransform(),
            .RenderTransformOrigin = New Point(0.5, 0.5)
        }

        Canvas.SetLeft(MyImage, 50) : Canvas.SetTop(MyImage, 50)
        MyCanvas.Children.Add(MyImage)

        Call MySetBinding()
    End Sub
    Private Function GetRenderTransform() As Transform
        Dim tg As New TransformGroup
        With tg.Children
            .Add(New ScaleTransform(1.0, 1.0))
            .Add(New SkewTransform)
            .Add(New RotateTransform)
        End With
        Return tg
    End Function

    'Binding
    Private Sub MySetBinding()
        Dim b As Binding

        b = New Binding With {
            .Source = MyImage,
            .Path = New PropertyPath(RenderOptions.EdgeModeProperty),
            .StringFormat = "EdgeMode = {0}"
        }
        tbEdge.SetBinding(TextBlock.TextProperty, b)

        b = New Binding With {
            .Source = MyImage,
            .Path = New PropertyPath(UseLayoutRoundingProperty),
            .StringFormat = "UseLayoutRounding = {0}"}
        tbUseLayout.SetBinding(TextBlock.TextProperty, b)

        b = New Binding With {
            .Source = MyImage,
            .Path = New PropertyPath(SnapsToDevicePixelsProperty),
            .StringFormat = "SnapsToDevicePixels = {0}"}
        tbSnapTo.SetBinding(TextBlock.TextProperty, b)

        b = New Binding With {
            .Source = MyImage,
            .Path = New PropertyPath(RenderOptions.BitmapScalingModeProperty),
            .StringFormat = "ScalingMode = {0}"}
        tbScalingMode.SetBinding(TextBlock.TextProperty, b)

        Dim st As ScaleTransform = GetTransform(GetType(ScaleTransform))
        b = New Binding With {.Source = st, .Path = New PropertyPath(ScaleTransform.ScaleXProperty),
            .StringFormat = "ScaleX = {0}"}
        sldScaleX.SetBinding(Slider.ValueProperty, b)
        tbScaleX.SetBinding(TextBlock.TextProperty, b)
        b = New Binding With {.Source = st, .Path = New PropertyPath(ScaleTransform.ScaleYProperty),
            .StringFormat = "ScaleY = {0}"}
        sldScaleY.SetBinding(Slider.ValueProperty, b)
        tbScaleY.SetBinding(TextBlock.TextProperty, b)

        Dim ro As RotateTransform = GetTransform(GetType(RotateTransform))
        b = New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty),
            .StringFormat = "RotateAngle = {0}"}
        sldRotateAngle.SetBinding(Slider.ValueProperty, b)
        tbRotateAngle.SetBinding(TextBlock.TextProperty, b)

        b = New Binding With {.Source = MyImage, .Path = New PropertyPath(Canvas.TopProperty),
            .StringFormat = "CanvasTop = {0}"}
        sldCanvasTop.SetBinding(Slider.ValueProperty, b)
        tbCanvasTop.SetBinding(TextBlock.TextProperty, b)

    End Sub

    Private Function GetTransform(t As Type) As Transform
        Dim tg As TransformGroup = MyImage.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = t Then
                Return c
            End If
        Next
        Return Nothing
    End Function

    'イベント

    Private Sub btnEdge_Click(sender As Object, e As RoutedEventArgs)
        If RenderOptions.GetEdgeMode(MyImage) = EdgeMode.Aliased Then
            RenderOptions.SetEdgeMode(MyImage, EdgeMode.Unspecified)
        Else
            RenderOptions.SetEdgeMode(MyImage, EdgeMode.Aliased)
        End If
    End Sub

    Private Sub btnUseLayoutRounding_Click(sender As Object, e As RoutedEventArgs)
        MyImage.UseLayoutRounding = Not MyImage.UseLayoutRounding
    End Sub

    Private Sub btnSnapsToDevicePixels_Click(sender As Object, e As RoutedEventArgs)
        MyImage.SnapsToDevicePixels = Not MyImage.SnapsToDevicePixels
    End Sub

    Private Sub btnNearestNeighbor_Click(sender As Object, e As RoutedEventArgs)
        If RenderOptions.GetBitmapScalingMode(MyImage) = BitmapScalingMode.NearestNeighbor Then
            RenderOptions.SetBitmapScalingMode(MyImage, BitmapScalingMode.Unspecified)
        Else
            RenderOptions.SetBitmapScalingMode(MyImage, BitmapScalingMode.NearestNeighbor)
        End If
    End Sub

    Private Sub btnReset_Click(sender As Object, e As RoutedEventArgs)
        sldRotateAngle.Value = 0
        sldScaleX.Value = 1
        sldScaleY.Value = 1
        sldCanvasTop.Value = 50
        'Canvas.SetTop(MyImage, 50)
    End Sub
End Class
