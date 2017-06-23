Imports System.Globalization

Class MainWindow
    Private ActBorder As Class1

    Private Sub ChangeRect()
        Dim gt As GeneralTransform = ActBorder.TransformToVisual(MyCanvas)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(ActBorder.ActualWidth, ActBorder.ActualHeight)))
        ActBorder.MyRect = r
    End Sub
    Private Sub MyCheck()
        MsgBox(ActBorder.MyRect.ToString)


        ActBorder.MyRect = New Rect(1, 12, 2, 2)



    End Sub
    Private Sub AngleAdd10()
        ActBorder.MyAngle += 10
    End Sub

    'バインディング作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty, strF As String) As Binding
        Dim b As New Binding With {
           .Source = sObj,
           .Path = New PropertyPath(sDp),
           .Mode = BindingMode.TwoWay,
           .StringFormat = strF}
        Return b
    End Function

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        ActBorder = New Class1(MyCanvas, 100, 30, Brushes.Tomato, 100, 100, 20)
        'MyCanvas.Children.Add(ActBorder)

        Dim bindL As Binding = GetMyBinding(ActBorder, Class1.MyCanvasLeftProperty, "CanvasLeft = {0:0.0}")
        sldCanvasLeft.SetBinding(Slider.ValueProperty, bindL)
        tbCanvasLeft.SetBinding(TextBlock.TextProperty, bindL)
        Dim bindT As Binding = GetMyBinding(ActBorder, Class1.MyCanvasTopProperty, "CanvasTop = {0:0.0}")
        sldCanvasTop.SetBinding(Slider.ValueProperty, bindT)
        tbCanvasTop.SetBinding(TextBlock.TextProperty, bindT)
        Dim bindA As Binding = GetMyBinding(ActBorder, Class1.MyAngleProperty, "Angle = {0:0.0}")
        sldAngle.SetBinding(Slider.ValueProperty, bindA)
        tbAngle.SetBinding(TextBlock.TextProperty, bindA)

        Dim bindR As Binding = GetMyBinding(ActBorder, Class1.MyRectProperty, "Rect ={0:  0.0}")
        bindR.Mode = BindingMode.OneWay
        tbRect.SetBinding(TextBlock.TextProperty, bindR)

        'Dim b As New Binding With {
        '    .Source = ro,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.OneWay,
        '    .Converter = New MyConverter,
        '    .ConverterParameter = Me}
        'tbText.SetBinding(TextBlock.TextProperty, b)




        'AddHandler sldAngle.ValueChanged, AddressOf ChangeRect
        AddHandler btCheck.Click, AddressOf MyCheck
        AddHandler btAngleAdd10.Click, AddressOf AngleAdd10

    End Sub

    'Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    '    ActBorder.SetMyRect()

    'End Sub
End Class






