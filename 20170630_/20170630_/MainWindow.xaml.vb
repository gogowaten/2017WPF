'WPF、変形後の要素(Thumb)のグリッドスナップ移動 ( ソフトウェア ) - 午後わてんのブログ - Yahoo!ブログ
'https://blogs.yahoo.co.jp/gogowaten/15001512.html


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
        End With
    End Sub

    Private Sub MyCheck()
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
        Dim b As New Binding(sName) With {.Source = so, .StringFormat = sName & " = {0:0.0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
    End Sub

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'グリッドサイズ指定
        sldGrid.Value = 30
        'グリッド罫線表示
        Call SetGridLine()

        AddHandler btnCheck.Click, AddressOf MyCheck
        AddHandler btn2.Click, AddressOf MyCheck2
        AddHandler btn4.Click, AddressOf MyMove
        AddHandler sldGrid.ValueChanged, AddressOf SetGridLine 'スライダーの値変更でグリッド(罫線)の表示更新

        'ExThumbに100x100の赤Borderを追加してMyCanvasに表示
        Dim ext As New ExThumb(New Border With {
                               .Width = 100, .Height = 100, .Background = Brushes.Orange, .Opacity = 0.5})
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

        '目印の青枠
        'ぴったり枠のRectを青枠のDataにバインディング
        '値はRectからRectangleGeometryに変換する必要があるのでコンバータ使用
        b = New Binding(NameOf(ExThumb.MyOutBounds)) With {
            .Source = MyExThumb, .Converter = New MyConverterRect}
        pathRect.SetBinding(Path.DataProperty, b)

    End Sub

    'マウスドラッグ移動
    Private Sub MyExThumb_DragDelta(sender As Object,
                                    e As DragDeltaEventArgs) Handles MyExThumb.DragDelta
        Dim GridSize As Integer = sldGrid.Value
        ''通常移動
        'MyExThumb.MyLeft += e.HorizontalChange
        'MyExThumb.MyTop += e.VerticalChange

        'グリッドスナップ移動
        Dim x, y As Double
        Dim xx, yy, xxx, yyy As Integer
        Select Case True
            Case rbNormal.IsChecked
                '変形前の左上を基準、赤枠
                With MyExThumb
                    x = .MyLeft + e.HorizontalChange
                    y = .MyTop + e.VerticalChange
                    xx = x \ GridSize : yy = y \ GridSize
                    xxx = xx * GridSize : yyy = yy * GridSize
                    .MyLeft = xxx : .MyTop = yyy
                End With
            Case rbFitFlame.IsChecked
                'ぴったり枠の左上を基準、青枠(OutBounds)
                With MyExThumb
                    x = .MyLeft + e.HorizontalChange + .MyDiffPoint.X
                    y = .MyTop + e.VerticalChange + .MyDiffPoint.Y
                    xx = x \ GridSize : yy = y \ GridSize
                    xxx = xx * GridSize : yyy = yy * GridSize
                    .SetPoint2(xxx, yyy)
                End With
            Case rbTopLeft.IsChecked
                '変形で移動した元左上座標を基準
                With MyExThumb
                    x = .MyLeft + e.HorizontalChange + .MyDiffPointTopLeft.X
                    y = .MyTop + e.VerticalChange + .MyDiffPointTopLeft.Y
                    xx = x \ GridSize : yy = y \ GridSize
                    xxx = xx * GridSize : yyy = yy * GridSize
                    .SetPoint3(xxx, yyy)
                End With
        End Select


        '目印の移動
        '変形で移動した元左上座標
        Canvas.SetLeft(Line1, xxx)
        Canvas.SetTop(Line1, yyy)
        '元の枠、赤枠
        Canvas.SetLeft(InBounds, MyExThumb.MyLeft)
        Canvas.SetTop(InBounds, MyExThumb.MyTop)

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

    'DiffPointとOutBoundsの更新
    '変形時に実行する
    Private Sub SetDiffPointAndOutSize()
        Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(RootCanvas.Width, RootCanvas.Height)))
        MyDiffPoint = r.Location 'ピッタリ座標
        MyOutSize = r.Size 'ぴったりサイズ
        MyDiffPointTopLeft = gt.Transform(New Point) '元左上差分
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
        MyTransformedTopLeft = p + MyDiffPointTopLeft '左上更新
    End Sub




#Region "Property"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(<System.Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

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
            End If
        End Set
    End Property

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