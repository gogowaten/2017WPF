
'Thumb
'   ┗ControlTemplate
'      ┗Canvas(TempCanvas) これに回転や拡大を適用
'          ┣Border(TempBorder)
'          ┗Image

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

    Private Sub TextUpdate()
        Dim r As Rect
        r = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(GetRect(MyThumb1))
        MyTextBlock1.Text = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"

        '使うのはこれ
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
            .Height = 20
            .Background = Brushes.Red
        End With
        '必要なかも？
        Canvas.SetLeft(TempBorder, 0) : Canvas.SetTop(TempBorder, 0)

        Dim tg As New TransformGroup
        Dim ro As New RotateTransform(0.2) '回転角度
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


        b = New Binding With {.Source = sldLeft, .Path = New PropertyPath(Slider.ValueProperty)}
        MyThumb1.SetBinding(LeftProperty, b)
        b = New Binding With {.Source = sldTop, .Path = New PropertyPath(Slider.ValueProperty)}
        MyThumb1.SetBinding(TopProperty, b)

        b = New Binding With {.Source = TempBorder, .Path = New PropertyPath(WidthProperty)}
        MyThumb1.SetBinding(WidthProperty, b)

        b = New Binding With {.Source = TempBorder, .Path = New PropertyPath(HeightProperty)}
        MyThumb1.SetBinding(HeightProperty, b)

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
        'Dim bmi As New BitmapImage(New Uri("D:\ブログ用\チェック用2\NEC_6506_2016_11_18_午後わてん.jpg"))
        'Dim bms As BitmapSource = bmi
        'Dim bmm As New WriteableBitmap(bms)
        '        Debugger Image Visualizer (Preview) - Visual Studio Marketplace
        'https://marketplace.visualstudio.com/items?itemName=AleksanderBerus.DebuggerImageVisualizerPreview



        bmp.Render(TempCanvas)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub
    'Borderを描画、回転などが適用されない元のものが描画される
    Private Sub SaveImage3(filePath As String)
        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height)
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(TempBorder)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub
    'MyThumbをRender、MyCanvasの左上の領域が描画されるので大幅なズレ
    Private Sub SaveImage4(filePath As String)
        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height)
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim bmp As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyThumb1)
        Call Bitmap2pngFile(bmp, filePath)
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
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'SaveImageTempCanvasVBを改良、これでOK
    'DrawRectangleのサイズをCIntで整数にすればいいだけみたい
    'アンチエイリアスなしならTempCanvasのEdgeModeをAliasedにする必要がある
    Private Sub SaveImageTempCanvasVB2(filePath As String)
        Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        'Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)'こっちの方がいい？
        Dim r As Rect = TempCanvas.TransformToVisual(MyThumb1).TransformBounds(New Rect(s))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(TempCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Size(CInt(r.Width), CInt(r.Height)))) '違いはこの一行だけ
            'dc.DrawRectangle(vb, Nothing, New Rect(New Size(Math.Ceiling(r.Width), Math.Ceiling(r.Height)))) '切り上げ
            'dc.DrawRectangle(vb, Nothing, New Rect(New Size(Math.Floor(r.Width), Math.Floor(r.Height)))) '切り捨て
        End Using
        Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32) 'test
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'SaveImageTempCanvasVB2を改良
    'DrawRectangleのサイズを切り上げにして、RenderTargetBitmapのサイズも切り上げにした
    '結果見た目にかなり近いものができた、これが1番良さそう
    Private Sub SaveImageTempCanvasVB3(filePath As String)
        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height)
        'Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)'こっちの方がいい？
        Dim r As Rect = TempCanvas.TransformToVisual(MyThumb1).TransformBounds(New Rect(s))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(TempCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Size(Math.Ceiling(r.Width), Math.Ceiling(r.Height)))) 'サイズ切り上げ
        End Using
        'Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        'サイズ切り上げ
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32) 'test
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'SaveImageTempCanvasVB2を改変、切り下げにしたけど失敗、これは使わない
    Private Sub SaveImageTempCanvasVB4(filePath As String)
        Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        'Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)'こっちの方がいい？
        Dim r As Rect = TempCanvas.TransformToVisual(MyThumb1).TransformBounds(New Rect(s))
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(TempCanvas)
            vb.Stretch = Stretch.None
            dc.DrawRectangle(vb, Nothing, New Rect(New Size(Math.Floor(r.Width), Math.Floor(r.Height)))) '切り捨て
        End Using
        'Dim rtb As New RenderTargetBitmap(r.Width, r.Height, 96, 96, PixelFormats.Pbgra32)
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32) 'test
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'SaveImageTempCanvasVBを改変
    Private Sub SaveImageTempCanvasVB5(filePath As String)
        'Dim dv As New DrawingBrush

        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height) '(100, 100) 
        'Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)'こっちの方がいい？
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyThumb1).TransformBounds(New Rect(s)) 'test

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(TempCanvas) 'MyThumb1にしても全く同じ
            vb.Stretch = Stretch.None
            Dim xx As Double = r1.X - Fix(r1.X)
            Dim yy As Double = r1.Y - Fix(r1.Y)

            'dc.DrawRectangle(vb, Nothing, New Rect(r.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, CInt(r.Width), CInt(r.Height))) '四捨五入
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, Math.Ceiling(r.Width), Math.Ceiling(r.Height))) 'サイズ切り上げ
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(-xx, -yy), r.Size)) '4
            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(xx, yy), r.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, CInt(r.Width), CInt(r.Height)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, Math.Ceiling(r.Width), Math.Ceiling(r.Height)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, r.Width - xx, r.Height - yy))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, CInt(r.Width - xx), CInt(r.Height - yy)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx / 2, -yy / 2, r.Width, r.Height))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx / 2, -yy / 2, CInt(r.Width), CInt(r.Height)))
            ''dc.DrawRectangle(vb, Nothing, New Rect(r.X - 100, r.Y - 100, CInt(r.Width), CInt(r.Height)))

            'dc.DrawRectangle(vb, Nothing, New Rect(xx, yy, r.Width, r.Height)) '12
            'dc.DrawRectangle(vb, Nothing, New Rect(xx, yy, CInt(r.Width), CInt(r.Height))) '13
            'dc.DrawRectangle(vb, Nothing, New Rect(xx / 2, yy / 2, r.Width, r.Height)) '14
            'dc.DrawRectangle(vb, Nothing, New Rect(xx / 2, yy / 2, CInt(r.Width), CInt(r.Height))) '15

            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, r.Width + xx, r.Height + yy)) '16
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, r.Width - xx, r.Height - yy))
            'dc.DrawRectangle(vb, Nothing, New Rect(xx, yy, r.Width + xx, r.Height + yy))
            'dc.DrawRectangle(vb, Nothing, New Rect(xx, yy, r.Width - xx, r.Height - yy))

            'dc.DrawRectangle(vb, Nothing, New Rect(New Point(xx, yy), r.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, CInt(r.Width), CInt(r.Height)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, Math.Ceiling(r.Width), Math.Ceiling(r.Height)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, r.Width - xx, r.Height - yy))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx, -yy, CInt(r.Width - xx), CInt(r.Height - yy)))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx / 2, -yy / 2, r.Width, r.Height))
            'dc.DrawRectangle(vb, Nothing, New Rect(-xx / 2, -yy / 2, CInt(r.Width), CInt(r.Height)))


        End Using
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Call Bitmap2pngFile(rtb, filePath)
    End Sub
    'サイズ決め打ちでテスト
    Private Sub SaveImageTempCanvasVB6(filePath As String)

        Dim s As Size = New Size(TempBorder.Width, TempBorder.Height) '(100, 100) 
        'Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)'こっちの方がいい？
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyThumb1).TransformBounds(New Rect(s)) 'test

        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            Dim vb As New VisualBrush(TempCanvas) 'MyThumb1にしても全く同じ
            vb.Stretch = Stretch.None

            'RenderOptions.SetEdgeMode(vb, EdgeMode.Aliased) '変化なし
            Dim xx As Double = r1.X - Fix(r1.X)
            Dim yy As Double = r1.Y - Fix(r1.Y)

            'dc.DrawRectangle(vb, Nothing, New Rect(r.Size))
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, CInt(r.Width), CInt(r.Height))) '四捨五入
            dc.DrawRectangle(vb, Nothing, New Rect(0, 0, Math.Ceiling(r.Width), Math.Ceiling(r.Height))) 'サイズ切り上げ


            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, 201, 201))
            'dc.DrawRectangle(vb, Nothing, New Rect(0.087, 0.087, 201, 201)) '0.1do

            'Dim xxx As Double = 50 - r.X
            'Dim yyy As Double = 50 - r.Y
            'xxx /= 2
            'yyy /= 2
            'Dim xxxx As Double = ((50 - r.X) - Fix(50 - r.X)) / 2
            'Dim yyyy As Double = ((50 - r.Y) - Fix(50 - r.Y)) / 2

            'dc.DrawRectangle(vb, Nothing, New Rect(-xxxx, -yyyy, Math.Ceiling(r.Width), Math.Ceiling(r.Height)))
        End Using
        Dim rtb As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        'Dim rtb As New RenderTargetBitmap(201, 201, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)

        Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'TempBorderをブラシにしてみたけど位置がずれる、イマイチ
    Private Sub SaveImageTempBorderVB1(filePath As String)
        Dim r As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, TempBorder.Width, TempBorder.Height))
        Dim vb As New VisualBrush(TempBorder)
        vb.Stretch = Stretch.None
        RenderOptions.SetEdgeMode(vb, EdgeMode.Aliased)
        RenderOptions.SetEdgeMode(TempBorder, EdgeMode.Aliased)
        'vb.Transform = TempCanvas.RenderTransform
        Dim dv As New DrawingVisual
        'dv.Transform = TempCanvas.RenderTransform
        Using dc As DrawingContext = dv.RenderOpen
            RenderOptions.SetEdgeMode(dv, EdgeMode.Aliased)
            dc.PushTransform(TempCanvas.RenderTransform)
            'dc.DrawRectangle(vb, Nothing, New Rect(0, 0, r.Width, r.Height))
            dc.DrawRectangle(vb, Nothing, New Rect(r.X / 2, -r.Y / 2, r.Width, r.Height))
        End Using
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        bmp.Render(dv)
        Call Bitmap2pngFile(bmp, filePath)
    End Sub

    '



    'ここからMyCanvasをブラシにして塗り
    'ほぼ完璧なんだけどアンチエイリアスなしでも半透明ピクセルが出てしまう？
    Private Sub SaveImageMyCanvasVB2(filePath As String)

        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
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
        Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'アンチエイリアスなしの場合でもOK、完璧！！！
    'MyCanvas全体の画像を作成、その画像をOffsetしてDrawImage
    Private Sub SaveImageMyCanvasVB4(filePath As String)
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

        Call Bitmap2pngFile(rtb, filePath)
    End Sub

    'SaveImageMyCanvasVB4の改変
    '他の要素を一時的に非表示
    Private Sub SaveImageMyCanvasVB5(filePath As String)
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)

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

        Call Bitmap2pngFile(rtb, filePath)
        MyCanvas.Background = Brushes.AliceBlue '背景色を戻す
        MyBlueBorder.Visibility = Visibility.Visible '青Borderを再表示
    End Sub
    'SaveImageMyCanvasVB5を改変して色々テストしたけどイマイチ
    Private Sub SaveImageMyCanvasVB6(filePath As String)
        MyBlueBorder.Visibility = Visibility.Hidden '青Borderを非表示
        MyCanvas.Background = Brushes.Transparent '背景色を透明にする
        '背景色を透明にするには再描画が必要
        MyCanvas.Dispatcher.Invoke(Threading.DispatcherPriority.Render, Sub()

                                                                        End Sub)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        'Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(-0.5, -0.5, 100, 100)) 'test
        'ここでの四捨五入は無意味だった
        'With r1
        '    .X = CInt(.X) : .Y = CInt(.Y) : .Width = CInt(.Width) : .Height = CInt(.Height)
        'End With

        Dim r2 As Rect = New Rect(0, 0, 10, 10)
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
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
        Dim r As Rect = Rect.Union(r1, r2)
        Dim bmp As New RenderTargetBitmap(Math.Ceiling(r.Width), Math.Ceiling(r.Height), 96, 96, PixelFormats.Pbgra32)
        bmp.Render(MyCanvas)

        Dim cb As New CroppedBitmap(bmp, New Int32Rect(Int(r1.X), Int(r1.Y), Math.Ceiling(r1.Width), Math.Ceiling(r1.Height)))

        Call Bitmap2pngFile(cb, filePath)
    End Sub
    'SaveImageCropped1式を改良、他の図形と重なっているときでもOK
    'これが1番良さそう
    Private Sub SaveImageCropped2式(filePath As String)
        Dim s As Size = New Size(TempBorder.ActualWidth, TempBorder.ActualHeight)
        'Dim s As Size = New Size(100, 100) '(TempBorder.Width, TempBorder.Height)
        Dim r1 As Rect = TempCanvas.TransformToVisual(MyCanvas).TransformBounds(New Rect(s))
        Dim r2 As Rect = New Rect(0, 0, 10, 10)
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
        'Call SaveImage2(filePath & "SaveImage2.png")
        'Call SaveImage3(filePath & "SaveImage3.png")
        'Call SaveImage4(filePath & "SaveImage4.png")
        'Call SaveImageTempCanvasVB(filePath & "SaveImageTempCanvasVB.png") '微妙にずれるけど他のがあっても単体で保存できる
        'Call SaveImageMyCanvasVB2(filePath & "SaveImageMyCanvasVB2.png") '半透明が出る？
        'Call SaveImageMyCanvasVB3(filePath & "SaveImageMyCanvasVB3.png") '半透明が出る
        'Call SaveImageMyCanvasVB4(filePath & "SaveImageMyCanvasVB4.png") 'できた！ただし他のと重なっているときはそのまま表示される
        'Call SaveImageMyCanvasVB5(filePath & "SaveImageMyCanvasVB5.png") 'できた！他のが重なっていても単体で保存できた！
        'Call SaveImageTempCanvasVB2(filePath & "SaveImageTempCanvasVB2.png") '多分これが1番完璧だと思います→フラグ回収

        Call SaveImageTempCanvasVB3(filePath & "SaveImageTempCanvasVB3.png") '多分これが1番完璧だと思います、サイズ切り上げ、見た目に近いのでこれが良さそう

        'Call SaveImageTempCanvasVB4(filePath & "SaveImageTempCanvasVB4.png") '多分これが1番完璧だと思います、サイズ切り下げ、失敗
        'Call SaveImageMyCanvasVB6(filePath & "SaveImageMyCanvasVB6.png") 'いまいち
        'Call SaveImageTempCanvasVB5(filePath & "SaveImageTempCanvasVB5.png")
        'Call SaveImageTempCanvasVB6(filePath & "SaveImageTempCanvasVB6.png") 'test

        'Call SaveImageTempBorderVB1(filePath & "TempBorderVB1.png") 'イマイチ

        'クロップ方式、これはMyCanvasVB方式と同じ結果になる
        'Call SaveImageCropped1式(filePath & "SaveImageCropped1式.png")
        Call SaveImageCropped2式(filePath & "SaveImageCropped2式.png")

        'まとめ
        'アンチエイリアスなしの場合に一番いいのはSaveImageCropped2式
        'なしの場合でも本当は素直だと思う方式のSaveImageTempCanvasVB3を使いたいけどズレる
        'アンチエイリアスありの場合は未調査だけどSaveImageTempCanvasVB3が良さそう
        '

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
        RenderOptions.SetEdgeMode(TempCanvas, EdgeMode.Aliased) '要る
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
