'回転の中心軸は中心、つまりRenderTransformOriginal=point(0.5,0.5)
'Borderを画像ファイル保存、アンチエイリアスなしでも表示されている見た目通りの画像にしたい


Class MainWindow

    '    Private MyControlTemplate As ControlTemplate
    '   Private MyRedBorder As Border 'ControlTemplateの中のBorder
    '  Private MyRedBorder As Canvas 'ControlTemplateの中のCanvas

    Private Sub TextUpdate()
        Dim r As Rect
        r = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(GetRect(MyRedBorder))
        MyTextBlock1.Text = $" {r.X:000.00}, {r.Y:000.00}, {r.Width:000.00}, {r.Height:000.00}"

        '使うのはこれ
        r = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, MyRedBorder.ActualWidth, MyRedBorder.ActualHeight))
        MyTextBlock2.Foreground = Brushes.Red
        MyTextBlock2.Text = $" {r.X:000.0000}, {r.Y:000.0000}, {r.Width:000.00}, {r.Height:000.00}"
        MyTextBlock7.Text = $"左上座標 = ({r.X:0.00}, {r.Y:0.00})"
        MyTextBlock8.Text = $"右下座標 = ({r.X + r.Width:0.00}, {r.Y + r.Height:0.00})"




        r = MyCanvas.TransformToVisual(MyRedBorder).TransformBounds(New Rect(100, 100, 100, 100))
        MyTextBlock3.Text = $" {r.X:000.00}, {r.Y:000.00}, {r.Width:000.00}, {r.Height:000.00}"

        r = MyCanvas.TransformToVisual(MyRedBorder).TransformBounds(New Rect(0, 0, 100, 100))
        MyTextBlock4.Text = $" {r.X:000.00}, {r.Y:000.00}, {r.Width:000.00}, {r.Height:000.00}"

        'Dim gt As GeneralTransform = MyRedBorder.RenderTransform
        'Dim gt0 As GeneralTransform = MyRedBorder.TransformToVisual(MyCanvas)
        'Dim gt1 As GeneralTransform = MyRedBorder.TransformToAncestor(MyCanvas)
        ''Dim gt2 As GeneralTransform = MyRedBorder.TransformToDescendant(MyCanvas)
        'Dim gt3 As GeneralTransform = MyRedBorder.TransformToAncestor(MyRedBorder)
        ''Dim gt4 As GeneralTransform = MyRedBorder.TransformToDescendant(MyRedBorder)
        ''Dim gt5 As GeneralTransform = MyRedBorder.TransformToAncestor(MyRedBorder)
        'Dim gt6 As GeneralTransform = MyRedBorder.TransformToDescendant(MyRedBorder)


        'RenderTransform
        r = MyRedBorder.RenderTransform.TransformBounds(GetRect(MyRedBorder)) 'na
        MyTextBlock5.Text = $" {r.X:000.00}, {r.Y:000.00}, {r.Width:000.00}, {r.Height:000.00}"

        r = MyRedBorder.RenderTransform.TransformBounds(New Rect(0, 0, 100, 100))
        MyTextBlock6.Text = $" {r.X:000.00}, {r.Y:000.00}, {r.Width:000.00}, {r.Height:000.00}"

    End Sub
    Private Function GetRect(b As FrameworkElement)
        'Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.ActualWidth, b.ActualHeight)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call TextUpdate()
    End Sub


    Private Sub MySetBorder()
        With MyRedBorder
            .Width = 100
            .Height = 1
            .Background = Brushes.Red
        End With
        '必要ないかも？
        Canvas.SetLeft(MyRedBorder, 100) : Canvas.SetTop(MyRedBorder, 120)

        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(10.6) '回転角度
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(ro)
        End With
        MyRedBorder.RenderTransform = tg
        'MyRedBorder.RenderTransformOrigin = New Point(0.0, 0.0)
        MyRedBorder.RenderTransformOrigin = New Point(0.5, 0.5)

        Dim b As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty)}
        Angle1.SetBinding(Slider.ValueProperty, b)

        b = New Binding With {.Source = sldLeft, .Path = New PropertyPath(Slider.ValueProperty)}
        MyRedBorder.SetBinding(LeftProperty, b)
        b = New Binding With {.Source = sldTop, .Path = New PropertyPath(Slider.ValueProperty)}
        MyRedBorder.SetBinding(TopProperty, b)

    End Sub

    '現在日時を文字列に変換
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
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

    'ここからVisualBrushを使う方式

    'SaveImageTempCanvasVB2を改良
    'DrawRectangleのサイズを切り上げにして、RenderTargetBitmapのサイズも切り上げにした
    '結果見た目にかなり近いものができた、これが1番良さそう
    Private Sub SaveImageTempCanvasVB3(filePath As String)
        Dim r As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyRedBorder)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Size(Math.Ceiling(r.Width), Math.Ceiling(r.Height)))) 'サイズ切り上げ
        End Using
        'Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'サイズ切り上げ
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32) 'test
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub




    'ここからMyCanvasをブラシにして塗り
    'ほぼ完璧なんだけどアンチエイリアスなしでも半透明ピクセルが出てしまう？
    Private Sub SaveImageMyCanvasVB2(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(CInt(-r1.X), CInt(-r1.Y)), r.Size))’半透明が出る
            dc.DrawRectangle(vb, Nothing, New Rect(CInt(-r1.X), CInt(-r1.Y), CInt(r.Width), CInt(r.Height))) '半透明が出ないけどズレる？
        End Using
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)

    End Sub
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
    'アンチエイリアスありのとき用
    Private Sub SaveImageMyCanvasVB4(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(CInt(-r1.X), CInt(-r1.Y)), r.Size))
        End Using
        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width + 10), Math.Ceiling(r1.Height + 10), 96, 96, PixelFormats.Pbgra32)
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'アンチエイリアスありのとき用
    Private Sub SaveImageMyCanvasVB5(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(Math.Floor(-r1.X), Math.Floor(-r1.Y)), r.Size))
        End Using
        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width + 10), Math.Ceiling(r1.Height + 10), 96, 96, PixelFormats.Pbgra32)
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'アンチエイリアスありのとき用
    Private Sub SaveImageMyCanvasVB6(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(Math.Ceiling(-r1.X), Math.Ceiling(-r1.Y)), r.Size))
        End Using
        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width + 10), Math.Ceiling(r1.Height + 10), 96, 96, PixelFormats.Pbgra32)
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'アンチエイリアスありのとき用
    Private Sub SaveImageMyCanvasVB7(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(-r1.X, -r1.Y), r.Size))
        End Using
        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width + 10), Math.Ceiling(r1.Height + 10), 96, 96, PixelFormats.Pbgra32)
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'アンチエイリアスありのとき用、完成
    Private Sub SaveImageMyCanvasVB8(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim xOffset As Integer = -Fix(r1.X)
        Dim yOffset As Integer = -Fix(r1.Y)
        Dim migi As Integer = Math.Ceiling(r1.X + r1.Width)
        Dim sita As Integer = Math.Ceiling(r1.Y + r1.Height)
        Dim wRender As Integer = migi + xOffset
        Dim hRender As Integer = sita + yOffset

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            'r1.x = 100.7を-100にしたい
            Dim x As Integer = Math.Floor(r1.X) '100
            Dim xx As Integer = Math.Floor(-r1.X) '-101
            Dim xxx As Integer = -Math.Floor(r1.X) '100
            Dim fx As Integer = Fix(-r1.X) '-100
            Dim fxx As Integer = -Fix(r1.X) '-100

            dc.DrawRectangle(vb, Nothing, New Rect(New Point(xOffset, yOffset), r.Size))
        End Using
        '
        Dim rtb As New RenderTargetBitmap(wRender, hRender, 96, 96, PixelFormats.Pbgra32)
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
    'アンチエイリアスなし用…これは失敗だった
    'MyCanvasのVisualBrushを使うこの方法は無理があった
    'Offset位置を整数に丸めるとラインが変化してしまう
    'かと言ってそのままの位置で描画するとムダな余白ができてしまう
    Private Sub SaveImageMyCanvasVB10(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        'ブラシで塗るOffset位置
        Dim xOffset As Integer = IIf(r1.X - Fix(r1.X) > 0.5, Math.Ceiling(r1.X), Fix(r1.X))
        Dim yOffset As Integer = IIf(r1.Y - Fix(r1.Y) > 0.5, Math.Ceiling(r1.Y), Fix(r1.Y))

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(-xOffset, -yOffset), r.Size))
        End Using
        '
        'RenderTargetBitmapの(サイズ)
        Dim xw As Double = r1.X + r1.Width
        Dim yh As Double = r1.Y + r1.Height
        Dim migi As Integer = IIf(xw - Fix(xw) > 0.5, Math.Ceiling(xw), Fix(xw))
        Dim sita As Integer = IIf(yh - Fix(yh) > 0.5, Math.Ceiling(yh), Fix(yh))
        Dim wRender As Integer = migi - xOffset
        Dim hRender As Integer = sita - yOffset
        'RenderTargetBitmap
        Dim rtb As New RenderTargetBitmap(wRender, hRender, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub


    'アンチエイリアスなしの場合でもOK、完璧！！！
    'MyCanvas全体の画像を作成、その画像をOffsetしてDrawImage
    Private Sub SaveImageMyCanvasDI1(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        Dim dv As New DrawingVisual
        RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
        Using dc As DrawingContext = dv.RenderOpen
            'dc.DrawImage(bmp, New Rect(r.Size))'これは半透明が出る
            'dc.DrawImage(bmp, New Rect(New Size(CInt(r.Width), CInt(r.Height))))
            dc.DrawImage(bmp, New Rect(CInt(-r1.X), CInt(-r1.Y), CInt(r.Width), CInt(r.Height)))

        End Using

        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'SaveImageMyCanvasDI1の改良、今のところDrawImageならこれが1番
    '他の要素を一時的に非表示
    Private Sub SaveImageMyCanvasDI2(filePath As String)
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        Dim dv As New DrawingVisual
        RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
        Using dc As DrawingContext = dv.RenderOpen
            'dc.DrawImage(bmp, New Rect(r.Size))'これは半透明が出る
            'dc.DrawImage(bmp, New Rect(New Size(CInt(r.Width), CInt(r.Height))))
            dc.DrawImage(bmp, New Rect(CInt(-r1.X), CInt(-r1.Y), CInt(r.Width), CInt(r.Height)))
        End Using

        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        Call Bitmap2pngFile(rtb, filePath)
        MyCanvas.Background = Brushes.AliceBlue '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
    End Sub
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




    'SaveImageMyCanvasVB5を改変して色々テストしたけどイマイチ
    Private Sub SaveImageMyCanvasVBtest1(filePath As String)
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)
        Dim s As Size = New Size(MyRedBorder.ActualWidth, MyRedBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(MyRedBorder.Width, MyRedBorder.Height)
        'Dim r1 As Rect = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r1 As Rect = MyRedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(-0.5, -0.5, 100, 100)) 'test
        'ここでの四捨五入は無意味だった
        'With r1
        '    .X = CInt(.X) : .Y = CInt(.Y) : .Width = CInt(.Width) : .Height = CInt(.Height)
        'End With

        Dim s2 As New Size(MyBlueBorder.ActualWidth, MyBlueBorder.ActualHeight)
        Dim r2 As Rect = MyBlueBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s2))
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        Dim dv As New DrawingVisual
        'RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawImage(bmp, New Rect(-r1.X, -r1.Y, r.Width, r.Height))
            'dc.DrawImage(bmp, New Rect(-r1.X, -r1.Y, CInt(r.Width), CInt(r.Height)))
            'dc.DrawImage(bmp, New Rect(CInt(-r1.X), CInt(-r1.Y), r.Width, r.Height))
            'dc.DrawImage(bmp, New Rect(CInt(-r1.X), CInt(-r1.Y), CInt(r.Width), CInt(r.Height)))
            'dc.DrawImage(bmp, New Rect(Math.Ceiling(-r1.X), Math.Ceiling(-r1.Y), r.Width, r.Height))
            'dc.DrawImage(bmp, New Rect(Math.Ceiling(-r1.X), Math.Ceiling(-r1.Y), Math.Ceiling(r.Width), Math.Ceiling(r.Height)))
        End Using

        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        Call Bitmap2pngFile(rtb, filePath)
        MyCanvas.Background = Brushes.AliceBlue '背景色を戻す
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
    'これが1番良さそう
    Private Sub SaveImageCropped2式(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)

        Dim bgc As Brush = MyCanvas.Background
        MyCanvas.Background = Brushes.Transparent
        MyBlueBorder.Visibility = Visibility.Hidden
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        bmp.Render(MyCanvas)

        MyCanvas.Background = bgc
        MyBlueBorder.Visibility = Visibility.Visible

        Dim cb As New CroppedBitmap(bmp, New Int32Rect(Int(r1.X), Int(r1.Y), Math.Ceiling(r1.Width), Math.Ceiling(r1.Height)))
        Call Bitmap2pngFile(cb, filePath)
    End Sub
    'SaveImageCropped2式を改変、余白をなくしてみる
    Private Sub SaveImageCropped3式(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)

        Dim bgc As Brush = MyCanvas.Background
        MyCanvas.Background = Brushes.Transparent
        MyBlueBorder.Visibility = Visibility.Hidden
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

        bmp.Render(MyCanvas)

        MyCanvas.Background = bgc
        MyBlueBorder.Visibility = Visibility.Visible

        ''切り抜き範囲の起点x,y
        'Dim x, y As Integer : x = Fix(r1.X) : y = Fix(r1.Y)
        ''切り抜き範囲Width、Height
        'Dim w As Double = r1.X + r1.Width
        'w = IIf((w - Fix(w) > 0.5), Math.Ceiling(w), Fix(w))
        'w = w - x
        'Dim h As Double = r1.Y + r1.Height
        'h = IIf((h - Fix(h) > 0.5), Math.Ceiling(h), Fix(h))
        'h = h - y
        'Dim cr1 As New Int32Rect(x, y, w, h)

        Dim xx As Double = r1.X - Fix(r1.X)
        Dim yy As Double = r1.Y - Fix(r1.Y)
        xx = IIf(xx > 0.5, Math.Ceiling(r1.X), Fix(r1.X))
        yy = IIf(yy > 0.5, Math.Ceiling(r1.Y), Fix(r1.Y))

        'Dim ww As Double = r1.Width - Fix(r1.Width)
        'ww = IIf(ww > 0.5, Math.Ceiling(r1.Width), Fix(r1.Width))
        'Dim hh As Double = r1.Height - Fix(r1.Height)
        'hh = IIf(hh > 0.5, Math.Ceiling(r1.Height), Fix(r1.Height))
        'Dim cr2 As New Int32Rect(xx, yy, ww, hh)

        Dim www As Double = r1.X + r1.Width
        www = IIf((www - Fix(www)) > 0.5, Math.Ceiling(www), Fix(www))
        www -= xx
        Dim hhh As Double = r1.Y + r1.Height 'bottom
        hhh = IIf((hhh - Fix(hhh)) > 0.5, Math.Ceiling(hhh), Fix(hhh))
        hhh -= yy
        Dim cr3 As New Int32Rect(xx, yy, www, hhh)

        'Dim cb As New CroppedBitmap(bmp, cr1)
        'Dim cb As New CroppedBitmap(bmp, cr2)
        Dim cb As New CroppedBitmap(bmp, cr3)

        Call Bitmap2pngFile(cb, filePath)
    End Sub

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
        Dim wSize As Integer = IIf((xw - Fix(xw)) > 0.5, Math.Ceiling(xw), Fix(xw)) '変形四捨五入
        wSize -= xOffset '変形四捨五入した右端 - 変形四捨五入した左端
        '高さ
        Dim yh As Double = r1.Y + r1.Height 'bottom
        Dim hSize As Integer = IIf((yh - Fix(yh)) > 0.5, Math.Ceiling(yh), Fix(yh))
        hSize -= yOffset
        Dim cropSize As New Int32Rect(xOffset, yOffset, hSize, wSize)

        'クロップ！
        Dim cb As New CroppedBitmap(bmp, cropSize)

        Call Bitmap2pngFile(cb, filePath)
    End Sub



    'MyCanvas全てを画像ファイルにする、OK
    Private Sub SaveAllImage(filePath As String)
        Dim r1 As Rect = GetBoundRect(MyRedBorder, MyCanvas)
        Dim r2 As Rect = GetBoundRect(MyBlueBorder, MyCanvas)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)
        Call Bitmap2pngFile(bmp, filePath)
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
        'Call SaveImage3(filePath & "SaveImage3.png")
        'Call SaveImage4(filePath & "SaveImage4.png")
        'Call SaveImageTempCanvasVB(filePath & "SaveImageTempCanvasVB.png") '微妙にずれるけど他のがあっても単体で保存できる
        'Call SaveImageMyCanvasVB2(filePath & "SaveImageMyCanvasVB2.png") '半透明が出る？
        'Call SaveImageMyCanvasVB3(filePath & "SaveImageMyCanvasVB3.png") '半透明が出る
        'Call SaveImageMyCanvasDI1(filePath & "SaveImageMyCanvasDI1.png") 'できた！ただし他のと重なっているときはそのまま表示される
        'Call SaveImageMyCanvasDI2(filePath & "SaveImageMyCanvasDI2.png") ''できた！他のが重なっていても単体で保存できた！
        'Call SaveImageMyCanvasDI3(filePath & "SaveImageMyCanvasDI3.png") ''できた！他のが重なっていても単体で保存できた！
        Call SaveImageMyCanvasDI3改(filePath & "SaveImageMyCanvasDI3改.png") ''できた！他のが重なっていても単体で保存できた！

        'Call SaveImageTempCanvasVB2(filePath & "SaveImageTempCanvasVB2.png") '多分これが1番完璧だと思います→フラグ回収

        'Call SaveImageTempCanvasVB3(filePath & "SaveImageTempCanvasVB3.png") ''多分これが1番完璧だと思います、サイズ切り上げ、見た目に近いのでこれが良さそう

        'Call SaveImageTempCanvasVB4(filePath & "SaveImageTempCanvasVB4.png") '多分これが1番完璧だと思います、サイズ切り下げ、失敗
        'Call SaveImageMyCanvasVB6(filePath & "SaveImageMyCanvasVB6.png") 'いまいち
        'Call SaveImageTempCanvasVB5(filePath & "SaveImageTempCanvasVB5.png")
        'Call SaveImageTempCanvasVB6(filePath & "SaveImageTempCanvasVB6.png") 'test

        'Call SaveImageTempBorderVB1(filePath & "TempBorderVB1.png") 'イマイチ

        'クロップ方式、これはMyCanvasVB方式と同じ結果になる
        'Call SaveImageCropped1式(filePath & "SaveImageCropped1式.png")
        'Call SaveImageCropped2式(filePath & "SaveImageCropped2式.png") ''
        'Call SaveImageCropped3式(filePath & "SaveImageCropped3式.png") ''
        Call SaveImageCropped3式改(filePath & "SaveImageCropped3式改.png") ''

        'アンチエイリアスありのとき用
        'Call SaveImageMyCanvasVB4(filePath & "SaveImageMyCanvasVB4.png")
        'Call SaveImageMyCanvasVB5(filePath & "SaveImageMyCanvasVB5.png")
        'Call SaveImageMyCanvasVB6(filePath & "SaveImageMyCanvasVB6.png")
        'Call SaveImageMyCanvasVB7(filePath & "SaveImageMyCanvasVB7.png")
        'Call SaveImageMyCanvasVB8(filePath & "SaveImageMyCanvasVB8.png") '完成
        'Call SaveImageMyCanvasVB9(filePath & "SaveImageMyCanvasVB9.png") ''完成

        'アンチエイリアスなし用でもう一度VB
        'Call SaveImageMyCanvasVB10(filePath & "SaveImageMyCanvasVB10.png") 'VBでは無理があった

        '全体画像
        'Call SaveAllImage(filePath & "AllImage.png") '全体保存OK


        'まとめ
        'アンチエイリアスなしの場合に一番いいのはSaveImageCropped2式
        'なしの場合でも本当は素直だと思う方式のSaveImageTempCanvasVB3を使いたいけどズレる
        'アンチエイリアスありの場合は未調査だけどSaveImageTempCanvasVB3が良さそう
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
        'RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Aliased) '要る
        'MyCanvasをアンチエイリアスなしで表示
        '中の要素には適用されないけど最終的に見えるのは一番上のMyCanvasなので
        'すべての要素に適用されているようにみえる
        RenderOptions.SetEdgeMode(MyCanvas, EdgeMode.Aliased) '要る
        RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Aliased) '要る

        'RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Aliased)'多分いらない
        'RenderOptions.SetEdgeMode(MyRedBorder, EdgeMode.Aliased)’多分いらない

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
