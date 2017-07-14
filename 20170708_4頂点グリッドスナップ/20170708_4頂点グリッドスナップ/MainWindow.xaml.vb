'WPF、変形後の要素の4辺をグリッドスナップしながらドラッグ移動 ( ソフトウェア ) - 午後わてんのブログ - Yahoo!ブログ
'https://blogs.yahoo.co.jp/gogowaten/15011638.html


Imports System.ComponentModel
Imports System.Globalization
Imports System.Windows.Controls.Primitives


Class MainWindow
    Private WithEvents MyExThumb As ExThumb

    'Canvasにグリッド(罫線)表示
    Private Sub SetGridLine()
        Dim gSize As Integer = sldGrid.Value
        Dim w As Double = MyCanvas.Width
        Dim h As Double = MyCanvas.Height
        Dim whMax As Integer = IIf(w > h, w, h)
        Dim pFigure As PathFigure
        Dim pGeometry As New PathGeometry

        For i As Integer = 0 To whMax / gSize
            '横線
            pFigure = New PathFigure With {.StartPoint = New Point(0, i * gSize)}
            pFigure.Segments.Add(New LineSegment(New Point(whMax, i * gSize), True))
            pGeometry.Figures.Add(pFigure)
            '縦線
            pFigure = New PathFigure With {.StartPoint = New Point(i * gSize, 0)}
            pFigure.Segments.Add(New LineSegment(New Point(i * gSize, whMax), True))
            pGeometry.Figures.Add(pFigure)
        Next

        With GridLine
            .Data = pGeometry
            .Stroke = Brushes.Cyan
            .StrokeThickness = 2.0
            .Opacity = 0.3
        End With
    End Sub

    Private Sub MyCheck()
        With MyExThumb
            .MyAngle = 20
            .MyLeft = 50
            .MyTop = 50
        End With
        Dim d = 119 \ 10
        Dim root = MyExThumb.testRootCanvas
    End Sub
    Private Sub MyCheck2()
        MyExThumb.SetPoint2(100, 100)
    End Sub
    Private Sub MyMove()
        MyExThumb.MyLeft = 100
        MyExThumb.MyTop = 100
    End Sub

    '数値確認用のTextBlockへのBinding
    Private Sub SetTextBlockBinding(so As Object, sName As String, tb As TextBlock)
        Dim b As New Binding(sName) With {.Source = so, .StringFormat = sName & " = {0:0.00}"}
        tb.SetBinding(TextBlock.TextProperty, b)
    End Sub

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized


        'グリッドサイズ指定
        sldGrid.Value = 50
        'グリッド罫線表示
        Call SetGridLine()
        pathInBounds.Visibility = Visibility.Visible
        AddHandler btnCheck.Click, AddressOf MyCheck
        AddHandler btn2.Click, AddressOf MyCheck2
        AddHandler btn4.Click, AddressOf MyMove
        AddHandler sldGrid.ValueChanged, AddressOf SetGridLine 'スライダーの値変更でグリッド(罫線)の表示更新

        'ExThumbに100x100の赤Borderを追加してMyCanvasに表示
        Dim ext As New ExThumb(New Border With {
                               .Width = 100, .Height = 100, .Background = Brushes.Orange, .Opacity = 0.7})
        Canvas.SetLeft(ext, 0) : Canvas.SetTop(ext, 0)
        MyCanvas.Children.Add(ext)
        MyExThumb = ext

        '回転角度をSliderにBinding
        Dim b As Binding
        b = New Binding(NameOf(ExThumb.MyAngle)) With {.Source = MyExThumb, .Mode = BindingMode.TwoWay}
        sldAngle.SetBinding(Slider.ValueProperty, b)

        '数値確認用のTextBlockへのBinding
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyAngle), tbAngle) '角度
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyDiffPoint), tbRect) '差分座標
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyLeft), tbLeft) '実際のX座標
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyOutBounds), tbBounds) '見た目のピッタリ枠
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyTop), tbTop)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyTransformedTopLeft), tbTTopLeft)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyTransformedTopRight), tbTTopRight)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyTransformedBottomRight), tbTBottomRight)
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyTransformedBottomLeft), tbTBottomLeft)


        '目印の青枠
        'ぴったり枠のRectを青枠のDataにバインディング
        '値はRectからRectangleGeometryに変換する必要があるのでコンバータ使用
        b = New Binding(NameOf(ExThumb.MyOutBounds)) With {
            .Source = MyExThumb, .Converter = New MyConverterRect}
        pathOutBounds.SetBinding(Path.DataProperty, b)

        '赤枠サイズバインディング
        b = New Binding(NameOf(ExThumb.MyInBounds)) With {
            .Source = MyExThumb, .Converter = New MyConverterRect}
        pathInBounds.SetBinding(Path.DataProperty, b)

        '青枠表示チェックボックスを青枠PathのVisibilityにバインディング
        'チェックボックスのIsCheckedをVisibilityに変換するコンバータ使用
        b = New Binding With {.Source = cbVisibleOutBounds,
            .Path = New PropertyPath(CheckBox.IsCheckedProperty),
            .Converter = New MyConverterVisibility}
        pathOutBounds.SetBinding(VisibilityProperty, b)

        '赤枠表示の有無
        b = New Binding With {
            .Source = cbVisibleInBounds,
            .Path = New PropertyPath(CheckBox.IsCheckedProperty),
            .Converter = New MyConverterVisibility}
        pathInBounds.SetBinding(VisibilityProperty, b)

        'グリッドライン表示の有無
        b = New Binding With {
            .Source = cbVisibleGridLine,
            .Path = New PropertyPath(CheckBox.IsCheckedProperty),
            .Converter = New MyConverterVisibility}
        GridLine.SetBinding(VisibilityProperty, b)

    End Sub

