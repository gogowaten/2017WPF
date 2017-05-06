Imports System.ComponentModel
Imports System.Windows.Controls.Primitives

Class MainWindow
    Private WithEvents _TargetEx2Thumb As Ex2Thumb
    Public Property TargetEx2Thumb As Ex2Thumb
        Get
            Return _TargetEx2Thumb
        End Get
        Set(value As Ex2Thumb)
            If (value Is _TargetEx2Thumb) Then Exit Property
            _TargetEx2Thumb = value
            Call MySetBinding()
        End Set
    End Property


    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnCheck.Click, AddressOf btnCheck_Click
        AddHandler btnSetEx2Angle.Click, AddressOf btnSetEx2Angle_Click
        AddHandler btnSetRotateAngle.Click, AddressOf btnSetRotateAngle_Click

        Dim ext As Ex2Thumb
        ext = New Ex2Thumb()
        AddHandler ext.PreviewMouseLeftButtonDown, AddressOf TargetEx2Thumb_PreviewMouseLeftButtonDown
        MyCanvas.Children.Add(ext)
        TargetEx2Thumb = ext

        ext = New Ex2Thumb(300, 200, 50, 50, 10)
        AddHandler ext.PreviewMouseLeftButtonDown, AddressOf TargetEx2Thumb_PreviewMouseLeftButtonDown
        MyCanvas.Children.Add(ext)
        TargetEx2Thumb = ext

        'Call MySetBinding()

    End Sub

    Public Sub MySetBinding()
        'Sliderとのバインディング
        Dim b As Binding
        'b = New Binding With {
        '    .Source = TargetEx2Thumb,
        '    .Path = New PropertyPath(Ex2Thumb.Angle2Property),
        '    .Mode = BindingMode.TwoWay}
        '↕どちらでも同じ結果だけど↑のほうが打ち間違いがなくていい
        'b = New Binding("Angle2") With {
        '    .Source = TargetEx2Thumb,
        '    .Mode = BindingMode.TwoWay}


        b = New Binding With {
            .Source = GetRotateTransform(TargetEx2Thumb),
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay}
        '↓は間違い
        'b = New Binding With {
        '    .Source = TargetEx2Thumb,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay}

        'SliderのValuePropertyにBinding
        sldEx2Angle.SetBinding(Slider.ValueProperty, b)

        'TextBlock、Angle2Propertyとのバインディング
        '
        b = New Binding With {
            .Source = TargetEx2Thumb,
            .Path = New PropertyPath(Ex2Thumb.Angle2Property),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "TargetEx2Thumb.Angle2 = {0:0.0}"}
        tbEx2Angle.SetBinding(TextBlock.TextProperty, b)


        'TextBlock、RotateTransformのAnglePropertyとのバインディング
        'b = New Binding With {
        '    .Source = TargetEx2Thumb,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay,
        '    .StringFormat = "Rotare.AngleProperty = {0:0.0}"}
        'tbEx2RotateAngle.SetBinding(TextBlock.TextProperty, b)
        '↕違いはSource、上だと動かない
        Dim ExRo As RotateTransform = GetRotateTransform(TargetEx2Thumb)
        b = New Binding With {
            .Source = ExRo,
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "RotateTransform.Angle = {0:0.0}"}
        tbEx2RotateAngle.SetBinding(TextBlock.TextProperty, b)


    End Sub


    Private Sub btnCheck_Click(sender As Object, e As RoutedEventArgs)
        Dim a As Double = GetRotateTransform(TargetEx2Thumb).Angle
        Dim a2 As Double = TargetEx2Thumb.Angle2
        MsgBox($"RotateTransform.Angle = {a}
TargetEx2Thumb.Angle2 = {a2}")

    End Sub

    'RenderTransformの中のRotateTransformを取得する
    Private Function GetRotateTransform(t As UIElement) As RotateTransform
        Dim tg As TransformGroup = t.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = GetType(RotateTransform) Then
                Return c
            End If
        Next
        Return Nothing
    End Function

    Private Sub btnSetEx2Angle_Click(sender As Object, e As RoutedEventArgs)
        TargetEx2Thumb.Angle2 = 30
    End Sub

    Private Sub btnSetRotateAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform(TargetEx2Thumb).Angle = 20
    End Sub


    Private Sub TargetEx2Thumb_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) 'Handles TargetEx2Thumb.PreviewMouseLeftButtonDown
        If (sender Is TargetEx2Thumb) Then Exit Sub
        TargetEx2Thumb = sender
        Call MySetBinding()
    End Sub

    Private Sub TargetEx2Thumb_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) 'Handles TargetEx2Thumb.MouseLeftButtonDown
        If (sender Is TargetEx2Thumb) Then Exit Sub
        TargetEx2Thumb = sender
        Call MySetBinding()
    End Sub
