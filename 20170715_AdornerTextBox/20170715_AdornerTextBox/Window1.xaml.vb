Public Class Window1
    Private Sub Window1_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        Dim bo As New MyBorder With {.Width = 100, .Height = 30, .Background = Brushes.AliceBlue}
        MyCanvas.Children.Add(bo)
        Canvas.SetLeft(bo, 10) : Canvas.SetTop(bo, 20)

        AddHandler bo.MouseDown, AddressOf ChangeColor

    End Sub
    Private Sub ChangeColor(sender As Object, e As RoutedEventArgs)
        Dim bo As MyBorder = sender
        bo.Background = Brushes.Cyan

    End Sub
End Class


Public Class MyBorder
    Inherits Border
    Protected Overrides Sub OnInitialized(e As EventArgs)
        MyBase.OnInitialized(e)
        AddHandler Me.Loaded, AddressOf InitializeAdorner
    End Sub
    Private Sub InitializeAdorner(sender As Object, e As RoutedEventArgs)
        AdornerLayer.GetAdornerLayer(Me).Add(New MyAdorner(Me))
    End Sub

End Class