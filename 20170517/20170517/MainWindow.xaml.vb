Imports System.Windows.Controls.Primitives

Class MainWindow
    Private MyControlTemplate As ControlTemplate
    Private WithEvents MyThumb1 As Thumb
    Private WithEvents MyThumb2 As Thumb
    Private WithEvents ss As Slider

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
        Dim ro As RotateTransform = GetTransform(GetTemplateImage(MyThumb1), GetType(RotateTransform))
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

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Call SetTemplateCanvasSize(MyThumb1)
        'Call SetTemplateCanvasSize(MyThumb2)
        Call SetCanvasSize()
    End Sub

    Private Sub SetAddButton()
        Dim btn As Button
        btn = New Button With {.Content = "Check"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf MyCheck

        btn = New Button With {.Content = "AdjustLocate"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf Adjust_Click

        btn = New Button With {.Content = "SetCanvasSize"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf SetCanvasSize_Click

        'btn = New Button With {.Content = "SetTemplateCanvasSize"}
        'MyStackPanel左.Children.Add(btn)
        'AddHandler btn.Click, AddressOf SetTemplateCanvasSize_Click


        btn = New Button With {.Content = "SaveImage"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf Save_Click

    End Sub
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

    'Thumbを設置
    Private Sub SetThumb(imagePath As String, x As Double, y As Double, t As Thumb, angle As Double)
        With t
            .Template = MyControlTemplate
        End With
        t.ApplyTemplate() 'Template再構築

        'TemplateImageにRenderTransformの初期設定
        Call SetRenderT(GetTemplateImage(t),,, angle)

        MyCanvas.Children.Add(t)
        Canvas.SetLeft(t, x)
        Canvas.SetTop(t, y)

        Call SetImageFile(imagePath, t)
    End Sub

    'Imageに画像ファイルを設定
    Private Sub SetImageFile(imagePath As String, t As Thumb)
        Dim fileP As New Uri(imagePath)
        Dim bmp As New BitmapImage(fileP)
        Dim ct As ControlTemplate = t.Template
        Dim i As Image = t.Template.FindName("TempImage", t)
        i.Source = bmp
    End Sub

    'RenderTransformを設定
    Private Sub SetRenderT(Obj As UIElement, Optional scale As Double = 1.0, Optional skew As Double = 0.0, Optional angle As Double = 0.0)
        Dim tg As New TransformGroup
        With tg.Children
            .Add(New ScaleTransform(scale, scale))
            .Add(New SkewTransform(skew, skew))
            .Add(New RotateTransform(angle))
        End With
        Obj.RenderTransform = tg
        Obj.RenderTransformOrigin = New Point(0.5, 0.5)
    End Sub

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

    Private Function GetTemplateImage(t As Thumb) As Image
        Return CType(t.Template.FindName("TempImage", t), Image)
    End Function
    Private Function GetTemplateCanvas(t As Thumb) As Canvas
        Return CType(t.Template.FindName("TempCanvas", t), Canvas)
    End Function

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
    Private Sub MyThumb_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles MyThumb1.DragDelta
        Canvas.SetLeft(MyThumb1, Canvas.GetLeft(MyThumb1) + e.HorizontalChange)
        Canvas.SetTop(MyThumb1, Canvas.GetTop(MyThumb1) + e.VerticalChange)

    End Sub

    ''TemplateCanvasのサイズ設定
    'Private Sub SetTemplateCanvasSize(t As Thumb)
    '    'Dim r As Rect = GetRect(GetTemplateImage(t))
    '    'Dim c As Canvas = GetTemplateCanvas(t)
    '    'c.Width = r.Width : c.Height = r.Height
    'End Sub
    'Private Sub SetTemplateCanvasSize_Click(sender As Object, e As RoutedEventArgs)
    '    'Call SetTemplateCanvasSize(MyThumb1)
    '    'Call SetTemplateCanvasSize(MyThumb2)
    'End Sub


    'MyCanvasのサイズ設定
    Private Sub SetCanvasSize()
        Dim r As Rect = GetUnionRect()
        MyCanvas.Width = r.Width
        MyCanvas.Height = r.Height
    End Sub
    Private Sub SetCanvasSize_Click(sender As Object, e As RoutedEventArgs)
        Call SetCanvasSize()
    End Sub

    '2つのThumbが収まるRectを取得
    Private Function GetUnionRect() As Rect
        Dim r1 As Rect = GetRect(GetTemplateImage(MyThumb1))
        Dim r2 As Rect = GetRect(GetTemplateImage(MyThumb2))
        Return Rect.Union(r1, r2)
    End Function

    'Thumbの位置合わせ、Canvasの左上に合わせる
    Private Sub AdjustLocate()
        Dim r1 As Rect = GetRect(GetTemplateImage(MyThumb1))
        Dim r2 As Rect = GetRect(GetTemplateImage(MyThumb2))
        Dim r As Rect = GetUnionRect()

        If r.Left <> 0 Then
            Canvas.SetLeft(MyThumb1, Canvas.GetLeft(MyThumb1) - r.Left)
            Canvas.SetLeft(MyThumb2, Canvas.GetLeft(MyThumb2) - r.Left)
        End If
        If r.Top <> 0 Then
            Canvas.SetTop(MyThumb1, Canvas.GetTop(MyThumb1) - r.Top)
            Canvas.SetTop(MyThumb2, Canvas.GetTop(MyThumb2) - r.Top)
        End If
    End Sub
    Private Sub Adjust_Click(sender As Object, e As RoutedEventArgs)
        Call AdjustLocate()
        Call ChangeText()
    End Sub

    'ボタンイベント用
    'チェック
    Private Sub MyCheck(obj As Object, e As RoutedEventArgs)
        Dim tempC = MyControlTemplate.FindName("TempCanvas", MyThumb1)
        Dim tempI As Image = MyControlTemplate.FindName("TempImage", MyThumb1)
        Dim ct As ControlTemplate = MyThumb1.Template
        Dim tc As Canvas = ct.FindName("TempCanvas", MyThumb1)
        Dim ti As Image = ct.FindName("TempImage", MyThumb1)
        Dim cs As Double = MyCanvas.Width
        Dim t2 As Double = MyThumb2.ActualHeight

    End Sub

    '画像ファイル保存
    Private Sub SaveFileImage(target As FrameworkElement)
        Dim fileName As String = GetNowToString() & "_Image.png"
        Dim bmp As New RenderTargetBitmap(
            target.ActualWidth, target.ActualHeight, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New System.IO.FileStream(fileName, System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub
    Private Sub SaveFileImage2(target As FrameworkElement)
        Dim fileName As String = GetNowToString() & "_Image2.png"
        Dim img As Image = GetTemplateImage(MyThumb1)
        Dim tf As GeneralTransform = img.RenderTransform
        Dim r As Rect = tf.TransformBounds(New Rect(0, 0, img.ActualWidth, img.ActualHeight))

        Dim bmp As New RenderTargetBitmap(
            r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New System.IO.FileStream(fileName, System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub
    Private Sub SaveFileImage3(target As FrameworkElement)
        Dim fileName As String = GetNowToString() & "_Image3.png"
        Dim bmp As New RenderTargetBitmap(
            target.ActualWidth, target.ActualHeight, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Dim encoder As New PngBitmapEncoder
        encoder.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New System.IO.FileStream(fileName, System.IO.FileMode.Create)
            encoder.Save(fs)
        End Using
    End Sub

    Private Function GetRect(target As FrameworkElement) As Rect
        Dim tf As GeneralTransform = target.TransformToVisual(MyCanvas)
        Return tf.TransformBounds(New Rect(New Size(target.ActualWidth, target.ActualHeight)))
    End Function
    Private Sub Save_Click(sender As Object, e As RoutedEventArgs)
        'Call SaveFileImage(MyThumb)
        'Call SaveFileImage(GetTemplateImage(MyThumb))
        'Call SaveFileImage2(GetTemplateImage(MyThumb))
        Call SaveFileImage3(MyCanvas)
        'Call SaveFileImage3(GetTemplateCanvas(MyThumb1))
    End Sub
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
    End Function

    Private Sub ChangeText()
        '再描画、しないと変形後のサイズが取れない
        MyThumb1.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)
        '表示更新
        Dim r As Rect = GetRect(GetTemplateImage(MyThumb1))
        Dim str As String = $"{r.Left.ToString("0.0")}, {r.Top.ToString("0.0")}, {r.Width.ToString("0.0")}, {r.Height.ToString("0.0")}"
        tbRectSize1.Text = str

        MyThumb2.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)
        '表示更新
        Dim r2 As Rect = GetRect(GetTemplateImage(MyThumb2))
        Dim str2 As String = $"{r2.Left.ToString("0.0")}, {r2.Top.ToString("0.0")}, {r2.Width.ToString("0.0")}, {r2.Height.ToString("0.0")}"
        tbRectSize2.Text = str2

        '表示更新
        Dim rAll As Rect = GetUnionRect()
        Dim strAll As String = $"{rAll.Left.ToString("0.0")}, {rAll.Top.ToString("0.0")}, {rAll.Width.ToString("0.0")}, {rAll.Height.ToString("0.0")}"
        tbRectSizeAll.Text = strAll

    End Sub

    Private Sub Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        Call ChangeText()
    End Sub
End Class