#Region "主にぴったり枠グリッドスナップ用"

    '方向判定、目標グリッドが移動方向と逆なら移動しない
    Private Function IsDirection(mMove As Double, targetGrid As Integer, locateX As Double) As Boolean
        If targetGrid - locateX > 0 And mMove > 0 Then
            Return True
        ElseIf targetGrid - locateX < 0 And mMove < 0 Then
            Return True
        End If
        Return False
    End Function
    '距離判定、マウスの移動距離が目標グリッドまで届いていなければ移動しない
    Private Function IsDistance(mMove As Double, targetGrid As Integer, locateX As Double) As Boolean
        If Math.Abs(mMove) >= Math.Abs(targetGrid - locateX) Then
            Return True
        End If
        Return False
    End Function
    '小数点を切り捨て、切り上げ、四捨五入する: .NET Tips: C#, VB.NET
    'http://dobon.net/vb/dotnet/programing/round.html
    ''' <summary>
    ''' 目標グリッドの座標を返す
    ''' </summary>
    ''' <param name="sideLocate">辺の座標</param>
    ''' <param name="mMove">マウスの移動距離</param>
    ''' <param name="gridSize"></param>
    ''' <returns></returns>
    Private Function GetTargetGridLocate(sideLocate As Double, mMove As Double, gridSize As Integer) As Integer
        Dim targetGrid As Integer
        Dim va As Double = (sideLocate + mMove) / gridSize
        '右か下移動なら負へ丸める、左か上なら正へ丸める
        If mMove > 0 Then
            targetGrid = Math.Floor(va) '負へ丸める
        Else
            targetGrid = Math.Ceiling(va) '正へ丸める
        End If
        Return targetGrid * gridSize '目標グリッドの座標
    End Function

    ''' <summary>
    ''' マウスの移動距離と今の位置とグリッドサイズからセットすべき座標を返す
    ''' </summary>
    ''' <param name="mMove">マウスの移動距離</param>
    ''' <param name="gridSize">グリッドサイズ</param>
    ''' <param name="oTopOrLeft">元の位置、上下移動なら上位置、左右移動なら左位置</param>
    ''' <param name="oBottomOrRight">元の位置、上下移動なら下位置、左右移動なら右位置</param>
    ''' <param name="oWidthOrHeight">ぴったり枠のサイズ、上下移動ならHeight、左右移動ならWidth</param>
    ''' <returns></returns>
    Private Function GetTargetSideLocate(mMove As Double, gridSize As Integer,
                                         oTopOrLeft As Double, oBottomOrRight As Double, oWidthOrHeight As Double) As Double
        '目標グリッドの座標
        Dim topLeftSideTargetGrid As Integer = GetTargetGridLocate(oTopOrLeft, mMove, gridSize)

        '方向判定、目標グリッドが移動方向と逆なら移動しない
        Dim isTopLeft方向 As Boolean = IsDirection(mMove, topLeftSideTargetGrid, oTopOrLeft)

        '距離判定、マウスの移動距離が目標グリッドまで届いていなければ移動しない
        Dim isTopLeft距離 As Boolean = IsDistance(mMove, topLeftSideTargetGrid, oTopOrLeft)

        '左辺判定、上辺判定
        Dim isTopLeft As Boolean = False
        If isTopLeft方向 And isTopLeft距離 Then isTopLeft = True


        '右辺、下辺
        Dim bottomRightSideTargetGrid As Integer = GetTargetGridLocate(oBottomOrRight, mMove, gridSize)
        Dim isBottomRight方向 As Boolean = IsDirection(mMove, bottomRightSideTargetGrid, oBottomOrRight)
        Dim isBottomRight距離 As Boolean = IsDistance(mMove, bottomRightSideTargetGrid, oBottomOrRight)
        Dim isBottomRight As Boolean = False
        If isBottomRight方向 And isBottomRight距離 Then isBottomRight = True


        '両辺を判定
        Dim targetLocate As Double
        If isTopLeft = False AndAlso isBottomRight = False Then
            '移動しない場合は元の位置を返す
            targetLocate = oTopOrLeft
        ElseIf isTopLeft AndAlso isBottomRight Then
            '目標グリッドまでの距離が近い辺を選択
            Dim l As Double = Math.Abs(topLeftSideTargetGrid - (oTopOrLeft + mMove))
            Dim r As Double = Math.Abs(bottomRightSideTargetGrid - (oBottomOrRight + mMove))
            If l > r Then
                targetLocate = bottomRightSideTargetGrid - oWidthOrHeight
            Else
                targetLocate = topLeftSideTargetGrid
            End If
        ElseIf isTopLeft Then
            targetLocate = topLeftSideTargetGrid
        Else
            targetLocate = bottomRightSideTargetGrid - oWidthOrHeight
        End If

        '指定する座標を返す
        Return targetLocate

    End Function
