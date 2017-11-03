'装飾の概要
'https://msdn.microsoft.com/ja-jp/library/ms743737%28v=vs.110%29.aspx?cs-save-lang=1&cs-lang=vb&f=255&MSPPError=-2147217396#code-snippet-1


Class MainWindow
    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim rect As New Rectangle With {.Width = 100, .Height = 30, .Fill = Brushes.Red}
        MyGrid.Children.Add(rect)
        Dim myAdornerLayer As AdornerLayer = AdornerLayer.GetAdornerLayer(rect)
        'Initializedイベントだとまだ描画が終わっていないせいか↑がNothingになるので
        'Loadedイベントで実行
        myAdornerLayer.Add(New SimpleCircleAdorner(rect))

        rect = New Rectangle With {.Width = 50, .Height = 40, .Fill = Brushes.Blue, .HorizontalAlignment = HorizontalAlignment.Left}
        MyGrid.Children.Add(rect)
        myAdornerLayer = AdornerLayer.GetAdornerLayer(rect)
        myAdornerLayer.Add(New SimpleCircleAdorner(rect))


        Dim w As New Window1
        w.Show()
    End Sub
    'Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
    '    wrapPanel.Children.Add(New Button With {.Style = Me.Resources("AddButtonStyle"),
    '                           .Width = 40, .Height = 100, .Content = "Forcusde"})
    'End Sub

End Class

Public Class SimpleCircleAdorner
    Inherits Adorner
    Sub New(adornerElement As UIElement)
        MyBase.New(adornerElement)
    End Sub

    Protected Overrides Sub OnRender(drawingContext As DrawingContext)
        MyBase.OnRender(drawingContext)
        Dim adornedElementRect As New Rect(AdornedElement.DesiredSize)
        Dim renderBrush As New SolidColorBrush(Colors.Green) With {.Opacity = 0.2}
        Dim renderPen As New Pen(New SolidColorBrush(Colors.Navy), 1.5)
        Dim renderRadius As Double = 5.0

        With drawingContext
            .DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius)
            .DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius)
            .DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius)
            .DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius)

        End With

    End Sub
End Class