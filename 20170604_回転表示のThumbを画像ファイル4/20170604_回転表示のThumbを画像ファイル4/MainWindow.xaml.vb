'回転表示のThumbを画像ファイル2からの改変
'画像ファイルを表示
'Thumb
'   ┗ControlTemplate
'      ┗Canvas(TempCanvas)
'          ┣Border(TempBorder)
'          ┗Image(TempImage) これに回転や拡大を適用

'Canvasの中にBorderとImageを入れたControlTemplateを作成、これを
'ThumbのControlTemplateに指定
'回転の中心軸は中心、RenderTransformOriginal=point(0.5,0.5)
'Thumbを画像ファイル保存、アンチエイリアスなしでも表示されている見た目通りの画像にしたい



Imports System.Windows.Controls.Primitives


Class MainWindow

    Private MyControlTemplate As ControlTemplate
    Private TempImage As Image 'ControlTemplateの中のImage
    Private TempCanvas As Canvas 'ControlTemplateの中のCanvas

    Private Sub TextUpdate()
        Dim r As Rect
        '使うのはこれ
        r = TempImage.TransformToVisual(MyCanvas).TransformBounds(GetRect(TempImage))
        MyTextBlock1.Text = $" {r.X:0.0000}, {r.Y:0.0000}, {r.Width:0.0000}, {r.Height:0.0000}"
        MyTextBlock7.Text = $"左上 = ({r.X:0.0000}, {r.Y:0.0000})"
        MyTextBlock8.Text = $"右下 = ({r.X + r.Width:0.0000}, {r.Y + r.Height:0.0000})"

        'r = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(GetRect(TempCanvas))
        'MyTextBlock2.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"


        'r = MyCanvas.TransformToVisual(TempCanvas).TransformBounds(New Rect(100, 100, 100, 100))
        'MyTextBlock3.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        'r = MyCanvas.TransformToVisual(TempCanvas).TransformBounds(New Rect(0, 0, 100, 100))
        'MyTextBlock4.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        ''Dim gt As GeneralTransform = TempCanvas.RenderTransform
        ''Dim gt0 As GeneralTransform = TempCanvas.TransformToVisual(MyCanvas)
        ''Dim gt1 As GeneralTransform = TempCanvas.TransformToAncestor(MyCanvas)
        ''Dim gt2 As GeneralTransform = TempCanvas.TransformToDescendant(MyCanvas)
        ''Dim gt3 As GeneralTransform = TempCanvas.TransformToAncestor(MyThumb1)
        ''Dim gt4 As GeneralTransform = TempCanvas.TransformToDescendant(MyThumb1)
        ''Dim gt5 As GeneralTransform = TempCanvas.TransformToAncestor(TempImage)
        ''Dim gt6 As GeneralTransform = TempImage.TransformToDescendant(TempImage)


        ''RenderTransform
        'r = TempCanvas.RenderTransform.TransformBounds(GetRect(MyThumb1)) 'na
        'MyTextBlock5.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        'r = TempCanvas.RenderTransform.TransformBounds(New Rect(0, 0, 100, 100))
        'MyTextBlock6.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"


    End Sub
    Private Function GetRect(b As FrameworkElement) As Rect
        'Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.ActualWidth, b.ActualHeight)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call TextUpdate()
    End Sub
    'ControlTemplate作成
    Private Sub SetControlTemplate()
        'FrameworkElementFactory作成
        Dim TempCanvas As New FrameworkElementFactory(GetType(Canvas), "TempCanvas")
        Dim TempBorder As New FrameworkElementFactory(GetType(Border), "TempBorder")
        Dim TempImage As New FrameworkElementFactory(GetType(Image), "TempImage")

        'Canvasの中にImageを設置
        TempCanvas.AppendChild(TempImage)
        TempCanvas.AppendChild(TempBorder)
        'ControlTemplate作成
        Dim cTemp As New ControlTemplate(GetType(Thumb))
        cTemp.VisualTree = TempCanvas
        MyControlTemplate = cTemp
    End Sub
    'Thumb
    Private Sub SetThumb()
        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(7.7) '回転角度
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(ro)
        End With

        MyThumb1.Template = MyControlTemplate
        MyThumb1.ApplyTemplate() 'Template再構築
        Dim ct As ControlTemplate = MyThumb1.Template
        TempImage = ct.FindName("TempImage", MyThumb1)
        TempCanvas = ct.FindName("TempCanvas", MyThumb1)
        'Dim bmp As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\border_round_red.png"))
        Dim bmp As New BitmapImage(New Uri("D:\ブログ用\テスト用画像\NEC_8041_2017_05_09_午後わてん_.jpg"))
        With TempImage
            .Width = bmp.PixelWidth
            .Height = bmp.PixelHeight
            .Source = bmp
            .RenderTransform = tg
            .RenderTransformOrigin = New Point(0.5, 0.5)
        End With
        '必要
        Canvas.SetLeft(TempImage, 0) : Canvas.SetTop(TempImage, 0)
        Canvas.SetLeft(TempCanvas, 0) : Canvas.SetTop(TempCanvas, 0)
        Canvas.SetLeft(MyCanvas, 0) : Canvas.SetTop(MyCanvas, 0)


        'TempCanvas.Width = TempImage.Width
        'TempCanvas.Height = TempImage.Height
        'TempCanvas.RenderTransform = tg
        'TempCanvas.RenderTransformOrigin = New Point(0.5, 0.5)

        Dim b As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty)}
        Angle1.SetBinding(Slider.ValueProperty, b)


        b = New Binding With {.Source = sldLeft, .Path = New PropertyPath(Slider.ValueProperty)}
        MyThumb1.SetBinding(LeftProperty, b)
        b = New Binding With {.Source = sldTop, .Path = New PropertyPath(Slider.ValueProperty)}
        MyThumb1.SetBinding(TopProperty, b)

        'b = New Binding With {.Source = TempImage, .Path = New PropertyPath(WidthProperty)}
        'MyThumb1.SetBinding(WidthProperty, b)

        'b = New Binding With {.Source = TempImage, .Path = New PropertyPath(HeightProperty)}
        'MyThumb1.SetBinding(HeightProperty, b)


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

        Dim s As New Size(TempImage.ActualWidth, TempImage.ActualHeight)
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
        'MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)


        Dim s As New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
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
            x = 0 : y = 0
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
        Clipboard.SetImage(rtb)
        'Call Bitmap2pngFile(rtb, filePath)

        MyCanvas.Background = b '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
    End Sub

    'まだ途中、DI3のしきい値0.5をより正確だと思われる0.533に変更
    'これでムダな余白ができるのを防ぐことができそうだけどそこまでする必要ないかも
    Private Sub SaveImageMyCanvasDI4(filePath As String)
        Dim rThreshold As Decimal = 0.533
        Dim lThreshold As Decimal = 1 - rThreshold
        Dim b As Brush = MyCanvas.Background
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)


        Dim s As New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, MyBlueBorder.ActualWidth, MyBlueBorder.ActualHeight)
        Dim r As Rect = Rect.Union(r1, r2)
        'MyCanvas全体をbmp画像にする
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        'x,y、DrawImageのOffset位置を求める
        '変形四捨五入(0.5より大きいければ切り上げ、0.5以下なら切り捨て)する
        Dim fx As Double = (r1.X - Fix(r1.X)) '小数点以下の値取得
        Dim x As Double = IIf(fx > lThreshold, Math.Ceiling(r1.X), Fix(r1.X)) '変形四捨五入
        Dim fy As Double = (r1.Y - Fix(r1.Y))
        Dim y As Double = IIf(fy > lThreshold, Math.Ceiling(r1.Y), Fix(r1.Y))
        'width,height、DrawImageのサイズ(幅と高さ)を求める
        Dim fw As Double = r.Width - Fix(r.Width)
        Dim w As Integer = IIf(fw > lThreshold, Math.Ceiling(r.Width), Fix(r.Width))
        Dim fh As Double = r.Height - Fix(r.Height)
        Dim h As Integer = IIf(fh > lThreshold, Math.Ceiling(r.Height), Fix(r.Height))

        'MyCanvasのbmpを使ってDrawImage
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawImage(bmp, New Rect(-x, -y, w, h))
        End Using

        'Renderするサイズを求める
        '幅 = 変形四捨五入した右端座標 - 変形四捨五入した左端座標
        Dim migi As Double = r1.X + r1.Width '右端座標
        Dim fMigi As Double = migi - Fix(migi) '右端座標の小数点以下の値取得
        Dim ww As Integer = IIf(fMigi > rThreshold, Math.Ceiling(migi) - x, Fix(migi) - x) '幅
        '高さ
        Dim sita As Double = r1.Y + r1.Height
        Dim fSita As Double = sita - Fix(sita)
        Dim hh As Integer = IIf(fSita > rThreshold, Math.Ceiling(sita) - y, Fix(sita) - y)
        'Render
        Dim rtb As New RenderTargetBitmap(ww, hh, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        'bmpをpng画像として保存
        Clipboard.SetImage(rtb)

        'Call Bitmap2pngFile(rtb, filePath)

        MyCanvas.Background = b '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
    End Sub




    'MyCanvasをBitmapにして、それをクロップ

    'SaveImageCropped1式を改良、他の図形と重なっているときでもOK

    'SaveImageCropped2式を改変、余白をなくしてみる

    'SaveImageCropped3式を清書
    Private Sub SaveImageCropped3式改(filePath As String)
        Dim s As New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
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
        Clipboard.SetImage(cb)
        'Call Bitmap2pngFile(cb, filePath)
    End Sub

    Private Sub SaveImageThumb(filePath As String)
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(New Size(TempImage.ActualWidth, TempImage.ActualHeight)))
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        bmp.Render(TempCanvas)
        Clipboard.SetImage(bmp)
        'Call Bitmap2pngFile(bmp, filePath)
    End Sub

    'だいたいOK、あとはサイズの微調整
    Private Sub SaveImageThumbVB(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = VisualTreeHelper.GetDescendantBounds(MyThumb1)
        Dim r3 As Point = MyCanvas.TransformToVisual(TempImage).Transform(New Point(0, 0))
        Dim r4 As Point = MyCanvas.TransformToVisual(TempCanvas).Transform(New Point(0, 0))
        Dim r5 As Point = TempImage.TransformToVisual(MyThumb1).Transform(New Point(0, 0))
        Dim r6 As Point = MyCanvas.TransformToVisual(MyThumb1).Transform(New Point(0, 0))
        Dim r7 As Point = MyThumb1.TransformToVisual(TempCanvas).Transform(New Point(0, 0))
        Dim r8 As Point = MyThumb1.TransformToVisual(TempImage).Transform(New Point(0, 0))
        Dim r9 As Point = MyThumb1.TransformToVisual(MyCanvas).Transform(New Point(0, 0))

        Dim vb As New VisualBrush(MyThumb1)
        'RenderOptions.SetEdgeMode(vb, EdgeMode.Aliased)
        With vb
            '.AlignmentY = AlignmentY.Center
            '.AlignmentX = AlignmentX.Center
            '.Stretch = Stretch.None
        End With
        Dim dv As New DrawingVisual
        'RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
        Using dc As DrawingContext = dv.RenderOpen
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
            dc.DrawRectangle(vb, Nothing, New Rect(0.5, 0.5, r1.Width - 0.37165, r1.Height - 0.37165))
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, 14, 14))
        End Using
        Dim bmp As New RenderTargetBitmap(r1.Width + 1, r1.Height + 1, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)

    End Sub

    Private Sub SaveImageThumbVB2(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))

        Dim vb As New VisualBrush(MyThumb1)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
        End Using
        Dim bmp As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)

    End Sub

    Private Sub SaveImageThumbVB3(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))

        Dim xOffset As Double
        Dim yOffset As Double

        Dim vWidth As Double = Math.Round(r1.Width)
        Dim vHeight As Double = Math.Round(r1.Height)
        'IIf((vWidth Mod 2) = 0, xOffset = 0, xOffset = 0.5)
        'IIf(vHeight Mod 2 = 0, yOffset = 0, yOffset = 0.5)
        If vWidth Mod 2 = 0 Then
            xOffset = 0
        Else
            xOffset = 0.5
        End If
        If vHeight Mod 2 = 0 Then
            yOffset = 0
        Else
            yOffset = 0.5
        End If
        xOffset = 0
        yOffset = 0
        vWidth = r1.Width
        vHeight = r1.Height
        Dim vb As New VisualBrush(MyThumb1)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r1.Width, r1.Height))
            dc.DrawRectangle(vb, Nothing, New Rect(xOffset, yOffset, vWidth, vHeight))

        End Using
        Dim bmp As New RenderTargetBitmap(vWidth + 1, vHeight + 1, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)

    End Sub
    'ブラシを回転させてみる→いまいち
    'DrawContextのpushTransformで回転させてみる→いまいち
    '中心軸の設定が無視される感じでどんどんズレる
    Private Sub SaveImageThumbVB4(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        'Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))

        Dim xOffset As Double
        Dim yOffset As Double

        Dim vWidth As Double = Math.Round(r1.Width)
        Dim vHeight As Double = Math.Round(r1.Height)
        xOffset = 0
        yOffset = 0
        vWidth = r1.Width
        vHeight = r1.Height
        Dim ro As New RotateTransform(Angle1.Value, 0.5, 0.5)
        Dim vb As New VisualBrush(TempImage)
        'vb.Transform = ro ' TempCanvas.RenderTransform
        RenderOptions.SetEdgeMode(vb, EdgeMode.Aliased)
        vb.RelativeTransform = New RotateTransform(7.7, 0.5, 0.5)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            'dc.PushTransform(TempCanvas.RenderTransform)
            dc.PushTransform(ro)
            dc.DrawRectangle(vb, Nothing, New Rect(xOffset, yOffset, vWidth, vHeight))

        End Using
        Dim bmp As New RenderTargetBitmap(vWidth + 1, vHeight + 1, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)

    End Sub

    Private Sub SaveImageThumbVB5(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        'Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim hanbun As Double = r1.Width / 2
        Dim xOffset As Double = r1.X - Fix(r1.X)
        Dim yOffset As Double = r1.Y - Fix(r1.Y)
        Dim vWidth As Double = r1.Width
        Dim vHeight As Double = r1.Height
        Dim vb As New VisualBrush(MyThumb1)

        Dim vbs = vb.TileMode
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(xOffset, yOffset, vWidth, vHeight))
        End Using
        Dim bmp As New RenderTargetBitmap(vWidth + 1, vHeight + 1, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    'TempCanvasをDrawImage、これはムリ、回転角度が大きくなるとズレていって修正もできないっぽい
    Private Sub SaveImageThumbDrawImage(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim tBmp As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        tBmp.Render(TempCanvas)
        Clipboard.SetImage(tBmp)

    End Sub

    'MyCanvasのVisualBrushで全体をDrawRectangleして
    'MyCanvasのVisualBrushをDrawingVisualのDrawingContextのDrawRectangleに使う
    'それをRenderTargetBitmapでRenderして画像作成
    'それをDrawingVisualのDrawingContextのDrawImageに使う
    'このときに適切なOffsetとサイズを指定する
    'RenderTargetBitmapでRenderしてできあがり
    'Offsetやサイズ指定は単純なCIntで四捨五入な感じにしているけどほぼ問題ない
    '細かく指定するなら0.53と0.54の間にしきい値があるみたい、またDPIも関係しているかも
    Private Sub SaveImageMyCanvasVBDRect(filePath As String)
        '再描画、MyCanvasのサイズが必要になるので再描画
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)
        Dim vb As New VisualBrush(MyCanvas)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = MyBlueBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(GetRect(MyBlueBorder).Size))
        Dim r As Rect = Rect.Union(r1, r2)
        Dim tr As Rect = VisualTreeHelper.GetDescendantBounds(MyCanvas) 'test

        'MyCanvasの画像作成
        Dim vWidth As Double = Math.Ceiling(r.Width)
        Dim vHeight As Double = Math.Ceiling(r.Height)
        Dim dv As New DrawingVisual()
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0, vWidth, vHeight))
        End Using
        Dim iBmp As New RenderTargetBitmap(vWidth, vHeight, 96, 96, PixelFormats.Pbgra32)
        iBmp.Render(dv)

        '画像をOffsetとサイズ指定してDrawImage
        Dim xOffset As Integer = r1.X 'たぶん四捨五入
        Dim yOffset As Integer = r1.Y
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawImage(iBmp, New Rect(-xOffset, -yOffset, vWidth, vHeight))
            '↑↓は全く同じ結果
            'dc.DrawRectangle(New ImageBrush(iBmp), Nothing, New Rect(-Fix(r1.X), -Fix(r1.Y), vWidth, vHeight))
        End Using

        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width) + 1, Math.Ceiling(r1.Height) + 1, 96, 96, PixelFormats.Pbgra32)
        Dim xf As Integer = CInt(r1.X + r1.Width) - xOffset
        Dim yf As Integer = CInt(r1.Y + r1.Height) - yOffset
        Dim rtb As New RenderTargetBitmap(xf + 1, yf + 1, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Clipboard.SetImage(rtb) '確認用
        'Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'あれ？これ良さそう？
    Private Sub test()
        Dim r As Rect = VisualTreeHelper.GetDescendantBounds(MyThumb1)
        Dim vb As New VisualBrush(MyThumb1)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(r.X - Fix(r.X), r.Y - Fix(r.Y), r.Width, r.Height))

        End Using
        Dim bmp As New RenderTargetBitmap(r.Width + 1, r.Height + 1, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
    End Sub

    'ここまで単独Thumbを画像ファイル保存




    'MyCanvas全てを画像ファイルにする、OK
    Private Sub SaveAllImage(filePath As String)
        Dim s As Size = New Size(TempImage.ActualWidth, TempImage.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempImage.Width, TempImage.Height)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
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
        Call FixMyCanvasSize()


        'DrawingImage式
        'Call SaveImageMyCanvasDI3改(filePath & "SaveImageMyCanvasDI3改.png")

        'クロップ方式、これはMyCanvasDI方式と同じ結果になる
        'Call SaveImageCropped3式改(filePath & "SaveImageCropped3式改.png")

        ''ThumbのVisualBrushでDrawRectangle、描画位置をOffsetで調整
        '上の2つの方がいいかな、半透明が出る
        'Call SaveImageThumb(filePath & "SaveImageThumb.png") '回転表示するとずれていく
        'Call SaveImageThumbVB(filePath & "SaveImageThumbVB.png") 'だいたいOK
        'Call SaveImageThumbVB3(filePath & "SaveImageThumbVB3.png") '
        'Call SaveImageThumbVB4(filePath & "SaveImageThumbVB4.png") '
        'Call SaveImageThumbVB5(filePath & "SaveImageThumbVB5.png") 'この中ではこれが最終型

        'test
        'Call SaveImageThumbDrawImage(filePath & "SaveImageThumbDrawImage.png") 'ムリだった
        'Call SaveImageMyCanvasDI4(filePath & "SaveImageMyCanvasDI4.png") '途中
        'Call SaveImageMyCanvasVBDRect(filePath & "SaveImageMyCanvasVBDRect.png") '途中
        'Call test()

        ''アンチエイリアスありのとき用
        'Call SaveImageMyCanvasVB9(filePath & "SaveImageMyCanvasVB9.png")

        'まとめ


        'Call SaveAllImage(filePath & "AllImage.png") '全体保存OK

        'test
        Dim r As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(New Size(TempImage.Width, TempImage.Height)))
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(TempCanvas)
        Dim str As String = GetNowToString() & ".png"
        Call Bitmap2pngFile(rtb, str)
        'test
        'test2
        'test3
        'test4
        'test5
        'test6-2
        'test1-7
        'test2-6

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
        AddHandler btnFixMyCanvasSize.Click, AddressOf FixMyCanvasSize

        Call SetTextBlockBinding(tbMyCanvasEdgeMode, MyCanvas, "MyCanvas")
        Call SetTextBlockBinding(tbTempCanvasEdgeMode, TempCanvas, "TempCanvas")

        'アンチエイリアス有無の設定、初期設定ではアリになっている
        'RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Aliased) '要る
        'MyCanvasをアンチエイリアスなしで表示
        '中の要素には適用されないけど最終的に見えるのは一番上のMyCanvasなので
        'すべての要素に適用されているようにみえる
        RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Aliased) '要る
        RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Aliased) '要る
        'RenderOptions.SetEdgeMode(MyThumb1, EdgeMode.Aliased) '多分いらない
        'RenderOptions.SetEdgeMode(TempImage, EdgeMode.Aliased) '多分いらない

        '下2つはあまり意味ないかも、特にSnapのほう
        'MyCanvas.UseLayoutRounding = True
        'MyThumb1.UseLayoutRounding = True
        'TempCanvas.UseLayoutRounding = True
        'TempImage.UseLayoutRounding = True

        'MyCanvas.SnapsToDevicePixels = True
        'MyThumb1.SnapsToDevicePixels = True
        'TempImage.SnapsToDevicePixels = True
        'TempCanvas.SnapsToDevicePixels = True
    End Sub
    Private Sub Window1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Call TextUpdate()
        Call FixMyCanvasSize()
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
    Private Sub FixMyCanvasSize()
        Dim r0 As Rect = VisualTreeHelper.GetDescendantBounds(MyCanvas)
        Dim r1 As Rect = TempImage.TransformToVisual(MyCanvas).TransformBounds(New Rect(GetRect(TempImage).Size))
        Dim r2 As Rect = New Rect(0, 0, MyBlueBorder.ActualWidth, MyBlueBorder.ActualHeight)
        Dim r As Rect = Rect.Union(r1, r2)
        MyCanvas.Width = r.Width
        MyCanvas.Height = r.Height
        tbMyCanvasSize.Text = $"{r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
    End Sub

    Private Sub btnFixMyCanvasSize_Click(sender As Object, e As RoutedEventArgs)
        Call FixMyCanvasSize()
    End Sub
End Class