#End Region

#Region "元の4頂点＆4辺グリッドスナップ用"
    ''' <summary>
    ''' 元の4頂点＆4辺グリッドスナップ用
    ''' </summary>
    ''' <param name="gridSize">グリッドサイズ</param>
    ''' <param name="mMove">マウスの移動距離</param>
    ''' <param name="originalLocate">変形前の左上の位置</param>
    ''' <param name="TopLeftXY">変形後の左上の位置</param>
    ''' <param name="TopRightXY">変形後の右上の位置</param>
    ''' <param name="BottomRightXY">変形後の右下の位置</param>
    ''' <param name="BottomLeftXY">変形後の左下の位置</param>
    ''' <returns></returns>
    Private Function GetMyTargetGrid4Point4Side(gridSize As Integer, mMove As Double, originalLocate As Double,
                          TopLeftXY As Double, TopRightXY As Double, BottomRightXY As Double, BottomLeftXY As Double) As MyTargetGridData
        '各4辺の目標グリッド、距離、有効判定、指定座標を取得
        Dim TopLTargetGrid As MyTargetGridData = GetMyTargetGrid(gridSize, mMove, TopLeftXY, originalLocate)
        Dim TopRTargetGrid As MyTargetGridData = GetMyTargetGrid(gridSize, mMove, TopRightXY, originalLocate)
        Dim BotRTargetGrid As MyTargetGridData = GetMyTargetGrid(gridSize, mMove, BottomRightXY, originalLocate)
        Dim BotLTargetGrid As MyTargetGridData = GetMyTargetGrid(gridSize, mMove, BottomLeftXY, originalLocate)

        '一旦リストに入れる
        Dim dataList As New List(Of MyTargetGridData) From {TopLTargetGrid, TopRTargetGrid, BotRTargetGrid, BotLTargetGrid}

        'SortedListに入れて並べ替える、キーに距離を指定
        Dim sl As New SortedList(Of Double, MyTargetGridData)
        '同じ値のkeyは追加できないのでtry
        For i As Integer = 0 To 3
            Try
                sl.Add(dataList(i).Distance, dataList(i))
            Catch ex As Exception

            End Try
        Next

        '一番近いものを取得、有効なものだけを大きい(遠い)方から入れていって上書きしていく
        Dim myTargetGrid As MyTargetGridData
        For i As Integer = sl.Count - 1 To 0 Step -1
            If sl.Values(i).IsValid = True Then
                myTargetGrid = sl.Values(i)
            End If
        Next
        Return myTargetGrid
    End Function
    Private Function GetMyTargetGrid(gridSize As Integer, mMove As Double, pointLocate As Double, originalLocate As Double) As MyTargetGridData
        Dim myTargetGrid As New MyTargetGridData
        Dim tGrid As Integer = GetTargetGridLocate(pointLocate, mMove, gridSize)
        With myTargetGrid
            '距離、方向ともに有効なら有効にする
            '判定結果がTrue And Trueの場合Trueになる
            .IsValid = IsDistance(mMove, tGrid, pointLocate) And IsDirection(mMove, tGrid, pointLocate)
            .Distance = Math.Abs((pointLocate + mMove) - tGrid)
            .TargetLocate = tGrid
            '指定する位置
            .SetLocate = tGrid - (pointLocate - originalLocate)
        End With
        Return myTargetGrid
    End Function

#End Region


#Region "元の4頂点グリッドスナップ用"

    '指定位置から一番近いグリッド位置を返す
    Private Function GetNearGridPointDistance(xy As Double, gridSize As Integer) As Double
        Dim m As Double = xy Mod gridSize
        If m > gridSize / 2 Then
            Return xy + gridSize - m
        Else
            Return xy - m
        End If
    End Function
    '指定座標から一番近いグリッド頂点の座標を返す
    Private Function GetNearestGridPoint(dp As Point, gridSize As Integer) As Point
        Dim x As Double = GetNearGridPointDistance(dp.X, gridSize) '最寄りのグリッドx座標
        Dim y As Double = GetNearGridPointDistance(dp.Y, gridSize) 'y座標
        Return New Point(x, y)
    End Function
    '2点間の距離を返す
    Private Function GetDistance(p1 As Point, p2 As Point) As Double
        Dim x As Double = p1.X - p2.X
        Dim y As Double = p1.Y - p2.Y
        Dim rd As Double = Math.Sqrt(x ^ 2 + y ^ 2)
        Return rd
    End Function
    ''' <summary>
    ''' 移動後の座標から一番近いグリッド頂点や距離、移動前からの距離などを返す
    ''' </summary>
    ''' <param name="gridSize">グリッドサイズ</param>
    ''' <param name="transformedPoint">移動前の座標</param>
    ''' <param name="mMove">マウスの移動座標</param>
    ''' <returns></returns>
    Private Function GetPointData(gridSize As Integer, transformedPoint As Point, mMove As Point) As MyTargetGridPointData
        Dim tPoint As New MyTargetGridPointData
        With tPoint
            '移動後地点の最寄りのグリッド頂点
            .TargetGridPoint = GetNearestGridPoint(transformedPoint + mMove, gridSize)
            'その距離
            .Distance = GetDistance(transformedPoint + mMove, .TargetGridPoint)
            '今回は未使用、値が有効かどうか
            .IsValid = True
            '移動前の地点から最寄りのグリッド頂点までの差分
            .DiffPoint = transformedPoint - .TargetGridPoint

        End With
        Return tPoint
    End Function
    ''' <summary>
    ''' 元の4頂点グリッドスナップ用、4頂点の最寄りのグリッド頂点を取得、一番近いグリッド頂点情報を返す
    ''' </summary>
    ''' <param name="gridSize">グリッドサイズ</param>
    ''' <param name="mMove">マウスの移動距離</param>
    ''' <param name="TopLeftXY">変形後の左上の位置</param>
    ''' <param name="TopRightXY">変形後の右上の位置</param>
    ''' <param name="BottomRightXY">変形後の右下の位置</param>
    ''' <param name="BottomLeftXY">変形後の左下の位置</param>
    ''' <returns></returns>
    Private Function GetMyTargetGrid4Point2(gridSize As Integer, mMove As Point,
                          TopLeftXY As Point, TopRightXY As Point, BottomRightXY As Point, BottomLeftXY As Point) As MyTargetGridPointData
        '移動後の各4頂点の最寄りのグリッド頂点のData取得
        Dim TLData As MyTargetGridPointData = GetPointData(gridSize, TopLeftXY, mMove)
        Dim TRData As MyTargetGridPointData = GetPointData(gridSize, TopRightXY, mMove)
        Dim BRData As MyTargetGridPointData = GetPointData(gridSize, BottomRightXY, mMove)
        Dim BLData As MyTargetGridPointData = GetPointData(gridSize, BottomLeftXY, mMove)
        '一番近いグリッド頂点を取得する
        '4データを一旦リストに入れる
        Dim dataList As New List(Of MyTargetGridPointData) From {TLData, TRData, BRData, BLData}

        'SortedListに入れて並べ替える、距離をkeyにして昇順
        Dim sl As New SortedList(Of Double, MyTargetGridPointData)
        '同じ距離があった場合にリストに追加しようとするとエラーになるのでtryで無視して次のDataを入れていく
        For i As Integer = 0 To 3
            Try
                sl.Add(dataList(i).Distance, dataList(i))
            Catch ex As Exception

            End Try
        Next
        'Dim rv As MyTargetGridPointData = sl.Values(0)
        '一番近いDataを返す
        Return sl.Values(0)

    End Function


