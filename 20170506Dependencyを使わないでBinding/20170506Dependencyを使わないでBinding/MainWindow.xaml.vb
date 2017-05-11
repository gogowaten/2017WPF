Imports System.ComponentModel
Imports System.Windows.Controls.Primitives

Class MainWindow
    Private _TargetThumb As Thumb
    Private Property TargetThumb As Thumb
        Get
            Return _TargetThumb
        End Get
        Set(value As Thumb)
            If (value Is _TargetThumb) Then Exit Property
            If Not _TargetThumb Is Nothing Then
                _TargetThumb.Background = New SolidColorBrush(SystemColors.ControlColor)
            End If
            _TargetThumb = value
            _TargetThumb.Background = Brushes.Red
            Call MySetBinding()
        End Set
    End Property

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnCheck.Click, AddressOf btnCheck_Click
        AddHandler btnSetSkewX.Click, AddressOf btnSetSkewX_Click
        AddHandler btnSetRotateAngle.Click, AddressOf btnSetRotateAngle_Click
        AddHandler btnSetScaleX.Click, AddressOf btnSetScaleX_Click
        Call AddThumb(300, 100, 40, 60, 10)
        Call AddThumb(300, 200, 40, 60, 10)
    End Sub
    Private Sub AddThumb(x As Double, y As Double, w As Double, h As Double, a As Double)
        Dim t As New Thumb With {.Width = w, .Height = h}
        Canvas.SetLeft(t, x) : Canvas.SetTop(t, y)
        Dim tg As New TransformGroup
        With tg.Children
            .Add(New ScaleTransform)
            .Add(New SkewTransform)
            .Add(New RotateTransform(a))
        End With
        t.RenderTransform = tg
        AddHandler t.PreviewMouseLeftButtonDown, AddressOf TargetThumb_PreviewMouseLeftButtonDown
        MyCanvas.Children.Add(t)
        TargetThumb = t

    End Sub
    Private Sub MySetBinding()
        'これで行けた！ソースにはThumbじゃなくて、その中のRotateTransformを指定する
        Dim ro As RotateTransform = GetRotateTransform()
        Dim b As New Binding With {
            .Source = ro,
            .Path = New PropertyPath(RotateTransform.AngleProperty),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "RotateTransform.AngleProperty = {0:0.0}"}
        sldAngle.SetBinding(Slider.ValueProperty, b)
        tbAngle.SetBinding(TextBlock.TextProperty, b)

        'SkewX
        b = New Binding With {
            .Source = GetSkewTransform(),
            .Path = New PropertyPath(SkewTransform.AngleXProperty),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "SkewX = {0:0.0}"}
        sldSkewX.SetBinding(Slider.ValueProperty, b)
        tbSkewX.SetBinding(TextBlock.TextProperty, b)

        'ScaleX
        b = New Binding With {
            .Source = GetScaleTransform(),
            .Path = New PropertyPath(ScaleTransform.ScaleXProperty),
            .Mode = BindingMode.TwoWay,
            .StringFormat = "ScaleX = {0:0.0}"}
        sldScaleX.SetBinding(Slider.ValueProperty, b)
        tbScale.SetBinding(TextBlock.TextProperty, b)

    End Sub
    Private Sub btnCheck_Click(sender As Object, e As RoutedEventArgs)
        MsgBox($"Angle = {GetRotateTransform().Angle}")
    End Sub

    Private Sub btnSetAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform().Angle = 30
    End Sub

    Private Function GetRotateTransform() As RotateTransform
        Return GetRenderTransform(GetType(RotateTransform))
    End Function
    Private Function GetSkewTransform() As SkewTransform
        Return GetRenderTransform(GetType(SkewTransform))
    End Function
    Private Function GetScaleTransform() As ScaleTransform
        Return GetRenderTransform(GetType(ScaleTransform))
    End Function
    Private Function GetRenderTransform(tType As Type) As Transform
        Dim tg As TransformGroup = TargetThumb.RenderTransform
        For Each c As Transform In tg.Children
            If c.GetType = tType Then
                Return c
            End If
        Next
        Return Nothing
    End Function


    Private Sub TargetThumb_PreviewMouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) 'Handles TargetEx2Thumb.PreviewMouseLeftButtonDown
        TargetThumb = sender
    End Sub
    Private Sub btnSetExAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform().Angle = 30
    End Sub

    Private Sub btnSetSkewX_Click(sender As Object, e As RoutedEventArgs)
        GetSkewTransform().AngleX = 30
    End Sub

    Private Sub btnSetRotateAngle_Click(sender As Object, e As RoutedEventArgs)
        GetRotateTransform().Angle = 20
    End Sub
    Private Sub btnSetScaleX_Click(sender As Object, e As RoutedEventArgs)
        GetScaleTransform().ScaleX = 2
    End Sub
End Class
