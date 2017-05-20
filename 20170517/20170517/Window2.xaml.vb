Imports System.Windows.Controls.Primitives


Public Class Window2
    Private MyControlTemplate As ControlTemplate
    Private WithEvents MyThumb1 As Thumb
    Private WithEvents MyThumb2 As Thumb


    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Call SetAddButton()
        Call SetControlTemplate()
        MyThumb1 = New Thumb
        Call SetThumb("D:\ブログ用\テスト用画像\hueRect255.png", 0, 0, MyThumb1, 10)
        MyThumb2 = New Thumb
        Call SetThumb("D:\ブログ用\テスト用画像\hueRect135.png", 50, 20, MyThumb2, 0)
        'Call SetThumb("D:\ブログ用\テスト用画像\RectangleBlack50x50.png", 50, 50)
        'Call SetCanvasSize()

        Dim sld As Slider
        Dim ro As RotateTransform = GetTransform(GetTemplateCanvas(MyThumb1), GetType(RotateTransform))
        With MyStackPanel左.Children
            sld = SetSliderBinding(CreateSlider(180, 1.0), ro, RotateTransform.AngleProperty)
            AddHandler sld.ValueChanged, AddressOf Slider_ValueChanged
            .Add(sld)
            .Add(CreateTextBlock(ro, RotateTransform.AngleProperty))

            'MyThumb1のImageの変形後のRectサイズ

            .Add(CreateTextBlock(MyThumb1, Image.ActualWidthProperty))
            .Add(CreateTextBlock(MyThumb1, Image.ActualHeightProperty))
        End With


    End Sub
    Private Function GetTemplateImage(t As Thumb) As Image
        Return CType(t.Template.FindName("TempImage", t), Image)
    End Function
    Private Function GetTemplateCanvas(t As Thumb) As Canvas
        Return CType(t.Template.FindName("TempCanvas", t), Canvas)
    End Function
    'ControlTemplate作成
    Private Sub SetControlTemplate()
        'FrameworkElementFactory作成
        Dim TempImage As New FrameworkElementFactory(GetType(Image), "TempImage")
        Dim TempCanvas As New FrameworkElementFactory(GetType(Canvas), "TempCanvas")
        'Canvasの中にImageを設置
        TempCanvas.AppendChild(TempImage)
        'ControlTemplate作成
        Dim cTemp As New ControlTemplate(GetType(Thumb))
        cTemp.VisualTree = TempCanvas
        MyControlTemplate = cTemp
    End Sub
    'ファイルパスからBitmapImage取得
    Private Function GetImageFromPath(imagePath As String) As BitmapImage
        Dim fileP As New Uri(imagePath)
        Dim bmp As New BitmapImage(fileP)
        Return bmp
    End Function

    'RenderTransformを作成
    Private Function GetRenderTransform(Optional scale As Double = 1.0, Optional skew As Double = 0.0, Optional angle As Double = 0.0) As Transform
        Dim tg As New TransformGroup
        With tg.Children
            .Add(New ScaleTransform(scale, scale))
            .Add(New SkewTransform(skew, skew))
            .Add(New RotateTransform(angle))
        End With
        Return tg
    End Function

    'TransformGroupの中の目的のTransform取得
    Private Function GetTransform(ele As UIElement, tt As Type) As Transform
        Dim tg As TransformGroup = ele.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = tt Then
                Return c
            End If
        Next
        Return Nothing
    End Function

    'Thumbを設置
    Private Sub SetThumb(imagePath As String, x As Double, y As Double, t As Thumb, angle As Double)
        'Template適用
        t.Template = MyControlTemplate
        t.ApplyTemplate() 'Template再構築

        '画像設定
        Call GetImageFromPath(imagePath)
        Dim bmp As BitmapImage = GetImageFromPath(imagePath)
        GetTemplateImage(t).Source = bmp

        'TemplateCanvasにRenderTransformの初期設定
        Dim c As Canvas = GetTemplateCanvas(t)
        c.RenderTransform = GetRenderTransform()
        c.RenderTransformOrigin = New Point(0.5, 0.5)
        c.Width = bmp.Width : c.Height = bmp.Height 'サイズ設定


        'Canvasに設置
        MyCanvas.Children.Add(t)
        Canvas.SetLeft(t, x)
        Canvas.SetTop(t, y)

    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Call SetCanvasSize()
    End Sub

    Private Sub SetAddButton()
        Dim btn As Button
        btn = New Button With {.Content = "Check"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf MyCheck

        'btn = New Button With {.Content = "AdjustLocate"}
        'MyStackPanel左.Children.Add(btn)
        'AddHandler btn.Click, AddressOf Adjust_Click

        'btn = New Button With {.Content = "SetCanvasSize"}
        'MyStackPanel左.Children.Add(btn)
        'AddHandler btn.Click, AddressOf SetCanvasSize_Click

        'btn = New Button With {.Content = "SetThumbSize"}
        'MyStackPanel左.Children.Add(btn)
        'AddHandler btn.Click, AddressOf SetThumbSize_Click

        'btn = New Button With {.Content = "SetTemplateCanvasSize"}
        'MyStackPanel左.Children.Add(btn)
        'AddHandler btn.Click, AddressOf SetTemplateCanvasSize_Click


        'btn = New Button With {.Content = "SaveImage"}
        'MyStackPanel左.Children.Add(btn)
        'AddHandler btn.Click, AddressOf Save_Click

    End Sub
    'チェック
    Private Sub MyCheck(obj As Object, e As RoutedEventArgs)
        Dim tempC = MyControlTemplate.FindName("TempCanvas", MyThumb1)
        Dim tempI As Image = MyControlTemplate.FindName("TempImage", MyThumb1)
        Dim ct As ControlTemplate = MyThumb1.Template
        Dim tc As Canvas = ct.FindName("TempCanvas", MyThumb1)
        Dim ti As Image = ct.FindName("TempImage", MyThumb1)
        Dim cs As Double = MyCanvas.Width
        Dim t2 As Double = MyThumb2.ActualHeight
        Dim gt1 As GeneralTransform = MyThumb1.TransformToVisual(MyCanvas)
        Dim gt2 As GeneralTransform = MyThumb1.TransformToVisual(MyThumb1)
        Dim gt3 As GeneralTransform = tempC.TransformToVisual(MyCanvas)
        Dim gt4 As GeneralTransform = tempC.TransformToVisual(MyThumb1)
        Dim gt5 As GeneralTransform = tempI.TransformToVisual(MyCanvas)
        Dim gt6 As GeneralTransform = tempI.TransformToVisual(MyThumb1)
        Dim p1 As Point = tempI.TransformToVisual(MyThumb1).Transform(New Point())
        Dim p2 As Point = tempI.TransformToVisual(tempC).Transform(New Point())
        Dim p3 As Point = tempC.TransformToVisual(MyThumb1).Transform(New Point())
        Dim p4 As Point = tempC.TransformToVisual(tempI).Transform(New Point())
        Dim p5 As Point = MyThumb1.TransformToVisual(tempC).Transform(New Point())
        Dim p6 As Point = MyThumb1.TransformToVisual(tempI).Transform(New Point())
        Dim p7 As Point = tempI.TransformToVisual(MyCanvas).Transform(New Point())
        Dim p8 As Point = tempC.TransformToVisual(MyCanvas).Transform(New Point())
        Dim p9 As Point = MyThumb1.TransformToVisual(MyCanvas).Transform(New Point())
        Dim p10 As Point = MyCanvas.TransformToVisual(MyThumb1).Transform(New Point())
        Dim p11 As Point = MyCanvas.TransformToVisual(tempI).Transform(New Point())
        Dim p12 As Point = MyCanvas.TransformToVisual(tempC).Transform(New Point())
        Dim s1 As Size = New Size(tempI.ActualWidth, tempI.ActualHeight)
        Dim s2 As Size = New Size(tempC.ActualWidth, tempC.ActualHeight)
        Dim s3 As Size = New Size(MyThumb1.ActualWidth, MyThumb1.ActualHeight)
        Dim s4 As Size = New Size(MyCanvas.ActualWidth, MyCanvas.ActualHeight)


    End Sub

    'スライダー設置
    Private Function CreateSlider(max As Double, tf As Double) As Slider
        Return New Slider With {.Maximum = max, .TickFrequency = tf,
            .LargeChange = tf, .IsSnapToTickEnabled = True}
    End Function
    Private Function SetSliderBinding(sld As Slider, dObj As DependencyObject, dPro As DependencyProperty) As Slider
        Dim b As New Binding With {
            .Source = dObj,
            .Path = New PropertyPath(dPro),
            .StringFormat = "{0}"}
        sld.SetBinding(Slider.ValueProperty, b)
        Return sld
    End Function

    'TextBlock設置
    Private Function CreateTextBlock(dObj As DependencyObject, dPro As DependencyProperty) As TextBlock
        Dim b As New Binding With {
            .Source = dObj,
            .Path = New PropertyPath(dPro),
            .StringFormat = dPro.ToString & " = {0:0.0}"}
        Dim tb As New TextBlock
        tb.SetBinding(TextBlock.TextProperty, b)
        Return tb
    End Function

    'ドラッグ移動
    Private Sub MyThumb_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles MyThumb1.DragDelta, MyThumb2.DragDelta
        Canvas.SetLeft(MyThumb1, Canvas.GetLeft(MyThumb1) + e.HorizontalChange)
        Canvas.SetTop(MyThumb1, Canvas.GetTop(MyThumb1) + e.VerticalChange)

    End Sub

    Private Function GetRectOnMyCanvas(target As FrameworkElement) As Rect
        Dim tf As GeneralTransform = target.TransformToVisual(MyCanvas)
        Return tf.TransformBounds(New Rect(New Size(target.ActualWidth, target.ActualHeight)))
    End Function
    Private Function GetRectOnMyCanvasWithLocate(target As FrameworkElement) As Rect
        Dim tf As GeneralTransform = target.TransformToVisual(MyCanvas)

        Return tf.TransformBounds(New Rect(New Size(target.ActualWidth, target.ActualHeight)))
    End Function

    Private Function GetRect(t As Thumb) As Rect
        Dim gt As GeneralTransform = GetTemplateCanvas(t).TransformToVisual(t)
        Return gt.TransformBounds(New Rect(New Size(t.ActualWidth, t.ActualHeight)))
    End Function
    '2つのThumbが収まるRectを取得
    Private Function GetUnionRect() As Rect
        Dim r1 As Rect = GetRectOnMyCanvas(GetTemplateCanvas(MyThumb1))
        Dim r2 As Rect = GetRectOnMyCanvas(GetTemplateCanvas(MyThumb2))
        Return Rect.Union(r1, r2)
    End Function

    '再描画
    Private Sub ReRender(t As Thumb)
        t.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                 End Sub)
    End Sub

    'TextBlock表示更新
    Private Sub SubUpdateInfo(r As Rect, tb As TextBlock, name As String)
        Dim str As String = $"{name} {r.Left.ToString("0.0")}, {r.Top.ToString("0.0")}, {r.Width.ToString("0.0")}, {r.Height.ToString("0.0")}"
        tb.Text = str
    End Sub
    Private Sub UpDateInfo()
        '再描画、しないと変形後のサイズが取れない
        Call ReRender(MyThumb1)

        'MyThumb1
        Dim r As Rect = GetRectOnMyCanvas(GetTemplateCanvas(MyThumb1))
        Call SubUpdateInfo(r, tbRectSize1, "MyThumb1")

        Call ReRender(MyThumb2)
        'MyThumb2
        r = GetRectOnMyCanvas(GetTemplateCanvas(MyThumb2))
        Call SubUpdateInfo(r, tbRectSize2, "MyThumb2")

        '全体が収まるRect
        r = GetUnionRect()
        Call SubUpdateInfo(r, tbRectSizeAll, "MyCanvas")

    End Sub

    'Slider動かしたとき
    Private Sub Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        '情報表示更新
        'Call SetThumbSize(MyThumb1)
        Call UpDateInfo()

    End Sub

End Class
