'Canvasの中に入れたBorderを画像ファイル保存、アンチエイリアスなしでも表示されている見た目通りの画像にしたい
'回転の中心軸は中心、つまりRenderTransformOriginal=point(0.5,0.5)
'回転させるのはBorderにして


Class MainWindow
    Private Sub UpDateMyRedCanvasSize()
        Dim r As Rect = MyRedBorder.TransformToVisual(MyRedCanvas).TransformBounds(New Rect(New Size(MyRedBorder.ActualWidth, MyRedBorder.ActualHeight)))
        Dim r2 As Rect = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(New Size(MyRedBorder.ActualWidth, MyRedBorder.ActualHeight)))

        MyRedCanvas.RenderSize = r.Size
    End Sub
    Private Sub TextUpdate()
        Dim r As Rect
        Dim rbWidth As Double = MyRedBorder.ActualWidth
        Dim rbHeight As Double = MyRedBorder.ActualHeight
        Dim mrc As Canvas = MyRedCanvas

        '使うのはこれ
        r = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, rbWidth, rbHeight))
        MyTextBlock1.Foreground = Brushes.Red
        MyTextBlock1.Text = $" {r.X:000.0000}, {r.Y:000.0000}, {r.Width:000.00}, {r.Height:000.00}"
        MyTextBlock2.Text = $"左上座標 = ({r.X:0.00}, {r.Y:0.00})"
        MyTextBlock3.Text = $"右下座標 = ({r.X + rbWidth:0.00}, {r.Y + rbHeight:0.00})"

    End Sub
    Private Function GetRect(b As FrameworkElement) As Rect
        'Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.ActualWidth, b.ActualHeight)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call TextUpdate()
    End Sub

    'MyRedBorderの初期設定とBinding
    Private Sub MySetBorder()
        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(10.6) '回転角度
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(ro)
        End With

        With MyRedBorder
            .Width = 100
            .Height = 1
            .Background = Brushes.Red
            .RenderTransform = tg
            .RenderTransformOrigin = New Point(0.5, 0.5)
        End With

        With MyRedCanvas
            '.Width = MyRedBorder.Width
            '.Height = MyRedBorder.Height
            '.RenderTransform = tg
            '.RenderTransformOrigin = New Point(0.5, 0.5)

        End With

        'スライダーとMyRedBorderの回転角度のBinding
        Dim b As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty)}
        Angle1.SetBinding(Slider.ValueProperty, b)
        'MyRedCanvas表示位置とのBinding
        b = New Binding With {.Source = sldLeft, .Path = New PropertyPath(Slider.ValueProperty)}
        MyRedCanvas.SetBinding(LeftProperty, b)
        b = New Binding With {.Source = sldTop, .Path = New PropertyPath(Slider.ValueProperty)}
        MyRedCanvas.SetBinding(TopProperty, b)

    End Sub

    '現在日時を文字列に変換
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
    End Function
    '小数点以下の値取得
    Private Function GetDecimalPointLessThenValue(value As Double) As Double
        Return value - Fix(value)
    End Function

    'MyRedBorderを描画
    '描画位置がずれる、たぶんRenderTransformOriginalが0,0ならズレない
    Private Sub SaveImage2(filePath As String)
        Dim r As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'Dim bmi As New BitmapImage(New Uri("D:\ブログ用\チェック用2\NEC_6506_2016_11_18_午後わてん.jpg"))
        'Dim bms As BitmapSource = bmi
        'Dim bmm As New WriteableBitmap(bms)
        '        Debugger Image Visualizer (Preview) - Visual Studio Marketplace
        'https://marketplace.visualstudio.com/items?itemName=AleksanderBerus.DebuggerImageVisualizerPreview



        bmp.Render(MyRedBorder)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    'MyRedBorderのVisualBrushを使う方式
    'SaveImageTempCanvasVB2を改良
    'DrawRectangleのサイズを切り上げにして、RenderTargetBitmapのサイズも切り上げにした
    '微妙にずれる
    Private Sub SaveImageMyRedBorderVB3(filePath As String)
        Dim r As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyRedBorder)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Size(Math.Ceiling(r.Width), Math.Ceiling(r.Height)))) 'サイズ切り上げ
        End Using
        'サイズ切り上げ
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32) 'test
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub


    'MyCanvasをブラシにして塗り
    'いろいろアンチエイリアスなしに設定してみたけど半透明が出る
    Private Sub SaveImageMyCanvasVB3(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
            RenderOptions.SetEdgeMode(vb, EdgeMode.Aliased)
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(CInt(-r1.X), CInt(-r1.Y)), r.Size))
        End Using

        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        RenderOptions.SetEdgeMode(rtb, EdgeMode.Aliased)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'アンチエイリアスありのとき用、SaveImageMyCanvasVB8を清書、他の図形考慮
    Private Sub SaveImageMyCanvasVB9(filePath As String)
        Dim b As Brush = MyCanvas.Background
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする

        '指定した背景色をここで反映させるには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
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




    'MyCanvasのbmpをDrawImage
    'SaveImageMyCanvasDI2を改良、描画位置やサイズを最適化
    Private Sub SaveImageMyCanvasDI3(filePath As String)
        Dim b As Brush = MyCanvas.Background
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        'MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

        '                                                                End Sub)

        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        'x,y
        Dim fx As Double = r1.X - Fix(r1.X)
        Dim x As Integer = IIf(fx > 0.5, Math.Ceiling(r1.X), Fix(r1.X))
        Dim fy As Double = r1.Y - Fix(r1.Y)
        Dim y As Integer = IIf(fy > 0.5, Math.Ceiling(r1.Y), Fix(r1.Y))
        'width,height
        Dim fw As Double = r.Width - Fix(r.Width)
        Dim w As Integer = IIf(fw > 0.5, Math.Ceiling(r.Width), Fix(r.Width))
        Dim fh As Double = r.Height - Fix(r.Height)
        Dim h As Integer = IIf(fh > 0.5, Math.Ceiling(r.Height), Fix(r.Height))

        Dim dv As New DrawingVisual
        RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
        Using dc As DrawingContext = dv.RenderOpen
            'dc.DrawImage(bmp, New Rect(r.Size))'これは半透明が出る
            'dc.DrawImage(bmp, New Rect(New Size(CInt(r.Width), CInt(r.Height))))
            'dc.DrawImage(bmp, New Rect(CInt(-r1.X), CInt(-r1.Y), CInt(r.Width), CInt(r.Height)))
            dc.DrawImage(bmp, New Rect(-x, -y, w, h))

        End Using

        'r1 width, height
        Dim migi As Double = r1.X + r1.Width
        Dim fMigi As Double = migi - Fix(migi)
        Dim ww As Integer = IIf(fMigi > 0.5, Math.Ceiling(migi) - x, Fix(migi) - x)

        Dim sita As Double = r1.Y + r1.Height
        Dim fSita As Double = sita - Fix(sita)
        Dim hh As Integer = IIf(fSita > 0.5, Math.Ceiling(sita) - y, Fix(sita) - y)

        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        Dim rtb As New RenderTargetBitmap(ww, hh, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        Call Bitmap2pngFile(rtb, filePath)
        MyCanvas.Background = b '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
    End Sub
    'SaveImageMyCanvasDI3を清書
    Private Sub SaveImageMyCanvasDI3改(filePath As String)
        Dim b As Brush = MyCanvas.Background
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
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
    'なので他の図形と重なっているときはその図形も写ってしまう
    Private Sub SaveImageCropped1式(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        Dim cb As New CroppedBitmap(bmp, New Int32Rect(Int(r1.X), Int(r1.Y), Math.Ceiling(r1.Width), Math.Ceiling(r1.Height)))

        Call Bitmap2pngFile(cb, filePath)
    End Sub
    'SaveImageCropped1式を改良、他の図形と重なっているときでもOK

    'SaveImageCropped2式を改変、余白をなくしてみる

    'SaveImageCropped3式を清書
    Private Sub SaveImageCropped3式改(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
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


    Private Sub SaveMyRedCanvas(filePath As String)
        Call UpDateMyRedCanvasSize()
        'Dim bmp As New RenderTargetBitmap(MyRedCanvas.ActualWidth, MyRedCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32)
        Dim r As Rect = MyRedCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, MyRedCanvas.ActualWidth, MyRedCanvas.ActualHeight))
        Dim r1 As Rect = MyRedCanvas.TransformToVisual(MyRedCanvas).TransformBounds(New Rect(0, 0, MyRedCanvas.ActualWidth, MyRedCanvas.ActualHeight))
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'Dim bmp As New RenderTargetBitmap(200,200, 96, 96, PixelFormats.Pbgra32)
        'bmp.Render(MyRedCanvas)
        bmp.Render(MyRedBorder)
        Dim dg As DrawingGroup = VisualTreeHelper.GetDrawing(MyRedBorder)


        Dim r2 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedBorder)
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)
        Dim r4 As Rect = VisualTreeHelper.GetDescendantBounds(MyCanvas)
        Dim r5 As Rect = VisualTreeHelper.GetContentBounds(MyRedBorder)
        Dim r6 As Rect = VisualTreeHelper.GetContentBounds(MyRedCanvas)
        Dim r7 As Rect = VisualTreeHelper.GetContentBounds(MyCanvas)
        Clipboard.SetImage(bmp)

        Call Bitmap2pngFile(bmp, filePath)
        'これはMyCanvasの座標が基準になってしまう
    End Sub

    Private Sub SaveMyRedCanvas2(filePath As String)
        Call UpDateMyRedCanvasSize()
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)
        Dim bmp As New RenderTargetBitmap(r3.Width, r3.Height, 96, 96, PixelFormats.Pbgra32)
        Dim r As Rect = MyRedCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, MyRedCanvas.ActualWidth, MyRedCanvas.ActualHeight))
        Dim r1 As Rect = MyRedCanvas.TransformToVisual(MyRedCanvas).TransformBounds(New Rect(0, 0, MyRedCanvas.ActualWidth, MyRedCanvas.ActualHeight))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyRedCanvas)
            vb.Stretch = Stretch.None
            'vb.AlignmentX
            'dc.DrawRectangle(vb, Nothing, New Rect(r3.TopLeft, r3.Size))
            dc.DrawRectangle(vb, Nothing, r3)
        End Using
        bmp.Render(dv)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub
    Private Sub SaveMyRedCanvas3(filePath As String)
        Call UpDateMyRedCanvasSize()
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)

        Dim bmp As New RenderTargetBitmap(r3.Width, r3.Height, 96, 96, PixelFormats.Pbgra32)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyRedCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point, r3.Size))
        End Using
        bmp.Render(dv)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    Private Sub SaveMyRedCanvas4(filePath As String)
        Call UpDateMyRedCanvasSize()
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)

        'Dim bmp As New RenderTargetBitmap(r3.Width, r3.Height, 96, 96, PixelFormats.Pbgra32)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r3.Width), Math.Ceiling(r3.Height), 96, 96, PixelFormats.Pbgra32)
        'RenderOptions.SetEdgeMode(bmp, EdgeMode.Aliased)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyRedCanvas)
            vb.Stretch = Stretch.None
            'vb.AlignmentX = AlignmentX.Left
            'vb.AlignmentY = AlignmentY.Top
            'RenderOptions.SetEdgeMode(dv,EdgeMode.Aliased)
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(0.119, 0.182), r3.Size))
        End Using
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
        'Call Bitmap2pngFile(bmp, filePath)
    End Sub

    Private Sub SaveMyRedCanvas5(filePath As String)
        'Call UpDateMyRedCanvasSize()
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)
        Dim r2 As Rect = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(GetRect(MyRedBorder))
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r3.Width + 100), Math.Ceiling(r3.Height), 96, 96, PixelFormats.Pbgra32)
        RenderOptions.SetEdgeMode(bmp, EdgeMode.Aliased)
        Dim dv As New DrawingVisual
        RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyRedCanvas)
            'vb.Stretch = Stretch.None
            'vb.AlignmentX = AlignmentX.Left
            'vb.AlignmentY = AlignmentY.Top
            'vb.Transform.Value.Translate(10, 0)
            vb.Transform = New TranslateTransform(GetDecimalPointLessThenValue(r3.Width), 0)

            Dim w As Double = GetDecimalPointLessThenValue(r3.Width) / 2
            Dim h As Double = GetDecimalPointLessThenValue(r3.Height) / 2
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(w, h), r3.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(0, 0), r3.Size))
            dc.DrawRectangle(vb, Nothing, New Rect(1, 0, Math.Ceiling(r3.Width), Math.Ceiling(r3.Height)))
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(0, 0), r2.Size))
        End Using
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
        'Call Bitmap2pngFile(bmp, filePath)
    End Sub

    'System.Drawingを参照に追加してWindowsForm方式
    Private Sub SaveMyRedCanvas6(filePath As String)
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)
        Using bb As New System.Drawing.Bitmap(CInt(r3.Width + 500), CInt(r3.Height + 300))
            Using img As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bb)
                img.CopyFromScreen(New System.Drawing.Point(CInt(Left), CInt(Top)),
                                   New System.Drawing.Point(0, 0), New System.Drawing.Size(500, 300))
                bb.Save("test.png", System.Drawing.Imaging.ImageFormat.Png)
            End Using
        End Using

        'Clipboard.SetImage(bmp)
        'Call Bitmap2pngFile(bmp, filePath)
    End Sub

    Private Sub SaveMyRedCanvas7(filePath As String)
        'Call UpDateMyRedCanvasSize()
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(Me)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r3.Width), Math.Ceiling(r3.Height), 96, 96, PixelFormats.Pbgra32)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(Me)
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(0, 0), r3.Size))

        End Using
        bmp.Render(dv)
        Clipboard.SetImage(bmp)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    Private Sub SaveMyRedCanvas8(filePath As String)
        'Call UpDateMyRedCanvasSize()
        Dim r3 As Rect = VisualTreeHelper.GetDescendantBounds(Me)
        Dim bmp As New RenderTargetBitmap(Me.Width, Me.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(Me)
        Clipboard.SetImage(bmp)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    'WindowsForm方式
    'デスクトップキャプチャからのクリップ
    '見た目に忠実にできるけど、画面に表示されていない部分はキャプチャできない
    Private Sub SaveMyRedCanvas9(filePath As String)
        Dim tp As Point = MyRedCanvas.PointToScreen(New Point)
        Dim tr As Rect = VisualTreeHelper.GetDescendantBounds(MyRedCanvas)
        Dim tr2 As Rect = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(GetRect(MyRedBorder).Size))
        'Dim r As Rect = New Rect(tp.X + 23, tp.Y - 42, tr2.Width, tr2.Height)
        Dim r As Rect = New Rect(tp.X + tr2.X - 100, tp.Y + tr.Y, tr2.Width, tr2.Height)
        Using bmp As New System.Drawing.Bitmap(CInt(r.Width), CInt(r.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            Using g As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bmp)
                g.CopyFromScreen(r.X, r.Y, 0, 0, bmp.Size)
                Dim bs As BitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions)
                'Call Bitmap2pngFile(bs, filePath)
                Clipboard.SetImage(bs)
            End Using
        End Using

    End Sub




    'MyCanvas全てを画像ファイルにする、アンチエイリアスなし用、微妙にずれる、なんで？
    Private Sub SaveAllImage(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)

        bmp.Render(MyCanvas)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub
    'MyCanvas全てを画像ファイルにする、アンチエイリアスのとき用、完璧
    Private Sub SaveAllImage2(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)

        bmp.Render(MyCanvas)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub
    'SaveAllImageを改変
    Private Sub SaveAllImage3(filePath As String)
        Dim r As Rect = VisualTreeHelper.GetDescendantBounds(MyCanvas)
        'Dim r1 As Rect = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(GetRect(MyRedBorder))
        'Dim r2 As Rect = MyBlueBorder.TransformToVisual(MyCanvas).TransformBounds(GetRect(MyBlueBorder))
        'Dim rr As Rect = Rect.Union(r1, r2)
        'rとrrは全く同じ値になる
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)

        bmp.Render(MyCanvas)
        Clipboard.SetImage(bmp)
        'Call Bitmap2pngFile(bmp, filePath)
    End Sub



    '拡縮回転されている要素がピッタリ収まるRect(位置とサイズ)を返す
    '返される位置は指定したPanelの左上を基準(0,0)にした位置になる
    Private Function GetBoundRect(fe As FrameworkElement, p As Panel) As Rect
        Dim s As New Size(fe.ActualWidth, fe.ActualHeight)
        Return fe.TransformToVisual(p).TransformBounds(New Rect(s))
    End Function
    '赤と青のBorderがピッタリ収まるRectを返す
    Private Function GetRedAndBlueBoundRect() As Rect
        Return Rect.Union(GetBoundRect(MyRedBorder, MyCanvas), GetBoundRect(MyBlueBorder, MyCanvas))
    End Function
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


        'Call SaveImage2(filePath & "SaveImage2.png")

        'Call SaveImageMyCanvasVB3(filePath & "SaveImageMyCanvasVB3.png") '半透明が出る
        'Call SaveImageMyCanvasDI3改(filePath & "SaveImageMyCanvasDI3改.png") ''できた！他のが重なっていても単体で保存できた！

        'Call SaveImageMyRedBorderVB3(filePath & "SaveImageMyRedBorderVB3.png") ''ズレる、以下29まで全てイマイチなのでこれで妥協

        ''クロップ方式、これはMyCanvasVB方式と同じ結果になる
        ''Call SaveImageCropped1式(filePath & "SaveImageCropped1式.png")
        'Call SaveImageCropped3式改(filePath & "SaveImageCropped3式改.png") ''

        ''アンチエイリアスありのとき用
        'Call SaveImageMyCanvasVB9(filePath & "SaveImageMyCanvasVB9.png") ''完成

        '個別Canvas
        'Call SaveMyRedCanvas(filePath & "SaveMyRedCanvas.png")
        'Call SaveMyRedCanvas2(filePath & "SaveMyRedCanvas2.png")
        'Call SaveMyRedCanvas3(filePath & "SaveMyRedCanvas3.png")
        'Call SaveMyRedCanvas4(filePath & "SaveMyRedCanvas4.png")
        'Call SaveMyRedCanvas5(filePath & "SaveMyRedCanvas5.png")
        'Call SaveMyRedCanvas6(filePath & "SaveMyRedCanvas6.png")
        'Call SaveMyRedCanvas7(filePath & "SaveMyRedCanvas7.png")
        'Call SaveMyRedCanvas8(filePath & "SaveMyRedCanvas8.png")
        Call SaveMyRedCanvas9(filePath & "SaveMyRedCanvas9.png")

        '全体画像
        'Call SaveAllImage(filePath & "AllImage.png") '全体保存、多少ズレる
        'Call SaveAllImage2(filePath & "AllImage2.png") '全体保存、アンチエイリアスのとき用
        'Call SaveAllImage3(filePath & "AllImage3.png") 'test


        '個別保存のまとめ
        'アンチエイリアスなしの場合に一番いいのはSaveImageCropped3式改
        '次にSaveImageMyCanvasDI3改、これはSaveImageCropped3式改と同じ結果になるはず
        'このふたつは多少ズレる
        'アンチエイリアスありの場合はSaveImageMyCanvasVB9、これは完璧
        'なしの場合でもこれを使いたいけどズレる
        '
        '確認用
        'For i As Decimal = 0.1 To 1.0 Step 0.1
        '    Angle1.Value = i
        '    filePath = GetNowToString() & "_" & i & "度_"
        '    '    '下の2つは全く同じものになるはず、1度刻みの1～9度は全く同じだった、0.1度刻みの0.1～1.0までも同じことを確認
        '    '    'アンチエイリアスなしなら下の2つで決まり
        '    'Call SaveImageCropped3式改(filePath & "SaveImageCropped3式改.png") ''
        'Call SaveImageMyCanvasDI3改(filePath & "SaveImageMyCanvasDI3改.png") ''できた！他のが重なっていても単体で保存できた！
        '    'アンチエイリアスありの場合はこれ
        '    Call SaveImageMyCanvasVB9(filePath & "SaveImageMyCanvasVB9.png")
        'Next
    End Sub

    Private Sub SetTextBlockBinding(tb As TextBlock, t As UIElement, name As String)
        Dim b As New Binding With {.Source = t, .Path = New PropertyPath(RenderOptions.EdgeModeProperty),
            .StringFormat = name & " = " & "{0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
    End Sub
    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        Call MySetBorder()
        AddHandler btnEdgeMode.Click, AddressOf btnEdgeMode_Click
        AddHandler btnTempCanvasEdgeMode.Click, AddressOf btnChangeRedBorderEdgeMode_Click
        AddHandler btnTextUpdate.Click, AddressOf TextUpdate_Click
        AddHandler btnSave.Click, AddressOf SaveImage_Click


        Call SetTextBlockBinding(tbMyCanvasEdgeMode, MyCanvas, "MyCanvas")
        Call SetTextBlockBinding(tbTempCanvasEdgeMode, MyRedBorder, "MyRedBorder")

        'アンチエイリアス有無の設定、初期設定ではアリになっている
        'MyCanvasをアンチエイリアスなしで表示
        '中の要素には適用されないけど最終的に見えるのは一番上のMyCanvasなので
        'すべての要素に適用されているようにみえる
        RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Aliased) '要る
        RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Aliased) '要る

        '下2つはあまり意味ないかも、特にSnapのほう
        'MyCanvas.UseLayoutRounding = True
        'MyRedBorder.UseLayoutRounding = True
        'MyRedBorder.UseLayoutRounding = True
        'MyRedBorder.UseLayoutRounding = True

        'MyCanvas.SnapsToDevicePixels = True
        'MyRedBorder.SnapsToDevicePixels = True
        'MyRedBorder.SnapsToDevicePixels = True
        'MyRedBorder.SnapsToDevicePixels = True
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
    Private Sub btnChangeRedBorderEdgeMode_Click(sender As Object, e As RoutedEventArgs)
        If RenderOptions.GetEdgeMode(MyRedBorder) = EdgeMode.Aliased Then
            RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Unspecified)
        Else
            RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Aliased)
        End If
    End Sub

End Class
