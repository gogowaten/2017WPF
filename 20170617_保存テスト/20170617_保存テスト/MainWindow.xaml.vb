Class MainWindow

    '現在日時を文字列にして取得
    Private Function GetNowString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
    End Function

    '対象がぴったり収まるRect取得
    Private Function GetRect(obj As FrameworkElement)
        Return obj.TransformToVisual(MyCanvas).TransformBounds(New Rect(New Size(obj.ActualWidth, obj.ActualHeight)))
    End Function

    'Bitmapをpng画像で保存
    Private Sub Bitmap2pngFile(bmp As BitmapSource, filePath As String)
        Dim enc As New PngBitmapEncoder
        enc.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New IO.FileStream(filePath, IO.FileMode.Create)
            enc.Save(fs)
        End Using
    End Sub

    'RenderTargetBitmapを使って対象をBitmapにして保存
    Private Sub SaveImage(obj As FrameworkElement)
        Dim r As Rect = GetRect(obj)
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(obj)
        Dim str As String = GetNowString() & obj.Name & ".png"
        Call Bitmap2pngFile(rtb, str)
    End Sub
    'VisualBrushとRenderTargetBitmapを使って対象をBitmapにして保存
    Private Sub SaveImageVisualBrush(obj As FrameworkElement)
        Dim r As Rect = GetRect(obj)
        Dim vb As New VisualBrush(obj) With {.Stretch = Stretch.None}
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(New Size(r.Width, r.Height)))
        End Using
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Dim str As String = GetNowString() & obj.Name & ".png"
        Call Bitmap2pngFile(rtb, str)
    End Sub

    'Private Sub Test2ImageFile(obj As FrameworkElement, parentPanel As Panel)
    '    '変形した要素がぴったり収まるRectを取得
    '    Dim gt As GeneralTransform = obj.TransformToVisual(parentPanel)
    '    Dim r As Rect = gt.TransformBounds(New Rect(0, 0, obj.ActualWidth, obj.ActualHeight))
    '    '要素のVisualBrush(ブラシ)作成、引き伸ばしされないようにStretch.Noneを指定
    '    Dim vb As New VisualBrush(obj) With {.Stretch = Stretch.None}
    '    '四角枠にブラシを使って塗る
    '    Dim dv As New DrawingVisual
    '    Using dc As DrawingContext = dv.RenderOpen
    '        dc.DrawRectangle(vb, Nothing, New Rect(New Size(r.Width, r.Height)))
    '    End Using
    '    'Bitmap作成してRender
    '    Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
    '    rtb.Render(dv)

    '    'Bitmapをpng形式の画像で保存
    '    Dim enc As New PngBitmapEncoder
    '    enc.Frames.Add(BitmapFrame.Create(rtb))
    '    'Using fs As New IO.FileStream("testImage.png", IO.FileMode.Create)
    '    Using fs As New IO.FileStream(obj.Name & ".png", IO.FileMode.Create)
    '        enc.Save(fs)
    '    End Using
    'End Sub



    Private Sub TestSave1()
        Call SaveImage(MyOrangeBorder)
        Call SaveImage(MyRedBorder)
        Call SaveImage(MyPurpleBorder)
        Call SaveImage(MyPinkBorder)
        Call SaveImage(MyCyanBorder)
    End Sub
    Private Sub TestSave2()
        Dim r As Rect = GetRect(MyOrangeBorder)
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(MyOrangeCanvas)
        Dim str As String = GetNowString() & "MyOrangeBorder.png"
        Call Bitmap2pngFile(rtb, str)

        r = GetRect(MyRedBorder)
        rtb = New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(MyRedCanvas)
        str = GetNowString() & "MyRedborder.png"
        Call Bitmap2pngFile(rtb, str)

        r = GetRect(MyPurpleBorder)
        rtb = New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(MyPurpleCanvas)
        str = GetNowString() & "MyPurpleborder.png"
        Call Bitmap2pngFile(rtb, str)

        r = GetRect(MyPinkBorder)
        rtb = New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(MyPinkCanvas)
        str = GetNowString() & "MyPinkborder.png"
        Call Bitmap2pngFile(rtb, str)

    End Sub
    Private Sub TestSave3()
        Call SaveImageVisualBrush(MyOrangeBorder)
        Call SaveImageVisualBrush(MyRedBorder)
        Call SaveImageVisualBrush(MyPurpleBorder)
        Call SaveImageVisualBrush(MyPinkBorder)
        Call SaveImageVisualBrush(MyCyanBorder)

        Call SaveImageVisualBrush(MyPinkCanvas)

        'Call Test2ImageFile(MyOrangeBorder, MyCanvas)
        'Call Test2ImageFile(MyRedBorder, MyCanvas)
        'Call Test2ImageFile(MyPurpleBorder, MyCanvas)
        'Call Test2ImageFile(MyPinkBorder, MyCanvas)
        'Call Test2ImageFile(MyCyanBorder, MyCanvas)
        'Call Test2ImageFile(MyPinkCanvas, MyCanvas)
    End Sub

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnSave1.Click, AddressOf TestSave1
        AddHandler btnSave2.Click, AddressOf TestSave2
        AddHandler btnSave3.Click, AddressOf TestSave3

    End Sub
End Class
