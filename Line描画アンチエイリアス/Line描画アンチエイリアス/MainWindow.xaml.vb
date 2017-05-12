'.NET TIPS： WPF/ UWP： コントロールのエッジをシャープに描画するには？［XAML］ - ＠IT
'http://www.atmarkit.co.jp/ait/articles/1602/17/news034.html


Class MainWindow
    Private MyRect As Path

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'Mode変更のボタン追加
        Call AddEdgeModeButton(MyStackP1)
        Call AddEdgeModeButton(MyStackP2)
        Call AddUselayoutRoundingButton(MyStackP1)
        Call AddUselayoutRoundingButton(MyStackP2)
        Call AddSnapsToDevicePixelsButton(MyStackP1)
        Call AddSnapsToDevicePixelsButton(MyStackP2)
        'Mode表示用のTextBlock追加
        Call AddTextBlock(MyStackP1)
        Call AddTextBlock(MyStackP2)

        'EdgeMode = Unspecified(初期設定)のLineを左のStackPanelに追加
        With MyStackP1.Children
            .Add(CreateLine(0.5, False)) '数値は線の太さ
            .Add(CreateLine(0.5, False))
            .Add(CreateLine(1.0, False))
            .Add(CreateLine(1.0, False))
            .Add(CreateLine(5.0, False))
            .Add(CreateLine(5.0, False))
            .Add(CreateRectangle(False))
            .Add(CreateEllipse(False))
        End With

        'EdgeMode = AliasedのLineを右のStackPanelに追加
        With MyStackP2.Children
            .Add(CreateLine(0.5, True))
            .Add(CreateLine(0.5, True))
            .Add(CreateLine(1.0, True))
            .Add(CreateLine(1.0, True))
            .Add(CreateLine(5.0, True))
            .Add(CreateLine(5.0, True))
            .Add(CreateRectangle(True))
            .Add(CreateEllipse(True))
        End With

    End Sub

    '図形作成
    'Line作成
    Private Function CreateLine(bold As Double, a As Boolean) As Line
        Dim l As New Line
        With l
            .Stroke = Brushes.Black
            .StrokeThickness = bold
            .X1 = 0 : .X2 = 150 : .Y1 = 0 : .Y2 = 10
        End With
        If a Then
            RenderOptions.SetEdgeMode(l, EdgeMode.Aliased)
        End If
        Return l
    End Function
    'Rectangle
    Private Function CreateRectangle(a As Boolean) As Rectangle
        Dim r As New Rectangle With {.Width = 50, .Height = 50,
            .Fill = Brushes.Black}
        If a Then RenderOptions.SetEdgeMode(r, EdgeMode.Aliased)
        Return r
    End Function
    'Ellipse
    Private Function CreateEllipse(a As Boolean) As Ellipse
        Dim el As New Ellipse With {.Width = 50, .Height = 50,
            .Fill = Brushes.Black}
        If a Then RenderOptions.SetEdgeMode(el, EdgeMode.Aliased)
        Return el
    End Function

    'ボタン作成
    'EdgeMode切り替えボタン作成
    Private Sub AddEdgeModeButton(p As StackPanel)
        Dim b As New Button With {.Content = "EdgeMode変更",
            .Margin = New Thickness(2)}
        p.Children.Add(b)
        'ボタンクリックイベントにくっつける
        AddHandler b.Click, AddressOf ChangeEdgeMode
    End Sub
    'UseLayoutRounding変更ボタン作成
    Private Sub AddUselayoutRoundingButton(p As StackPanel)
        Dim b As New Button With {.Content = "UseLayoutRounding変更",
            .Margin = New Thickness(2)}
        p.Children.Add(b)
        AddHandler b.Click, AddressOf ChangeUseLayoutRounding
    End Sub
    'SnapsToDevicePixels変更ボタン作成
    Private Sub AddSnapsToDevicePixelsButton(p As StackPanel)
        Dim b As New Button With {.Content = "SnapsToDevicePixels変更",
            .Margin = New Thickness(2)}
        p.Children.Add(b)
        AddHandler b.Click, AddressOf ChangeSnapsToDevicePixels
    End Sub


    'ボタンクリックイベント用
    '親パネルのRenderOptions.EdgeModeを切り替える
    Private Sub ChangeEdgeMode(sender As Object, e As RoutedEventArgs)
        Dim p As StackPanel = DirectCast(sender.parent, StackPanel)
        If RenderOptions.GetEdgeMode(p) = EdgeMode.Aliased Then
            RenderOptions.SetEdgeMode(p, EdgeMode.Unspecified)
        Else
            RenderOptions.SetEdgeMode(p, EdgeMode.Aliased)
        End If
    End Sub
    '親パネルのUseLayoutRoundingを切り替える
    Private Sub ChangeUseLayoutRounding(sender As Object, e As RoutedEventArgs)
        Dim p As StackPanel = DirectCast(sender.parent, StackPanel)
        p.UseLayoutRounding = Not p.UseLayoutRounding
    End Sub
    '親パネルのSnapsToDevicePixelsを切り替える
    Private Sub ChangeSnapsToDevicePixels(sender As Object, e As RoutedEventArgs)
        Dim p As StackPanel = DirectCast(sender.parent, StackPanel)
        p.SnapsToDevicePixels = Not p.SnapsToDevicePixels
    End Sub



    '    文字列をフォーマットして表示するはなし with 多言語対応とかマークアップ拡張とか - Qiita
    'http://qiita.com/wonderful_panda/items/a45ffaaca7f9c6e0d494

    'Mode表示用TextBlock作成
    Private Sub AddTextBlock(p As StackPanel)
        'Bindingソース作成、StackPanelのEdgeModePropertyをソースにする
        Dim b As New Binding With {
            .Source = p,
            .Path = New PropertyPath(RenderOptions.EdgeModeProperty),
            .StringFormat = "PanelのEdgeMode = {0}"
        }

        Dim tb As New TextBlock
        'TextBlockのTextPropertyにBinding
        tb.SetBinding(TextBlock.TextProperty, b)
        p.Children.Add(tb)

        'UseLayoutRounding表示用TextBlock
        tb = New TextBlock
        b = New Binding With {
            .Source = p,
            .Path = New PropertyPath(StackPanel.UseLayoutRoundingProperty),
            .StringFormat = "PanelのUseLayoutRounding = {0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
        p.Children.Add(tb)

        'SnapToDevicePixels表示用TextBlock
        tb = New TextBlock
        b = New Binding With {
            .Source = p,
            .Path = New PropertyPath(StackPanel.SnapsToDevicePixelsProperty),
            .StringFormat = "PanelのSnapToDevicePixels = {0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
        p.Children.Add(tb)
    End Sub
End Class