#End Region


    'マウスドラッグ移動
    Private Sub MyExThumb_DragDelta(sender As Object,
                                    e As DragDeltaEventArgs) Handles MyExThumb.DragDelta
        Dim GridSize As Integer = sldGrid.Value
        Dim hChange As Double = e.HorizontalChange
        Dim vChange As Double = e.VerticalChange


        ''グリッドスナップ移動
        Dim x, y As Double
        Dim xx, yy, xxx, yyy As Integer
        'Dim outTop As Double = MyExThumb.MyOutBounds.Top
        'Dim outLeft As Double = MyExThumb.MyOutBounds.Left
        'Dim outBottom As Double = MyExThumb.MyOutBounds.Bottom
        'Dim outRight As Double = MyExThumb.MyOutBounds.Right

        With MyExThumb
            Select Case True
                Case rbNone.IsChecked
                    '通常移動
                    MyExThumb.MyLeft += e.HorizontalChange
                    MyExThumb.MyTop += e.VerticalChange

                Case rbNormal.IsChecked
                    '変形前の左上を基準、赤枠

                    x = .MyLeft + hChange
                    y = .MyTop + vChange
                    xx = x \ GridSize : yy = y \ GridSize
                    xxx = xx * GridSize : yyy = yy * GridSize
                    .MyLeft = xxx : .MyTop = yyy

                Case rbFitFrame.IsChecked
                    'ぴったり枠の左上を基準、青枠(OutBounds)

                    x = .MyLeft + hChange + .MyDiffPoint.X
                    y = .MyTop + vChange + .MyDiffPoint.Y
                    xx = x \ GridSize : yy = y \ GridSize
                    xxx = xx * GridSize : yyy = yy * GridSize
                    .SetPoint2(xxx, yyy)

                Case rbFitFrame4辺.IsChecked 'ぴったり枠の4辺を基準
                    '縦移動
                    Dim topPoint As Double = GetTargetSideLocate(vChange, GridSize, MyExThumb.MyOutBounds.Top, MyExThumb.MyOutBounds.Bottom, MyExThumb.MyOutBounds.Height)
                    If topPoint <> MyExThumb.MyOutBounds.Top Then
                        MyExThumb.SetOutBoundTop(topPoint)
                    End If

                    '横移動
                    Dim lPoint As Double = GetTargetSideLocate(hChange, GridSize,
                                                               MyExThumb.MyOutBounds.Left,
                                                               MyExThumb.MyOutBounds.Right,
                                                               MyExThumb.MyOutBounds.Width)
                    If lPoint <> MyExThumb.MyOutBounds.Left Then
                        MyExThumb.SetOutBoundLeft(lPoint)
                    End If

                Case rbFitPoint4頂点.IsChecked
                    '変形後の元の4頂点をグリッドの頂点にスナップ
                    '4頂点それぞれの最寄りのグリッド頂点の中から一番近いグリッド頂点やその距離の差分などを取得
                    Dim pData As MyTargetGridPointData = GetMyTargetGrid4Point2(GridSize, New Point(hChange, vChange),
                                                                               MyExThumb.MyTransformedTopLeft,
                                                                               MyExThumb.MyTransformedTopRight,
                                                                               MyExThumb.MyTransformedBottomRight,
                                                                               MyExThumb.MyTransformedBottomLeft)
                    '今と違う位置(距離の差分が0以外)ならその場所へ移動
                    If pData.IsValid Then
                        If pData.DiffPoint.X <> 0 Then MyExThumb.MyLeft -= pData.DiffPoint.X
                        If pData.DiffPoint.Y <> 0 Then MyExThumb.MyTop -= pData.DiffPoint.Y
                    End If

                Case rbFitPoint4頂点と4辺.IsChecked
                    '変形後の元の4頂点とぴったり枠の4辺のどちらでもスナップ
                    'つまり上の2つの処理を合わせた感じの処理になる
                    '縦移動
                    With MyExThumb
                        Dim myTargetGridY As MyTargetGridData =
                            GetMyTargetGrid4Point4Side(GridSize, vChange, .MyTop,
                                                       .MyTransformedTopLeft.Y, .MyTransformedTopRight.Y,
                                                       .MyTransformedBottomRight.Y, .MyTransformedBottomLeft.Y)
                        If myTargetGridY.IsValid Then
                            .MyTop = myTargetGridY.SetLocate
                        End If
                    End With

                    '横移動
                    Dim myTargetGridX As MyTargetGridData = GetMyTargetGrid4Point4Side(GridSize, hChange, MyExThumb.MyLeft, MyExThumb.MyTransformedTopLeft.X,
                                                          MyExThumb.MyTransformedTopRight.X,
                                                          MyExThumb.MyTransformedBottomRight.X,
                                                          MyExThumb.MyTransformedBottomLeft.X)
                    If myTargetGridX.IsValid Then
                        MyExThumb.MyLeft = myTargetGridX.SetLocate
                    End If

                Case rbTopLeft.IsChecked
                    '変形で移動した元左上座標を基準

                    x = .MyLeft + hChange + .MyDiffPointTopLeft.X
                    y = .MyTop + vChange + .MyDiffPointTopLeft.Y
                    xx = x \ GridSize : yy = y \ GridSize
                    xxx = xx * GridSize : yyy = yy * GridSize
                    .SetPoint3(xxx, yyy)
            End Select
        End With

        '四隅の一番近い
        '左上に合わせる場合
        '115で右に1動いたら116、116/10＝11.6の切り捨て11*10=100に合わせる
        'マウス進行方向のグリッドに合わせる場合
        '116/10=11.6の切り上げ12*10=120に合わせる


        '目印の移動
        ''変形で移動した元左上座標
        'Canvas.SetLeft(Line1, xxx)
        'Canvas.SetTop(Line1, yyy)

    End Sub

    'ドラッグ中はマウスカーソルを手の形に
    Private Sub MyExThumb_DragStarted(sender As Object, e As DragStartedEventArgs) Handles MyExThumb.DragStarted
        MyExThumb.Cursor = Cursors.Hand
    End Sub
    'ドラッグ終了で元のマウスカーソル
    Private Sub MyExThumb_DragCompleted(sender As Object, e As DragCompletedEventArgs) Handles MyExThumb.DragCompleted
        MyExThumb.Cursor = Cursors.Arrow
    End Sub
