'TemplateのCanvasを変形させる版

Imports System.Windows.Controls.Primitives


Public Class Window1
    Private MyControlTemplate As ControlTemplate
    Private WithEvents MyThumb1 As Thumb
    Private WithEvents MyThumb2 As Thumb
    Private TemplateCanvas1 As Canvas
    Private TemplateImage1 As Image
    Private TemplateCanvas2 As Canvas
    Private TemplateImage2 As Image


    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Call SetAddButton()
        Call SetControlTemplate()
        MyThumb1 = New Thumb
        Call SetThumb("D:\ブログ用\テスト用画像\hueRect255.png", 50, 20, MyThumb1, 10)
        MyThumb2 = New Thumb
        Call SetThumb("D:\ブログ用\テスト用画像\hueRect135.png", 0, 0, MyThumb2, 0)
        'Call SetThumb("D:\ブログ用\テスト用画像\RectangleBlack50x50.png", 50, 50)
        'Call SetCanvasSize()

        TemplateCanvas1 = GetTemplateCanvas(MyThumb1)
        TemplateCanvas2 = GetTemplateCanvas(MyThumb2)
        TemplateImage1 = GetTemplateImage(MyThumb1)
        TemplateImage2 = GetTemplateImage(MyThumb2)

        Dim sld As Slider
        Dim ro As RotateTransform = GetTransform(TemplateCanvas1, GetType(RotateTransform))
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

        btn = New Button With {.Content = "SetThumbSize"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf SetThumbSize_Click

        btn = New Button With {.Content = "SetTemplateCanvasSize"}
        MyStackPanel左.Children.Add(btn)
        AddHandler btn.Click, AddressOf SetTemplateCanvasSize_Click


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


    ''TempCanvasのサイズ設定
    'Private Sub SetTemplateCanvasSize(t As Thumb)
    '    Dim c As Canvas = GetTemplateCanvas(t)
    '    Dim r As Rect = GetRect(GetTemplateImage(t))
    '    c.Width = r.Width
    '    c.Height = r.Height
    'End Sub
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



    'MyCanvasのサイズ設定
    Private Sub SetCanvasSize()
        Dim r As Rect = GetUnionRect()
        MyCanvas.Width = r.Width
        MyCanvas.Height = r.Height
    End Sub
    Private Sub SetCanvasSize_Click(sender As Object, e As RoutedEventArgs)
        Call SetCanvasSize()
    End Sub

    'MyThumbのサイズ設定
    Private Sub SetThumbSize(t As Thumb)
        Dim r As Rect = GetRect(GetTemplateCanvas(t))
        t.Width = r.Width
        t.Height = r.Height
        'Dim x As Double = Canvas.GetLeft(t)
        'Dim xx As Double = r.X - x
        'Dim xxx As Double = x + xx
        'Canvas.SetLeft(t, r.X)
        'Canvas.SetTop(t, r.Y)
    End Sub
    Private Sub SetThumbSize_Click(sender As Object, e As RoutedEventArgs)
        Call SetThumbSize(MyThumb1)
        Call SetThumbSize(MyThumb2)
    End Sub

    'TemplateCanvasのサイズ設定
    Private Sub SetTemplateCanvasSize(t As Thumb)
        Dim c As Canvas = GetTemplateCanvas(t)
        Dim r As Rect = GetRect(c)
        c.Width = r.Width
        c.Height = r.Height
    End Sub
    Private Sub SetTemplateCanvasSize_Click(sender As Object, e As RoutedEventArgs)
        Call SetTemplateCanvasSize(MyThumb1)
        Call SetTemplateCanvasSize(MyThumb2)
    End Sub
    '2つのThumbが収まるRectを取得
    Private Function GetUnionRect() As Rect
        Dim r1 As Rect = GetRect(GetTemplateCanvas(MyThumb1))
        Dim r2 As Rect = GetRect(GetTemplateCanvas(MyThumb2))
        Return Rect.Union(r1, r2)
    End Function

    'Thumbの位置合わせ、Canvasの左上に合わせる
    Private Sub AdjustLocate()
        Dim r1 As Rect = GetRect(GetTemplateCanvas(MyThumb1))
        Dim r2 As Rect = GetRect(GetTemplateCanvas(MyThumb2))
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
        Call UpDateInfo()
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
    Private Sub SaveFileImage2(target As FrameworkElement, fn As String)
        Dim fileName As String = GetNowToString() & fn & ".png"
        Dim img As Image = GetTemplateImage(MyThumb1)
        Dim tf As GeneralTransform = img.RenderTransform
        Dim r As Rect = tf.TransformBounds(New Rect(0, 0, img.ActualWidth, img.ActualHeight))

        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Call BitmapToPngFile(bmp, fileName)
    End Sub
    Private Sub SaveFileImage3(target As FrameworkElement, fn As String)
        Dim fileName As String = GetNowToString() & fn & ".png"
        Dim bmp As New RenderTargetBitmap(target.ActualWidth, target.ActualHeight, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Call BitmapToPngFile(bmp, fileName)
    End Sub
    Private Sub SaveFileImage4(target As FrameworkElement, fn As String)
        Dim fileName As String = GetNowToString() & fn & ".png"
        Dim img As Image = GetTemplateImage(MyThumb1)
        Dim tf As GeneralTransform = img.RenderTransform
        Dim r As Rect = tf.TransformBounds(New Rect(0, 0, img.ActualWidth, img.ActualHeight))

        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(target)
        Call BitmapToPngFile(bmp, fileName)
    End Sub

    Private Sub BitmapToPngFile(bmp As BitmapSource, fileName As String)
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
    Private Function GetRect2(c As Canvas)
        Dim gt As GeneralTransform = c.TransformToAncestor(MyThumb1)

    End Function
    Private Sub Save_Click(sender As Object, e As RoutedEventArgs)
        'Call SaveFileImage2(GetTemplateImage(MyThumb1), "GetTemplateImage")
        'Call SaveFileImage2(GetTemplateCanvas(MyThumb1), "GetTemplateCanvas")
        'Call SaveFileImage2(MyThumb1, "MyThumb1")
        'Call SaveFileImage3(MyCanvas)
        Call SaveFileImage3(GetTemplateCanvas(MyThumb1), "TemplateCanvas")
        'Call SaveFileImage3(GetTemplateImage(MyThumb1), "TemplateImage")
        'Call SaveFileImage3(MyThumb1, "MyThumb1")
    End Sub
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
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
        Dim r As Rect = GetRect(GetTemplateCanvas(MyThumb1))
        Call SubUpdateInfo(r, tbRectSize1, "MyThumb1")

        Call ReRender(MyThumb2)
        'MyThumb2
        r = GetRect(GetTemplateCanvas(MyThumb2))
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
