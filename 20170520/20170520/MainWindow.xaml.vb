Class MainWindow

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Call SetTextBlock()
        AddHandler btnTextUpdate.Click, AddressOf TextUpdate_Click


    End Sub
    Private Sub SetTextBlock()
        Dim r As Rect
        r = RedBorder.TransformToVisual(MyCanvas).TransformBounds(GetBorderRect(RedBorder))
        Dim str As String
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock1.Text = str
        r = BlueBorder.TransformToVisual(MyCanvas).TransformBounds(GetBorderRect(BlueBorder))
        str = $" {r.X:0.00}, {r.Y:0.00}, {r.Width:0.00}, {r.Height:0.00}"
        MyTextBlock2.Text = str

    End Sub
    Private Function GetBorderRect(b As Border)
        Return New Rect(Canvas.GetLeft(b), Canvas.GetTop(b), b.Width, b.Height)
    End Function

    Private Sub TextUpdate_Click(sender As Object, e As RoutedEventArgs)
        Call SetTextBlock()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim w1 As New Window1 : w1.Show()
        Dim w2 As New Window2 : w2.Show()
    End Sub

    'Private Sub Angle1_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Angle1.ValueChanged

    '    Call SetTextBlock()
    'End Sub
End Class
