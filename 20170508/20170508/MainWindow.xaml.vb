Imports System.Windows.Controls.Primitives
'Imports System.Windows.Threading
'Imports System.IO
'Imports System.ComponentModel
'Imports System.IO.Compression
'Imports System.Runtime.Serialization
'Imports System.Globalization

Class MainWindow
    Private MyPath As Path
    Private MyImage As Image
    Private MyLine As Line


    Private Shared Function IncrementOne(ByVal i As Integer) As Integer
        Return i + 1
    End Function
    Sub Mytest()

    End Sub
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        AddHandler Button1.Click, AddressOf Button1_Click
        AddHandler Button2.Click, AddressOf Button2_Click
        AddHandler btnCheck1.Click, AddressOf btnCheck1_Click
        AddHandler btnEdge.Click, AddressOf btnEdge_Click
        AddHandler btnUseLayoutRounding.Click, AddressOf btnUseLayoutRounding_Click
        AddHandler btnSnapsToDevicePixels.Click, AddressOf btnSnapsToDevicePixels_Click
        AddHandler btnScalingMode.Click, AddressOf btnNearestNeighbor_Click
        AddHandler btnLocateReset.Click, AddressOf btnLocateReset_Click

        'Me.VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor



        'MyPath = New Path With {.Stroke = Brushes.Red, .StrokeThickness = 1}
        'MyPath.Data = New RectangleGeometry(New Rect(0, 0, 50, 50))
        'Canvas.SetLeft(MyPath, 100) : Canvas.SetTop(MyPath, 10)
        'MyCanvas.Children.Add(MyPath)
        'RenderOptions.SetEdgeMode(MyPath, EdgeMode.Aliased)

        'Dim bi As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\hueRect135.png"))
        Dim bi As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\border_round.bmp"))


        MyImage = New Image
        MyImage.Source = bi
        MyImage.RenderTransform = GetRenderTransform()

        MyImage.RenderTransformOrigin = New Point(0.5, 0.5)

        Canvas.SetLeft(MyImage, 20) : Canvas.SetTop(MyImage, 20)
        'Canvas.SetLeft(MyImage, 20.5) : Canvas.SetTop(MyImage, 50.5)
        MyCanvas.Children.Add(MyImage)


        Call MySetBinding()

    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim w1 As New Window1 : w1.Show()
        Dim w2 As New Window2 : w2.Show()

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
    Private Sub CanvasToImageSave(target As FrameworkElement)
        Dim bmp As New RenderTargetBitmap(
            target.ActualWidth, target.ActualHeight, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New System.IO.FileStream("testImage.png", System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub
    Private Sub ElemantToImageSave(target As FrameworkElement)
        Dim img As Image = target

        Dim bf As BitmapFrame = BitmapFrame.Create(img.Source)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bf))
        Using fs As New System.IO.FileStream("testImage.png", System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub
    Private Function GetRect(target As FrameworkElement) As Rect
        Dim gt As GeneralTransform = target.TransformToVisual(MyCanvas)
        Dim s As Size = New Size(target.ActualWidth, target.ActualHeight)
        Dim r As Rect = gt.TransformBounds(New Rect(s))
        Return r
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

    Private Sub Button1_Click(sender As Object, e As RoutedEventArgs)
        Call CanvasToImageSave(MyCanvas)
    End Sub
    Private Sub Button2_Click(sender As Object, e As RoutedEventArgs)
        Call ElemantToImageSave(MyImage)
    End Sub

    Private Sub btnCheck1_Click(sender As Object, e As RoutedEventArgs)
        MsgBox($"{MyImage.ActualWidth}, {MyImage.ActualHeight}")
        MsgBox(Canvas.GetLeft(MyImage))
    End Sub

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
        Dim sm As Integer = RenderOptions.GetBitmapScalingMode(MyImage)
        If sm < 3 Then
            sm += 1
        Else
            sm = 0
        End If
        RenderOptions.SetBitmapScalingMode(MyImage, sm)
    End Sub

    Private Sub btnLocateReset_Click(sender As Object, e As RoutedEventArgs)
        Canvas.SetTop(MyImage, 20)
    End Sub

End Class
