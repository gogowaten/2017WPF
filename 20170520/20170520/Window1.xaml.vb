Public Class Window1

    Private Sub TextUpdate()
        Dim r As Rect
        Dim str As String
        r = RedBorder.TransformToVisual(MyCanvas).TransformBounds(GetBorderRect(RedBorder))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock1.Text = str

        r = RedBorder.TransformToVisual(MyCanvas).TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock2.Text = str

        r = RedBorder.RenderTransform.TransformBounds(GetBorderRect(RedBorder))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock3.Text = str

        r = RedBorder.RenderTransform.TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock4.Text = str

        r = MyCanvas.TransformToVisual(RedBorder).TransformBounds(New Rect(100, 100, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock5.Text = str

        r = MyCanvas.TransformToVisual(RedBorder).TransformBounds(New Rect(0, 0, 100, 100))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock6.Text = str

    End Sub
    Private Function GetBorderRect(b As Border)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call TextUpdate()
    End Sub

    Private Sub SaveImage()

    End Sub
    Private Sub SaveImage_Click(sender As Object, e As RoutedEventArgs)
        Call SaveImage()
    End Sub


    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler btnTextUpdate.Click, AddressOf TextUpdate_Click

    End Sub
    Private Sub Window1_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Call TextUpdate()
    End Sub
End Class