End Class


'////////////////////////////////////////////////////////////////////////////////////////////////////


Public Class ExThumb
    Inherits Thumb 'Thumbを継承
    Implements ComponentModel.INotifyPropertyChanged '通知プロパティ用
    Private RootCanvas As Canvas
    Private RootRotate As RotateTransform
    Public testRootCanvas As Canvas

    'OutBoundsの左上座標を指定
    Public Sub SetPoint2(x As Double, y As Double)
        MyLeft = x + (-MyDiffPoint.X)
        MyTop = y + (-MyDiffPoint.Y)
    End Sub
    Public Sub SetPoint3(x As Double, y As Double)
        MyLeft = x + (-MyDiffPointTopLeft.X)
        MyTop = y + (-MyDiffPointTopLeft.Y)
    End Sub
    Public Sub SetOutBoundLeft(x As Double)
        MyLeft = x + (-MyDiffPoint.X)
    End Sub
    Public Sub SetOutBoundTop(y As Double)
        MyTop = y + (-MyDiffPoint.Y)
    End Sub
    'DiffPointとOutBoundsの更新
    '変形時に実行する
    Private Sub SetDiffPointAndOutSize()
        Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(RootCanvas.Width, RootCanvas.Height)))
        MyDiffPoint = r.Location 'ピッタリ座標
        MyOutSize = r.Size 'ぴったりサイズ
        '元の4頂点と変形後の4頂点の差分取得
        MyDiffPointTopLeft = gt.Transform(New Point(0, 0)) '元左上差分
        MyDiffPointTopRight = gt.Transform(New Point(RootCanvas.Width, 0)) '右上
        MyDiffPointBottomRight = gt.Transform(New Point(RootCanvas.Width, RootCanvas.Height)) '右下
        MyDiffPointBottomLeft = gt.Transform(New Point(0, RootCanvas.Height)) '左下

        Call SetOutBounds()

        '未使用
        Dim p1 As Point = gt.Transform(New Point) '変形前後の左上座標の差分
        Dim p2 As Point = gt.Transform(New Point(RootCanvas.Width, 0)) '右上
        Dim p3 As Point = gt.Transform(New Point(RootCanvas.Width, RootCanvas.Height)) '右下
        Dim p4 As Point = gt.Transform(New Point(0, RootCanvas.Height)) '左下
    End Sub

    'OutBoundsとTransformedTopLeftの更新、
    '移動時はこちらだけ実行する
    '変形時はDiffだけ
    Private Sub SetOutBounds()
        'Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = New Rect(New Point(MyDiffPoint.X + MyLeft, MyDiffPoint.Y + MyTop), MyOutSize)
        MyOutBounds = r '外枠更新
        Dim p As New Point(MyLeft, MyTop)
        MyTransformedTopLeft = p + MyDiffPointTopLeft '左上更新 ┏
        MyTransformedTopRight = p + MyDiffPointTopRight '右上    ┓
        MyTransformedBottomRight = p + MyDiffPointBottomRight   '┛
        MyTransformedBottomLeft = p + MyDiffPointBottomLeft     '┗
    End Sub

    'InBounds更新、移動時に実行する
    Private Sub SetInBounds()
        MyInBounds = New Rect(New Point(MyLeft, MyTop), MyInBounds.Size)
    End Sub