End Class



Public Class Ex2Thumb
    Inherits Thumb

    Public Sub New()
        Me.New(300, 100, 50, 50, 45)
    End Sub
    ''' <summary>
    ''' Ex2Thumb作成
    ''' </summary>
    ''' <param name="x">左位置</param>
    ''' <param name="y">上位置</param>
    ''' <param name="w">幅</param>
    ''' <param name="h">高さ</param>
    ''' <param name="a">回転角度</param>
    Public Sub New(x As Double, y As Double, w As Double, h As Double, a As Double)

        '初期設定、位置とサイズ
        Canvas.SetLeft(Me, x) : Canvas.SetTop(Me, y)
        Width = w : Height = h
        'RenderTransform設定
        Dim tg As New TransformGroup
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(New RotateTransform(a)) '回転角度
        End With
        Me.RenderTransform = tg
        'Me.RenderTransformOrigin = New Point(0.5, 0.5)
        'Angle2 = a

        'Binding
        Call MySetBinging()

    End Sub
    Private Sub MySetBinging()
        'Angle2とRotateTransformのAnglePropertyをBinding
        'Bindingのソース作成
        Dim bind As Binding
        'bind = New Binding With {
        '    .Source = Me,
        '    .Path = New PropertyPath(Angle2Property),
        '    .Mode = BindingMode.TwoWay}
        'Me.SetBinding(RotateTransform.AngleProperty, bind)

        'bind = New Binding With {
        '    .Source = Me,
        '    .Path = New PropertyPath(RotateTransform.AngleProperty),
        '    .Mode = BindingMode.TwoWay}
        'Me.SetBinding(Angle2Property, bind)

        'RotateTransform.AnglePropertyをソースにして自身のAngle2Propertyをターゲットにする場合
        'Bindingソース作成
        bind = New Binding With {
            .Source = GetRotateTransform(),
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay}
        'ターゲット(自身のAngle2Property)にBindingする
        Me.SetBinding(Angle2Property, bind)
        '↕どちらでも同じ結果
        'BindingOperations.SetBinding(Me, RotateTransform.AngleProperty, bind)

        ''自身のAngle2PropertyをソースにしてRotateTransform.AnglePropertyをターゲットにする場合
        ''Bindingソース作成
        'bind = New Binding With {
        '    .Source = Me,
        '    .Path = New PropertyPath(Angle2Property),
        '    .Mode = BindingMode.TwoWay}
        ''ターゲット(RotateTransform.AngleProperty)にBindingする
        'BindingOperations.SetBinding(GetRotateTransform(), RotateTransform.AngleProperty, bind)

    End Sub

    '回転角度DependencyProperty
    Public Shared ReadOnly Property Angle2Property As DependencyProperty =
        DependencyProperty.Register("Angle2", GetType(Double), GetType(Ex2Thumb))
    Public Property Angle2 As Double
        Get
            Return GetValue(Angle2Property)
        End Get
        Set(value As Double)
            SetValue(Angle2Property, value)
        End Set
    End Property

    'RotateTransformの中のRotateTransformを取得する
    Private Function GetRotateTransform() As RotateTransform
        Dim tg As TransformGroup = Me.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = GetType(RotateTransform) Then
                Return c
            End If
        Next
        Return Nothing
    End Function

    '左クリックされたらMainWindowのTargetEx2Thumbを入れ替えてBindingもし直す
    'Private Sub Ex2Thumb_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs) Handles Me.PreviewMouseDown
    '    If (MainW.TargetEx2Thumb Is sender) Then Exit Sub
    '    MainW.TargetEx2Thumb = Me
    '    MainW.MySetBinding()
    'End Sub
    'Protected Overrides Sub OnMouseLeftButtonDown(e As MouseButtonEventArgs)
    '    MyBase.OnMouseLeftButtonDown(e)
    '    MainW.TargetEx2Thumb = Me
    'End Sub


End Class
