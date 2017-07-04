'WPF、変形した要素を指定位置に移動、NotifyProperty ( ソフトウェア ) - 午後わてんのブログ - Yahoo!ブログ
'https://blogs.yahoo.co.jp/gogowaten/14998511.html


Imports System.ComponentModel
Imports System.Windows.Controls.Primitives


Class MainWindow
    Private WithEvents MyExThumb As ExThumb

    Private Sub MyCheck()
        Dim root = MyExThumb.testRootCanvas
    End Sub
    Private Sub MyCheck2()
        MyExThumb.SetPoint2(100, 100)
    End Sub
    Private Sub MyMove()
        MyExThumb.MyLeft = 100
        MyExThumb.MyTop = 100
    End Sub
    Private Sub MyMove2()
        MyExThumb.SetPoint2(0, 0)
    End Sub
    Private Sub MyMove3()
        MyExThumb.MyLeft = 0
        MyExThumb.MyTop = 0
    End Sub
    '数値確認用のTextBlockへのBinding
    Private Sub SetTextBlockBinding(so As Object, sName As String, tb As TextBlock)
        Dim b As New Binding(sName) With {.Source = so, .StringFormat = sName & " = {0:0.0}"}
        tb.SetBinding(TextBlock.TextProperty, b)
    End Sub

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnCheck.Click, AddressOf MyCheck
        'AddHandler btn1.Click, AddressOf MyMove2
        AddHandler btn2.Click, AddressOf MyCheck2
        'AddHandler btn3.Click, AddressOf MyMove3
        AddHandler btn4.Click, AddressOf MyMove

        'ExThumbに100x100の赤Borderを追加してMyCanvasに表示
        Dim ext As New ExThumb(New Border With {
                               .Width = 100, .Height = 100, .Background = Brushes.Red, .Opacity = 0.5})
        Canvas.SetLeft(ext, 0) : Canvas.SetTop(ext, 0)
        MyCanvas.Children.Add(ext)
        MyExThumb = ext

        '回転角度をSliderにBinding
        Dim b As Binding
        b = New Binding(NameOf(ExThumb.MyAngle)) With {.Source = MyExThumb, .Mode = BindingMode.TwoWay}
        sldAngle.SetBinding(Slider.ValueProperty, b)

        '数値確認用のTextBlockへのBinding
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyAngle), tbAngle) '角度
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.DiffPoint), tbRect) '差分座標
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyLeft), tbLeft) '実際のX座標
        Call SetTextBlockBinding(MyExThumb, NameOf(ExThumb.MyOutBounds), tbBounds) '見た目のピッタリ枠

    End Sub

    Private Sub MyExThumb_DragDelta(sender As Object, e As DragDeltaEventArgs) Handles MyExThumb.DragDelta
        MyExThumb.MyLeft += e.HorizontalChange
        MyExThumb.MyTop += e.VerticalChange
    End Sub
End Class




Public Class ExThumb
    Inherits Thumb 'Thumbを継承
    Implements ComponentModel.INotifyPropertyChanged '通知プロパティ用
    Private RootCanvas As Canvas
    Private RootRotate As RotateTransform
    Public testRootCanvas As Canvas

    'OutBoundsの左上座標を指定
    Public Sub SetPoint2(x As Double, y As Double)
        MyLeft = x + (-DiffPoint.X)
        MyTop = y + (-DiffPoint.Y)
    End Sub

    'DiffPointとOutBoundsの更新、変形時に実行する
    Private Sub SetDiffPointAndOutSize()
        Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(RootCanvas.Width, RootCanvas.Height)))
        DiffPoint = r.Location
        OutSize = r.Size
        Call SetOutBounds()
        '未使用
        Dim p1 As Point = gt.Transform(New Point) '変形後の左上座標
        Dim p2 As Point = gt.Transform(New Point(RootCanvas.Width, 0)) '右上
        Dim p3 As Point = gt.Transform(New Point(RootCanvas.Width, RootCanvas.Height)) '右下
        Dim p4 As Point = gt.Transform(New Point(0, RootCanvas.Height)) '左下
    End Sub

    'OutBoundsの更新、移動時に実行する
    Private Sub SetOutBounds()
        'Dim gt As GeneralTransform = RootCanvas.TransformToVisual(Me)
        Dim r As Rect = New Rect(New Point(DiffPoint.X + MyLeft, DiffPoint.Y + MyTop), OutSize)
        MyOutBounds = r

    End Sub