#Region "Property"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(<System.Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#Region "回転角度とx,y座標"
    '回転角度
    Private Property _MyAngle As Double
    Public Property MyAngle As Double
        Get
            Return _MyAngle
        End Get
        Set(value As Double)
            If value <> _MyAngle Then
                _MyAngle = value
                RootRotate.Angle = value
                Call NotifyPropertyChanged()
                Call SetDiffPointAndOutSize()
            End If
        End Set
    End Property
    'X座標
    Private Property _MyLeft As Double
    Public Property MyLeft As Double
        Get
            Return _MyLeft
        End Get
        Set(value As Double)
            If value <> _MyLeft Then
                _MyLeft = value
                Canvas.SetLeft(Me, value)
                Call NotifyPropertyChanged()
                Call SetOutBounds()
                Call SetInBounds()
            End If
        End Set
    End Property
    'Y座標
    Private Property _MyTop As Double
    Public Property MyTop As Double
        Get
            Return _MyTop
        End Get
        Set(value As Double)
            If value <> _MyTop Then
                _MyTop = value
                Canvas.SetTop(Me, value)
                Call NotifyPropertyChanged()
                Call SetOutBounds()
                Call SetInBounds()
            End If
        End Set
    End Property
#End Region


#Region "変形後のRectと元のRect"
    '変形前後のぴったり枠の左上座標の差分
    Private Property _MyDiffPoint As Point
    Public Property MyDiffPoint As Point
        Get
            Return _MyDiffPoint
        End Get
        Set(value As Point)
            _MyDiffPoint = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '変形後の要素がピッタリ収まるサイズ
    Private Property _MyOutSize As Size
    Public Property MyOutSize As Size
        Get
            Return _MyOutSize
        End Get
        Set(value As Size)
            _MyOutSize = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '変形後の要素がピッタリ収まる四角枠
    Private Property _MyOutBounds As Rect
    Public Property MyOutBounds As Rect
        Get
            Return _MyOutBounds
        End Get
        Set(value As Rect)
            _MyOutBounds = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '元のRect
    Private Property _MyInBounds As Rect
    Public Property MyInBounds As Rect
        Get
            Return _MyInBounds
        End Get
        Set(value As Rect)
            _MyInBounds = value
            Call NotifyPropertyChanged()
        End Set
    End Property
