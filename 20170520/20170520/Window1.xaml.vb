Public Class Window1

    Private Sub TextUpdate()
        Dim r As Rect
        Dim str As String
        r = RedBorder.TransformToVisual(MyCanvas).TransformBounds(GetBorderRect(RedBorder))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock1.Text = str

        r = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock2.Text = str

        r = RedBorder.RenderTransform.TransformBounds(GetBorderRect(RedBorder))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock3.Text = str

        r = RedBorder.RenderTransform.TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock4.Text = str

        r = MyCanvas.TransformToVisual(RedBorder).TransformBounds(New Rect(100, 100, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock5.Text = str

        r = MyCanvas.TransformToVisual(RedBorder).TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock6.Text = str

    End Sub
    Private Function GetBorderRect(b As Border)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call TextUpdate()
    End Sub
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
    End Function
    'RedBorderのVisualBrush微妙にずれるけど妥協できる範囲
    Private Sub SaveImage(filePath As String)
        'Dim dv As New DrawingBrush

        Dim s As Size = New Size(RedBorder.Width, RedBorder.Height)
        Dim r As Rect = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(RedBorder)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(r.Size))
        End Using
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call SaveSub(rtb, filePath)
    End Sub
    Private Sub SaveImage1(filePath As String)
        'Dim dv As New DrawingBrush

        Dim s As Size = New Size(RedBorder.Width, RedBorder.Height)
        Dim r As Rect = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r1 As Rect = MyCanvas.TransformToVisual(RedBorder).TransformBounds(New Rect(New Point(100, 100), s))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(RedBorder)
            vb.Stretch = Stretch.None
            'dc.PushTransform(MyCanvas.TransformToVisual(RedBorder))
            'dc.PushTransform(RedBorder.TransformToVisual(MyCanvas))
            dc.PushTransform(New TranslateTransform(r1.X, r1.Y))
            dc.DrawRectangle(vb, Nothing, New Rect(r.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(r1.Location, r.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(r1.X / 2, r1.Y / 2), r.Size))
        End Using
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call SaveSub(rtb, filePath)
    End Sub
    'MyCanvasのVisualBrush、これも同じ結果で微妙にずれる
    'DrawRectangleでOffsetしている
    Private Sub SaveImage2(filePath As String)
        Dim s As Size = New Size(RedBorder.Width, RedBorder.Height)
        Dim r1 As Rect = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10) 'BlueBorder
        Dim r As Rect = Rect.Union(r1, r2)

        Dim vb As New VisualBrush(MyCanvas)
        vb.Stretch = Stretch.None
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawRectangle(vb, Nothing, New Rect(-r1.X, -r1.Y, r.Width, r.Height))
        End Using
        Dim rtb As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call SaveSub(rtb, filePath)
    End Sub
    '
    Private Sub SaveImage3(filePath As String)
        Dim s As Size = New Size(RedBorder.Width, RedBorder.Height)
        Dim r1 As Rect = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10) 'BlueBorder
        Dim r As Rect = Rect.Union(r1, r2)
        'Dim r3 As Rect = MyCanvas.TransformToVisual(RedBorder).TransformBounds(New Rect(New Point(100, 100), s))
        Dim vb As New VisualBrush(MyCanvas)
        vb.Stretch = Stretch.None
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            'dc.PushClip(New RectangleGeometry(New Rect(r1.X, r1.Y, r.Width, r.Height)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-99, -99, r.Width, r.Height)) 'ok
            'dc.DrawRectangle(vb, Nothing, New Rect(-r1.X, -r1.Y, r.Width, r.Height)) '微妙にずれる
            '1
            'Dim xx As Integer = -r1.X
            'Dim yy As Integer = -r1.Y
            'dc.DrawRectangle(vb, Nothing, New Rect(xx, yy, r.Width, r.Height))
            '2
            'Dim xx As Integer = r1.X
            'Dim yy As Integer = r1.Y
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, CInt(r.Width), CInt(r.Height)))
            '3
            'dc.DrawRectangle(vb, Nothing, New Rect(-r1.X, -r1.Y, CInt(r.Width), CInt(r.Height)))
            '4、1番より良好なものは得られなかった
            Dim x As Integer = r1.X
            Dim xx As Integer = Math.Floor(r1.X)
            Dim xxx As Integer = Math.Ceiling(r1.X)
            Dim y As Integer = r1.Y
            Dim yy As Integer = Math.Floor(r1.Y)
            Dim yyy As Integer = Math.Ceiling(r1.Y)
            Dim w As Integer = r.Width
            Dim ww As Integer = Math.Floor(r.Width)
            Dim www As Integer = Math.Ceiling(r.Width)
            Dim h As Integer = r.Height
            Dim hh As Integer = Math.Floor(r.Height)
            Dim hhh As Integer = Math.Ceiling(r.Height)
            'dc.DrawRectangle(vb, Nothing, New Rect(-x, -y, www, hhh))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, www, hhh))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xxx, -yyy, www, hhh))
            'dc.DrawRectangle(vb, Nothing, New Rect(-x, -y, w, h))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, w, h))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xxx, -yyy, w, h))
            'dc.DrawRectangle(vb, Nothing, New Rect(-x, -y, ww, hh))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, ww, hh))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xxx, -yyy, ww, hh))
            'dc.DrawRectangle(vb, Nothing, New Rect(-x, -y, r.Width, r.Height))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, r.Width, r.Height))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xxx, -yyy, r.Width, r.Height))



            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r.Width, r.Height))
        End Using
        'Dim rtb As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        '切り上げサイズ
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        'Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'Dim db As New DrawingBrush
        'Dim di As New DrawingImage
        'Dim id As New ImageDrawing(rtb, r)


        rtb.Render(dv)
        Call SaveSub(rtb, filePath)
    End Sub
    'SaveImage2のズレを軽減、Integerに変換で軽減できた、ほぼ完璧
    'MyCanvasのVisualBrushをOffsetして塗る
    Private Sub SaveImage4(filePath As String)
        Dim s As Size = New Size(RedBorder.Width, RedBorder.Height) '(100,100)
        Dim r1 As Rect = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10) 'BlueBorder
        Dim r As Rect = Rect.Union(r1, r2) 'MyCanvas全体(赤と青のBorderが収まる)のサイズ

        Dim vb As New VisualBrush(MyCanvas)
        vb.Stretch = Stretch.None 'ブラシの引き伸ばしなし設定、いらないかも
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            'Offsetは整数に変換、これをしないと見た目と違ってくる
            dc.DrawRectangle(vb, Nothing, New Rect(CInt(-r1.X), CInt(-r1.Y), r.Width, r.Height))
        End Using
        'Dim rtb As New RenderTargetBitmap(r1.Width, r1.Height, 96, 96, PixelFormats.Pbgra32)
        '切り上げサイズ、切り上げないとたまに切れてしまう
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call SaveSub(rtb, filePath)
    End Sub
    '
    Private Sub SaveMyCanvas(filePath As String)
        Dim s As Size = New Size(RedBorder.Width, RedBorder.Height)
        Dim r1 As Rect = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10) 'BlueBorder
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)
        Call SaveSub(bmp, filePath)
    End Sub
    Private Sub SaveSub(bmp As BitmapSource, filePath As String)
        Dim enc As New PngBitmapEncoder
        enc.Frames.Add(BitmapFrame.Create(bmp))
        Using fs As New IO.FileStream(filePath, IO.FileMode.Create)
            enc.Save(fs)
        End Using
    End Sub
    Private Sub SaveImage_Click(sender As Object, e As RoutedEventArgs)
        Dim filePath As String = GetNowToString()
        'Call SaveImage(filePath & "RedBorder" & ".png") 'RedBorderだけ保存、だいたいOK、少しズレる
        'Call SaveImage2(filePath & "MyCanvas" & ".png") 'あかん
        'Call SaveMyCanvas(filePath & "MyCanvas.png") 'MyCanvas全体
        'Call SaveImage1(filePath & "RedBorder1.png")'意味なし
        'Call SaveImage3(filePath & "Image3.png")
        Call SaveImage4(filePath & "Image4.png") 'ほぼ完璧
    End Sub


    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnTextUpdate.Click, AddressOf TextUpdate_Click
        AddHandler btnSave.Click, AddressOf SaveImage_Click
        RenderOptions.SetEdgeMode(RedBorder, EdgeMode.Aliased)
        'RedBorder.UseLayoutRounding = True
    End Sub
    Private Sub Window1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Call TextUpdate()
    End Sub
End Class