#Region "Property"
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(<System.Runtime.CompilerServices.CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
    '変形前後の左上座標の差分
    Private Property _DiffPoint As Point
    Public Property DiffPoint As Point
        Get
            Return _DiffPoint
        End Get
        Set(value As Point)
            _DiffPoint = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '要素がピッタリ収まるサイズ
    Private Property _OutSize As Size
    Public Property OutSize As Size
        Get
            Return _OutSize
        End Get
        Set(value As Size)
            _OutSize = value
            Call NotifyPropertyChanged()
        End Set
    End Property
    '回転角度
    Private Property _MyAngle As Double
    Public Property MyAngle As Double
        Get
            Return _MyAngle
        End Get
        Set(value As Double)
            _MyAngle = value
            RootRotate.Angle = value
            Call NotifyPropertyChanged()
            Call SetDiffPointAndOutSize()
        End Set
    End Property
    'X座標
    Private Property _MyLeft As Double
    Public Property MyLeft As Double
        Get
            Return _MyLeft
        End Get
        Set(value As Double)
            _MyLeft = value
            Canvas.SetLeft(Me, value)
            Call NotifyPropertyChanged()
            Call SetOutBounds()
        End Set
    End Property
    'Y座標
    Private Property _MyTop As Double
    Public Property MyTop As Double
        Get
            Return _MyTop
        End Get
        Set(value As Double)
            _MyTop = value
            Canvas.SetTop(Me, value)
            Call NotifyPropertyChanged()
            Call SetOutBounds()
        End Set
    End Property
    '要素がピッタリ収まる四角枠
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
#End Region



    ''これは失敗、DependencyPropertyとPropertyChangedCallbackの組み合わせ
    ''回転後の枠を求めるタイミングが早いみたいで変更前の角度が採用されてしまう
    'Public Shared Sub SetMyRect2(obj As DependencyObject, e As DependencyPropertyChangedEventArgs)
    '    Dim t As ExThumb = DirectCast(obj, ExThumb)
    '    Dim c As Canvas = t.RootCanvas
    '    Dim r As Rect = c.TransformToVisual(t).TransformBounds(New Rect(New Size(c.Width, c.Height)))
    '    t.MyOutBounds = r
    'End Sub

    'Public Shared ReadOnly AngleProperty As DependencyProperty = DependencyProperty.Register(
    '    NameOf(Angle), GetType(Double), GetType(ExThumb),
    '    New PropertyMetadata(0.0, New PropertyChangedCallback(AddressOf SetMyRect2)))
    'Public Property Angle As Double
    '    Get
    '        Return GetValue(AngleProperty)
    '    End Get
    '    Set(value As Double)
    '        SetValue(AngleProperty, value)
    '    End Set
    'End Property




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
            .Children.Add(elm)
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

        'ピッタリ枠とかを更新するために角度指定
        MyAngle = 0.0


        ''DependencyPropertyではうまくできなかった
        'BindingOperations.SetBinding(
        '    RootRotate, RotateTransform.AngleProperty,
        '    New Binding With {.Source = Me, .Path = New PropertyPath(AngleProperty), .Mode = BindingMode.TwoWay})

        'Dim b As Binding
        'b = New Binding(NameOf(ExThumb.MyAngle))
        'BindingOperations.SetBinding(RootRotate, RotateTransform.AngleProperty, b)


    End Sub

End Class