#End Region

#Region "元の四隅座標用、左上以外はいらないかも"
    '変形前後の元左上座標の差分
    Private Property _MyDiffPointTopLeft As Point
    Public Property MyDiffPointTopLeft As Point
        Get
            Return _MyDiffPointTopLeft
        End Get
        Set(value As Point)
            _MyDiffPointTopLeft = value
        End Set
    End Property
    '変形後の元左上座標
    Private Property _MyTransformedTopLeft As Point
    Public Property MyTransformedTopLeft As Point
        Get
            Return _MyTransformedTopLeft
        End Get
        Set(value As Point)
            _MyTransformedTopLeft = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '変形前後の元右上座標の差分
    Private Property _MyDiffPointTopRight As Point
    Public Property MyDiffPointTopRight As Point
        Get
            Return _MyDiffPointTopRight
        End Get
        Set(value As Point)
            _MyDiffPointTopRight = value
        End Set
    End Property
    '変形後の元右上座標
    Private Property _MyTransformedTopRight As Point
    Public Property MyTransformedTopRight As Point
        Get
            Return _MyTransformedTopRight
        End Get
        Set(value As Point)
            _MyTransformedTopRight = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '変形前後の元右下座標の差分
    Private Property _MyDiffPointBottomRight As Point
    Public Property MyDiffPointBottomRight As Point
        Get
            Return _MyDiffPointBottomRight
        End Get
        Set(value As Point)
            _MyDiffPointBottomRight = value
        End Set
    End Property
    '変形後の元右下座標
    Private Property _MyTransformedBottomRight As Point
    Public Property MyTransformedBottomRight As Point
        Get
            Return _MyTransformedBottomRight
        End Get
        Set(value As Point)
            _MyTransformedBottomRight = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '変形前後の元左下座標の差分
    Private Property _MyDiffPointBottomLeft As Point
    Public Property MyDiffPointBottomLeft As Point
        Get
            Return _MyDiffPointBottomLeft
        End Get
        Set(value As Point)
            _MyDiffPointBottomLeft = value
        End Set
    End Property
    '変形後の元左下座標
    Private Property _MyTransformedBottomLeft As Point
    Public Property MyTransformedBottomLeft As Point
        Get
            Return _MyTransformedBottomLeft
        End Get
        Set(value As Point)
            _MyTransformedBottomLeft = value
            Call NotifyPropertyChanged()
        End Set
    End Property
