'2017/05/22 やっとできた
'ThumbのControlTemplateにCanvasの中にBorderとImageを入れたものを適用

'Thumb
'   ┗Canvas(TempCanvas) これに回転や拡大を適用
'      ┣Border(TempBorder)
'      ┗Image

Imports System.Windows.Controls.Primitives

Public Class Window2
    Private MyControlTemplate As ControlTemplate
    Private TempBorder As Border
    Private TempCanvas As Canvas

    Private Sub TextUpdate()
        Dim r As Rect
        Dim str As String
        r = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(GetCanvasRect(TempCanvas)) 'na
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock1.Text = str
        '使うのはこれ
        r = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock2.Text = str

        r = TempCanvas.RenderTransform.TransformBounds(GetCanvasRect(TempCanvas)) 'na
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock3.Text = str

        r = TempCanvas.RenderTransform.TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock4.Text = str

        r = MyCanvas.TransformToVisual(TempCanvas).TransformBounds(New Rect(100, 100, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock5.Text = str

        r = MyCanvas.TransformToVisual(TempCanvas).TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock6.Text = str

    End Sub
    Private Function GetCanvasRect(b As Canvas)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
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
        MyThumb1.Template = MyControlTemplate
        MyThumb1.ApplyTemplate() 'Template再構築
        Dim ct As ControlTemplate = MyThumb1.Template

        TempBorder = ct.FindName("TempBorder", MyThumb1)
        TempCanvas = ct.FindName("TempCanvas", MyThumb1)
        With TempBorder
            .Width = 100
            .Height = 100
            .Background = Brushes.Red
        End With

        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(10)
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(ro)
        End With
        TempCanvas.RenderTransform = tg
        'TempCanvas.RenderTransformOrigin = New Point(0.0, 0.0)
        TempCanvas.RenderTransformOrigin = New Point(0.5, 0.5)

        Dim b As New Binding With {.Source = ro, .Path = New PropertyPath(RotateTransform.AngleProperty)}
        Angle1.SetBinding(Slider.ValueProperty, b)

    End Sub

    '現在日時を文字列に変換
    Private Function GetNowToString() As String
        Dim str As String = Now.ToString
        str = Replace(str, "/", "")
        str = Replace(str, ":", "")
        str = Replace(str, " ", "_")
        Return str
    End Function

    'TempCanvasを描画
    '描画位置がずれる、たぶんRenderTransformOriginalが0,0ならズレない
    Private Sub SaveImage2(filePath As String)
        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height)
        Dim r As Rect = TempBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(TempCanvas)
        Call SaveSub(bmp, filePath)
    End Sub
    'Borderを描画、回転などが適用されない元のものが描画される
    Private Sub SaveImage3(filePath As String)
        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height)
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(TempBorder)
        Call SaveSub(bmp, filePath)
    End Sub
    'MyThumbをRender、MyCanvasの左上の領域が描画されるので大幅なズレ
    Private Sub SaveImage4(filePath As String)
        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height)
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyThumb1)
        Call SaveSub(bmp, filePath)
    End Sub

    'ここからVisualBrushを使う方式
    'TempCanvasをブラシにして塗り
    'アンチエイリアスなしだとズレているのが分かる程度、妥協できる範囲
    '描画位置をOffsetすれば良さそうだけど数値がわからん、わかればこれが一番いい
    Private Sub SaveImageTempCanvasVB(filePath As String)
        'Dim dv As New DrawingBrush

        Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        'Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)'こっちの方がいい？
        Dim r As Rect = TempCanvas.TransformToVisual(MyThumb1).TransformBounds(New Rect(s))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(TempCanvas) 'MyThumb1にしても全く同じ
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(r.Size))
        End Using
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call SaveSub(rtb, filePath)
    End Sub
    'MyCanvasをブラシにして塗り
    'ほぼ完璧なんだけどアンチエイリアスなしでも半透明ピクセルが出てしまう
    Private Sub SaveImageTempCanvasVB2(filePath As String)

        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(MyCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Point(CInt(-r1.X), CInt(-r1.Y)), r.Size))
        End Using
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r1.Width), Math.Ceiling(r1.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call SaveSub(rtb, filePath)

    End Sub
    'いろいろアンチエイリアスなしに設定してみたけど半透明が出る
    Private Sub SaveImageTempCanvasVB3(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
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
        Call SaveSub(rtb, filePath)
    End Sub

    'アンチエイリアスなしの場合でもOK、完璧！！！
    'MyCanvas全体の画像を作成、その画像をOffsetしてDrawImage
    Private Sub SaveImageTempCanvasVB4(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
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
        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        Call SaveSub(rtb, filePath)
    End Sub

    '他の要素を一時的に非表示
    'アンチエイリアスなしの場合でもOK、完璧！！！
    'MyCanvas全体の画像を作成、その画像をOffsetしてDrawImage
    Private Sub SaveImageTempCanvasVB5(filePath As String)
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示

        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
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

        Call SaveSub(rtb, filePath)
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
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

        'Call SaveImage2(filePath & "SaveImage2.png")
        'Call SaveImage3(filePath & "SaveImage3.png")
        'Call SaveImage4(filePath & "SaveImage4.png")
        'Call SaveImageTempCanvasVB(filePath & "SaveImageTempCanvasVB.png") '微妙にずれるけど他のがあっても単体で保存できる
        'Call SaveImageTempCanvasVB2(filePath & "SaveImageTempCanvasVB2.png") '半透明が出る
        'Call SaveImageTempCanvasVB3(filePath & "SaveImageTempCanvasVB3.png") '半透明が出る
        'Call SaveImageTempCanvasVB4(filePath & "SaveImageTempCanvasVB4.png") 'できた！ただし他のと重なっているときはそのまま表示される
        Call SaveImageTempCanvasVB5(filePath & "SaveImageTempCanvasVB5.png") 'できた！他のが重なっていても単体で保存できた！
        Call SaveAllImage(filePath & "AllImage.png") '全体保存OK
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
