Imports System.Windows.Controls.Primitives

Class MainWindow
    Private WithEvents NowExThumb1 As ExThumb
    Private tList As New System.Collections.ObjectModel.ObservableCollection(Of ExThumb)
    Private Const GridValue As Integer = 8 'グリッド

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnCheck.Click, AddressOf BtnCheck_Click
        AddHandler BtnFixMyThumbLocate.Click, AddressOf BtnFixMyThumbLocate_Click
        AddHandler btnFixMyCanvasSize.Click, AddressOf btnFixMyCanvasSize_Click

        '縁取り
        '枠
        '下線
        'パディング
        '背景色
        '行間隔
        '文字間隔

        Dim ext As ExThumb
        '文字の描画1、TypeFace→FormattedText→BuildGeometry→GeometryDrawing→DrawingBrush→BorderのBackGroundに指定
        'WPFサンプル:DrawingBrushで枠付きテキストを表示する:Gushwell's C# Dev Notes
        'http://gushwell.ldblog.jp/archives/52312432.html
        'Dim tft As New TextFormatting.TextBounds
        'Dim tt As New TextPointer

        Dim lgBrush As New LinearGradientBrush(Colors.Magenta, Colors.Cyan, New Point(0.2, 0), New Point(0.8, 1))
        'Dim ff As New FontFamily("自由の翼フォント")
        'Dim ff As New FontFamily("ＭＳ ゴシック")
        Dim ff As New FontFamily("Meiryo UI")
        'Dim typeFace As New Typeface("自由の翼フォント")
        Dim typeFace As New Typeface(ff, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)
        'Dim typeFace As New Typeface(ff, FontStyles.Italic, FontWeights.Bold, FontStretches.Normal)
        'Dim typeFace As New Typeface(ff, FontStyles.Oblique, FontWeights.Bold, FontStretches.Normal)

        Dim formattedText As New FormattedText("Pixtack紫陽花",
                                    Globalization.CultureInfo.CurrentCulture,
                                    FlowDirection.LeftToRight,
                                    typeFace,
                                    48,
                                    New SolidColorBrush)
        'formattedText.SetForegroundBrush(Brushes.Red, 2, 3) '今回の場合は効果なしになる
        'formattedText.LineHeight = 150 '行の高さしてい？
        'formattedText.MaxTextWidth = 100 '描画領域の最大幅してい

        '書式設定されたテキストの描画
        'https://msdn.microsoft.com/ja-jp/library/ms752098(v=vs.110).aspx
        'よりフォントサイズの測定単位をポイントに変換
        'formattedText.SetFontSize(48 * (96.0 / 72.0)) 'フォントサイズ48をポイントに変換？
        formattedText.SetFontSize(48)

        '下線や取り消し線
        'Dim td1 As New TextDecoration With {.Pen = New Pen(Brushes.Red, 2), .Location = TextDecorationLocation.Baseline}
        'Dim tdc As New TextDecorationCollection From {td1}
        'formattedText.SetTextDecorations(tdc)

        Debug.WriteLine($"BaseLine = {formattedText.Baseline}")
        Debug.WriteLine($"Extent = {formattedText.Extent}")
        'Debug.WriteLine($"FlowDirection = {formattedText.FlowDirection}")
        Debug.WriteLine($"Height = {formattedText.Height}")
        Debug.WriteLine($"LineHeight行間隔 = {formattedText.LineHeight}")
        Debug.WriteLine($"MaxLineCount = {formattedText.MaxLineCount}")
        Debug.WriteLine($"MaxTextHeight = {formattedText.MaxTextHeight}")
        Debug.WriteLine($"MaxTextWidth = {formattedText.MaxTextWidth}")
        Debug.WriteLine($"MinWidth = {formattedText.MinWidth}")
        Debug.WriteLine($"OverhangAfter = {formattedText.OverhangAfter}")
        Debug.WriteLine($"OverhangLeading = {formattedText.OverhangLeading}")
        Debug.WriteLine($"OverhangTrailing = {formattedText.OverhangTrailing}")
        Debug.WriteLine($"TextAlignment = {formattedText.TextAlignment}")
        Debug.WriteLine($"Trimming = {formattedText.Trimming}")
        Debug.WriteLine($"Width = {formattedText.Width}")
        Debug.WriteLine($"WidthIncludingTrailingWhitespace = {formattedText.WidthIncludingTrailingWhitespace}")


        Dim textGeometry As Geometry = formattedText.BuildGeometry(New Point)
        'textGeometry.Transform = New ScaleTransform(2, 2, 0.5, 0.5)'2倍拡大
        'Dim textGeometry2 As Geometry = formattedText.BuildHighlightGeometry(New Point)
        Debug.WriteLine($"textGeometry.Bounds = {textGeometry.Bounds}")
        Debug.WriteLine($"textGeometry.GetRenderBounds(New Pen(Brushes.Red, 1)) = {textGeometry.GetRenderBounds(New Pen(Brushes.Red, 1))}")


        'Dim drawing As New GeometryDrawing(Brushes.Blue, New Pen(lgb, 2), textGeometry)
        'Dim geometryDrawing As New GeometryDrawing(lgBrush, New Pen(Brushes.White, 2), textGeometry)
        'Dim drawing As New GeometryDrawing(Nothing, New Pen(lgb, 1), textGeometry)
        Dim geometryDrawing As New GeometryDrawing(lgBrush, Nothing, textGeometry)
        Debug.WriteLine($"geometryDrawing.Bounds = {geometryDrawing.Bounds}")


        Dim dBrush As DrawingBrush
        dBrush = New DrawingBrush(geometryDrawing) With {.Stretch = Stretch.None}

        '普通の
        Dim bo As Border
        bo = New Border With {
            .Width = formattedText.Width,
            .Height = formattedText.Height,
            .Background = dBrush}
        Call AddElementToMyCanvas(bo, 10, 10)

        Dim bo0 As New Border With {
            .Width = textGeometry.Bounds.Width,
            .Height = textGeometry.Bounds.Height,
            .Background = dBrush}
        Call AddElementToMyCanvas(bo0, 10, 100)


        Dim bo1 As New Border With {
            .Width = geometryDrawing.Bounds.Width,
            .Height = geometryDrawing.Bounds.Height,
            .Background = dBrush}
        Call AddElementToMyCanvas(bo1, 10, 200)

        '余白なし
        Dim bo2 As Border
        bo2 = New Border With {
            .Width = formattedText.Width - formattedText.OverhangLeading - formattedText.OverhangTrailing,
            .Height = formattedText.Extent,
            .Background = dBrush}
        '垂直水平中央揃え
        'dBrush = New DrawingBrush(drawing) With {.Stretch = Stretch.None}
        'bo2 = New Border With {.Width = formattedText.Width, .Height = formattedText.Height, .Background = dBrush}
        '垂直左寄せ＋水平中央揃え？
        'dBrush = New DrawingBrush(drawing) With {.Stretch = Stretch.None, .AlignmentX = AlignmentX.Left}
        'bo2 = New Border With {.Width = formattedText.Width, .Height = formattedText.Height, .Background = dBrush, .Margin = New Thickness(Math.Ceiling(formattedText.OverhangLeading), 0, formattedText.OverhangTrailing, 0)}

        Call AddElementToMyCanvas(bo2, 10, 300)
        'ext = New ExThumb(bo2)
        'AddHandler ext.PreviewMouseDown, AddressOf NowExThumb1_PreviewMouseDown 'クリックイベント
        'MyCanvas.Children.Add(ext)
        'ext.SetLocate(10, 10)
        'tList.Add(ext)

        'Dim gtf As New Glyphs
        'Dim gr As New GlyphRunDrawing()





        '文字の描画2、TypeFace→FormattedText→BuildGeometry→ここまで1と同じ
        'PathのDataにGeometryを指定、PathをCanvasに表示
        Dim pText As New Path
        With pText
            .Data = textGeometry
            .Stroke = lgBrush
            .StrokeThickness = 1
            .Width = formattedText.Width
            .Height = formattedText.Height
            .Fill = Brushes.Blue
            .Margin = New Thickness(0, formattedText.OverhangAfter, 0, 0)
        End With
        ext = New ExThumb(pText)
        AddHandler ext.PreviewMouseDown, AddressOf NowExThumb1_PreviewMouseDown 'クリックイベント
        MyCanvas.Children.Add(ext)
        ext.SetLocate(0, 350)
        tList.Add(ext)

        '文字の描画3、TypeFace→FormattedText→BuildGeometry→ここまで1と同じ
        'DrawingVisualのDrawingContextでDrawGeometry→VisualBrushにしてBorderの背景に指定
        Dim dv As New DrawingVisual
        Using dc As DrawingContext = dv.RenderOpen
            dc.DrawGeometry(Nothing, New Pen(Brushes.White, 20), textGeometry)
            dc.DrawGeometry(lgBrush, Nothing, textGeometry)
            dc.DrawGeometry(Nothing, New Pen(Brushes.Pink, 1), formattedText.BuildHighlightGeometry(New Point))
        End Using
        Dim vb As New VisualBrush(dv) With {.Stretch = Stretch.None}
        Dim bo3 As New Border With {.Width = formattedText.Width + 20, .Height = formattedText.Height, .Background = vb}
        ext = New ExThumb(bo3)
        AddHandler ext.PreviewMouseDown, AddressOf NowExThumb1_PreviewMouseDown 'クリックイベント
        MyCanvas.Children.Add(ext)
        ext.SetLocate(0, 400)
        tList.Add(ext)

        '文字の描画4、TypeFace→FormattedText→BuildGeometry→ここまで1と同じ
        'DrawingVisualのDrawingContextでDrawGeometryここまで3と同じ→RenderTargetBitmapにRender
        'Image作成→SourceにRenderTargetBitmapを指定
        'dv.Effect = New Effects.BlurEffect With {.Radius = 5} '全体をぼやかす
        Dim rtb As New RenderTargetBitmap(formattedText.Width, formattedText.Height, 96, 96, PixelFormats.Pbgra32)
        rtb.Render(dv)
        Dim ef As New Effects.BlurEffect With {.Radius = 5} 'bitmap全体をぼやかす
        Dim eff As New Effects.DropShadowEffect With {.BlurRadius = 5, .Color = Colors.Green}

        Dim img2 As New Image With {.Width = rtb.PixelWidth, .Height = rtb.PixelHeight, .Source = rtb, .Effect = eff}
        ext = New ExThumb(img2)
        AddHandler ext.PreviewMouseDown, AddressOf NowExThumb1_PreviewMouseDown 'クリックイベント
        MyCanvas.Children.Add(ext)
        ext.SetLocate(0, 500)
        tList.Add(ext)

        '文字の描画5、ぼかし
        Dim r As New Run(3)
        r.Typography.Variants = FontVariants.Subscript
        Dim pg As New Paragraph(r)

        'Dim bhg As Geometry = formattedText.BuildHighlightGeometry(New Point)

        '再描画に使えそう？
        'Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, Sub()
        '                                                                               MyCanvas.Children.Add(ext)
        '                                                                               Canvas.SetLeft(ext, 20)
        '                                                                           End Sub)



        Call InitializedBinding()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

        'NowExThumb1.SetLocate(0, 100)
    End Sub

    'Binding
    Private Sub InitializedBinding()
        'Dim b As Binding
        'b = New Binding With {
        '    .Source = NowExThumb1.MyRotateTransform,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay} 'ok

        'Angle1.SetBinding(Slider.ValueProperty, b)

        'DataContextでバインディングするようにした
        MyStackPanel.DataContext = NowExThumb1
        'angle
        BindingOperations.SetBinding(Angle1, Slider.ValueProperty, New Binding(NameOf(ExThumb.AngleNotify))) '"AngleNotify"))
        'left
        BindingOperations.SetBinding(sldLeft, Slider.ValueProperty, New Binding(NameOf(ExThumb.ExInLeft)))
        'top
        BindingOperations.SetBinding(sldTop, Slider.ValueProperty, New Binding(NameOf(ExThumb.ExInTop)))


        Dim b As New Binding With {.Path = New PropertyPath(NameOf(ExThumb.OutRect)),
            .StringFormat = "OutRect = {0:0.00}"} ' NameOf(ExThumb.OutRect)}
        BindingOperations.SetBinding(MyTextBlock6, TextBlock.TextProperty, b)
        BindingOperations.SetBinding(MyTextBlock7, TextBlock.TextProperty, New Binding(NameOf(ExThumb.OutRect)))
        BindingOperations.SetBinding(MyTextBlock4, TextBlock.TextProperty, New Binding(NameOf(ExThumb.OutLocate)))
    End Sub

    Private Sub BtnCheck_Click(sender As Object, e As RoutedEventArgs)
        Dim elm0 = tList.Item(0).ExElement
        'Dim r = NowExThumb1.MyRotateTransform.Angle
        'Dim ang = NowExThumb1.Angle
        'NowExThumb1.Angle += 5
        'NowExThumb1.MyRotateTransform.Angle += 5
        With NowExThumb1
            tbMyCanvasSize.Text = MyCanvas.RenderSize.ToString

            Dim outLeft = .ExOutLeft
            Dim elm = .ExElement

            '.AngleNotify += 10
            Dim l = .ExOutLeft
            '.OutsideLeft += 10
            MyTextBlock1.Text = $"OutsideLeft = { .ExOutLeft}"

            'MyTextBlock2.Text = $"GapLeft = { .GapLeft}"

            MyTextBlock3.Text = $"{NameOf(.ExInLeft)} ={ .ExInLeft}" ' {Canvas.GetLeft(NowExThumb1)}"

            MyTextBlock5.Text = $"{NameOf(.OutRect)} = { .OutRect:0.00}"


        End With
    End Sub

    Private Sub NowExThumb1_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) ' Handles NowExThumb1.PreviewMouseDown
        If Not sender.Equals(NowExThumb1) Then
            NowExThumb1 = sender
            Call InitializedBinding()

        End If
    End Sub

    'Thumbの座標をグリッドに合わせる、グリッドスナップ
    '0以下なら0に移動、0以上なら左上に移動
    Private Sub AdjustLocate()
        With NowExThumb1
            Dim x, y As Double : x = .OutRect.X : y = .OutRect.Y

            If x <= 0 Then
                '.ExInLeft = 0 - .GapLeft
                .ExInLeft = -(.OutRect.X - .ExInLeft)
            Else
                x = x Mod GridValue
                .ExInLeft -= x
            End If
            If y <= 0 Then
                '.ExInTop = 0 - .GapTop
                .ExInTop = -(.OutRect.Y - .ExInTop)
            Else
                y = y Mod GridValue
                .ExInTop -= y
            End If
        End With
    End Sub
    Private Sub BtnFixMyThumbLocate_Click(sender As Object, e As RoutedEventArgs)
        Call AdjustLocate()
    End Sub
    'ドラッグ移動
    Private Sub NowExThumb1_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles NowExThumb1.DragDelta
        With NowExThumb1
            '.SetLocate(.ExInLeft + e.HorizontalChange, .ExInTop + e.VerticalChange)

            'これはいまいち、移動量がグリッドの倍数のときだけ移動
            'ピッタリの倍数以外は移動しなくなるので追従性が悪い
            'Dim x, y As Double
            'If e.HorizontalChange Mod GridValue = 0 Then
            '    x = .ExInLeft + e.HorizontalChange
            '    .ExInLeft = x
            'End If
            'If e.VerticalChange Mod GridValue = 0 Then
            '    y = .ExInTop + e.VerticalChange
            '    .ExInTop = y
            'End If

            'Pixtack紫陽花と同じ方式、常に移動させようとして、移動距離からmodから得た端数を引くことでグリッドスナップ
            '追従性がいい
            Dim xf As Double = e.HorizontalChange Mod GridValue
            .ExInLeft += e.HorizontalChange - xf
            Dim yf As Double = e.VerticalChange Mod GridValue
            .ExInTop += e.VerticalChange - yf
        End With
    End Sub
    'ドラッグ移動開始時
    Private Sub NowExThumb1_DragStarted(sender As Object, e As DragStartedEventArgs) Handles NowExThumb1.DragStarted
        '見た目のRectを更新
        NowExThumb1.UpdateRect()
        'Thumbの座標をグリッドに合わせる、グリッドスナップ
        Call AdjustLocate()
    End Sub
    'MyCanvasサイズ修正
    Private Sub SetMyCanvasSize()
        Dim r As Rect = VisualTreeHelper.GetDescendantBounds(MyCanvas)
        MyCanvas.Width = r.Width : MyCanvas.Height = r.Height
    End Sub
    Private Sub btnFixMyCanvasSize_Click(sender As Object, e As RoutedEventArgs)
        Call SetMyCanvasSize()
    End Sub

    'MyCanvasに追加
    Private Sub AddElementToMyCanvas(elm As FrameworkElement, x As Double, y As Double)
        Dim ext As New ExThumb(elm)
        AddHandler ext.PreviewMouseDown, AddressOf NowExThumb1_PreviewMouseDown 'クリックイベント
        MyCanvas.Children.Add(ext)
        ext.SetLocate(x, y)
        tList.Add(ext)

    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click

    End Sub
End Class