#End Region

#End Region


    'ControlTemplate作成、Canvasを一個入れるだけ
    Private Function CreateTemplate() As ControlTemplate
        Dim ct As New ControlTemplate(GetType(Thumb))
        Dim c As New FrameworkElementFactory With {.Name = "RootCanvas", .Type = GetType(Canvas)}
        ct.VisualTree = c
        Return ct
    End Function

    'コンストラクタ
    '渡された要素をTemplateの中のCanvasに追加する
    Public Sub New(elm As FrameworkElement)
        Template = CreateTemplate()
        ApplyTemplate() 'Templateを再構築、必要
        'TemplateのCanvasを取得して渡された要素を追加
        RootCanvas = Me.Template.FindName("RootCanvas", Me)
        With RootCanvas
            .Children.Add(elm) 'TemplateのCanvasに追加
            .Children.Add(New Label With {.Content = "左上"}) '目印用にLabelを追加
            .Height = elm.Height
            .Width = elm.Width
        End With
        MyInBounds = New Rect(0, 0, elm.Width, elm.Height)

        testRootCanvas = RootCanvas 'test

        '各種TransformをGroupにしてTemplateのCanvasのRenderTransformに指定
        RootRotate = New RotateTransform
        Dim sc As New ScaleTransform
        Dim sk As New SkewTransform
        Dim tg As New TransformGroup
        With tg.Children
            .Add(sc) : .Add(sk) : .Add(RootRotate)
        End With
        With RootCanvas
            .RenderTransformOrigin = New Point(0.5, 0.5)
            .RenderTransform = tg
            .Background = Brushes.Transparent
        End With

        'ピッタリ枠とかを更新するため
        Call SetDiffPointAndOutSize()


    End Sub

End Class

'////////////////////////////////////////////////////////////////////////////////////////////////////

'RectをRectangleGeometryに変換
Public Class MyConverterRect
    Implements IValueConverter

    Public Function Convert(
                           value As Object,
                           targetType As Type,
                           parameter As Object,
                           culture As CultureInfo) As Object Implements IValueConverter.Convert
        '    Throw New NotImplementedException()
        'Dim r As Rect = value
        'Dim rg As New RectangleGeometry(r)
        'Return rg
        Return New RectangleGeometry(value)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class


Public Class MyConverterVisibility
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        '    Throw New NotImplementedException()
        Dim cbIsChecked As Boolean = value
        Dim visible As Visibility
        If cbIsChecked Then
            visible = Visibility.Visible
        Else
            visible = Visibility.Hidden
        End If
        Return visible

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Structure MyTargetGridData
    Dim TargetLocate As Integer
    Dim Distance As Double
    Dim IsValid As Boolean
    Dim SetLocate As Double
End Structure

Public Structure MyTargetGridPointData
    Dim TargetGridPoint As Point '目標グリッド位置
    Dim Distance As Double '距離
    Dim IsValid As Boolean '
    Dim DiffPoint As Point '元の位置からの差分
End Structure