Public Class Class1
    Inherits Border 'Borderクラスをを継承
    Private MyScaleTransform As ScaleTransform
    Private MyRotateTransform As RotateTransform
    Private ParentPanel As Canvas

    '依存関係プロパティ
    'CanvasLeft用
    Public Shared ReadOnly Property MyCanvasLeftProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyCanvasLeft), GetType(Double), GetType(Class1), New PropertyMetadata(0.0))
    Public Property MyCanvasLeft As Double
        Get
            Return GetValue(MyCanvasLeftProperty)
        End Get
        Set(value As Double)
            SetValue(MyCanvasLeftProperty, value)
        End Set
    End Property

    'CanvasTop用
    Public Shared ReadOnly Property MyCanvasTopProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyCanvasTop), GetType(Double), GetType(Class1), New PropertyMetadata(0.0))
    Public Property MyCanvasTop As Double
        Get
            Return GetValue(MyCanvasTopProperty)
        End Get
        Set(value As Double)
            SetValue(MyCanvasTopProperty, value)
        End Set
    End Property

    'Angle
    Public Shared ReadOnly Property MyAngleProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyAngle), GetType(Double), GetType(Class1))
    'Public Shared ReadOnly MyAngleProperty As DependencyProperty =
    'DependencyProperty.Register(NameOf(MyAngle), GetType(Double), GetType(Class1),
    '                            New PropertyMetadata(0.0, New PropertyChangedCallback(AddressOf AnglePropertyChanged)))
    Public Property MyAngle As Double
        Get
            Return GetValue(MyAngleProperty)
        End Get
        Set(value As Double)
            SetValue(MyAngleProperty, value)
            'MsgBox("change angle")
        End Set
    End Property
    'Private Shared Sub AnglePropertyChanged(dObj As DependencyObject, e As DependencyPropertyChangedEventArgs)
    '    'MsgBox("changed angle")
    '    Dim cl1 As Class1 = dObj
    '    Dim gt As GeneralTransform = cl1.TransformToVisual(cl1.ParentPanel) ',Me.Parentだとインスタンス化のときにNothingなのでエラーになる
    '    Dim r As Rect = gt.TransformBounds(New Rect(New Size(cl1.ActualWidth, cl1.ActualHeight)))
    '    cl1.MyRect = r

    'End Sub

    '見た目のRect
    Public Shared ReadOnly Property MyRectProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(MyRect), GetType(Rect), GetType(Class1))
    Public Property MyRect As Rect
        Get
            Return GetValue(MyRectProperty)
        End Get
        Set(value As Rect)
            SetValue(MyRectProperty, value)
        End Set
    End Property

    'バインディングソースの作成用
    Private Function GetMyBinding(sObj As DependencyObject, sDp As DependencyProperty) As Binding
        Dim b As New Binding With {
            .Source = sObj,
            .Path = New PropertyPath(sDp),
            .Mode = BindingMode.TwoWay,
            .NotifyOnSourceUpdated = True,
            .NotifyOnTargetUpdated = True}
        Return b

    End Function

    'ScaleXとScaleYを同期するかしないかの切り替え
    Public Sub SetScaleLink(IsLink As Boolean)
        If IsLink Then '同期する場合
            'Xをソースにして、Yをターゲットにする
            BindingOperations.SetBinding(Me, MyCanvasTopProperty, GetMyBinding(Me, Class1.MyCanvasLeftProperty))

        Else '同期しない場合(別々に戻す)
            'Yのバインディングを外す(空のBindingをバインディングする)
            BindingOperations.SetBinding(Me, MyCanvasTopProperty, New Binding)
            '今の値を継続したいのでXの値をYにコピー、これをしないとYの値が初期値の1になってしまう
            MyCanvasTop = MyCanvasLeft
        End If
    End Sub

    '見た目のRect
    Private Sub SetMyRect()
        'If Me.Parent Is Nothing Then Exit Sub
        Dim gt As GeneralTransform = Me.TransformToVisual(ParentPanel) ',Me.Parentだとインスタンス化のときにNothingなのでエラーになる
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(Me.ActualWidth, Me.ActualHeight)))
        MyRect = r
    End Sub
    'Private Shared Sub SetMyRect(child As Class1)
    '    If child.Parent Is Nothing Then Exit Sub
    '    Dim gt As GeneralTransform = child.TransformToVisual(child.ParentPanel) ',Me.Parentだとインスタンス化のときにNothingなのでエラーになる
    '    Dim r As Rect = gt.TransformBounds(New Rect(New Size(child.ActualWidth, child.ActualHeight)))
    '    child.MyRect = r
    'End Sub


    'コンストラクタ
    Public Sub New(panel As Panel, w As Double, h As Double, b As Brush)
        Call Me.New(panel, w, h, b, 1.0, 1.0, 0.0)
    End Sub
    Public Sub New(panel As Panel, w As Double, h As Double, b As Brush, cLeft As Double, cTop As Double, angle As Double)
        ParentPanel = panel
        ParentPanel.Children.Add(Me)
        Width = w : Height = h : Background = b

        MyCanvasLeft = cLeft
        MyCanvasTop = cTop
        MyAngle = angle

        '各種トランスフォームをグループにしてRenderTransformに指定
        MyScaleTransform = New ScaleTransform '拡縮 今回は未使用
        Dim sk As New SkewTransform '並行変形、今回は未使用
        'Dim ro As New RotateTransform '回転
        MyRotateTransform = New RotateTransform

        'グループ作成
        Dim tg As New TransformGroup
        With tg.Children
            .Add(MyScaleTransform) : .Add(sk) : .Add(MyRotateTransform)
        End With
        Me.RenderTransform = tg '指定
        Me.RenderTransformOrigin = New Point(0.5, 0.5) '変形の基準点は中心

        'バインディング
        'ソース：用意した依存関係プロパティ
        'ターゲット：
        BindingOperations.SetBinding(Me, Canvas.LeftProperty, GetMyBinding(Me, MyCanvasLeftProperty))
        BindingOperations.SetBinding(Me, Canvas.TopProperty, GetMyBinding(Me, MyCanvasTopProperty))
        BindingOperations.SetBinding(MyRotateTransform, RotateTransform.AngleProperty, GetMyBinding(Me, MyAngleProperty))


    End Sub


    Private Sub Class1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '見た目のRectをAnglePropertyにバインディング
        'これはコンストラクタで行うとまだ描画されていないので正しい値が取得できないのでLoaded
        Dim bind As New Binding With {
            .Source = MyRotateTransform,
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay,
            .Converter = New MyConverter,
            .ConverterParameter = {Me, ParentPanel}}
        BindingOperations.SetBinding(Me, MyRectProperty, bind)


    End Sub
End Class

'[C# WPF]Binding ConverterParameterプロパティを使う: C# WPF 入門
'http://cswpf.seesaa.net/article/313843710.html?seesaa_related=related_article

Public Class MyConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        'Throw New NotImplementedException()
        'Dim angle As Double = value
        Dim bo As Class1 = DirectCast(parameter(0), Class1)
        Dim panel As Panel = DirectCast(parameter(1), Panel)
        'If bo.Parent Is Nothing Then Return New Rect

        Dim gt As GeneralTransform = bo.TransformToVisual(panel)
        Dim r As Rect = gt.TransformBounds(New Rect(New Size(bo.ActualWidth, bo.ActualHeight)))
        Return r

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        'Throw New NotImplementedException()
        'Dim bo As Class1 = DirectCast(parameter(0), Class1)
        'Return bo.MyAngle
    End Function
End Class