'これは失敗、前回の方で決まり


'Thumb
'   ┗ControlTemplate
'      ┗Canvas(TempTopCanvas)
'         ┗Canvas(TempCanvas) 
'             ┣Border(TempBorder) これに回転や拡大を適用
'             ┗Image

'Canvasの中にBorderとImageを入れたControlTemplateを作成、これを
'ThumbのControlTemplateに指定
'回転や拡大はControlTemplateの中のCanvasに適用する
'回転の中心軸は中心、RenderTransformOriginal=point(0.5,0.5)
'Thumbを画像ファイル保存、アンチエイリアスなしでも表示されている見た目通りの画像にしたい



Imports System.Windows.Controls.Primitives


Class MainWindow

    Private MyControlTemplate As ControlTemplate
    Private TempBorder As Border 'ControlTemplateの中のBorder
    Private TempCanvas As Canvas 'ControlTemplateの中のCanvas
    Private TempTopCanvas As Canvas 'ControlTemplateの一番上のCanvas


    Private Sub TextUpdate()
        Dim r As Rect
        r = TempBorder.TransformToVisual(MyCanvas).TransformBounds(GetRect(TempBorder))
        MyTextBlock1.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        r = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, TempCanvas.ActualWidth, TempCanvas.ActualHeight))
        MyTextBlock2.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        MyTextBlock7.Text = $"左上 = ({r.X:0.00}, {r.Y:0.00})"
        MyTextBlock8.Text = $"右下 = ({r.X + r.Width:0.00}, {r.Y + r.Height:0.00})"

        r = MyCanvas.TransformToVisual(TempCanvas).TransformBounds(New Rect(100, 100, 100, 100))
        MyTextBlock3.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        r = MyCanvas.TransformToVisual(TempCanvas).TransformBounds(New Rect(0, 0, 100, 100))
        MyTextBlock4.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        'Dim gt As GeneralTransform = TempCanvas.RenderTransform
        'Dim gt0 As GeneralTransform = TempCanvas.TransformToVisual(MyCanvas)
        'Dim gt1 As GeneralTransform = TempCanvas.TransformToAncestor(MyCanvas)
        ''Dim gt2 As GeneralTransform = TempCanvas.TransformToDescendant(MyCanvas)
        'Dim gt3 As GeneralTransform = TempCanvas.TransformToAncestor(MyThumb1)
        ''Dim gt4 As GeneralTransform = TempCanvas.TransformToDescendant(MyThumb1)
        ''Dim gt5 As GeneralTransform = TempCanvas.TransformToAncestor(TempBorder)
        'Dim gt6 As GeneralTransform = TempBorder.TransformToDescendant(TempBorder)


        'RenderTransform
        r = TempCanvas.RenderTransform.TransformBounds(GetRect(MyThumb1)) 'na
        MyTextBlock5.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        r = TempCanvas.RenderTransform.TransformBounds(New Rect(0, 0, 100, 100))
        MyTextBlock6.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"


    End Sub
    Private Function GetRect(b As FrameworkElement)
        'Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.ActualWidth, b.ActualHeight)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call TextUpdate()
    End Sub
    'ControlTemplate作成
    Private Sub SetControlTemplate()
        'FrameworkElementFactory作成
        Dim TTopCanvas As New FrameworkElementFactory(GetType(Canvas), "TempTopCanvas")
        Dim TCanvas As New FrameworkElementFactory(GetType(Canvas), "TempCanvas")
        Dim TBorder As New FrameworkElementFactory(GetType(Border), "TempBorder")
        Dim TImage As New FrameworkElementFactory(GetType(Image), "TempImage")

        'Canvasの中にImageを設置
        TTopCanvas.AppendChild(TCanvas)
        TCanvas.AppendChild(TImage)
        TCanvas.AppendChild(TBorder)
        'ControlTemplate作成
        Dim cTemp As New ControlTemplate(GetType(Thumb))
        cTemp.VisualTree = TTopCanvas
        MyControlTemplate = cTemp
    End Sub
    'Thumb
    Private Sub SetThumb()
        MyThumb1.Template = MyControlTemplate
        MyThumb1.ApplyTemplate() 'Template再構築
        Dim ct As ControlTemplate = MyThumb1.Template
        TempTopCanvas = ct.FindName("TempTopCanvas", MyThumb1)
        TempCanvas = ct.FindName("TempCanvas", MyThumb1)
        TempBorder = ct.FindName("TempBorder", MyThumb1)
        With TempBorder
            .Width = 100
            .Height = 1
            .Background = Brushes.Red
        End With
        '必要なかも？
        Canvas.SetLeft(TempBorder, 0) : Canvas.SetTop(TempBorder, 0)

        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(0.6) '回転角度
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(ro)
        End With
        TempBorder.RenderTransform = tg
        'TempCanvas.RenderTransformOrigin = New Point(0.0, 0.0)
        TempBorder.RenderTransformOrigin = New Point(0.5, 0.5)

        Dim b As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty)}
        Angle1.SetBinding(Slider.ValueProperty, b)


        b = New Binding With {.Source = sldLeft, .Path = New PropertyPath(Slider.ValueProperty)}
        MyThumb1.SetBinding(LeftProperty, b)
        b = New Binding With {.Source = sldTop, .Path = New PropertyPath(Slider.ValueProperty)}
        MyThumb1.SetBinding(TopProperty, b)

        b = New Binding With {.Source = TempBorder, .Path = New PropertyPath(WidthProperty)}
        MyThumb1.SetBinding(WidthProperty, b)

        b = New Binding With {.Source = TempBorder, .Path = New PropertyPath(HeightProperty)}
        MyThumb1.SetBinding(HeightProperty, b)

        sldHeight.SetBinding(Slider.ValueProperty, b)

    End Sub

    '現在日時を文字列に変換
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
    End Function



    'アンチエイリアスありのとき用、SaveImageMyCanvasVB8を清書、他の図形考慮
    Private Sub SaveImageMyCanvasVB9(filePath As String)
        Dim b As Brush = MyCanvas.Background
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする

        '指定した背景色をここで反映させるには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        Dim s As New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, MyBlueBorder.ActualWidth, MyBlueBorder.ActualHeight)
        Dim r As Rect = Rect.Union(r1, r2)
        'ブラシで塗るOffset位置
        Dim xOffset As Integer = -Fix(r1.X)
        Dim yOffset As Integer = -Fix(r1.Y)

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(xOffset, yOffset), r.Size))
        End Using
        '
        'RenderTargetBitmapの(サイズ)
        Dim migi As Integer = Math.Ceiling(r1.X + r1.Width)
        Dim sita As Integer = Math.Ceiling(r1.Y + r1.Height)
        Dim wRender As Integer = migi + xOffset
        Dim hRender As Integer = sita + yOffset

        'RenderTargetBitmap
        Dim rtb As New RenderTargetBitmap(wRender, hRender, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        MyCanvas.Background = b '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示

        Call Bitmap2pngFile(rtb, filePath)
    End Sub


    Private Sub SaveImageMyCanvasDI3改(filePath As String)
        Dim b As Brush = MyCanvas.Background
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)


        Dim s As New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, MyBlueBorder.ActualWidth, MyBlueBorder.ActualHeight)
        Dim r As Rect = Rect.Union(r1, r2)
        'MyCanvas全体をbmp画像にする
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        'x,y、DrawImageのOffset位置を求める
        '変形四捨五入(0.5より大きいければ切り上げ、0.5以下なら切り捨て)する
        Dim fx As Double = r1.X - Fix(r1.X) '小数点以下の値取得
        Dim x As Integer = IIf(fx > 0.5, Math.Ceiling(r1.X), Fix(r1.X)) '変形四捨五入
        Dim fy As Double = r1.Y - Fix(r1.Y)
        Dim y As Integer = IIf(fy > 0.5, Math.Ceiling(r1.Y), Fix(r1.Y))
        'width,height、DrawImageのサイズ(幅と高さ)を求める
        Dim fw As Double = r.Width - Fix(r.Width)
        Dim w As Integer = IIf(fw > 0.5, Math.Ceiling(r.Width), Fix(r.Width))
        Dim fh As Double = r.Height - Fix(r.Height)
        Dim h As Integer = IIf(fh > 0.5, Math.Ceiling(r.Height), Fix(r.Height))

        'MyCanvasのbmpを使ってDrawImage
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawImage(bmp, New Rect(-x, -y, w, h))
        End Using

        'Renderするサイズを求める
        '幅 = 変形四捨五入した右端座標 - 変形四捨五入した左端座標
        Dim migi As Double = r1.X + r1.Width '右端座標
        Dim fMigi As Double = migi - Fix(migi) '右端座標の小数点以下の値取得
        Dim ww As Integer = IIf(fMigi > 0.5, Math.Ceiling(migi) - x, Fix(migi) - x) '幅
        '高さ
        Dim sita As Double = r1.Y + r1.Height
        Dim fSita As Double = sita - Fix(sita)
        Dim hh As Integer = IIf(fSita > 0.5, Math.Ceiling(sita) - y, Fix(sita) - y)
        'Render
        Dim rtb As New RenderTargetBitmap(ww, hh, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        'bmpをpng画像として保存
        Call Bitmap2pngFile(rtb, filePath)

        MyCanvas.Background = b '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
    End Sub




    'MyCanvasをBitmapにして、それをクロップ

    'SaveImageCropped1式を改良、他の図形と重なっているときでもOK

    'SaveImageCropped2式を改変、余白をなくしてみる

    'SaveImageCropped3式を清書
    Private Sub SaveImageCropped3式改(filePath As String)
        Dim s As New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, MyBlueBorder.ActualWidth, MyBlueBorder.ActualHeight)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)

        Dim bgc As Brush = MyCanvas.Background
        MyCanvas.Background = Brushes.Transparent '背景色透明
        MyBlueBorder.Visibility = Visibility.Hidden '他の要素を非表示
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        bmp.Render(MyCanvas)
        MyCanvas.Background = bgc
        MyBlueBorder.Visibility = Visibility.Visible

        'クロップする座標は変形の四捨五入(0.5より大きいければ切り上げ、0.5以下なら切り捨て)
        Dim xOffset As Double = r1.X - Fix(r1.X)
        Dim yOffset As Double = r1.Y - Fix(r1.Y)
        xOffset = IIf(xOffset > 0.5, Math.Ceiling(r1.X), Fix(r1.X))
        yOffset = IIf(yOffset > 0.5, Math.Ceiling(r1.Y), Fix(r1.Y))
        'クロップサイズ
        '幅＝変形四捨五入した右端 - 変形四捨五入した左端
        Dim xw As Double = r1.X + r1.Width '右端
        Dim wSize As Integer = IIf((xw - Fix(xw)) > 0.5, Math.Ceiling(xw), Fix(xw)) '右端座標を変形四捨五入
        wSize -= xOffset '変形四捨五入した右端 - 変形四捨五入した左端
        '高さ
        Dim yh As Double = r1.Y + r1.Height 'bottom
        Dim hSize As Integer = IIf((yh - Fix(yh)) > 0.5, Math.Ceiling(yh), Fix(yh))
        hSize -= yOffset
        Dim cropSize As New Int32Rect(xOffset, yOffset, wSize, hSize)

        'クロップ！
        Dim cb As New CroppedBitmap(bmp, cropSize)

        Call Bitmap2pngFile(cb, filePath)
    End Sub

    Private Sub SaveImageThumb(filePath As String)
        Dim r As Rect = TempBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)))
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        'bmp.Render(TempBorder)
        'bmp.Render(TempCanvas)
        bmp.Render(TempTopCanvas)
        Clipboard.SetImage(bmp)
        'Call Bitmap2pngFile(bmp, filePath)
    End Sub

    'だいたいOK、あとはサイズの微調整
    Private Sub SaveImageThumbVB(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        'Dim r2 As Rect = VisualTreeHelper.GetDescendantBounds(MyThumb1)
        'Dim r3 As Point = MyCanvas.TransformToVisual(TempBorder).Transform(New Point(0, 0))
        'Dim r4 As Point = MyCanvas.TransformToVisual(TempCanvas).Transform(New Point(0, 0))
        'Dim r5 As Point = TempBorder.TransformToVisual(MyThumb1).Transform(New Point(0, 0))
        'Dim r6 As Point = MyCanvas.TransformToVisual(MyThumb1).Transform(New Point(0, 0))
        'Dim r7 As Point = MyThumb1.TransformToVisual(TempCanvas).Transform(New Point(0, 0))
        'Dim r8 As Point = MyThumb1.TransformToVisual(TempBorder).Transform(New Point(0, 0))
        'Dim r9 As Point = MyThumb1.TransformToVisual(MyCanvas).Transform(New Point(0, 0))

        Dim vb As New VisualBrush(MyThumb1)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
        End Using
        Dim bmp As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)

    End Sub

    Private Sub SaveImageTempTopCanvasVB(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim vb As New VisualBrush(TempTopCanvas)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
        End Using
        Dim bmp As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
    End Sub

    Private Sub SaveImageTempCanvasVB(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim vb As New VisualBrush(TempCanvas)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
        End Using
        Dim bmp As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
    End Sub

    Private Sub SaveImageTempBorderVB(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        Dim r1 As Rect = TempBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim vb As New VisualBrush(TempBorder)
        Dim ox As Double = (r1.Width Mod 2) / 2
        Dim oy As Double = 1 - ((r1.Height - 1) / 2)

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0.45, r1.Width, 3))
        End Using
        'Dim bmp As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        Dim bmp As New RenderTargetBitmap(110, 10, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
    End Sub





    'MyCanvas全てを画像ファイルにする、OK
    Private Sub SaveAllImage(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    Private Sub Bitmap2pngFile(bmp As BitmapSource, filePath As String)
        Dim enc As New PngBitmapEncoder
        enc.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New IO.FileStream(filePath, IO.FileMode.Create)
            enc.Save(fs)
        End Using
    End Sub
    Private Sub SaveImage_Click(sender As Object, e As RoutedEventArgs)
        Dim angle As String = Angle1.Value.ToString
        Dim filePath As String = GetNowToString() & "_" & angle & "度_"


        'DrawingImage式
        'Call SaveImageMyCanvasDI3改(filePath & "SaveImageMyCanvasDI3改.png")

        ''クロップ方式、これはMyCanvasDI方式と同じ結果になる
        'Call SaveImageCropped3式改(filePath & "SaveImageCropped3式改.png")

        ''test
        'Call SaveImageThumb(filePath & "SaveImageThumb.png") '回転表示するとずれていく
        'Call SaveImageThumbVB(filePath & "SaveImageThumbVB.png") 'だいたいOK
        'Call SaveImageTempTopCanvasVB(filePath & "SaveImageTempTopCanvasVB.png") '
        'Call SaveImageTempCanvasVB(filePath & "SaveImageTempCanvasVB.png") '
        Call SaveImageTempBorderVB(filePath & "SaveImageTempBorderVB.png") '

        ''アンチエイリアスありのとき用
        'Call SaveImageMyCanvasVB9(filePath & "SaveImageMyCanvasVB9.png")

        'まとめ


        'Call SaveAllImage(filePath & "AllImage.png") '全体保存OK
    End Sub

    Private Sub SetTextBlockBinding(tb As TextBlock, t As UIElement, name As String)
        Dim b As New Binding With {.Source = t, .Path = New PropertyPath(RenderOptions.EdgeModeProperty),
            .StringFormat = name & " = " & "{0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
    End Sub
    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Call SetControlTemplate()
        Call SetThumb()
        AddHandler btnEdgeMode.Click, AddressOf btnEdgeMode_Click
        AddHandler btnTempCanvasEdgeMode.Click, AddressOf btnChangeTempCanvasEdgeMode_Click
        AddHandler btnTextUpdate.Click, AddressOf TextUpdate_Click
        AddHandler btnSave.Click, AddressOf SaveImage_Click
        AddHandler MyThumb1.DragDelta, AddressOf MyThumb1_DragDelta

        Call SetTextBlockBinding(tbMyCanvasEdgeMode, MyCanvas, "MyCanvas")
        Call SetTextBlockBinding(tbTempCanvasEdgeMode, TempCanvas, "TempCanvas")

        'アンチエイリアス有無の設定、初期設定ではアリになっている
        'RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Aliased) '要る
        'MyCanvasをアンチエイリアスなしで表示
        '中の要素には適用されないけど最終的に見えるのは一番上のMyCanvasなので
        'すべての要素に適用されているようにみえる
        RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Aliased) '要る
        RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Aliased) '要る
        RenderOptions.SetEdgeMode(TempBorder, EdgeMode.Aliased)

        'RenderOptions.SetEdgeMode(MyThumb1, EdgeMode.Aliased)'多分いらない
        'RenderOptions.SetEdgeMode(TempBorder, EdgeMode.Aliased)’多分いらない

        '下2つはあまり意味ないかも、特にSnapのほう
        'MyCanvas.UseLayoutRounding = True
        'MyThumb1.UseLayoutRounding = True
        'TempCanvas.UseLayoutRounding = True
        'TempBorder.UseLayoutRounding = True

        'MyCanvas.SnapsToDevicePixels = True
        'MyThumb1.SnapsToDevicePixels = True
        'TempBorder.SnapsToDevicePixels = True
        'TempCanvas.SnapsToDevicePixels = True
    End Sub
    Private Sub Window1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Call TextUpdate()
    End Sub

    Private Sub btnEdgeMode_Click(sender As Object, e As RoutedEventArgs)
        If RenderOptions.GetEdgeMode(MyCanvas) = EdgeMode.Aliased Then
            RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Unspecified)
        Else
            RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Aliased)
        End If
    End Sub
    Private Sub btnChangeTempCanvasEdgeMode_Click(sender As Object, e As RoutedEventArgs)
        If RenderOptions.GetEdgeMode(TempCanvas) = EdgeMode.Aliased Then
            RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Unspecified)
        Else
            RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Aliased)
        End If
    End Sub

    Private Sub MyThumb1_DragDelta(sender As Object, e As DragDeltaEventArgs)
        Canvas.SetLeft(sender, Canvas.GetLeft(sender) + e.HorizontalChange)
        Canvas.SetTop(sender, Canvas.GetTop(sender) + e.VerticalChange)
    End Sub


End Class
